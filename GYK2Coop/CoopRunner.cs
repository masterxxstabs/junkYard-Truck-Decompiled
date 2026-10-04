using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using BepInEx;
using GYK2Coop.Game;
using GYK2Coop.Net;
using UnityEngine;

namespace GYK2Coop
{
    /// <summary>
    /// Owns the co-op session: hosting / joining, the handshake and world transfer, and the
    /// per-frame sync of players, time and world objects. Also draws the F8 panel.
    /// </summary>
    internal class CoopRunner : MonoBehaviour
    {
        private enum Role
        {
            None,
            Host,
            Guest,
        }

        private enum Phase
        {
            None,
            Connecting,   // guest: TCP connect in progress
            Handshake,    // waiting for Hello / Welcome
            Transfer,     // world being sent / received
            Loading,      // guest is loading the received world
            Playing,
        }

        private const float StateInterval = 1f / 15f;
        private const float StateHeartbeat = 0.5f;
        private const float TimeInterval = 1f;
        private const float PingInterval = 2f;
        private const float CharacterUploadInterval = 30f;
        private const double Timeout = 30;
        private const double LongTimeout = 180;

        private Role role;
        private Phase phase;
        private Listener listener;
        private Connection conn;
        private string remoteName = "";
        private Puppet puppet;
        private string status = "Not connected.";

        private readonly object connectLock = new object();
        private TcpClient connectResult;
        private string connectError;
        private bool connectDone;

        private bool gameStartedFlag;
        private bool returnToMenuPending;
        private bool controlTaken;

        private float nextState, nextStateHeartbeat, nextTime, nextPing, nextCharacterUpload, nextPuppetAttempt;
        private Vector3 lastSentPos;
        private int lastSentAnim = -1;
        private Vector2 lastSentDir;
        private string lastSentCarry = "";
        private float[] lastSentLayers = new float[0];
        private readonly List<string> overheadIcons = new List<string>();

        // UI
        private bool panelOpen;
        private bool legacyInputBroken;
        private Rect windowRect = new Rect(20, 20, 380, 10);
        private string nameField;
        private string addressField;
        private string portField;
        private string chatField = "";
        private readonly List<KeyValuePair<float, string>> chat = new List<KeyValuePair<float, string>>();
        private GUIStyle tagStyle, shadowStyle;

        private void Start()
        {
            nameField = CoopPlugin.PlayerName.Value;
            addressField = CoopPlugin.LastAddress.Value;
            portField = CoopPlugin.Port.Value.ToString();
            WorldSync.Send = SendFrame;
            MainGame.OnGameStarted = (Action)Delegate.Combine(MainGame.OnGameStarted, (Action)(() => gameStartedFlag = true));
        }

        // ================================================================ frame loop

        private void Update()
        {
            if (!legacyInputBroken)
            {
                try
                {
                    if (Input.GetKeyDown(CoopPlugin.ToggleKey.Value))
                        SetPanelOpen(!panelOpen);
                }
                catch (InvalidOperationException)
                {
                    // Game uses only the new Input System; OnGUI handles the key instead.
                    legacyInputBroken = true;
                }
            }

            try
            {
                PollConnect();
                PollAccept();
                PollPackets();
                PollConnectionHealth();
                PollGameState();
                if (phase == Phase.Playing)
                    TickPlaying();
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogError("Co-op update error: " + e);
            }
        }

        private void LateUpdate()
        {
            if (phase == Phase.Playing)
            {
                try
                {
                    WorldSync.Flush();
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogError("World sync error: " + e);
                }
            }
        }

        private void OnApplicationQuit()
        {
            if (role == Role.Guest && phase == Phase.Playing)
                UploadCharacter();
            Disconnect("game closed", goToMenu: false);
        }

