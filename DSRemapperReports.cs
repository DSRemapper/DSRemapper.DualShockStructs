using DSRemapper.DSRMath;
using DSRemapper.Types;
using DSRemapper.DualCommon;
using System.Runtime.Versioning;

#region DulaShock4
namespace DSRemapper.DualShock
{
    public class DualShockInputReport : IDSRInputReport
    {
        private float bat = 0;
        public IDS4InReport Raw { get; set; }
        public float Battery { get => bat; set => bat = Math.Clamp(value, 0f, 1f); }
        public bool Charging { get; set; }
        public float[] Axes { get; set; } = [0, 0, 0, 0, 0, 0];

        public float[] Sliders { get => []; set => _ = value; }

        public bool[] Buttons { get; set; } = [false, false, false, false, false, false, false, false, false, false, false, false, false, false];

        public DSRPov[] Povs { get; set; } = [new()];

        public DSRVector3[] SixAxes { get; set; } = [new(), new(), new(), new()];

        public DSRQuaternion[] Quaternions { get; set; } = [new(), new()];

        public DSRTouch[] Touch { get; set; } = [new(), new()];
        public DSRVector2 TouchPadSize { get => new(1920, 943); set => _ = value; }
        public float DeltaTime { get; set; }
        public DSRVector3 RawAccel { get { return SixAxes[0]; } set { SixAxes[0] = value; } }
        public DSRVector3 Gyro { get { return SixAxes[1]; } set { SixAxes[1] = value; } }
        public DSRVector3 Grav { get { return SixAxes[2]; } set { SixAxes[2] = value; } }
        public DSRVector3 Accel { get { return SixAxes[3]; } set { SixAxes[3] = value; } }
        public DSRQuaternion DeltaRotation { get { return Quaternions[0]; } set { Quaternions[0] = value; } }
        public DSRQuaternion Rotation { get { return Quaternions[1]; } set { Quaternions[1] = value; } }
        public DualShockInputReport(IDS4InReport raw)
        {
            Set(raw);
        }
        public void Set(IDS4InReport raw)
        {
            Raw = raw;
            Update();
        }
        public void Update()
        {
            Axes[0] = Raw.Basic.LX.ToFloat();
            Axes[1] = Raw.Basic.LY.ToFloat();
            Axes[2] = Raw.Basic.RX.ToFloat();
            Axes[3] = Raw.Basic.RY.ToFloat();
            Axes[4] = Raw.Basic.LTrigger.ToFloat();
            Axes[5] = Raw.Basic.RTrigger.ToFloat();

            Povs[0].SetDSPov(Raw.Basic.DPad);

            Buttons[0] = Raw.Basic.Square;
            Buttons[1] = Raw.Basic.Cross;
            Buttons[2] = Raw.Basic.Circle;
            Buttons[3] = Raw.Basic.Triangle;

            Buttons[4] = Raw.Basic.L1;
            Buttons[5] = Raw.Basic.R1;
            Buttons[6] = Raw.Basic.L2;
            Buttons[7] = Raw.Basic.R2;
            Buttons[8] = Raw.Basic.Share;
            Buttons[9] = Raw.Basic.Options;
            Buttons[10] = Raw.Basic.L3;
            Buttons[11] = Raw.Basic.R3;

            Buttons[12] = Raw.Basic.PS;
            Buttons[13] = Raw.Basic.TPad;

            Charging = Raw.Extended.USB;
            Battery = Raw.Extended.Battery / 10f + (Charging ? 0 : 0.1f);

            SixAxes[1].X = -Raw.Extended.AngularVelocityX * (2000f / 32767f);
            SixAxes[1].Y = -Raw.Extended.AngularVelocityY * (2000f / 32767f);
            SixAxes[1].Z = Raw.Extended.AngularVelocityZ * (2000f / 32767f);
            SixAxes[0].X = -Raw.Extended.AccelerometerX / 8192f;
            SixAxes[0].Y = -Raw.Extended.AccelerometerY / 8192f;
            SixAxes[0].Z = Raw.Extended.AccelerometerZ / 8192f;

            Touch[0].Pressed = Raw.Touch.Fingers[0].FingerTouch;
            Touch[0].Id = Raw.Touch.Fingers[0].FingerId;
            Touch[0].Pos.X = Raw.Touch.Fingers[0].FingerX;
            Touch[0].Pos.Y = Raw.Touch.Fingers[0].FingerY;
            Touch[0].Pos /= TouchPadSize;

            Touch[1].Pressed = Raw.Touch.Fingers[1].FingerTouch;
            Touch[1].Id = Raw.Touch.Fingers[1].FingerId;
            Touch[1].Pos.X = Raw.Touch.Fingers[1].FingerX;
            Touch[1].Pos.Y = Raw.Touch.Fingers[1].FingerY;
            Touch[1].Pos /= TouchPadSize;
        }
        public void UpdateRaw()
        {
            BasicInState basic = Raw.Basic;
            ExtendedInState extended = Raw.Extended;

            basic.LX = Axes[0].ToSByte();
            basic.LY = Axes[1].ToSByte();
            basic.RX = Axes[2].ToSByte();
            basic.RY = Axes[3].ToSByte();
            basic.LTrigger = Axes[4].ToByte();
            basic.RTrigger = Axes[5].ToByte();

            basic.Square = Buttons[0];
            basic.Cross = Buttons[1];
            basic.Circle = Buttons[2];
            basic.Triangle = Buttons[3];

            basic.L1 = Buttons[4];
            basic.R1 = Buttons[5];
            basic.L2 = Buttons[6];
            basic.R2 = Buttons[7];
            basic.Share = Buttons[8];
            basic.Options = Buttons[9];
            basic.L3 = Buttons[10];
            basic.R3 = Buttons[11];

            basic.PS = Buttons[12];
            basic.TPad = Buttons[13];

            extended.Battery = (byte)(Battery * 10);
            extended.USB = Charging;

            extended.AngularVelocityX = (short)(-SixAxes[1].X * (32767f / 2000f));
            extended.AngularVelocityY = (short)(-SixAxes[1].Y * (32767f / 2000f));
            extended.AngularVelocityZ = (short)(SixAxes[1].Z * (32767f / 2000f));
            extended.AccelerometerX = (short)(-SixAxes[0].X * 8192f);
            extended.AccelerometerY = (short)(-SixAxes[0].Y * 8192f);
            extended.AccelerometerZ = (short)(SixAxes[0].Z * 8192f);

            Raw.Touch.Fingers[0].FingerTouch = Touch[0].Pressed;
            Raw.Touch.Fingers[0].FingerId = (byte)Touch[0].Id;
            Raw.Touch.Fingers[0].FingerX = (short)(Touch[0].Pos.X * TouchPadSize.X);
            Raw.Touch.Fingers[0].FingerY = (short)(Touch[0].Pos.Y * TouchPadSize.Y);

            Raw.Touch.Fingers[1].FingerTouch = Touch[1].Pressed;
            Raw.Touch.Fingers[1].FingerId = (byte)Touch[1].Id;
            Raw.Touch.Fingers[1].FingerX = (short)(Touch[1].Pos.X * TouchPadSize.X);
            Raw.Touch.Fingers[1].FingerY = (short)(Touch[1].Pos.Y * TouchPadSize.Y);

            Raw.Basic = basic;
            Raw.Extended = extended;
        }
    }

