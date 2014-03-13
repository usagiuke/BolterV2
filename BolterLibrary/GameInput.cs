using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace BolterLibrary
{
    public class Imports
    {
        protected static string ErrorCode
        {
            get { return string.Format("PostMessage Error {0:X8}", Marshal.GetLastWin32Error()); }
        }

        protected static IntPtr Hwnd;

        protected delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        protected static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, UIntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        protected static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        protected static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        protected static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        protected static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        protected static extern bool IsWindowVisible(IntPtr hWnd);

        protected static bool EnumTheWindows(IntPtr hWnd, IntPtr lParam)
        {
            var size = GetWindowTextLength(hWnd);

            if (size++ <= 0 || !IsWindowVisible(hWnd)) return true;

            var sb = new StringBuilder(size);

            GetWindowText(hWnd, sb, size);

            if (sb.ToString() != "FINAL FANTASY XIV: A Realm Reborn") return true;

            uint PID;

            GetWindowThreadProcessId(hWnd, out PID);

            if (PID == Process.GetCurrentProcess().Id)
                Hwnd = hWnd;
            return true;
        }

    }
    public class GameInput : Imports
    {
        private readonly IntPtr _ffxivHWnd;

        public GameInput()
        {
            EnumWindows(EnumTheWindows, IntPtr.Zero);
            _ffxivHWnd = Hwnd;
        }

        public void SendKeyPress(KeyStates state, Key key)
        {

            if (state != KeyStates.Toggled)
            {
                if (PostMessage(
                    hWnd: _ffxivHWnd,
                    Msg: state == KeyStates.Down ? 0x100u : 0x101u,
                    wParam: (IntPtr)KeyInterop.VirtualKeyFromKey(key),
                    lParam: state == KeyStates.Down ? (UIntPtr)0x00500001 : (UIntPtr)0xC0500001)) return;
                Console.WriteLine(ErrorCode);
                return;
            }
            if (!PostMessage(
                    hWnd: _ffxivHWnd,
                    Msg: 0x100u,
                    wParam: (IntPtr)KeyInterop.VirtualKeyFromKey(key),
                    lParam: (UIntPtr)0x00500001))
            {
                Console.WriteLine(ErrorCode);
                return;
            }
            Thread.Sleep(1);
            if (!PostMessage(
                hWnd: _ffxivHWnd,
                Msg: 0x101u,
                wParam: (IntPtr)KeyInterop.VirtualKeyFromKey(key),
                lParam: (UIntPtr)0xC0500001))
                Console.WriteLine(ErrorCode);
        }
    }
}
