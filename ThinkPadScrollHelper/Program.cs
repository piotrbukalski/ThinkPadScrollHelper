using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ThinkPadScrollHelper
{

    public class Util
    {
        static IntPtr m_found;
        public static IntPtr FindMousePropertiesWindow()
        {
            m_found = IntPtr.Zero;
            Win32Api.EnumWindows(EnumWindowCallBack, IntPtr.Zero);
            return m_found;
        }

        static string m_findOption;
        public static IntPtr FindChildWindowByCaption(IntPtr hwnd, string caption)
        {
            m_findOption = "@Text:" + caption.ToLowerInvariant();
            m_found = IntPtr.Zero;
            Win32Api.EnumChildWindows(hwnd, EnumChildWindowCallBack, IntPtr.Zero);
            return m_found;
        }
        public static IntPtr FindChildWindowByClassName(IntPtr hwnd, string className)
        {
            m_findOption = "@Class:" + className;
            m_found = IntPtr.Zero;
            Win32Api.EnumChildWindows(hwnd, EnumChildWindowCallBack, IntPtr.Zero);
            return m_found;
        }

        private static bool EnumChildWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            var textLen = Win32Api.GetWindowTextLength(hWnd);
            if (textLen >= 1)
            {
                var winText = new StringBuilder(textLen + 1);
                Win32Api.GetWindowText(hWnd, winText, winText.Capacity);
                // Console.WriteLine(tsb.ToString());
                if ("@Text:" + winText.ToString().ToLowerInvariant() == m_findOption)
                {
                    m_found = hWnd;
                    return false;
                }
            }

            //ウィンドウのクラス名を取得する
            var winClass = new StringBuilder(256);
            Win32Api.GetClassName(hWnd, winClass, winClass.Capacity);
            if ("@Class:" + winClass != m_findOption)
            {
                return true;
            }

            m_found = hWnd;
            return false;
        }

        private static bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            //ウィンドウのタイトルの長さを取得する
            var textLen = Win32Api.GetWindowTextLength(hWnd);
            if (0 >= textLen)
            {
                return true;
            }

            //ウィンドウのタイトルを取得する
            var winText = new StringBuilder(textLen + 1);
            Win32Api.GetWindowText(hWnd, winText, winText.Capacity);

            //ウィンドウのクラス名を取得する
            var winClass = new StringBuilder(256);
            Win32Api.GetClassName(hWnd, winClass, winClass.Capacity);

            //結果を表示する
            if (!winText.ToString().Contains("Mouse Properties"))
            {
                return true;
            }

            Console.WriteLine("WindowClass:" + winClass);
            Console.WriteLine("WindowText:" + winText);
            m_found = hWnd;
            return false;

            //すべてのウィンドウを列挙する
        }

    }
    
    public static class MouseWatcher
    {
        public static string GetProcessPathUnderMouseCursor()
        {
            var p = Cursor.Position;
            var hwnd = Win32Api.WindowFromPoint(p);
            Win32Api.GetWindowThreadProcessId(hwnd, out var pid);
            if (pid == 0)
            {
                return "";
            }

            var process = Process.GetProcessById((int)pid);
            try
            {
                return process.MainModule.FileName.ToLowerInvariant();
            }
            catch
            {
                return "";
            }
        }
    }

    internal class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                MainLogic();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private static void MainLogic()
        {
            RichScrollDialog.Init();

            var lastProcessPath = "";
            bool? lastRichScroll = null;
            while (true)
            {
                Thread.Sleep(80);

                RichScrollDaemon.RestartIfCrashed();

                var processPath = MouseWatcher.GetProcessPathUnderMouseCursor();
                if (lastProcessPath == processPath)
                {
                    continue;
                }

                lastProcessPath = processPath;
                Console.WriteLine($"Process: {processPath}");

                var richScroll = !processPath.Contains(@"\microsoft visual studio\");
                if (processPath.EndsWith(@"\syswow64\cmd.exe")) richScroll = false;
                else if (processPath.EndsWith(@"\scriptedsandbox64.exe")) richScroll = false;
                else if (processPath.EndsWith(@"\ssms.exe")) richScroll = false;
                else if (processPath.EndsWith(@"\sourcetree.exe")) richScroll = false;
                else if (processPath.EndsWith(@"\hscrollfun.exe")) richScroll = false;
                else if (processPath == "") richScroll = true;

                if (richScroll == lastRichScroll)
                {
                    continue;
                }
                lastRichScroll = richScroll;
                Console.WriteLine($"RichScroll: {richScroll}");
                RichScrollDialog.SetEnabled(lastRichScroll.Value);
            }
        }
    }
}