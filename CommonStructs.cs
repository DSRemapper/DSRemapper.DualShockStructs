using System.Collections;
using DSRemapper.Types;
using DSRemapper.Core.Types;
using System.Runtime.InteropServices;
using System.Linq;

namespace DSRemapper.DualCommon
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct FingerStatus
    {
        private BitVector<uint> fingerData = new();

        private static readonly BitVector<uint>.Section fingerId = BitVector<uint>.CreateSection(0x7F);
        private static readonly BitVector<uint>.Section fingerX = BitVector<uint>.CreateSection(0xFFF, 8);
        private static readonly BitVector<uint>.Section fingerY = BitVector<uint>.CreateSection(0xFFF, fingerX);

        public byte FingerId { get => (byte)fingerData[fingerId]; set => fingerData[fingerId] = value; }
        public bool FingerTouch { get => !fingerData[0x80]; set => fingerData[0x80] = !value; }
        public short FingerX { get => (short)fingerData[fingerX]; set => fingerData[fingerX] = (uint)value; }
        public short FingerY { get => (short)fingerData[fingerY]; set => fingerData[fingerY] = (uint)value; }
        public FingerStatus() { }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TouchStatus
    {
        private byte timestamp = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private FingerStatus[] fingers = new FingerStatus[2];

        public byte Timestamp { get => timestamp; set => timestamp = value; }
        public FingerStatus[] Fingers { get => fingers; set => fingers = value; }
        public TouchStatus() { }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TouchStatusSense
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        private FingerStatus[] fingers = new FingerStatus[2];
        private byte timestamp = 0;

        public byte Timestamp { get => timestamp; set => timestamp = value; }
        public FingerStatus[] Fingers { get => fingers; set => fingers = value; }
        public TouchStatusSense() { }
    }
    public enum EffectTypes : byte
    {
        Off = 0x05, // 00 00 0 101
        Feedback = 0x21, // 00 10 0 001
        Weapon = 0x25, // 00 10 0 101
        Vibration = 0x26, // 00 10 0 110

        Bow = 0x22, // 00 10 0 010
        Galloping = 0x23, // 00 10 0 011
        Machine = 0x27, // 00 10 0 111

        Simple_Feedback = 0x01, // 00 00 0 001
        Simple_Weapon = 0x02, // 00 00 0 010
        Simple_Vibration = 0x06, // 00 00 0 110

        Limited_Feedback = 0x11, // 00 01 0 001
        Limited_Weapon = 0x12, // 00 01 0 010
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TriggerEffect : IDSRFeedback, IEnumerable<byte>
    {
        private EffectTypes effectType = EffectTypes.Off;
        private BitVector<ushort> active = new();
        private BitVector<ulong> force = new();
        public ushort EffectType
        {
            get
            {
                return RawEffectType switch
                {
                    EffectTypes.Off => 0,
                    EffectTypes.Feedback => 1,
                    EffectTypes.Weapon => 2,
                    EffectTypes.Vibration => 3,
                    EffectTypes.Bow => 4,
                    EffectTypes.Galloping => 5,
                    EffectTypes.Machine => 6,
                    EffectTypes.Simple_Feedback => 7,
                    EffectTypes.Simple_Weapon => 8,
                    EffectTypes.Simple_Vibration => 9,
                    EffectTypes.Limited_Feedback => 10,
                    EffectTypes.Limited_Weapon => 11,
                    _ => 0,
                };
            }
            set
            {
                RawEffectType = value switch
                {
                    0 => EffectTypes.Off,
                    1 => EffectTypes.Feedback,
                    2 => EffectTypes.Weapon,
                    3 => EffectTypes.Vibration,
                    /*4 => EffectTypes.Bow,
                    5 => EffectTypes.Galloping,
                    6 => EffectTypes.Machine,
                    7 => EffectTypes.Simple_Feedback,
                    8 => EffectTypes.Simple_Weapon,
                    9 => EffectTypes.Simple_Vibration,
                    10 => EffectTypes.Limited_Feedback,
                    11 => EffectTypes.Limited_Weapon,*/
                    _ => EffectTypes.Off,
                };
            }
        }
        public byte[] EffectFlags { get => BitConverter.GetBytes(active.Data); set => active.Data = BitConverter.ToUInt16(value); }
        public float Frequency { get => RawFrecuency; set => RawFrecuency = (ushort)value; }
        public float[] Strengths
        {
            get => [.. this.Select(s => s / 8f)];
            set
            {
                for (byte i = 0; i < 15 && i < value.Length; i++)
                    this[i] = (byte)(value[i] * 8);
            }
        }
        public byte Presets { get => 0; set
            {
                if (value > 0)
                {
                    this = value switch
                    {
                        2 => FullPress,
                        3 => Pulse,
                        4 => Rigid,
                        5 => RigidMid,
                        6 => RigidHard,
                        7 => SimpleFullPress,
                        8 => SimplePulse,
                        _ => Off,
                    };
                }
            }
        }
        public EffectTypes RawEffectType { get => effectType; set => effectType = value; }
        /// <summary>
        /// Sets the bitmask for the active zones.
        /// </summary>
        /// <returns>A bitmask for the active zones</returns>
        public ushort Active { get => active.Data; set => active.Data = value; }
        /// <summary>
        /// Sets strength only for Weapon effect. Ranges from 0 to 7.
        /// </summary>
        /// <returns>The strength of the effect</returns>
        public ulong Strength { get => force[0xFFFFFFFFFFFF, 0]; set => force[0xFFFFFFFFFFFF, 0] = value; }
        /// <summary>
        /// Sets the frequency for the Vibration effect.
        /// </summary>
        /// <returns>The frequency in Hz</returns>
        public ushort RawFrecuency { get => (ushort)force[0xFFFF, 48]; set => force[0xFFFF, 48] = value; }
        /// <summary>
        /// Access the feedback zones using an index between 0 and 15.
        /// </summary>
        /// <param name="i">Zone to set/get</param>
        /// <returns>A value between 0 and 8; where 0 is off, 1 is the minimun strength and 8 is the maximum strength.</returns>
        public byte this[byte i]
        {
            get => (byte)(active[(ushort)(1 << (i % 16))] ? (byte)force[0x7, (byte)(i % 16 * 3)] + 1 : 0);
            set
            {
                active[(ushort)(1 << (i % 16))] = value > 0;
                force[0x7, (byte)(i % 16 * 3)] = (byte)(value > 0 ? value - 1 : 0);
            }
        }
        public TriggerEffect() { }
        public TriggerEffect(byte strength)
        {
            RawEffectType = EffectTypes.Feedback;
            RawFrecuency = 0;
            for (byte i = 0; i < 15; i++)
                this[i] = strength;
        }

        public IEnumerator<byte> GetEnumerator()
        {
            for (byte i = 0; i < 15; i++)
                yield return this[i];
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static TriggerEffect Off => new();
        public static TriggerEffect SimpleFullPress => new()
        {
            RawEffectType = EffectTypes.Simple_Weapon,
            RawFrecuency = 0,
            [4] = 1,
            [7] = 1,
            [13] = 1,
            [15] = 1,
            Strength = 0xFF,
        };
        public static TriggerEffect FullPress => new()
        {
            RawEffectType = EffectTypes.Weapon,
            RawFrecuency = 0,
            [5] = 1,
            [8] = 1,
            Strength = 0xFF,
        };
        public static TriggerEffect Rigid => new(1);
        public static TriggerEffect RigidMid => new(4);
        public static TriggerEffect RigidHard => new(8);
        public static TriggerEffect Pulse => new()
        {
            RawEffectType = EffectTypes.Feedback,
            RawFrecuency = 0,
            [0] = 8,
        };
        public static TriggerEffect SimplePulse => new()
        {
            RawEffectType = EffectTypes.Simple_Weapon,
            RawFrecuency = 0,
            Strength = 0,
        };
    }
    public static class StructExtensions
    {
        public static float ToFloat(this byte axis) => axis / 255f;
        public static float ToFloat(this sbyte axis) => axis / (axis < 0 ? 128f : 127f);
        public static byte ToByte(this float axis) => (byte)(axis * 255);
        public static sbyte ToSByte(this float axis) => (sbyte)(axis * (axis < 0 ? 128f : 127f));
        public static sbyte AxisConvertion(this byte b) => (sbyte)(b ^ 0x80);
        public static byte AxisConvertion(this sbyte b) => (byte)AxisConvertion((byte)b);
    }
}