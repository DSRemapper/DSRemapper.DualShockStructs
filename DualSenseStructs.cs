using DSRemapper.Core.Types;
using System.Runtime.InteropServices;
using DSRemapper.DualCommon;

namespace DSRemapper.DualSense
{
    #region Input
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct USBReport
    {
        public byte ReportId = 0x02;
        public InState State = new();

        public USBReport() { }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BTReport
    {
        public byte ReportId = 0x31;
        public BitVector<byte> misc = new(0x02);
        public InState State;
        private byte unk1 = 0;
        public byte CRCFailCount = 0;
        private byte unk2 = 0;
        private byte unk3 = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        private byte[] pad = [];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CRC = [];

        public BTReport() { }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 63)]
    public struct InState
    {
        private byte lx = 0;
        private byte ly = 0;
        private byte rx = 0;
        private byte ry = 0;
        private byte lTrigger = 0;
        private byte rTrigger = 0;
        private byte secNumber = 0;
        private BitVector<uint> buttons = new();
        private uint unkCounter = 0;
        private short angularVelocityX = 0;
        private short angularVelocityY = 0;
        private short angularVelocityZ = 0;
        private short accelerometerX = 0;
        private short accelerometerY = 0;
        private short accelerometerZ = 0;
        private uint timestamp = 0;
        private byte temperature = 0;
        private TouchStatusSense touchStatus = new();
        private BitVector<ushort> triggerStatus = new();
        private uint hostTimestamp = 0;
        private BitVector<byte> triggerEffect = new();
        private uint deviceTimestamp = 0;
        private BitVector<ushort> power = new();
        private BitVector<byte> misc = new();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] AesCmac = [];

        public sbyte LX { get => lx.AxisConvertion(); set => lx = value.AxisConvertion(); }
        public sbyte LY { get => ly.AxisConvertion(); set => ly = value.AxisConvertion(); }
        public sbyte RX { get => rx.AxisConvertion(); set => rx = value.AxisConvertion(); }
        public sbyte RY { get => ry.AxisConvertion(); set => ry = value.AxisConvertion(); }
        public byte LTrigger { get => lTrigger; set => lTrigger = value; }
        public byte RTrigger { get => rTrigger; set => rTrigger = value; }

