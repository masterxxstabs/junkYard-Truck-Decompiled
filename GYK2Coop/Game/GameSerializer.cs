using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace GYK2Coop.Game
{
    /// <summary>
    /// Serializes game data with the exact serializer the game uses for its save files
    /// (SaveSystem.OdinBinaryFileSerializer), falling back to Odin's SerializationUtility for
    /// types that serializer refuses. Reached by reflection because both live outside
    /// Assembly-CSharp.
    /// </summary>
    internal static class GameSerializer
    {
        private static bool initialized;
        private static object fileSerializer;
        private static MethodInfo fileSerialize;
        private static MethodInfo fileDeserialize;

        private static MethodInfo odinSerializeValue;
        private static MethodInfo odinDeserializeValue;
        private static object odinBinaryFormat;

        private static readonly Dictionary<Type, Func<object, byte[]>> serializers = new Dictionary<Type, Func<object, byte[]>>();
        private static readonly Dictionary<Type, Func<byte[], object>> deserializers = new Dictionary<Type, Func<byte[], object>>();

        private static void Init()
        {
            if (initialized)
                return;
            initialized = true;

            try
            {
                PropertyInfo p = AccessTools.Property(typeof(SaveSystem), "OdinBinaryFileSerializer");
                fileSerializer = p?.GetValue(null, null);
                if (fileSerializer != null)
                {
                    MethodInfo[] methods = fileSerializer.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public);
                    fileSerialize = methods.FirstOrDefault(m => m.Name == "Serialize" && m.IsGenericMethodDefinition
                        && m.GetParameters().Length == 1 && m.ReturnType == typeof(byte[]));
                    fileDeserialize = methods.FirstOrDefault(m => m.Name == "Deserialize" && m.IsGenericMethodDefinition
                        && m.GetParameters().Length == 1 && m.GetParameters()[0].ParameterType == typeof(byte[]));
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Save serializer lookup failed: " + e.Message);
            }

            try
            {
                Type util = AccessTools.TypeByName("Sirenix.Serialization.SerializationUtility");
                Type format = AccessTools.TypeByName("Sirenix.Serialization.DataFormat");
                if (util != null && format != null)
                {
                    odinBinaryFormat = Enum.Parse(format, "Binary");
                    MethodInfo[] methods = util.GetMethods(BindingFlags.Static | BindingFlags.Public);
                    odinSerializeValue = methods.FirstOrDefault(m =>
                    {
                        if (m.Name != "SerializeValue" || !m.IsGenericMethodDefinition || m.ReturnType != typeof(byte[]))
                            return false;
                        ParameterInfo[] ps = m.GetParameters();
                        return ps.Length == 3 && ps[1].ParameterType == format && !ps[2].ParameterType.IsByRef;
                    });
                    odinDeserializeValue = methods.FirstOrDefault(m =>
                    {
                        if (m.Name != "DeserializeValue" || !m.IsGenericMethodDefinition)
                            return false;
                        ParameterInfo[] ps = m.GetParameters();
                        return ps.Length == 3 && ps[0].ParameterType == typeof(byte[]) && ps[1].ParameterType == format
                            && !ps[2].ParameterType.IsGenericType;
                    });
                }
            }
            catch (Exception e)
            {
                CoopPlugin.Log.LogWarning("Odin serializer lookup failed: " + e.Message);
            }

            if (fileSerialize == null && odinSerializeValue == null)
                CoopPlugin.Log.LogError("No usable serializer found - world transfer will not work with this game version.");
        }

        public static byte[] Serialize<T>(T value) where T : class
        {
            Init();
            Type t = typeof(T);
            if (!serializers.TryGetValue(t, out Func<object, byte[]> fn))
            {
                fn = BuildSerializer(t);
                serializers[t] = fn;
            }
            if (fn == null)
                throw new InvalidOperationException("No serializer for " + t.Name);
            return fn(value);
        }

        public static T Deserialize<T>(byte[] bytes) where T : class
        {
            Init();
            Type t = typeof(T);
            if (!deserializers.TryGetValue(t, out Func<byte[], object> fn))
            {
                fn = BuildDeserializer(t);
                deserializers[t] = fn;
            }
            if (fn == null)
                throw new InvalidOperationException("No deserializer for " + t.Name);
            return fn(bytes) as T;
        }

        private static Func<object, byte[]> BuildSerializer(Type t)
        {
            if (fileSerialize != null)
            {
                try
                {
                    MethodInfo m = fileSerialize.MakeGenericMethod(t);
                    return o => (byte[])Invoke(m, fileSerializer, o);
                }
                catch (ArgumentException)
                {
                    // Generic constraint not satisfied for this type; fall through to Odin.
                }
            }
            if (odinSerializeValue != null)
            {
                MethodInfo m = odinSerializeValue.MakeGenericMethod(t);
                return o => (byte[])Invoke(m, null, o, odinBinaryFormat, null);
            }
            return null;
        }

        private static Func<byte[], object> BuildDeserializer(Type t)
        {
            if (fileDeserialize != null)
            {
                try
                {
                    MethodInfo m = fileDeserialize.MakeGenericMethod(t);
                    return b => Invoke(m, fileSerializer, b);
                }
                catch (ArgumentException)
                {
                }
            }
            if (odinDeserializeValue != null)
            {
                MethodInfo m = odinDeserializeValue.MakeGenericMethod(t);
                return b => Invoke(m, null, b, odinBinaryFormat, null);
            }
            return null;
        }

        private static object Invoke(MethodInfo m, object target, params object[] args)
        {
            try
            {
                return m.Invoke(target, args);
            }
            catch (TargetInvocationException e) when (e.InnerException != null)
            {
                throw e.InnerException;
            }
        }
    }
}
