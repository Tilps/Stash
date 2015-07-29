using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace LOTROChat
{
    class KeySender
    {

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendInput(uint nInputs, ref INPUT pInputs, int cbSize);

        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct KEYBDINPUT
        {
            public ushort wVk;
            public ushort wScan;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        struct HARDWAREINPUT
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct MOUSEKEYBDHARDWAREINPUT
        {
            [FieldOffset(0)]
            public MOUSEINPUT mi;

            [FieldOffset(0)]
            public KEYBDINPUT ki;

            [FieldOffset(0)]
            public HARDWAREINPUT hi;
        }

        struct INPUT
        {
            public int type;
            public MOUSEKEYBDHARDWAREINPUT mkhi;
        }
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const uint KEYEVENTF_KEYUP = 0x0002;
        const uint KEYEVENTF_UNICODE = 0x0004;
        const uint KEYEVENTF_SCANCODE = 0x0008;

        public static void SendString(string toSend)
        {
            INPUT input = new INPUT();
            input.type = 1;
            input.mkhi.ki.dwFlags |= KEYEVENTF_SCANCODE;

            foreach (char c in toSend)
            {
                ushort code;
                bool shift;
                MapToCode(c, out code, out shift);
                if (code == 0)
                    continue;
                if (shift)
                {
                    input.mkhi.ki.wScan = 0x2A;
                    SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
                }
                input.mkhi.ki.wScan = code;
                SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
                input.mkhi.ki.dwFlags |= KEYEVENTF_KEYUP;
                SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
                if (shift)
                {
                    input.mkhi.ki.wScan = 0x2A;
                    SendInput(1, ref input, Marshal.SizeOf(typeof(INPUT)));
                }
                input.mkhi.ki.dwFlags &= ~KEYEVENTF_KEYUP;
            }
        }

        private static void MapToCode(char c, out ushort code, out bool shift)
        {
            shift = (char.IsLetter(c) && char.IsUpper(c)) || (!char.IsLetterOrDigit(c) && @"`-=[]\;',./".IndexOf(c) == -1 && c != '\n');
            code = 0;
            switch (char.ToLower(c))
            {
                case '1':
                case '!':
                    code = 0x02;
                    break;
                case '2':
                case '@':
                    code = 0x03;
                    break;
                case '3':
                case '#':
                    code = 0x04;
                    break;
                case '4':
                case '$':
                    code = 0x05;
                    break;
                case '5':
                case '%':
                    code = 0x06;
                    break;
                case '6':
                case '^':
                    code = 0x07;
                    break;
                case '7':
                case '&':
                    code = 0x08;
                    break;
                case '8':
                case '*':
                    code = 0x09;
                    break;
                case '9':
                case '(':
                    code = 0x0A;
                    break;
                case '0':
                case ')':
                    code = 0x0B;
                    break;
                case '-':
                case '_':
                    code = 0x0C;
                    break;
                case '=':
                case '+':
                    code = 0x0D;
                    break;
                case 'q':
                    code = 0x10;
                    break;
                case 'w':
                    code = 0x11;
                    break;
                case 'e':
                    code = 0x12;
                    break;
                case 'r':
                    code = 0x13;
                    break;
                case 't':
                    code = 0x14;
                    break;
                case 'y':
                    code = 0x15;
                    break;
                case 'u':
                    code = 0x16;
                    break;
                case 'i':
                    code = 0x17;
                    break;
                case 'o':
                    code = 0x18;
                    break;
                case 'p':
                    code = 0x19;
                    break;
                case '[':
                case '{':
                    code = 0x1A;
                    break;
                case ']':
                case '}':
                    code = 0x1B;
                    break;
                case '\n':
                    code = 0x1C;
                    break;
                case 'a':
                    code = 0x1e;
                    break;
                case 's':
                    code = 0x1f;
                    break;
                case 'd':
                    code = 0x20;
                    break;
                case 'f':
                    code = 0x21;
                    break;
                case 'g':
                    code = 0x22;
                    break;
                case 'h':
                    code = 0x23;
                    break;
                case 'j':
                    code = 0x24;
                    break;
                case 'k':
                    code = 0x25;
                    break;
                case 'l':
                    code = 0x26;
                    break;
                case ';':
                case ':':
                    code = 0x27;
                    break;
                case '\'':
                case '"':
                    code = 0x28;
                    break;
                case '`':
                case '~':
                    code = 0x29;
                    break;
                case '\\':
                case '|':
                    code = 0x2B;
                    break;
                case 'z':
                    code = 0x2C;
                    break;
                case 'x':
                    code = 0x2D;
                    break;
                case 'c':
                    code = 0x2E;
                    break;
                case 'v':
                    code = 0x2F;
                    break;
                case 'b':
                    code = 0x30;
                    break;
                case 'n':
                    code = 0x31;
                    break;
                case 'm':
                    code = 0x32;
                    break;
                case ',':
                case '<':
                    code = 0x33;
                    break;
                case '.':
                case '>':
                    code = 0x34;
                    break;
                case '/':
                case '?':
                    code = 0x35;
                    break;
                case ' ':
                    code = 0x39;
                    break;
            }
        }
    }
}
