using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Jellyfish.Virtu.Settings
{
    public sealed class MachineSettings
    {
        public MachineSettings()
        {
            Cpu = new CpuSettings { Is65C02 = true, IsThrottled = true, Multiplier = 1 };
            DiskII = new DiskIISettings
            {
                Disk1 = new DiskSettings { Name = string.Empty, IsWriteProtected = false }, 
                Disk2 = new DiskSettings { Name = string.Empty, IsWriteProtected = false }
            };
            Keyboard = new KeyboardSettings
            {
                UseGamePort = false, 
                Key = new KeySettings
                {
                    Joystick0 = new JoystickSettings { UpLeft = 0, Up = 'I', UpRight = 0, Left = 'J', Right = 'L', DownLeft = 0, Down = 'K', DownRight = 0 }, 
                    Joystick1 = new JoystickSettings { UpLeft = 0, Up = 'E', UpRight = 0, Left = 'S', Right = 'F', DownLeft = 0, Down = 'D', DownRight = 0 }, 
                    Button0 = 0, Button1 = 0, Button2 = 0
                }
            };
            GamePort = new GamePortSettings
            {
                UseKeyboard = false, 
                Key = new KeySettings
                {
                    Joystick0 = new JoystickSettings { UpLeft = 0, Up = 0, UpRight = 0, Left = 0, Right = 0, DownLeft = 0, Down = 0, DownRight = 0 }, 
                    Joystick1 = new JoystickSettings { UpLeft = 0, Up = 0, UpRight = 0, Left = 0, Right = 0, DownLeft = 0, Down = 0, DownRight = 0 }, 
                    Button0 = 0, Button1 = 0, Button2 = 0
                }
            };
            Audio = new AudioSettings { Volume = 0.5 };
            Video = new VideoSettings
            {
                IsFullScreen = false, IsMonochrome = false, ScannerOptions = ScannerOptions.None, 
                Color = new ColorSettings
                {
                    Black = 0xFF000000, // BGRA
                    DarkBlue = 0xFF000099, 
                    DarkGreen = 0xFF117722, 
                    MediumBlue = 0xFF0000FF, 
                    Brown = 0xFF885500, 
                    LightGrey = 0xFF99AAAA, 
                    Green = 0xFF00EE11, 
                    Aquamarine = 0xFF55FFAA, 
                    DeepRed = 0xFFFF1111, 
                    Purple = 0xFFDD00DD, 
                    DarkGrey = 0xFF445555, 
                    LightBlue = 0xFF33AAFF, 
                    Orange = 0xFFFF4411, 
                    Pink = 0xFFFF9988, 
                    Yellow = 0xFFFFFF11, 
                    White = 0xFFFFFFFF, 
                    Monochrome = 0xFF00AA00
                }
            };
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void Deserialize(Stream stream)
        {
            try
            {
                using (var reader = XmlReader.Create(stream))
                {
                    var ns = Namespace;
                    var root = XElement.Load(reader);
                    var cpu = root.Element(ns + "Cpu");
                    Cpu = new CpuSettings
                    {
                        Is65C02 = (bool)cpu.Attribute("Is65C02"), 
                        IsThrottled = (bool)cpu.Attribute("IsThrottled"), 
                        Multiplier = (int)cpu.Attribute("Multiplier")
                    };
                    var diskII = root.Element(ns + "DiskII");
                    var disk1 = diskII.Element(ns + "Disk1");
                    var disk2 = diskII.Element(ns + "Disk2");
                    DiskII = new DiskIISettings
                    {
                        Disk1 = new DiskSettings
                        {
                            Name = (string)disk1.Attribute("Name") ?? string.Empty, 
                            IsWriteProtected = (bool)disk1.Attribute("IsWriteProtected")
                        },
                        Disk2 = new DiskSettings
                        {
                            Name = (string)disk2.Attribute("Name") ?? string.Empty, 
                            IsWriteProtected = (bool)disk2.Attribute("IsWriteProtected")
                        },
                    };
                    var keyboard = root.Element(ns + "Keyboard");
                    var key = keyboard.Element(ns + "Key");
                    var joystick0 = key.Element(ns + "Joystick0");
                    var joystick1 = key.Element(ns + "Joystick1");
                    var buttons = key.Element(ns + "Buttons");
                    Keyboard = new KeyboardSettings
                    {
                        UseGamePort = (bool)keyboard.Attribute("UseGamePort"), 
                        Key = new KeySettings
                        {
                            Joystick0 = new JoystickSettings
                            {
                                UpLeft = (int)joystick0.Attribute("UpLeft"), 
                                Up = (int)joystick0.Attribute("Up"), 
                                UpRight = (int)joystick0.Attribute("UpRight"), 
                                Left = (int)joystick0.Attribute("Left"), 
                                Right = (int)joystick0.Attribute("Right"), 
                                DownLeft = (int)joystick0.Attribute("DownLeft"), 
                                Down = (int)joystick0.Attribute("Down"), 
                                DownRight = (int)joystick0.Attribute("DownRight")
                            }, 
                            Joystick1 = new JoystickSettings
                            {
                                UpLeft = (int)joystick1.Attribute("UpLeft"), 
                                Up = (int)joystick1.Attribute("Up"), 
                                UpRight = (int)joystick1.Attribute("UpRight"), 
                                Left = (int)joystick1.Attribute("Left"), 
                                Right = (int)joystick1.Attribute("Right"), 
                                DownLeft = (int)joystick1.Attribute("DownLeft"), 
                                Down = (int)joystick1.Attribute("Down"), 
                                DownRight = (int)joystick1.Attribute("DownRight")
                            }, 
                            Button0 = (int)buttons.Attribute("Button0"), 
                            Button1 = (int)buttons.Attribute("Button1"), 
                            Button2 = (int)buttons.Attribute("Button2")
                        }
                    };
                    var gamePort = root.Element(ns + "GamePort");
                    key = gamePort.Element(ns + "Key");
                    joystick0 = key.Element(ns + "Joystick0");
                    joystick1 = key.Element(ns + "Joystick1");
                    buttons = key.Element(ns + "Buttons");
                    GamePort = new GamePortSettings
                    {
                        UseKeyboard = (bool)gamePort.Attribute("UseKeyboard"), 
                        Key = new KeySettings
                        {
                            Joystick0 = new JoystickSettings
                            {
                                UpLeft = (int)joystick0.Attribute("UpLeft"), 
                                Up = (int)joystick0.Attribute("Up"), 
                                UpRight = (int)joystick0.Attribute("UpRight"), 
                                Left = (int)joystick0.Attribute("Left"), 
                                Right = (int)joystick0.Attribute("Right"), 
                                DownLeft = (int)joystick0.Attribute("DownLeft"), 
                                Down = (int)joystick0.Attribute("Down"), 
                                DownRight = (int)joystick0.Attribute("DownRight")
                            }, 
                            Joystick1 = new JoystickSettings
                            {
                                UpLeft = (int)joystick1.Attribute("UpLeft"), 
                                Up = (int)joystick1.Attribute("Up"), 
                                UpRight = (int)joystick1.Attribute("UpRight"), 
                                Left = (int)joystick1.Attribute("Left"), 
                                Right = (int)joystick1.Attribute("Right"), 
                                DownLeft = (int)joystick1.Attribute("DownLeft"), 
                                Down = (int)joystick1.Attribute("Down"), 
                                DownRight = (int)joystick1.Attribute("DownRight")
                            }, 
                            Button0 = (int)buttons.Attribute("Button0"), 
                            Button1 = (int)buttons.Attribute("Button1"), 
                            Button2 = (int)buttons.Attribute("Button2")
                        }
                    };
                    var audio = root.Element(ns + "Audio");
                    Audio = new AudioSettings
                    {
                        Volume = (double)audio.Attribute("Volume")
                    };
                    var video = root.Element(ns + "Video");
                    var color = video.Element(ns + "Color");
                    Video = new VideoSettings
                    {
                        IsFullScreen = (bool)video.Attribute("IsFullScreen"), 
                        IsMonochrome = (bool)video.Attribute("IsMonochrome"), 
                        ScannerOptions = (ScannerOptions)Enum.Parse(typeof(ScannerOptions), (string)video.Attribute("ScannerOptions"), true), 
                        Color = new ColorSettings
                        {
                            Black = (uint)color.Attribute("Black"), 
                            DarkBlue = (uint)color.Attribute("DarkBlue"), 
                            DarkGreen = (uint)color.Attribute("DarkGreen"), 
                            MediumBlue = (uint)color.Attribute("MediumBlue"), 
                            Brown = (uint)color.Attribute("Brown"), 
                            LightGrey = (uint)color.Attribute("LightGrey"), 
                            Green = (uint)color.Attribute("Green"), 
                            Aquamarine = (uint)color.Attribute("Aquamarine"), 
                            DeepRed = (uint)color.Attribute("DeepRed"), 
                            Purple = (uint)color.Attribute("Purple"), 
                            DarkGrey = (uint)color.Attribute("DarkGrey"), 
                            LightBlue = (uint)color.Attribute("LightBlue"), 
                            Orange = (uint)color.Attribute("Orange"), 
                            Pink = (uint)color.Attribute("Pink"), 
                            Yellow = (uint)color.Attribute("Yellow"), 
                            White = (uint)color.Attribute("White"), 
                            Monochrome = (uint)color.Attribute("Monochrome")
                        }
                    };
                }
            }
            catch (Exception)
            {
            }
        }

        public void Serialize(Stream stream)
        {
            var ns = Namespace;
            var xml = new XElement(ns + "MachineSettings",
                new XElement(ns + "Cpu",
                    new XAttribute("Is65C02", Cpu.Is65C02),
                    new XAttribute("IsThrottled", Cpu.IsThrottled),
                    new XAttribute("Multiplier", Cpu.Multiplier)),
                new XElement(ns + "DiskII",
                    new XElement(ns + "Disk1",
                        new XAttribute("Name", DiskII.Disk1.Name),
                        new XAttribute("IsWriteProtected", DiskII.Disk1.IsWriteProtected)),
                    new XElement(ns + "Disk2",
                        new XAttribute("Name", DiskII.Disk2.Name),
                        new XAttribute("IsWriteProtected", DiskII.Disk2.IsWriteProtected))),
                new XElement(ns + "Keyboard",
                    new XAttribute("UseGamePort", Keyboard.UseGamePort),
                    new XElement(ns + "Key",
                        new XElement(ns + "Joystick0",
                            new XAttribute("UpLeft", Keyboard.Key.Joystick0.UpLeft),
                            new XAttribute("Up", Keyboard.Key.Joystick0.Up),
                            new XAttribute("UpRight", Keyboard.Key.Joystick0.UpRight),
                            new XAttribute("Left", Keyboard.Key.Joystick0.Left),
                            new XAttribute("Right", Keyboard.Key.Joystick0.Right),
                            new XAttribute("DownLeft", Keyboard.Key.Joystick0.DownLeft),
                            new XAttribute("Down", Keyboard.Key.Joystick0.Down),
                            new XAttribute("DownRight", Keyboard.Key.Joystick0.DownRight)),
                        new XElement(ns + "Joystick1",
                            new XAttribute("UpLeft", Keyboard.Key.Joystick1.UpLeft),
                            new XAttribute("Up", Keyboard.Key.Joystick1.Up),
                            new XAttribute("UpRight", Keyboard.Key.Joystick1.UpRight),
                            new XAttribute("Left", Keyboard.Key.Joystick1.Left),
                            new XAttribute("Right", Keyboard.Key.Joystick1.Right),
                            new XAttribute("DownLeft", Keyboard.Key.Joystick1.DownLeft),
                            new XAttribute("Down", Keyboard.Key.Joystick1.Down),
                            new XAttribute("DownRight", Keyboard.Key.Joystick1.DownRight)),
                        new XElement(ns + "Buttons",
                            new XAttribute("Button0", Keyboard.Key.Button0),
                            new XAttribute("Button1", Keyboard.Key.Button1),
                            new XAttribute("Button2", Keyboard.Key.Button2)))),
                new XElement(ns + "GamePort",
                    new XAttribute("UseKeyboard", GamePort.UseKeyboard),
                    new XElement(ns + "Key",
                        new XElement(ns + "Joystick0",
                            new XAttribute("UpLeft", GamePort.Key.Joystick0.UpLeft),
                            new XAttribute("Up", GamePort.Key.Joystick0.Up),
                            new XAttribute("UpRight", GamePort.Key.Joystick0.UpRight),
                            new XAttribute("Left", GamePort.Key.Joystick0.Left),
                            new XAttribute("Right", GamePort.Key.Joystick0.Right),
                            new XAttribute("DownLeft", GamePort.Key.Joystick0.DownLeft),
                            new XAttribute("Down", GamePort.Key.Joystick0.Down),
                            new XAttribute("DownRight", GamePort.Key.Joystick0.DownRight)),
                        new XElement(ns + "Joystick1",
                            new XAttribute("UpLeft", GamePort.Key.Joystick1.UpLeft),
                            new XAttribute("Up", GamePort.Key.Joystick1.Up),
                            new XAttribute("UpRight", GamePort.Key.Joystick1.UpRight),
                            new XAttribute("Left", GamePort.Key.Joystick1.Left),
                            new XAttribute("Right", GamePort.Key.Joystick1.Right),
                            new XAttribute("DownLeft", GamePort.Key.Joystick1.DownLeft),
                            new XAttribute("Down", GamePort.Key.Joystick1.Down),
                            new XAttribute("DownRight", GamePort.Key.Joystick1.DownRight)),
                        new XElement(ns + "Buttons",
                            new XAttribute("Button0", Keyboard.Key.Button0),
                            new XAttribute("Button1", Keyboard.Key.Button1),
                            new XAttribute("Button2", Keyboard.Key.Button2)))),
                new XElement(ns + "Audio",
                    new XAttribute("Volume", Audio.Volume)),
                new XElement(ns + "Video",
                    new XAttribute("IsFullScreen", Video.IsFullScreen),
                    new XAttribute("IsMonochrome", Video.IsMonochrome),
                    new XAttribute("ScannerOptions", Video.ScannerOptions),
                    new XElement(ns + "Color",
                        new XAttribute("Black", Video.Color.Black),
                        new XAttribute("DarkBlue", Video.Color.DarkBlue),
                        new XAttribute("DarkGreen", Video.Color.DarkGreen),
                        new XAttribute("MediumBlue", Video.Color.MediumBlue),
                        new XAttribute("Brown", Video.Color.Brown),
                        new XAttribute("LightGrey", Video.Color.LightGrey),
                        new XAttribute("Green", Video.Color.Green),
                        new XAttribute("Aquamarine", Video.Color.Aquamarine),
                        new XAttribute("DeepRed", Video.Color.DeepRed),
                        new XAttribute("Purple", Video.Color.Purple),
                        new XAttribute("DarkGrey", Video.Color.DarkGrey),
                        new XAttribute("LightBlue", Video.Color.LightBlue),
                        new XAttribute("Orange", Video.Color.Orange),
                        new XAttribute("Pink", Video.Color.Pink),
                        new XAttribute("Yellow", Video.Color.Yellow),
                        new XAttribute("White", Video.Color.White),
                        new XAttribute("Monochrome", Video.Color.Monochrome))));

            using (var writer = XmlWriter.Create(stream))
            {
                xml.WriteTo(writer);
            }
        }

        public CpuSettings Cpu { get; set; }
        public DiskIISettings DiskII { get; set; }
        public KeyboardSettings Keyboard { get; set; }
        public GamePortSettings GamePort { get; set; }
        public AudioSettings Audio { get; set; }
        public VideoSettings Video { get; set; }

        public const string FileName = "Settings.xml";
        [SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly XNamespace Namespace = "http://schemas.jellyfish.co.nz/virtu/settings";
    }

    public sealed class CpuSettings
    {
        public bool Is65C02 { get; set; }
        public bool IsThrottled { get; set; }
        public int Multiplier { get; set; }
    }

    public sealed class DiskSettings
    {
        public string Name { get; set; }
        public bool IsWriteProtected { get; set; }
    };

    public sealed class DiskIISettings
    {
        public DiskSettings Disk1 { get; set; }
        public DiskSettings Disk2 { get; set; }
    };

    public sealed class JoystickSettings
    {
        public int UpLeft { get; set; }
        public int Up { get; set; }
        public int UpRight { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int DownLeft { get; set; }
        public int Down { get; set; }
        public int DownRight { get; set; }
    };

    public sealed class KeySettings
    {
        public JoystickSettings Joystick0 { get; set; }
        public JoystickSettings Joystick1 { get; set; }
        public int Button0 { get; set; }
        public int Button1 { get; set; }
        public int Button2 { get; set; }
    };

    public sealed class KeyboardSettings
    {
        public bool UseGamePort { get; set; }
        public KeySettings Key { get; set; }
    }

    public sealed class GamePortSettings
    {
        public bool UseKeyboard { get; set; }
        public KeySettings Key { get; set; }
    }

    public sealed class AudioSettings
    {
        public double Volume { get; set; }
    };

    public sealed class ColorSettings
    {
        public uint Black { get; set; }
        public uint DarkBlue { get; set; }
        public uint DarkGreen { get; set; }
        public uint MediumBlue { get; set; }
        public uint Brown { get; set; }
        public uint LightGrey { get; set; }
        public uint Green { get; set; }
        public uint Aquamarine { get; set; }
        public uint DeepRed { get; set; }
        public uint Purple { get; set; }
        public uint DarkGrey { get; set; }
        public uint LightBlue { get; set; }
        public uint Orange { get; set; }
        public uint Pink { get; set; }
        public uint Yellow { get; set; }
        public uint White { get; set; }
        public uint Monochrome { get; set; }
    }

    [Flags]
    public enum ScannerOptions { None = 0x0, AppleII = 0x1, Pal = 0x2 } // defaults to AppleIIe, Ntsc

    public sealed class VideoSettings
    {
        public bool IsFullScreen { get; set; }
        public bool IsMonochrome { get; set; }
        public ScannerOptions ScannerOptions { get; set; }
        public ColorSettings Color { get; set; }
    };
}
