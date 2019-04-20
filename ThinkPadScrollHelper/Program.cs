using System;
using System.Collections.Generic;
using System.Threading;
using ThinkPadScrollHelper.Utils;

namespace ThinkPadScrollHelper
{
    internal class Program
    {
        private static readonly HashSet<string> RichScrollBlackList = new HashSet<string>(
            new [] { "devenv", "powershell_ise", "notepad++", "conhost", "ssms", "sourcetree", "hscrollfun" }, 
            StringComparer.OrdinalIgnoreCase);

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

            var lastProcessName = string.Empty;
            bool? lastRichScroll = null;

            while (true)
            {
                Thread.Sleep(100);

                RichScrollDaemon.RestartIfCrashed();

                var processName = MouseWatcher.GetProcessNameUnderMouseCursor();
                if (lastProcessName == processName)
                {
                    continue;
                }

                lastProcessName = processName;
                Console.WriteLine($"Process name: {processName}");

                var richScroll = GetCurrentRichScroll(processName);
                if (richScroll == lastRichScroll)
                {
                    continue;
                }

                lastRichScroll = richScroll;
                Console.WriteLine($"RichScroll: {richScroll}");
                RichScrollDialog.SetEnabled(lastRichScroll.Value);
            }
        }

        private static bool GetCurrentRichScroll(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                return true;
            }

            return !RichScrollBlackList.Contains(processName);
        }
    }
}