    public class DualShockOutputReport : IDSROutputReport
    {
        public IDS4OutReport Raw { get; set; }
        public float[] Rumble { get; set; } = [0, 0];
        public DSRLight Led { get; set; } = new();
        public float[] ExtLeds { get => []; set => _ = value; }
        public IDSRFeedback[] Feedbacks { get => []; set => _ = value; }

        /*public DualShockOutputReport() : this(new USBOutReport())
        {
            Led.SetRGB(0, 0, 1, 0.125f);
        }*/
        public DualShockOutputReport(IDS4OutReport raw)
        {
            Set(raw);
        }
        public void Set(IDS4OutReport raw)
        {
            Raw = raw;
            Update();
        }
        public void Update()
        {
            Rumble[0] = Raw.State.RumbleRight.ToFloat();
            Rumble[1] = Raw.State.RumbleLeft.ToFloat();
            Led.SetRGB(Raw.State.Red.ToFloat(), Raw.State.Green.ToFloat(), Raw.State.Blue.ToFloat());
            Led.OnTime = Raw.State.FlashOn.ToFloat();
            Led.OffTime = Raw.State.FlashOff.ToFloat();
        }
        public void UpdateRaw()
        {
            SetStateData stateData = Raw.State;
            stateData.RumbleRight = Rumble[0].ToByte();
            stateData.RumbleLeft = Rumble[1].ToByte();
            stateData.Red = Led.Red.ToByte();
            stateData.Green = Led.Green.ToByte();
            stateData.Blue = Led.Blue.ToByte();
            stateData.FlashOn = Led.OnTime.ToByte();
            stateData.FlashOff = Led.OffTime.ToByte();
            Raw.State = stateData;
        }
    }
}
#endregion DualShock4