        public byte DPad { get => (byte)buttons[0x0000000F, 0]; set => buttons[0x0000000F, 0] = value; }
        public bool Square { get => buttons[0x00000010]; set => buttons[0x00000010] = value; }
        public bool Cross { get => buttons[0x00000020]; set => buttons[0x00000020] = value; }
        public bool Circle { get => buttons[0x00000040]; set => buttons[0x00000040] = value; }
        public bool Triangle { get => buttons[0x00000080]; set => buttons[0x00000080] = value; }
        public bool L1 { get => buttons[0x00000100]; set => buttons[0x00000100] = value; }
        public bool R1 { get => buttons[0x00000200]; set => buttons[0x00000200] = value; }
        public bool L2 { get => buttons[0x00000400]; set => buttons[0x00000400] = value; }
        public bool R2 { get => buttons[0x00000800]; set => buttons[0x00000800] = value; }
        public bool Create { get => buttons[0x00001000]; set => buttons[0x00001000] = value; }
        public bool Options { get => buttons[0x00002000]; set => buttons[0x00002000] = value; }
        public bool L3 { get => buttons[0x00004000]; set => buttons[0x00004000] = value; }
        public bool R3 { get => buttons[0x00008000]; set => buttons[0x00008000] = value; }
        public bool PS { get => buttons[0x00010000]; set => buttons[0x00010000] = value; }
        public bool TPad { get => buttons[0x00020000]; set => buttons[0x00020000] = value; }
        public bool Mute { get => buttons[0x00040000]; set => buttons[0x00040000] = value; }
        public bool LFunc { get => buttons[0x00100000]; set => buttons[0x00100000] = value; }
        public bool RFunc { get => buttons[0x00200000]; set => buttons[0x00200000] = value; }
        public bool LPad { get => buttons[0x00400000]; set => buttons[0x00400000] = value; }
        public bool RPad { get => buttons[0x00800000]; set => buttons[0x00800000] = value; }
        public uint Timestamp { get => timestamp; set => timestamp = value; }
        public byte Temperature { get => temperature; set => temperature = value; }
        public short AngularVelocityX { get => angularVelocityX; set => angularVelocityX = value; }
        public short AngularVelocityY { get => angularVelocityY; set => angularVelocityY = value; }
        public short AngularVelocityZ { get => angularVelocityZ; set => angularVelocityZ = value; }
        public short AccelerometerX { get => accelerometerX; set => accelerometerX = value; }
        public short AccelerometerY { get => accelerometerY; set => accelerometerY = value; }
        public short AccelerometerZ { get => accelerometerZ; set => accelerometerZ = value; }
        public TouchStatusSense Touch { get => touchStatus; set => touchStatus = value; }
        public byte RTriggerStop { get => (byte)triggerStatus[0xF, 0]; set => triggerStatus[0xF, 0] = value; }
        public byte RTriggerStatus { get => (byte)triggerStatus[0xF, 4]; set => triggerStatus[0xF, 4] = value; }
        public byte LTriggerStop { get => (byte)triggerStatus[0xF, 8]; set => triggerStatus[0xF, 8] = value; }
        public byte LTriggerStatus { get => (byte)triggerStatus[0xF, 12]; set => triggerStatus[0xF, 12] = value; }
        public byte RTriggerEffect { get => triggerEffect[0xF, 0]; set => triggerEffect[0xF, 0] = value; }
        public byte LTriggerEffect { get => triggerEffect[0xF, 4]; set => triggerEffect[0xF, 4] = value; }
        public byte Battery { get => (byte)power[0xF, 0]; set => power[0xF, 0] = value; }
        public byte BatteryState { get => (byte)power[0xF, 4]; set => power[0xF, 4] = value; }
        public bool Headphone { get => power[0x0100]; set => power[0x0100] = value; }
        public bool Mic { get => power[0x0200]; set => power[0x0200] = value; }
        public bool Muted { get => power[0x0400]; set => power[0x0400] = value; }
        public bool UsbData { get => power[0x0800]; set => power[0x0800] = value; }
        public bool UsbPower { get => power[0x1000]; set => power[0x1000] = value; }
        public bool UsbBT { get => power[0x2000]; set => power[0x2000] = value; }
        public bool Dock { get => power[0x4000]; set => power[0x4000] = value; }
        public bool ExtMic { get => misc[0x01]; set => misc[0x01] = value; }
        public bool HapticLowPassFilter { get => misc[0x02]; set => misc[0x02] = value; }
        public InState() { }
    }
    #endregion Input

    #region Output
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct USBOutReport
    {
        public byte ReportId = 0;
        public OutState State = new();

        public USBOutReport() { }
        public byte[] ToArray()
        {
            byte[] raw = new byte[48];
            GCHandle ptr = GCHandle.Alloc(raw, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, ptr.AddrOfPinnedObject(), false);
            ptr.Free();
            return raw;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]//, Size = 78
    public struct BTOutReport
    {
        public byte ReportId = 0;
        public BitVector<byte> misc = new();
        public OutState State = new();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 25)]
        private byte[] pad = new byte[25];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        byte[] CRC = new byte[4];

        public BTOutReport() { }
        public byte[] ToArray()
        {
            byte[] raw = new byte[74];
            GCHandle ptr = GCHandle.Alloc(raw, GCHandleType.Pinned);
            Marshal.StructureToPtr(this, ptr.AddrOfPinnedObject(), false);
            ptr.Free();
            return raw;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 47)]
    public struct OutState
    {
        BitVector<ushort> config1 = new();
        byte rumbleRight = 0;
        byte rumbleLeft = 0;
        byte volumeHeadphone = 0;
        byte volumeSpeaker = 0;
        byte volumeMic = 0;
        BitVector<byte> micConfig = new();
        byte muteLightMode = 0;
        BitVector<byte> powerConfig = new();
        public TriggerEffect RTriggerFFB = new();
        public TriggerEffect LTriggerFFB = new();
        uint hostTimestamp = 0;
        BitVector<uint> config2 = new();
        byte unk = 0;
        byte lightFade = 0;
        byte lightBrightness = 0;
        BitVector<byte> playerLight = new();
        byte red = 0;
        byte green = 0;
        byte blue = 0;

        public bool EnableRumbleEmulation { get => config1[0x0001]; set => config1[0x0001] = value; }
        public bool UseRumbleNotHaptics { get => config1[0x0002]; set => config1[0x0002] = value; }
        public bool AllowRightTriggerFeedback { get => config1[0x0004]; set => config1[0x0004] = value; }
        public bool AllowLeftTriggerFeedback { get => config1[0x0008]; set => config1[0x0008] = value; }
        public bool AllowHeadphoneVolume { get => config1[0x0010]; set => config1[0x0010] = value; }
        public bool AllowSpeakerVolume { get => config1[0x0020]; set => config1[0x0020] = value; }
        public bool AllowMicVolume { get => config1[0x0040]; set => config1[0x0040] = value; }
        public bool AllowAudioControl { get => config1[0x0080]; set => config1[0x0080] = value; }
        public bool AllowMuteLight { get => config1[0x0100]; set => config1[0x0100] = value; }
        public bool AllowAudioMute { get => config1[0x0200]; set => config1[0x0200] = value; }
        public bool AllowLedColor { get => config1[0x0400]; set => config1[0x0400] = value; }
        public bool ResetLights { get => config1[0x0800]; set => config1[0x0800] = value; }
        public bool AllowPlayerLights { get => config1[0x1000]; set => config1[0x1000] = value; }
        public bool AllowHapticLowPassFilter { get => config1[0x2000]; set => config1[0x2000] = value; }
        public bool AllowMotorPowerLevel { get => config1[0x4000]; set => config1[0x4000] = value; }
        public bool AllowAudioControl2 { get => config1[0x8000]; set => config1[0x8000] = value; }
        public byte RumbleRight { get => rumbleRight; set => rumbleRight = value; }
        public byte RumbleLeft { get => rumbleLeft; set => rumbleLeft = value; }
        public byte VolumeHeadphone { get => volumeHeadphone; set => volumeHeadphone = value; }
        public byte VolumeSpeaker { get => volumeSpeaker; set => volumeSpeaker = value; }
        public byte VolumeMic { get => volumeMic; set => volumeMic = value; }
        public byte MicSelect { get => micConfig[0x03, 0]; set => micConfig[0x03, 0] = value; }
        public bool EchoCancelEnable { get => micConfig[0x04]; set => micConfig[0x04] = value; }
        public bool NoiseCancelEnable { get => micConfig[0x08]; set => micConfig[0x08] = value; }
        public byte OutputPathSelect { get => micConfig[0x3, 4]; set => micConfig[0x3, 4] = value; }
        public byte InputPathSelect { get => micConfig[0x3, 6]; set => micConfig[0x3, 6] = value; }
        public byte MuteLightMode { get => muteLightMode; set => muteLightMode = value; }
        public bool TouchPowerSave { get => powerConfig[0x01]; set => powerConfig[0x01] = value; }
        public bool MotionPowerSave { get => powerConfig[0x02]; set => powerConfig[0x02] = value; }
        public bool HapticPowerSave { get => powerConfig[0x04]; set => powerConfig[0x04] = value; }
        public bool AudioPowerSave { get => powerConfig[0x08]; set => powerConfig[0x08] = value; }
        public bool MicMute { get => powerConfig[0x10]; set => powerConfig[0x10] = value; }
        public bool SpeakerMute { get => powerConfig[0x20]; set => powerConfig[0x20] = value; }
        public bool HeadphoneMute { get => powerConfig[0x40]; set => powerConfig[0x40] = value; }
        public bool HapticMute { get => powerConfig[0x80]; set => powerConfig[0x80] = value; }
        public uint HostTimestamp { get => hostTimestamp; set => hostTimestamp = value; }
        public byte TriggerPowerReduction { get => (byte)config2[0xF, 0]; set => config2[0xF, 0] = value; }
        public byte RumblePowerReduction { get => (byte)config2[0xF, 4]; set => config2[0xF, 4] = value; }
        public byte SpeakerCompPreGain {get => (byte)config2[0x7, 8]; set => config2[0x7, 8] = value; }
        public bool BeamformingEnable { get => config2[0x0800]; set => config2[0x0800] = value; }
        public byte unkAudioControl { get => (byte)config2[0xF, 12]; set => config2[0xF, 12] = value; }
        public bool AllowLightBritnessChange { get => config2[0x010000]; set => config2[0x010000] = value; }
        public bool AllowColorLightFadeAnimation { get => config2[0x020000]; set => config2[0x020000] = value; }
        public bool EnableImprovedRumbleEmulation { get => config2[0x040000]; set => config2[0x040000] = value; }
        public bool HapticLowPassFilter { get => config2[0x100000]; set => config2[0x100000] = value; }
        public byte LightFadeAnimation { get => lightFade; set => lightFade = value; }
        public byte LightBrightness { get => lightBrightness; set => lightBrightness = value; }
        public byte PlayerLight { get => playerLight[0x1F,0]; set => playerLight[0x1F,0] = value; }
        public bool Player1Light { get => playerLight[0x01]; set => playerLight[0x01] = value; }
        public bool Player2Light { get => playerLight[0x02]; set => playerLight[0x02] = value; }
        public bool Player3Light { get => playerLight[0x04]; set => playerLight[0x04] = value; }
        public bool Player4Light { get => playerLight[0x08]; set => playerLight[0x08] = value; }
        public bool Player5Light { get => playerLight[0x10]; set => playerLight[0x10] = value; }
        public bool PlayerLightFade { get => playerLight[0x20]; set => playerLight[0x20] = value; }
        public byte Red { get => red; set => red = value; }
        public byte Green { get => green; set => green = value; }
        public byte Blue { get => blue; set => blue = value; }

        public OutState() { }
    }
    #endregion Output
}