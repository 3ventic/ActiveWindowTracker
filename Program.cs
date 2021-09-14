using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ActiveWindowTracker.Win32;

var (titlePrev, pidPrev) = GetActiveWindowTitle();
Log("Started listening to active window changes");
while (true)
{
    var (title, pid) = GetActiveWindowTitle();
    if (pid != pidPrev)
    {
        Log($"Active program changed to PID {pid}: {title}");
    }
    else if (title != titlePrev)
    {
        Log($"Active program title changed to: {title}");
    }

    titlePrev = title;
    pidPrev = pid;
    await Task.Delay(TimeSpan.FromMilliseconds(100));
}

static void Log(string text)
{
    Console.WriteLine($"[{DateTime.Now:u}] {text}");
}

// adapted from https://stackoverflow.com/a/115905/1780502, https://stackoverflow.com/a/18184700/1780502
static (string, uint) GetActiveWindowTitle()
{
    const int nChars = 256;
    StringBuilder buff = new(nChars);
    var handle = GetForegroundWindow();
    var processId = uint.MaxValue;


    GetWindowText(handle, buff, nChars);
    GetWindowThreadProcessId(handle, out processId);
    return (buff.ToString(), processId);
}

namespace ActiveWindowTracker
{
    public static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern void GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
    }
}