        private void PollConnect()
        {
            if (role != Role.Guest || phase != Phase.Connecting)
                return;
            TcpClient client;
            string error;
            lock (connectLock)
            {
                if (!connectDone)
                    return;
                connectDone = false;
                client = connectResult;
                error = connectError;
                connectResult = null;
            }
            if (client == null)
            {
                Fail("Could not connect: " + error);
                return;
            }
            conn = new Connection(client);
            phase = Phase.Handshake;
            status = "Connected, saying hello...";
            Send(MsgType.Hello, w =>
            {
                w.Write(CoopPlugin.ProtocolVersion);
                w.Write(CoopPlugin.Version);
                w.Write(Application.version ?? "");
                w.Write(CoopPlugin.PlayerName.Value);
            });
        }

        private void PollAccept()
        {
            if (role != Role.Host || listener == null)
                return;
            while (listener.Accepted.TryDequeue(out TcpClient c))
            {
                var incoming = new Connection(c);
                if (conn != null && !conn.IsClosed)
                {
                    incoming.Send(Protocol.Build(MsgType.Refuse, w => w.Write("This world already has a guest.")));
                    incoming.CloseAfterFlush("refused");
                    continue;
                }
                conn = incoming;
                phase = Phase.Handshake;
                status = "Someone is connecting from " + conn.RemoteEndPoint + "...";
                CoopPlugin.Log.LogInfo("Incoming connection from " + conn.RemoteEndPoint);
            }
        }

        private void PollPackets()
        {
            if (conn == null)
                return;
            int budget = 400;
            while (budget-- > 0 && conn != null && conn.Incoming.TryDequeue(out Packet p))
            {
                try
                {
                    Handle(p);
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogError("Error handling " + p.Type + ": " + e);
                }
            }
        }

        private void PollConnectionHealth()
        {
            if (conn == null)
                return;
            if (conn.IsClosed)
            {
                OnConnectionLost(conn.CloseReason ?? "connection closed");
                return;
            }
            double limit = phase == Phase.Playing ? Timeout : LongTimeout;
            if (conn.SecondsSinceReceive > limit)
            {
                conn.Close("timed out");
                OnConnectionLost("the other player stopped responding");
                return;
            }
            if (Time.unscaledTime >= nextPing && phase >= Phase.Handshake)
            {
                nextPing = Time.unscaledTime + PingInterval;
                Send(MsgType.Ping, null);
            }
        }

        private void PollGameState()
        {
            if (role == Role.Host && !GameBridge.InGame)
            {
                StopAll("The host left the world.", "host left the world", goToMenu: false);
                return;
            }

            if (role == Role.Guest)
            {
                if (phase == Phase.Loading && gameStartedFlag && GameBridge.InGame)
                {
                    phase = Phase.Playing;
                    status = "Playing in " + remoteName + "'s world.";
                    Send(MsgType.Ready, null);
                    BeginPlaying();
                    AddChat("You joined " + remoteName + "'s world.");
                }
                else if (phase == Phase.Playing && !GameBridge.InGame)
                {
                    // The guest quit to the main menu on their own.
                    UploadCharacter();
                    Disconnect("left the world", goToMenu: false);
                }
            }

            if (returnToMenuPending && GameBridge.InGame)
            {
                returnToMenuPending = false;
                try
                {
                    MainGame.Instance.GoToMenu(null, false, FadeFlag.Common);
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogError("GoToMenu failed: " + e);
                }
            }

            if (role == Role.None && Patch_SaveSystem_Save.BlockSaves && GameBridge.AtMainMenu && !returnToMenuPending)
                Patch_SaveSystem_Save.BlockSaves = false;
        }

