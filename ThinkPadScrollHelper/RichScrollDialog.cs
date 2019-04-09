using System;
using System.Diagnostics;
using System.Threading;
using ThinkPadScrollHelper.Utils;

namespace ThinkPadScrollHelper
{
    public static class RichScrollDialog
    {
        private static IntPtr _hwndPropertyDialog;
        private static IntPtr _hwndCheck;
        private static IntPtr _hwndApplyButton;

        public static void Init()
        {
            var mousePropertyPath = Environment.ExpandEnvironmentVariables(@"%windir%\system32\control.exe");
            var mousePropertyProcess = Process.Start(mousePropertyPath, "mouse");
            if (mousePropertyProcess == null)
            {
                return;
            }

            mousePropertyProcess.WaitForExit();
            try
            {
                Thread.Sleep(500);
            }
            catch
            {
                // ignored
            }

            _hwndPropertyDialog = WindowUtil.FindMousePropertiesWindow();
            if (_hwndPropertyDialog == IntPtr.Zero)
            {
                throw new Exception("Mouse Properties Dialog not found");
            }

            Console.WriteLine("hwndProperty = " + _hwndPropertyDialog);

            var hwndTab = WindowUtil.FindChildWindowByClassName(_hwndPropertyDialog, "SysTabControl32");
            if (hwndTab == IntPtr.Zero)
            {
                throw new Exception("Mouse properties TabControl not found");
            }
            Console.WriteLine("hwndTab = " + hwndTab);

            var tabCount = Win32Api.SendMessage(hwndTab, Win32Api.TCM_GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero);
            Win32Api.PostMessage(hwndTab, Win32Api.TCM_SETCURFOCUS, new IntPtr(tabCount - 1), IntPtr.Zero);
            try
            {
                Thread.Sleep(500);
            }
            catch
            {
                // ignored
            }

            _hwndCheck = WindowUtil.FindChildWindowByCaption(_hwndPropertyDialog, "Thinkpad Preferred Scrolling");
            if (_hwndCheck == IntPtr.Zero)
            {
                throw new Exception("Mouse Properties Checkbox not found");
            }
            Console.WriteLine("hwndCheck = " + _hwndCheck);

            _hwndApplyButton = WindowUtil.FindChildWindowByCaption(_hwndPropertyDialog, "&Apply");
            if (_hwndApplyButton == IntPtr.Zero)
            {
                throw new Exception("Mouse Properties ApplyButton not found");
            }
            Console.WriteLine("hwndApplyButton = " + _hwndApplyButton);
        }

        public static void RestartIfClosed()
        {
            if (Win32Api.IsWindowEnabled(_hwndPropertyDialog))
            {
                return;
            }

            Console.WriteLine("---- Restart Properties dialog ----");
            Init();
        }

        public static void SetEnabled(bool rich)
        {
            RestartIfClosed();

            var currentState = Win32Api.SendMessage(_hwndCheck, Win32Api.BM_GETCHECK, IntPtr.Zero, IntPtr.Zero);
            var currentChecked = (currentState & Win32Api.BST_CHECKED) != 0;

            if (currentChecked == rich)
            {
                return;
            }

            if (rich)
            {
                Win32Api.PostMessage(_hwndCheck, Win32Api.BM_SETCHECK, new IntPtr(Win32Api.BST_CHECKED), IntPtr.Zero);
                Win32Api.PostMessage(_hwndApplyButton, Win32Api.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                Win32Api.PostMessage(_hwndCheck, Win32Api.BM_SETCHECK, new IntPtr(Win32Api.BST_UNCHECKED), IntPtr.Zero);
                Win32Api.PostMessage(_hwndApplyButton, Win32Api.BM_CLICK, IntPtr.Zero, IntPtr.Zero);
            }
        }
    }
}