#region DualSense
namespace DSRemapper.DualSense
{
    public class DualSenseInputReport : IDSRInputReport
    {
        private float bat = 0;
        public float Battery { get => bat; set => bat = Math.Clamp(value, 0f, 1f); }
        public bool Charging { get; set; }
        public InState Raw;
        public float[] Axes { get; set; } = new float[6];
        public float[] Sliders { get; set; } = new float[0];
        public bool[] Buttons { get; set; } = new bool[15];
        public DSRPov[] Povs  { get; set; } = [new()];
        public DSRVector3[] SixAxes { get; set; } = [new(), new(), new(), new()];
        public DSRQuaternion[] Quaternions { get; set; } = [new(), new()];
        public DSRTouch[] Touch { get; set; } = [new(), new()];
        public DSRVector2 TouchPadSize { get => new(1919, 1079); set => _ = value; }
        public float DeltaTime { get; set; }
        public DSRVector3 RawAccel { get { return SixAxes[0]; } set { SixAxes[0] = value; } }
        public DSRVector3 Gyro { get { return SixAxes[1]; } set { SixAxes[1] = value; } }
        public DSRVector3 Grav { get { return SixAxes[2]; } set { SixAxes[2] = value; } }
        public DSRVector3 Accel { get { return SixAxes[3]; } set { SixAxes[3] = value; } }
        public DSRQuaternion DeltaRotation { get { return Quaternions[0]; } set { Quaternions[0] = value; } }
        public DSRQuaternion Rotation { get { return Quaternions[1]; } set { Quaternions[1] = value; } }