        private void TickPlaying()
        {
            float now = Time.unscaledTime;
            if (!GameBridge.InGame)
                return;

            if (puppet == null && now >= nextPuppetAttempt)
            {
                nextPuppetAttempt = now + 3f;
                puppet = Puppet.Create(remoteName);
                if (puppet != null)
                    DontDestroyOnLoad(puppet.gameObject);
            }

            if (now >= nextState)
            {
                nextState = now + StateInterval;
                Vector3 pos = GameBridge.PlayerVisualPosition;
                Vector2 dir = GameBridge.PlayerDirection;
                int anim = GameBridge.PlayerAnimState;
                float[] layers = GameBridge.GetPlayerLayerWeights();
                GameBridge.GetOverheadIcons(overheadIcons);
                string carry = string.Join("|", overheadIcons.ToArray());
                bool changed = (pos - lastSentPos).sqrMagnitude > 0.0004f || anim != lastSentAnim || (dir - lastSentDir).sqrMagnitude > 0.0001f
                    || carry != lastSentCarry || LayersChanged(layers);
                if (changed || now >= nextStateHeartbeat)
                {
                    nextStateHeartbeat = now + StateHeartbeat;
                    lastSentPos = pos;
                    lastSentAnim = anim;
                    lastSentDir = dir;
                    lastSentCarry = carry;
                    lastSentLayers = layers;
                    string scene = GameBridge.PlayerSceneId;
                    Send(MsgType.PlayerState, w =>
                    {
                        w.WriteVec3(pos);
                        w.Write(dir.x);
                        w.Write(dir.y);
                        w.Write(anim);
                        w.WriteStr(scene);
                        w.Write((byte)Math.Min(layers.Length, 255));
                        for (int i = 0; i < layers.Length && i < 255; i++)
                            w.Write(layers[i]);
                        w.WriteStr(carry);
                    });
                }
            }

            if (role == Role.Host && CoopPlugin.SyncTime.Value && now >= nextTime)
            {
                nextTime = now + TimeInterval;
                GameBridge.GetTime(out int day, out float tod);
                Send(MsgType.Time, w =>
                {
                    w.Write(day);
                    w.Write(tod);
                });
            }

            if (role == Role.Guest && now >= nextCharacterUpload)
            {
                nextCharacterUpload = now + CharacterUploadInterval;
                UploadCharacter();
            }
        }

        private bool LayersChanged(float[] layers)
        {
            if (layers.Length != lastSentLayers.Length)
                return true;
            for (int i = 0; i < layers.Length; i++)
                if (Mathf.Abs(layers[i] - lastSentLayers[i]) > 0.02f)
                    return true;
            return false;
        }

        private void BeginPlaying()
        {
            WorldSync.Reset();
            WorldSync.Active = true;
            lastSentAnim = -1;
            lastSentCarry = null;
            nextCharacterUpload = Time.unscaledTime + CharacterUploadInterval;
        }

        // ================================================================ messages

        private void Handle(Packet p)
        {
            using (BinaryReader r = Protocol.Reader(p.Payload))
            {
                switch (p.Type)
                {
                    case MsgType.Ping:
                        break;
                    case MsgType.Hello:
                        OnHello(r);
                        break;
                    case MsgType.Welcome:
                        remoteName = r.ReadString();
                        phase = Phase.Transfer;
                        status = "Downloading " + remoteName + "'s world...";
                        break;
                    case MsgType.Refuse:
                        Fail("The host refused: " + r.ReadString());
                        break;
                    case MsgType.World:
                        OnWorld(r);
                        break;
                    case MsgType.Ready:
                        if (role == Role.Host)
                        {
                            phase = Phase.Playing;
                            status = remoteName + " is in your world.";
                            BeginPlaying();
                            AddChat(remoteName + " joined your world.");
                        }
                        break;
                    case MsgType.PlayerState:
                        OnPlayerState(r);
                        break;
                    case MsgType.Time:
                        int day = r.ReadInt32();
                        float tod = r.ReadSingle();
                        if (role == Role.Guest && phase == Phase.Playing && CoopPlugin.SyncTime.Value && GameBridge.InGame)
                            GameBridge.ApplyTime(day, tod);
                        break;
                    case MsgType.WgoUpsert:
                        if (phase == Phase.Playing)
                            WorldSync.ApplyUpsert(r);
                        break;
                    case MsgType.WgoRemove:
                        if (phase == Phase.Playing)
                            WorldSync.ApplyRemove(r);
                        break;
                    case MsgType.DropAdd:
                        if (phase == Phase.Playing)
                            DropSync.ApplyAdd(r);
                        break;
                    case MsgType.DropRemove:
                        if (phase == Phase.Playing)
                            DropSync.ApplyRemove(r);
                        break;
                    case MsgType.GuestCharacter:
                        if (role == Role.Host)
                            StoreGuestCharacter(r.ReadBlob());
                        break;
                    case MsgType.Chat:
                        AddChat(remoteName + ": " + r.ReadString());
                        break;
                    case MsgType.Bye:
                        string reason = r.ReadString();
                        conn?.Close(reason);
                        OnConnectionLost(reason);
                        break;
                }
            }
        }

