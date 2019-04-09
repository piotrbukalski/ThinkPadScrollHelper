using System;
using System.Text;

namespace ThinkPadScrollHelper.Utils
{
    public static class WindowUtil
    {
        private static IntPtr _foundWindowPtr;
        private static string _findOptions;

        public static IntPtr FindMousePropertiesWindow()
        {
            _foundWindowPtr = IntPtr.Zero;
            Win32Api.EnumWindows(EnumWindowCallBack, IntPtr.Zero);
            return _foundWindowPtr;
        }


        public static IntPtr FindChildWindowByCaption(IntPtr hwnd, string caption)
        {
            _findOptions = "@Text:" + caption.ToLowerInvariant();
            _foundWindowPtr = IntPtr.Zero;
            Win32Api.EnumChildWindows(hwnd, EnumChildWindowCallBack, IntPtr.Zero);
            return _foundWindowPtr;
        }

        public static IntPtr FindChildWindowByClassName(IntPtr hwnd, string className)
        {
            _findOptions = "@Class:" + className;
            _foundWindowPtr = IntPtr.Zero;
            Win32Api.EnumChildWindows(hwnd, EnumChildWindowCallBack, IntPtr.Zero);
            return _foundWindowPtr;
        }

        private static bool EnumChildWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            var textLen = Win32Api.GetWindowTextLength(hWnd);
            if (textLen >= 1)
            {
                var winText = new StringBuilder(textLen + 1);
                Win32Api.GetWindowText(hWnd, winText, winText.Capacity);
                if ("@Text:" + winText.ToString().ToLowerInvariant() == _findOptions)
                {
                    _foundWindowPtr = hWnd;
                    return false;
                }
            }

            var winClass = new StringBuilder(256);
            Win32Api.GetClassName(hWnd, winClass, winClass.Capacity);
            if ("@Class:" + winClass != _findOptions)
            {
                return true;
            }

            _foundWindowPtr = hWnd;
            return false;
        }

        private static bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            var textLen = Win32Api.GetWindowTextLength(hWnd);
            if (0 >= textLen)
            {
                return true;
            }

            var winText = new StringBuilder(textLen + 1);
            Win32Api.GetWindowText(hWnd, winText, winText.Capacity);

            var winClass = new StringBuilder(256);
            Win32Api.GetClassName(hWnd, winClass, winClass.Capacity);

            if (!winText.ToString().Contains("Mouse Properties"))
            {
                return true;
            }

            Console.WriteLine("WindowClass:" + winClass);
            Console.WriteLine("WindowText:" + winText);
            _foundWindowPtr = hWnd;
            return false;
        }
    }
}