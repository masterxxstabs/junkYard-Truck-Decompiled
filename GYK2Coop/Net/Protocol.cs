using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;

namespace GYK2Coop.Net
{
    internal enum MsgType : byte
    {
        Hello = 1,          // guest -> host: protocol, mod version, game version, name
        Welcome = 2,        // host -> guest: host name
        Refuse = 3,         // host -> guest: reason
        World = 4,          // host -> guest: full game save + guest character
        Ready = 5,          // guest -> host: guest finished loading
        PlayerState = 6,    // both: position, direction, animation, scene
        Time = 7,           // host -> guest: day + time of day
        WgoAdd = 8,         // both: a new world object appeared near the player who made it
        WgoRemove = 9,      // both: a world object was removed
        GuestCharacter = 10,// guest -> host: guest's PlayerData for safe keeping
        Chat = 11,          // both
        Ping = 12,          // both
        Bye = 13,           // both: clean disconnect, reason
        DropAdd = 14,       // both: an item was put on the ground (bodies, crates, loot)
        DropRemove = 15,    // both: an item on the ground was picked up / removed
        WgoChange = 16,     // both: an object changed in place (empty grave -> grave with body) + its contents
        WgoItems = 17,      // both: contents of an object a player is using changed
        QuestState = 18,    // host -> guest: quest statuses + objective arrow target
    }

    internal struct Packet
    {
        public MsgType Type;
        public byte[] Payload;
    }

    internal static class Protocol
    {
        // Frame: int32 length (type byte + payload), byte type, payload.
        public const int MaxFrame = 512 * 1024 * 1024;

        public static byte[] Build(MsgType type, Action<BinaryWriter> write)
        {
            using (var ms = new MemoryStream())
            using (var w = new BinaryWriter(ms))
            {
                w.Write(0);
                w.Write((byte)type);
                write?.Invoke(w);
                w.Flush();
                byte[] buf = ms.ToArray();
                int len = buf.Length - 4;
                buf[0] = (byte)len;
                buf[1] = (byte)(len >> 8);
                buf[2] = (byte)(len >> 16);
                buf[3] = (byte)(len >> 24);
                return buf;
            }
        }

        public static BinaryReader Reader(byte[] payload)
        {
            return new BinaryReader(new MemoryStream(payload, false));
        }

        public static void WriteVec3(this BinaryWriter w, Vector3 v)
        {
            w.Write(v.x);
            w.Write(v.y);
            w.Write(v.z);
        }

        public static Vector3 ReadVec3(this BinaryReader r)
        {
            return new Vector3(r.ReadSingle(), r.ReadSingle(), r.ReadSingle());
        }

        public static void WriteBlob(this BinaryWriter w, byte[] data)
        {
            if (data == null)
            {
                w.Write(-1);
                return;
            }
            w.Write(data.Length);
            w.Write(data);
        }

        public static byte[] ReadBlob(this BinaryReader r)
        {
            int n = r.ReadInt32();
            if (n < 0)
                return null;
            return r.ReadBytes(n);
        }

        public static void WriteStr(this BinaryWriter w, string s)
        {
            w.Write(s ?? "");
        }

        public static byte[] Compress(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress))
                    gz.Write(data, 0, data.Length);
                return ms.ToArray();
            }
        }

        public static byte[] Decompress(byte[] data)
        {
            using (var input = new MemoryStream(data))
            using (var gz = new GZipStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                gz.CopyTo(output);
                return output.ToArray();
            }
        }
    }
}