        private void OnHello(BinaryReader r)
        {
            if (role != Role.Host || phase != Phase.Handshake)
                return;
            int protocol = r.ReadInt32();
            string modVersion = r.ReadString();
            string gameVersion = r.ReadString();
            string name = r.ReadString();

            string refuse = null;
            if (protocol != CoopPlugin.ProtocolVersion)
                refuse = "mod version mismatch (host " + CoopPlugin.Version + ", you " + modVersion + "). Install the same version of the mod.";
            else if (gameVersion != (Application.version ?? ""))
                refuse = "game version mismatch (host " + Application.version + ", you " + gameVersion + "). Update both games.";
            else if (!GameBridge.InGame)
                refuse = "the host is not in a world right now.";
            if (refuse != null)
            {
                SendFrame(Protocol.Build(MsgType.Refuse, w => w.Write(refuse)));
                conn.CloseAfterFlush("refused: " + refuse);
                conn = null;
                phase = Phase.None;
                status = "Refused a guest: " + refuse;
                return;
            }

            remoteName = string.IsNullOrEmpty(name) ? "Guest" : name;
            Send(MsgType.Welcome, w => w.Write(CoopPlugin.PlayerName.Value));
            status = "Sending your world to " + remoteName + "...";
            phase = Phase.Transfer;

            byte[] world;
            try
            {
                world = Protocol.Compress(GameSerializer.Serialize<GameSave>(MainGame.Instance.GameSave));
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogError("Could not package the world: " + e);
                SendFrame(Protocol.Build(MsgType.Refuse, w => w.Write("the host could not package the world (see host's BepInEx log).")));
                conn.CloseAfterFlush("world serialization failed");
                conn = null;
                phase = Phase.None;
                status = "Failed to send the world - see BepInEx/LogOutput.log.";
                return;
            }
            byte[] character = LoadGuestCharacter(remoteName);
            Vector3 hostPos = GameBridge.LocalPlayerData != null ? GameBridge.GetPlayerDataPosition(GameBridge.LocalPlayerData) : Vector3.zero;
            CoopPlugin.Log.LogInfo("Sending world to " + remoteName + ": " + world.Length / 1024 + " KB compressed, returning character: " + (character != null));
            Send(MsgType.World, w =>
            {
                w.WriteVec3(hostPos);
                w.WriteBlob(world);
                w.WriteBlob(character);
            });
            phase = Phase.Loading;
            status = remoteName + " is loading your world...";
        }

        private void OnWorld(BinaryReader r)
        {
            if (role != Role.Guest || phase != Phase.Transfer)
                return;
            Vector3 hostPos = r.ReadVec3();
            byte[] world = r.ReadBlob();
            byte[] character = r.ReadBlob();

            if (!GameBridge.AtMainMenu)
            {
                Fail("Go back to the main menu before joining.");
                return;
            }

            GameSave save;
            try
            {
                save = GameSerializer.Deserialize<GameSave>(Protocol.Decompress(world));
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogError("Could not read the host's world: " + e);
                Fail("Could not read the host's world (see BepInEx/LogOutput.log).");
                return;
            }
            if (save == null)
            {
                Fail("The host's world arrived empty.");
                return;
            }

            if (character != null)
            {
                try
                {
                    PlayerData mine = GameSerializer.Deserialize<PlayerData>(character);
                    if (mine != null)
                        save.playerData = mine;
                }
                catch (Exception e)
                {
                    CoopPlugin.Log.LogWarning("Could not restore your saved character, starting as a copy of the host's: " + e.Message);
                }
            }
            // Start next to the host.
            try
            {
                GameBridge.SetPlayerDataPosition(save.playerData, hostPos);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not move you next to the host: " + e.Message);
            }

            Patch_SaveSystem_Save.BlockSaves = true;
            gameStartedFlag = false;
            phase = Phase.Loading;
            status = "Loading " + remoteName + "'s world...";
            SetPanelOpen(false);
            try
            {
                var slot = new SaveSlotData { slotName = "gyk2coop_guest" };
                GameBridge.CloseMainMenu();
                MainGame.Instance.ContinueGame(slot, save);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogError("Loading the host's world failed: " + e);
                Fail("Loading the host's world failed (see BepInEx/LogOutput.log).");
            }
        }

