using QDev.WakeX.Core;

namespace QDev.WakeX.TestUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            var wakeService = new XboxWakeService();
            wakeService.Wake();
        }
    }
}
