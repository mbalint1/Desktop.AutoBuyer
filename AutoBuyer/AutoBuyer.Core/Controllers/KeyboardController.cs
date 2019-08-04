using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using AutoBuyer.Core.Interfaces;

namespace AutoBuyer.Core.Controllers
{
    public class KeyboardController : IKeyboardController
    {
        public void SendInput(string input)
        {
            input = input.ToUpperInvariant();
            foreach (var s in input.ToCharArray())
            {
                try
                {
                    Thread.Sleep(250);
                    Send(KeyMappings[s]);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void Send(ScanCodeShort a)
        {
            var inputs = new INPUT[1];
            var input = new INPUT { type = 1 }; // 1 = Keyboard Input
            input.U.ki.wScan = a;
            input.U.ki.dwFlags = KEYEVENTF.SCANCODE;
            inputs[0] = input;
            SendInput(1, inputs, INPUT.Size);
        }

        private static readonly Dictionary<char, ScanCodeShort> KeyMappings = new Dictionary<char, ScanCodeShort>
        {
            {'A', ScanCodeShort.KEY_A}, {'B', ScanCodeShort.KEY_B}, {'C', ScanCodeShort.KEY_C},
            {'D', ScanCodeShort.KEY_D}, {'E', ScanCodeShort.KEY_E}, {'F', ScanCodeShort.KEY_F},
            {'G', ScanCodeShort.KEY_G}, {'H', ScanCodeShort.KEY_H}, {'I', ScanCodeShort.KEY_I},
            {'J', ScanCodeShort.KEY_J}, {'K', ScanCodeShort.KEY_K}, {'L', ScanCodeShort.KEY_L},
            {'M', ScanCodeShort.KEY_M}, {'N', ScanCodeShort.KEY_N}, {'O', ScanCodeShort.KEY_O},
            {'P', ScanCodeShort.KEY_P}, {'Q', ScanCodeShort.KEY_Q}, {'R', ScanCodeShort.KEY_R},
            {'S', ScanCodeShort.KEY_S}, {'T', ScanCodeShort.KEY_T}, {'U', ScanCodeShort.KEY_U},
            {'V', ScanCodeShort.KEY_V}, {'W', ScanCodeShort.KEY_W}, {'X', ScanCodeShort.KEY_X},
            {'Y', ScanCodeShort.KEY_Y}, {'Z', ScanCodeShort.KEY_Z}, {' ', ScanCodeShort.SPACE},

            {'0', ScanCodeShort.KEY_0}, {'1', ScanCodeShort.KEY_1}, {'2', ScanCodeShort.KEY_2},
            { '3', ScanCodeShort.KEY_3}, {'4', ScanCodeShort.KEY_4}, {'5', ScanCodeShort.KEY_5},
            { '6', ScanCodeShort.KEY_6}, {'7', ScanCodeShort.KEY_7}, {'8', ScanCodeShort.KEY_8},
            { '9', ScanCodeShort.KEY_9}
        };

        /// <summary>
        /// Declaration of external SendInput method
        /// </summary>
        [DllImport("user32.dll")]
        internal static extern uint SendInput(
            uint nInputs,
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs,
            int cbSize);


        // Declare the INPUT struct
        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public uint type;
            public InputUnion U;
            public static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        // Declare the InputUnion struct
        [StructLayout(LayoutKind.Explicit)]
        public struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal MouseEventDataXButtons mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum MouseEventDataXButtons : uint
        {
            Nothing = 0x00000000,
            XBUTTON1 = 0x00000001,
            XBUTTON2 = 0x00000002
        }

        [Flags]
        public enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            internal VirtualKeyShort wVk;
            internal ScanCodeShort wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [Flags]
        public enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        public enum VirtualKeyShort : short
        {
            SPACE = 0x20,
            KEY_0 = 0x30,
            KEY_1 = 0x31,
            KEY_2 = 0x32,
            KEY_3 = 0x33,
            KEY_4 = 0x34,
            KEY_5 = 0x35,
            KEY_6 = 0x36,
            KEY_7 = 0x37,
            KEY_8 = 0x38,
            KEY_9 = 0x39,
            KEY_A = 0x41,
            KEY_B = 0x42,
            KEY_C = 0x43,
            KEY_D = 0x44,
            KEY_E = 0x45,
            KEY_F = 0x46,
            KEY_G = 0x47,
            KEY_H = 0x48,
            KEY_I = 0x49,
            KEY_J = 0x4A,
            KEY_K = 0x4B,
            KEY_L = 0x4C,
            KEY_M = 0x4D,
            KEY_N = 0x4E,
            KEY_O = 0x4F,
            KEY_P = 0x50,
            KEY_Q = 0x51,
            KEY_R = 0x52,
            KEY_S = 0x53,
            KEY_T = 0x54,
            KEY_U = 0x55,
            KEY_V = 0x56,
            KEY_W = 0x57,
            KEY_X = 0x58,
            KEY_Y = 0x59,
            KEY_Z = 0x5A
        }

        public enum ScanCodeShort : short
        {
            SPACE = 57,
            KEY_0 = 11,
            KEY_1 = 2,
            KEY_2 = 3,
            KEY_3 = 4,
            KEY_4 = 5,
            KEY_5 = 6,
            KEY_6 = 7,
            KEY_7 = 8,
            KEY_8 = 9,
            KEY_9 = 10,
            KEY_A = 30,
            KEY_B = 48,
            KEY_C = 46,
            KEY_D = 32,
            KEY_E = 18,
            KEY_F = 33,
            KEY_G = 34,
            KEY_H = 35,
            KEY_I = 23,
            KEY_J = 36,
            KEY_K = 37,
            KEY_L = 38,
            KEY_M = 50,
            KEY_N = 49,
            KEY_O = 24,
            KEY_P = 25,
            KEY_Q = 16,
            KEY_R = 19,
            KEY_S = 31,
            KEY_T = 20,
            KEY_U = 22,
            KEY_V = 47,
            KEY_W = 17,
            KEY_X = 45,
            KEY_Y = 21,
            KEY_Z = 44,
        }

        /// <summary>
        /// Define HARDWAREINPUT struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }
    }
}