        private void OnPlayerState(BinaryReader r)
        {
            Vector3 pos = r.ReadVec3();
            var dir = new Vector2(r.ReadSingle(), r.ReadSingle());
            int anim = r.ReadInt32();
            string scene = r.ReadString();
            int layerCount = r.ReadByte();
            var layers = new float[layerCount];
            for (int i = 0; i < layerCount; i++)
                layers[i] = r.ReadSingle();
            string carry = r.ReadString();
            if (phase == Phase.Playing && puppet != null)
            {
                puppet.PushState(pos, dir, anim, scene);
                puppet.SetLayerWeights(layers);
                puppet.SetCarry(carry);
            }
        }

        // ================================================================ guest character storage (host side)

        private static string Sanitize(string s)
        {
            var sb = new StringBuilder();
            foreach (char c in s ?? "")
                sb.Append(char.IsLetterOrDigit(c) || c == '-' || c == '_' ? c : '_');
            return sb.Length == 0 ? "_" : sb.ToString();
        }

        private static string GuestFile(string guestName)
        {
            string dir = Path.Combine(Path.Combine(Paths.ConfigPath, "GYK2Coop"), "guests");
            return Path.Combine(dir, Sanitize(GameBridge.HostSlotName) + "__" + Sanitize(guestName) + ".bin");
        }

        private byte[] LoadGuestCharacter(string guestName)
        {
            try
            {
                string f = GuestFile(guestName);
                return File.Exists(f) ? File.ReadAllBytes(f) : null;
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not read stored guest character: " + e.Message);
                return null;
            }
        }

        private void StoreGuestCharacter(byte[] data)
        {
            if (data == null || data.Length == 0 || string.IsNullOrEmpty(remoteName))
                return;
            try
            {
                string f = GuestFile(remoteName);
                Directory.CreateDirectory(Path.GetDirectoryName(f));
                string tmp = f + ".tmp";
                File.WriteAllBytes(tmp, data);
                if (File.Exists(f))
                    File.Delete(f);
                File.Move(tmp, f);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not store guest character: " + e.Message);
            }
        }

        private void UploadCharacter()
        {
            if (role != Role.Guest || conn == null || conn.IsClosed)
                return;
            PlayerData pd = GameBridge.LocalPlayerData;
            if (pd == null)
                return;
            try
            {
                byte[] b = GameSerializer.Serialize<PlayerData>(pd);
                Send(MsgType.GuestCharacter, w => w.WriteBlob(b));
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Could not send your character to the host: " + e.Message);
            }
        }

        // ================================================================ session control

        private void StartHosting()
        {
            if (!GameBridge.InGame)
            {
                status = "Load your save first, then host.";
                return;
            }
            if (!int.TryParse(portField, out int port) || port <= 0 || port > 65535)
            {
                status = "Port must be a number between 1 and 65535.";
                return;
            }
            SaveFields();
            try
            {
                listener = new Listener(port);
            }
            catch (Exception e)
            {
                status = "Could not open port " + port + ": " + e.Message;
                return;
            }
            role = Role.Host;
            phase = Phase.None;
            status = "Hosting on port " + port + ". Waiting for your guest...";
            CoopPlugin.Log.LogInfo("Hosting on port " + port);
        }

