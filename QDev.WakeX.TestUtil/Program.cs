using QDev.WakeX.Core;
using System;

namespace QDev.WakeX.TestUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var wakeService = new XboxWakeService();
            Console.WriteLine("Attempting to wake Xbox...");
            Console.WriteLine(wakeService.WakeAsync().Result);
            Console.ReadKey();
        }
    }
}
