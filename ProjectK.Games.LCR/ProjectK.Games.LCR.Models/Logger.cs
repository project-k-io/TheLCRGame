using System.Diagnostics;

namespace ProjectK.Games.LCR.Models
{
    public class Logger
    {
        public static void LogDebug(string text)
        {
            Debug.WriteLine(text);
        }
    }
}