        private void StartJoining()
        {
            if (!GameBridge.AtMainMenu)
            {
                status = "Join from the main menu (quit your current game first).";
                return;
            }
            SaveFields();
            string host = addressField.Trim();
            int port = CoopPlugin.Port.Value;
            int colon = host.LastIndexOf(':');
            if (colon > 0 && host.IndexOf(':') == colon && int.TryParse(host.Substring(colon + 1), out int p))
            {
                port = p;
                host = host.Substring(0, colon);
            }
            if (host.Length == 0)
            {
                status = "Type the host's address first.";
                return;
            }
            role = Role.Guest;
            phase = Phase.Connecting;
            status = "Connecting to " + host + ":" + port + "...";
            lock (connectLock)
            {
                connectDone = false;
                connectResult = null;
                connectError = null;
            }
            Connection.ConnectAsync(host, port, (client, error) =>
            {
                lock (connectLock)
                {
                    connectResult = client;
                    connectError = error;
                    connectDone = true;
                }
            });
        }

        private void SaveFields()
        {
            CoopPlugin.PlayerName.Value = string.IsNullOrEmpty(nameField) ? "Keeper" : nameField.Trim();
            CoopPlugin.LastAddress.Value = addressField ?? "";
            if (int.TryParse(portField, out int port) && port > 0 && port <= 65535)
                CoopPlugin.Port.Value = port;
        }

        private void OnConnectionLost(string reason)
        {
            if (role == Role.Host)
            {
                string who = string.IsNullOrEmpty(remoteName) ? "The guest" : remoteName;
                if (phase == Phase.Playing || phase == Phase.Loading)
                    AddChat(who + " left (" + reason + ").");
                conn = null;
                phase = Phase.None;
                DestroyPuppet();
                WorldSync.Reset();
                remoteName = "";
                status = "Hosting. Waiting for your guest...";
                return;
            }
            if (role == Role.Guest)
            {
                bool wasInWorld = phase == Phase.Playing || phase == Phase.Loading;
                StopAll("Disconnected: " + reason, null, goToMenu: wasInWorld);
                if (wasInWorld)
                    AddChat("Disconnected from the host: " + reason);
            }
        }

        private void Fail(string message)
        {
            CoopPlugin.Log.LogWarning(message);
            StopAll(message, null, goToMenu: false);
        }

        private void Disconnect(string reason, bool goToMenu)
        {
            StopAll(role == Role.Host ? "Stopped hosting." : "Disconnected.", reason, goToMenu);
        }

        /// <summary>Ends hosting/joining. <paramref name="byeReason"/> is sent to the other side if non-null.</summary>
        private void StopAll(string newStatus, string byeReason, bool goToMenu)
        {
            if (conn != null)
            {
                if (byeReason != null && !conn.IsClosed)
                {
                    SendFrame(Protocol.Build(MsgType.Bye, w => w.Write(byeReason)));
                    conn.CloseAfterFlush(byeReason);
                }
                else
                {
                    conn.Close(byeReason ?? "stopped");
                }
                conn = null;
            }
            listener?.Stop();
            listener = null;
            DestroyPuppet();
            WorldSync.Reset();

            bool wasGuest = role == Role.Guest;
            role = Role.None;
            phase = Phase.None;
            remoteName = "";
            status = newStatus;

            // A guest that was in (or loading) the host's world is sent back to the main menu.
            // Saves stay blocked until it gets there (PollGameState lifts the block), so the
            // host's world never ends up in one of the guest's save slots.
            if (wasGuest && goToMenu)
                returnToMenuPending = true;
        }

        private void DestroyPuppet()
        {
            if (puppet != null)
            {
                Destroy(puppet.gameObject);
                puppet = null;
            }
        }

        private void Send(MsgType type, Action<BinaryWriter> write)
        {
            SendFrame(Protocol.Build(type, write));
        }

