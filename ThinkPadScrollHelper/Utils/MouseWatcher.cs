using System.Diagnostics;
using System.Windows.Forms;

namespace ThinkPadScrollHelper.Utils
{
    public static class MouseWatcher
    {
        public static string GetProcessNameUnderMouseCursor()
        {
            var p = Cursor.Position;
            var hwnd = Win32Api.WindowFromPoint(p);

            Win32Api.GetWindowThreadProcessId(hwnd, out var pid);
            if (pid == 0)
            {
                return string.Empty;
            }

            var process = Process.GetProcessById((int) pid);
            try
            {
                return process.ProcessName.ToLowerInvariant();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}