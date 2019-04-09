using System;
using System.Linq;
using System.Threading;
using ThinkPadScrollHelper.Utils;

namespace ThinkPadScrollHelper
{
    internal class Program
    {
        private const string VisualStudioProcessName = "devenv";

        private static readonly string[] RichScrollBlackList = 
        {
            "conhost", "ssms", "sourcetree", "hscrollfun"
        };

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

            // quick path for VS
            if (processName.Contains(VisualStudioProcessName))
            {
                return false;
            }

            return !RichScrollBlackList.Any(processName.Contains);
        }
    }
}