using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace GYK2Coop.Game
{
    /// <summary>
    /// The other player's character. A visual-only copy of the local player's sprite rig: no
    /// controller, physics, input, wisp or interaction components, so it can never act on the
    /// world. Driven purely by network snapshots.
    /// </summary>
    internal class Puppet : MonoBehaviour
    {
        private struct Snapshot
        {
            public float Time;
            public Vector3 Position;
        }

        // Animation states that only move sprites. Others (attacks, death, fishing...) have
        // gameplay logic in their animation events, so they are shown as idle/walk instead.
        private static readonly HashSet<int> SafeStates = new HashSet<int>
        {
            (int)AnimationState.Idle,
            (int)AnimationState.Walk,
            (int)AnimationState.Climbing,
            (int)AnimationState.FocusedWalk,
            (int)AnimationState.ToolAxe,
            (int)AnimationState.ToolShovel,
            (int)AnimationState.ToolPickaxe,
            (int)AnimationState.ToolHammer,
            (int)AnimationState.WorkHands,
            (int)AnimationState.Planting,
            (int)AnimationState.JumpPrepare,
            (int)AnimationState.Jump,
            (int)AnimationState.JumpLand,
        };

        private const float InterpolationDelay = 0.12f;
        private const float SnapDistance = 8f;

        private readonly List<Snapshot> snapshots = new List<Snapshot>();
        private PlayerAnimation anim;
        private Animator animator;
        private GameObject visual;
        private int currentState = -1;
        private Vector2 currentDirection;

        public string DisplayName;
        public string SceneId = "";
        public bool HasState;

        public Vector3 HeadPosition => transform.position + Vector3.up * 2.2f;
        public bool IsShown => visual != null && visual.activeSelf;

        public static Puppet Create(string displayName)
        {
            PlayerController local = GameBridge.Player;
            if (local == null || local.View == null)
                return null;

            var root = new GameObject("GYK2Coop_Puppet_" + displayName);
            Puppet puppet = root.AddComponent<Puppet>();
            puppet.DisplayName = displayName;

            // Clone under an inactive parent so none of the copied components run Awake/OnEnable
            // before we have stripped them.
            var holder = new GameObject("holder");
            holder.SetActive(false);
            holder.transform.SetParent(root.transform, false);

            GameObject clone;
            try
            {
                clone = Instantiate(local.View.gameObject, holder.transform);
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogError("Could not copy player visuals: " + e);
                Destroy(root);
                return null;
            }
            clone.name = "Visual";
            clone.transform.localPosition = Vector3.zero;
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localScale = local.View.transform.lossyScale;

            Strip(clone);

            puppet.visual = clone;
            puppet.anim = clone.GetComponent<PlayerAnimation>();
            puppet.animator = clone.GetComponentInChildren<Animator>(true);
            if (puppet.animator != null)
            {
                // Animation events call into gameplay code (stamina, attacks, sounds tied to the
                // local player). The puppet must not fire them.
                puppet.animator.fireEvents = false;
                puppet.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            }

            clone.transform.SetParent(root.transform, false);
            Destroy(holder);
            clone.SetActive(true);

            puppet.ApplySkin();
            root.transform.position = local.View.transform.position;
            puppet.visual.SetActive(false);
            return puppet;
        }

        private static void Strip(GameObject clone)
        {
            // Destroy behaviours in passes because some have [RequireComponent] dependencies.
            for (int pass = 0; pass < 4; pass++)
            {
                bool any = false;
                foreach (Component c in clone.GetComponentsInChildren<Component>(true))
                {
                    if (c == null || c is Transform || c is Renderer || c is MeshFilter || c is Animator)
                        continue;
                    if (c is AnimationComponentBase || c is DropViewAtomMesh)
                        continue;
                    if (c is Behaviour || c is Collider || c is Rigidbody || c is Joint || c is AudioSource
                        || c.GetType().Name.Contains("Collider") || c.GetType().Name.Contains("Rigidbody"))
                    {
                        if (c is Behaviour b && IsRenderingHelper(b))
                            continue;
                        try
                        {
                            DestroyImmediate(c);
                            any = true;
                        }
                        catch
                        {
                        }
                    }
                }
                if (!any)
                    break;
            }

            // Wisp, banner, fishing line, sermon icon etc. live in child objects; hide anything
            // whose behaviour we just removed but whose renderer would show a frozen leftover.
            foreach (Transform t in clone.GetComponentsInChildren<Transform>(true))
            {
                string n = t.name.ToLowerInvariant();
                if (n.Contains("wisp") || n.Contains("banner") || n.Contains("fishing") || n.Contains("sermon") || n.Contains("bubble"))
                    t.gameObject.SetActive(false);
            }
        }

        private static bool IsRenderingHelper(Behaviour b)
        {
            // Sorting groups decide draw order of the layered sprites; keep them.
            string n = b.GetType().Name;
            return n == "SortingGroup";
        }

        private void ApplySkin()
        {
            if (anim == null)
                return;
            try
            {
                // SkinPresetGK2 derives from a LazyBearTechnology type, so go through reflection.
                object preset = AccessTools.Property(typeof(PlayerSkinHelper), "CurrentPreset")?.GetValue(null, null);
                if (preset != null && !(preset is UnityEngine.Object uo && uo == null))
                {
                    CallWithArg(anim, "Init", preset);
                    CallWithArg(anim, "ChangeSkinPreset", preset);
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Puppet skin setup failed (it may look like the default keeper): " + e.Message);
            }
        }

        public void PushState(Vector3 position, Vector2 direction, int animState, string sceneId)
        {
            HasState = true;
            SceneId = sceneId ?? "";
            float now = Time.unscaledTime;
            if (snapshots.Count > 0 && (snapshots[snapshots.Count - 1].Position - position).sqrMagnitude > SnapDistance * SnapDistance)
                snapshots.Clear();
            snapshots.Add(new Snapshot { Time = now, Position = position });
            if (snapshots.Count > 30)
                snapshots.RemoveAt(0);

            if (animState != currentState)
            {
                currentState = animState;
                int shown = SafeStates.Contains(animState) ? animState : (int)AnimationState.Idle;
                if (animator != null && animator.isActiveAndEnabled)
                    animator.SetInteger(AnimationComponentBase.idStateAnimator, shown);
            }
            if (direction.sqrMagnitude > 0.0001f && (direction - currentDirection).sqrMagnitude > 0.0001f)
            {
                currentDirection = direction;
                try
                {
                    anim?.SetDirection(direction.normalized);
                }
                catch
                {
                }
            }
        }

        private void Update()
        {
            if (visual == null)
                return;

            bool visible = HasState && SceneLoadedLocally();
            if (visual.activeSelf != visible)
            {
                visual.SetActive(visible);
                if (visible)
                {
                    // Re-apply after re-enable; the animator resets its parameters.
                    int s = currentState;
                    currentState = -1;
                    Vector2 d = currentDirection;
                    currentDirection = Vector2.zero;
                    if (snapshots.Count > 0)
                        PushState(snapshots[snapshots.Count - 1].Position, d, s, SceneId);
                }
            }
            if (!visible || snapshots.Count == 0)
                return;

            float renderTime = Time.unscaledTime - InterpolationDelay;
            Vector3 pos = snapshots[snapshots.Count - 1].Position;
            for (int i = snapshots.Count - 1; i > 0; i--)
            {
                Snapshot a = snapshots[i - 1];
                Snapshot b = snapshots[i];
                if (a.Time <= renderTime && renderTime <= b.Time)
                {
                    float t = b.Time > a.Time ? (renderTime - a.Time) / (b.Time - a.Time) : 1f;
                    pos = Vector3.Lerp(a.Position, b.Position, t);
                    break;
                }
            }
            if (snapshots.Count > 0 && renderTime < snapshots[0].Time)
                pos = snapshots[0].Position;
            transform.position = pos;
        }

        private bool SceneLoadedLocally()
        {
            if (string.IsNullOrEmpty(SceneId) || SceneId == GameBridge.PlayerSceneId)
                return true;
            List<string> loaded = GameBridge.LoadedSceneIds;
            return loaded == null || loaded.Count == 0 || loaded.Contains(SceneId);
        }

        private void OnDestroy()
        {
            // AnimationComponent.OnDestroy releases its skin preset asset; it is shared with the
            // local player, so make sure the puppet releases nothing.
            try
            {
                if (anim != null)
                    AccessTools.Field(typeof(AnimationComponentBase), "skinPreset")?.SetValue(anim, null);
            }
            catch
            {
            }
        }

        /// <summary>Calls the instance method <paramref name="name"/> whose single parameter accepts <paramref name="arg"/>.</summary>
        private static void CallWithArg(object target, string name, object arg)
        {
            for (Type t = target.GetType(); t != null; t = t.BaseType)
            {
                foreach (MethodInfo m in t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (m.Name != name)
                        continue;
                    ParameterInfo[] ps = m.GetParameters();
                    if (ps.Length == 1 && ps[0].ParameterType.IsInstanceOfType(arg))
                    {
                        m.Invoke(target, new[] { arg });
                        return;
                    }
                }
            }
            CoopPlugin.Log.LogWarning("Puppet: method " + name + " not found");
        }
    }
}
