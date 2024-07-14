using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;

namespace WWAutoClicker
{
    public class KeySimulator
    {
        private readonly IntPtr gameWindowHandle;
        private const int KeyDownDuration = 100; // 0.1 seconds in milliseconds

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;

        public KeySimulator()
        {
            gameWindowHandle = FindWindow(null, "鸣潮  ");
            if (gameWindowHandle == IntPtr.Zero)
            {
                throw new Exception("Game window not found!");
            }
        }

        public void ExecuteKeyPresses(string key, int repeatCount, CancellationToken cancellationToken)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (key == "LeftMouse")
                {
                    MouseClick();
                }
                else
                {
                    SimulateKeyPress(key);
                }
                Debug.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}: Key: {key} pressed {i + 1} times");

                Thread.Sleep(KeyDownDuration);
            }
        }

        private void SimulateKeyPress(string key)
        {
            ushort keyCode = GetVirtualKeyCode(key);

            // Send WM_KEYDOWN and WM_KEYUP messages
            PostMessage(gameWindowHandle, WM_KEYDOWN, (IntPtr)keyCode, IntPtr.Zero);
            Thread.Sleep(KeyDownDuration);
            PostMessage(gameWindowHandle, WM_KEYUP, (IntPtr)keyCode, IntPtr.Zero);
        }
        
        private void MouseClick()
        {
            // Send WM_LBUTTONDOWN and WM_LBUTTONUP messages
            PostMessage(gameWindowHandle, WM_LBUTTONDOWN, IntPtr.Zero, IntPtr.Zero);
            Thread.Sleep(KeyDownDuration);
            PostMessage(gameWindowHandle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
        }
        
        private ushort GetVirtualKeyCode(string key)
        {
            return key switch
            {
                "1" => (ushort)VirtualKeyCode.VK_1,
                "2" => (ushort)VirtualKeyCode.VK_2,
                "3" => (ushort)VirtualKeyCode.VK_3,
                "Q" => (ushort)VirtualKeyCode.VK_Q,
                "E" => (ushort)VirtualKeyCode.VK_E,
                "R" => (ushort)VirtualKeyCode.VK_R,
                "F" => (ushort)VirtualKeyCode.VK_F,
                _ => throw new ArgumentException("Unsupported key: " + key),
            };
        }
    }
}