        public DualSenseInputReport() { }
        public DualSenseInputReport(InState raw)
        {
            Set(raw);
        }
        public void Set(InState raw)
        {
            Raw = raw;
            Update();
        }
        public void Update()
        {
            Axes[0] = Raw.LX.ToFloat();
            Axes[1] = Raw.LY.ToFloat();
            Axes[2] = Raw.RX.ToFloat();
            Axes[3] = Raw.RY.ToFloat();
            Axes[4] = Raw.LTrigger.ToFloat();
            Axes[5] = Raw.RTrigger.ToFloat();

            Povs[0].SetDSPov(Raw.DPad);

            Buttons[0] = Raw.Square;
            Buttons[1] = Raw.Cross;
            Buttons[2] = Raw.Circle;
            Buttons[3] = Raw.Triangle;

            Buttons[4] = Raw.L1;
            Buttons[5] = Raw.R1;
            Buttons[6] = Raw.L2;
            Buttons[7] = Raw.R2;
            Buttons[8] = Raw.Create;
            Buttons[9] = Raw.Options;
            Buttons[10] = Raw.L3;
            Buttons[11] = Raw.R3;

            Buttons[12] = Raw.PS;
            Buttons[13] = Raw.TPad;
            Buttons[14] = Raw.Mute;

            Charging = Raw.BatteryState == 0x01;
            Battery = Raw.BatteryState == 0x02 ? 1f : (Raw.Battery / 10f) + (Charging ? 0 : 0.1f);

            SixAxes[1].X = -Raw.AngularVelocityX * (2000f / 32767f);
            SixAxes[1].Y = -Raw.AngularVelocityY * (2000f / 32767f);
            SixAxes[1].Z = Raw.AngularVelocityZ * (2000f / 32767f);
            SixAxes[0].X = -Raw.AccelerometerX / 8192f;
            SixAxes[0].Y = -Raw.AccelerometerY / 8192f;
            SixAxes[0].Z = Raw.AccelerometerZ / 8192f;

            Touch[0].Pressed = Raw.Touch.Fingers[0].FingerTouch;
            Touch[0].Id = Raw.Touch.Fingers[0].FingerId;
            Touch[0].Pos.X = Raw.Touch.Fingers[0].FingerX;
            Touch[0].Pos.Y = Raw.Touch.Fingers[0].FingerY;
            Touch[0].Pos /= TouchPadSize;

            Touch[1].Pressed = Raw.Touch.Fingers[1].FingerTouch;
            Touch[1].Id = Raw.Touch.Fingers[1].FingerId;
            Touch[1].Pos.X = Raw.Touch.Fingers[1].FingerX;
            Touch[1].Pos.Y = Raw.Touch.Fingers[1].FingerY;
            Touch[1].Pos /= TouchPadSize;
        }
        public void UpdateRaw()
        {
            Raw.LX = Axes[0].ToSByte();
            Raw.LY = Axes[1].ToSByte();
            Raw.RX = Axes[2].ToSByte();
            Raw.RY = Axes[3].ToSByte();
            Raw.LTrigger = Axes[4].ToByte();
            Raw.RTrigger = Axes[5].ToByte();

            Raw.Square = Buttons[0];
            Raw.Cross = Buttons[1];
            Raw.Circle = Buttons[2];
            Raw.Triangle = Buttons[3];

            Raw.L1 = Buttons[4];
            Raw.R1 = Buttons[5];
            Raw.L2 = Buttons[6];
            Raw.R2 = Buttons[7];
            Raw.Create = Buttons[8];
            Raw.Options = Buttons[9];
            Raw.L3 = Buttons[10];
            Raw.R3 = Buttons[11];

            Raw.PS = Buttons[12];
            Raw.TPad = Buttons[13];
            Raw.Mute = Buttons[14];

            Raw.Battery = (byte)(Battery * 10);
            Raw.BatteryState = (byte)(Charging ? 0x01 : 0x00);

            Raw.AngularVelocityX = (short)(-SixAxes[1].X * (32767f / 2000f));
            Raw.AngularVelocityY = (short)(-SixAxes[1].Y * (32767f / 2000f));
            Raw.AngularVelocityZ = (short)(SixAxes[1].Z * (32767f / 2000f));
            Raw.AccelerometerX = (short)(-SixAxes[0].X * 8192f);
            Raw.AccelerometerY = (short)(-SixAxes[0].Y * 8192f);
            Raw.AccelerometerZ = (short)(SixAxes[0].Z * 8192f);

            Raw.Touch.Fingers[0].FingerTouch = Touch[0].Pressed;
            Raw.Touch.Fingers[0].FingerId = (byte)Touch[0].Id;
            Raw.Touch.Fingers[0].FingerX = (short)(Touch[0].Pos.X * TouchPadSize.X);
            Raw.Touch.Fingers[0].FingerY = (short)(Touch[0].Pos.Y * TouchPadSize.Y);

            Raw.Touch.Fingers[1].FingerTouch = Touch[1].Pressed;
            Raw.Touch.Fingers[1].FingerId = (byte)Touch[1].Id;
            Raw.Touch.Fingers[1].FingerX = (short)(Touch[1].Pos.X * TouchPadSize.X);
            Raw.Touch.Fingers[1].FingerY = (short)(Touch[1].Pos.Y * TouchPadSize.Y);
        }
    }
    public class DualSenseOutputReport : IDSROutputReport
    {
        public OutState Raw;
        public float[] Rumble { get; set; } = [0, 0];
        public DSRLight Led { get; set; } = new(0, 0, 1, 0.5f);
        public float[] ExtLeds { get; set; } = [0, 0, 0];
        public IDSRFeedback[] Feedbacks
        {
            get => [Raw.LTriggerFFB, Raw.RTriggerFFB];
            set
            {
                int i = 0;
                if (value.Length > 0)
                {
                    Raw.LTriggerFFB.EffectType = value[i].EffectType;
                    Raw.LTriggerFFB.Frequency = value[i].Frequency;
                    if (value[i].Strengths.Length > 0)
                    {
                        if (Raw.LTriggerFFB.RawEffectType == EffectTypes.Weapon)
                            Raw.LTriggerFFB.Strength = (byte)(value[i].Strengths[0] * 7f);
                        else
                            Raw.LTriggerFFB.Strengths = value[i].Strengths;
                    }
                    if (value[i].EffectFlags.Length > 0)
                        Raw.LTriggerFFB.EffectFlags = value[i].EffectFlags;
                    Raw.LTriggerFFB.Presets = value[i].Presets;
                }
                if (value.Length > 1)
                {
                    i++;
                    Raw.RTriggerFFB.EffectType = value[i].EffectType;
                    Raw.RTriggerFFB.Frequency = value[i].Frequency;
                    if (value[i].Strengths.Length > 0)
                    {
                        if (Raw.RTriggerFFB.RawEffectType == EffectTypes.Weapon)
                            Raw.RTriggerFFB.Strength = (byte)(value[i].Strengths[0] * 7f);
                        else
                            Raw.RTriggerFFB.Strengths = value[i].Strengths;
                    }
                    if (value[i].EffectFlags.Length > 0)
                        Raw.RTriggerFFB.EffectFlags = value[i].EffectFlags;
                    Raw.RTriggerFFB.Presets = value[i].Presets;
                }
            }
        }

