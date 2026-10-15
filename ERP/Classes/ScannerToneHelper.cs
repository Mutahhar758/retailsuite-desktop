using System;
using System.Media;
using System.Threading;

namespace ERP.Classes
{
    public static class ScannerToneHelper
    {
        public static void PlaySuccess()
        {
            if (!ApiSession.FastScanSound) return;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    Console.Beep(2100, 80);
                }
                catch
                {
                    try { SystemSounds.Beep.Play(); } catch { }
                }
            });
        }

        public static void PlayError()
        {
            if (!ApiSession.FastScanSound) return;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    Console.Beep(450, 160);
                }
                catch
                {
                    try { SystemSounds.Hand.Play(); } catch { }
                }
            });
        }
    }
}