        private void SendFrame(byte[] frame)
        {
            if (conn != null && !conn.IsClosed)
                conn.Send(frame);
        }

        private void AddChat(string line)
        {
            CoopPlugin.Log.LogInfo("[chat] " + line);
            chat.Add(new KeyValuePair<float, string>(Time.unscaledTime, line));
            if (chat.Count > 50)
                chat.RemoveAt(0);
        }

        // ================================================================ UI

        private void SetPanelOpen(bool open)
        {
            panelOpen = open;
            // Typing into the panel should not walk the character around.
            if (GameBridge.InGame)
            {
                if (open && !controlTaken)
                {
                    GameBridge.SetPlayerControlByUI(false);
                    controlTaken = true;
                }
                else if (!open && controlTaken)
                {
                    GameBridge.SetPlayerControlByUI(true);
                    controlTaken = false;
                }
            }
            else
            {
                controlTaken = false;
            }
        }

        private void OnGUI()
        {
            if (tagStyle == null)
            {
                tagStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, fontSize = 14 };
                tagStyle.normal.textColor = new Color(1f, 0.92f, 0.6f);
                shadowStyle = new GUIStyle(tagStyle);
                shadowStyle.normal.textColor = new Color(0, 0, 0, 0.8f);
            }

            if (legacyInputBroken && Event.current.type == EventType.KeyDown && Event.current.keyCode == CoopPlugin.ToggleKey.Value)
            {
                SetPanelOpen(!panelOpen);
                Event.current.Use();
            }

            DrawNameTag();
            DrawOverlay();

