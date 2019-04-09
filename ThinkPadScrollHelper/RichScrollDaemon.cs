using System;
using System.Diagnostics;

namespace ThinkPadScrollHelper
{
    public static class RichScrollDaemon
    {
        private const string ScrollBackgroundPath = @"C:\Program Files (x86)\Lenovo\ThinkPad Compact Keyboard with TrackPoint driver\HScrollFun.exe";
        private const string ScrollBackgroundName = @"HScrollFun";

        public static void RestartIfCrashed()
        {
            var processesScroll = Process.GetProcessesByName(ScrollBackgroundName);
            if (processesScroll.Length >= 1)
            {
                return;
            }

            Console.WriteLine($"---- Restart {ScrollBackgroundName} ----");
            Process.Start(ScrollBackgroundPath);
        }
    }
}