        public DualSenseOutputReport(bool init = true)
        {
            Raw = new();
            Raw.AllowMuteLight = true;
            Raw.AllowLedColor = true;
            Raw.AllowLightBritnessChange = true;
            Raw.AllowColorLightFadeAnimation = true;

            Raw.AllowPlayerLights = true;
            Raw.PlayerLightFade = false;
            Raw.ResetLights = false;

            Raw.EnableRumbleEmulation = false;
            Raw.EnableImprovedRumbleEmulation = true;
            Raw.UseRumbleNotHaptics = true;

            Raw.AllowRightTriggerFeedback = true;
            Raw.AllowLeftTriggerFeedback = true;

            //Feedbacks = [Raw.LTriggerFFB, Raw.RTriggerFFB];

            UpdateRaw();
        }
        public DualSenseOutputReport(OutState raw)
        {
            Set(raw);
        }
        public void Set(OutState raw)
        {
            Raw = raw;
            //Feedbacks = [Raw.LTriggerFFB, Raw.RTriggerFFB];
            Update();
        }
        public void Update()
        {
            Rumble[0] = Raw.RumbleRight.ToFloat();
            Rumble[1] = Raw.RumbleLeft.ToFloat();
            Led.SetRGB(Raw.Red.ToFloat(), Raw.Green.ToFloat(), Raw.Blue.ToFloat());
            Led.Player = Raw.PlayerLight;
            ExtLeds[0] = Raw.MuteLightMode;
            ExtLeds[1] = Raw.LightBrightness;
            ExtLeds[1] = Raw.LightFadeAnimation;
        }
        public void UpdateRaw()
        {
            Raw.RumbleRight = Rumble[0].ToByte();
            Raw.RumbleLeft = Rumble[1].ToByte();
            Raw.Red = Led.Red.ToByte();
            Raw.Green = Led.Green.ToByte();
            Raw.Blue = Led.Blue.ToByte();
            Raw.PlayerLight = (byte)Led.Player;
            Raw.MuteLightMode = (byte)(ExtLeds[0] % 3);
            Raw.LightBrightness = (byte)(ExtLeds[1] % 3);
            Raw.LightFadeAnimation = (byte)(ExtLeds[2] % 3);

            /*if (Feedbacks.Length > 0)
            {
                Raw.LTriggerFFB.EffectType = Feedbacks[0].EffectType;
                Raw.LTriggerFFB.Frequency = Feedbacks[0].Frequency;
                if (Feedbacks[0].Strengths.Length > 0)
                {
                    if (Raw.LTriggerFFB.RawEffectType == EffectTypes.Weapon)
                        Raw.LTriggerFFB.Strength = (byte)(Feedbacks[0].Strengths[0] * 7f);
                    else
                        Raw.LTriggerFFB.Strengths = Feedbacks[0].Strengths;
                }
                if (Feedbacks[0].EffectFlags.Length > 0)
                    Raw.LTriggerFFB.EffectFlags = Feedbacks[0].EffectFlags;
                Raw.LTriggerFFB.Presets = Feedbacks[0].Presets;
            }
            if (Feedbacks.Length > 1)
            {
                Raw.RTriggerFFB.EffectType = Feedbacks[1].EffectType;
                Raw.RTriggerFFB.Frequency = Feedbacks[1].Frequency;
                if (Feedbacks[1].Strengths.Length > 0)
                {
                    if (Raw.RTriggerFFB.RawEffectType == EffectTypes.Weapon)
                        Raw.RTriggerFFB.Strength = (byte)(Feedbacks[1].Strengths[0] * 7f);
                    else
                        Raw.RTriggerFFB.Strengths = Feedbacks[1].Strengths;
                }
                if (Feedbacks[1].EffectFlags.Length > 0)
                    Raw.RTriggerFFB.EffectFlags = Feedbacks[1].EffectFlags;
                Raw.RTriggerFFB.Presets = Feedbacks[1].Presets;
            }*/
        }
    }
}
#endregion DualSense