            if (panelOpen)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                windowRect.height = 10;
                windowRect = GUILayout.Window(0x6B32C0, windowRect, DrawWindow, "Graveyard Keeper 2 Co-op  (" + CoopPlugin.ToggleKey.Value + " to close)");
            }
        }

        private void DrawNameTag()
        {
            if (puppet == null || !puppet.IsShown)
                return;
            Camera cam = GameBridge.WorldCamera;
            if (cam == null)
                return;
            Vector3 sp = cam.WorldToScreenPoint(puppet.HeadPosition);
            if (sp.z <= 0)
                return;
            var r = new Rect(sp.x - 100, Screen.height - sp.y - 12, 200, 24);
            GUI.Label(new Rect(r.x + 1, r.y + 1, r.width, r.height), puppet.DisplayName, shadowStyle);
            GUI.Label(r, puppet.DisplayName, tagStyle);
        }

        private void DrawOverlay()
        {
            float y = 6;
            if (role != Role.None && !panelOpen)
            {
                GUI.Label(new Rect(9, y + 1, 600, 22), "Co-op: " + status, shadowStyle == null ? GUI.skin.label : LeftShadow());
                GUI.Label(new Rect(8, y, 600, 22), "Co-op: " + status);
                y += 20;
            }
            if (panelOpen)
                return;
            float now = Time.unscaledTime;
            int shown = 0;
            for (int i = chat.Count - 1; i >= 0 && shown < 5; i--)
            {
                if (now - chat[i].Key > 12f)
                    break;
                shown++;
            }
            for (int i = chat.Count - shown; i < chat.Count; i++)
            {
                GUI.Label(new Rect(9, y + 1, 800, 22), chat[i].Value, LeftShadow());
                GUI.Label(new Rect(8, y, 800, 22), chat[i].Value);
                y += 20;
            }
        }

        private GUIStyle leftShadow;

        private GUIStyle LeftShadow()
        {
            if (leftShadow == null)
            {
                leftShadow = new GUIStyle(GUI.skin.label);
                leftShadow.normal.textColor = new Color(0, 0, 0, 0.85f);
            }
            return leftShadow;
        }

        private void DrawWindow(int id)
        {
            GUILayout.Label(status);
            GUILayout.Space(4);

            if (role == Role.None)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Your name", GUILayout.Width(80));
                nameField = GUILayout.TextField(nameField ?? "", 24);
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
                GUILayout.Label("<b>Host</b> - load your save, then:");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Port", GUILayout.Width(80));
                portField = GUILayout.TextField(portField ?? "", 5, GUILayout.Width(70));
                GUI.enabled = GameBridge.InGame;
                if (GUILayout.Button("Host this world"))
                    StartHosting();
                GUI.enabled = true;
                GUILayout.EndHorizontal();

                GUILayout.Space(6);
                GUILayout.Label("<b>Join</b> - from the main menu:");
                GUILayout.BeginHorizontal();
                GUILayout.Label("Address", GUILayout.Width(80));
                addressField = GUILayout.TextField(addressField ?? "", 64);
                GUI.enabled = GameBridge.AtMainMenu;
                if (GUILayout.Button("Join", GUILayout.Width(60)))
                    StartJoining();
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                GUILayout.Label("Address is the host's IP, optionally with :port (e.g. 100.64.1.2:7777).");
            }
            else if (role == Role.Host)
            {
                GUILayout.Label("Your guest connects to one of:");
                foreach (string ip in LocalAddresses())
                    GUILayout.Label("   " + ip + ":" + CoopPlugin.Port.Value);
                GUILayout.Label("Over the internet: use your public IP with the port forwarded, or a VPN like Tailscale / ZeroTier / Radmin VPN and its address.");
                DrawTransferProgress();
                if (GUILayout.Button("Stop hosting"))
                    Disconnect("host stopped hosting", goToMenu: false);
            }
            else
            {
                DrawTransferProgress();
                if (GUILayout.Button(phase == Phase.Playing ? "Leave (returns to main menu)" : "Cancel"))
                {
                    if (phase == Phase.Playing)
                        UploadCharacter();
                    Disconnect("guest left", goToMenu: phase == Phase.Playing || phase == Phase.Loading);
                }
            }

            if (phase == Phase.Playing)
            {
                GUILayout.Space(8);
                int start = Math.Max(0, chat.Count - 8);
                for (int i = start; i < chat.Count; i++)
                    GUILayout.Label(chat[i].Value);
                GUILayout.BeginHorizontal();
                bool enter = Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
                GUI.SetNextControlName("gyk2coop_chat");
                chatField = GUILayout.TextField(chatField ?? "", 200);
                if ((GUILayout.Button("Say", GUILayout.Width(50)) || (enter && GUI.GetNameOfFocusedControl() == "gyk2coop_chat")) && chatField.Trim().Length > 0)
                {
                    string msg = chatField.Trim();
                    chatField = "";
                    Send(MsgType.Chat, w => w.Write(msg));
                    AddChat(CoopPlugin.PlayerName.Value + ": " + msg);
                }
                GUILayout.EndHorizontal();
            }

            GUI.DragWindow();
        }

        private void DrawTransferProgress()
        {
            if (conn == null)
                return;
            long size = System.Threading.Interlocked.Read(ref conn.IncomingFrameSize);
            if (size > 64 * 1024)
            {
                long got = System.Threading.Interlocked.Read(ref conn.IncomingFrameReceived);
                GUILayout.Label("Receiving world: " + (got * 100 / size) + "%  (" + got / 1024 + " / " + size / 1024 + " KB)");
            }
            long pending = System.Threading.Interlocked.Read(ref conn.PendingOutgoingBytes);
            if (pending > 64 * 1024)
                GUILayout.Label("Sending: " + pending / 1024 + " KB left");
        }

        private static List<string> cachedAddresses;
        private static float addressesTime = -100;

        private static List<string> LocalAddresses()
        {
            if (cachedAddresses != null && Time.unscaledTime - addressesTime < 10)
                return cachedAddresses;
            addressesTime = Time.unscaledTime;
            var list = new List<string>();
            try
            {
                foreach (IPAddress a in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
                    if (a.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(a))
                        list.Add(a.ToString());
            }
            catch
            {
            }
            if (list.Count == 0)
                list.Add("(could not detect - check ipconfig)");
            cachedAddresses = list;
            return list;
        }
    }
}
