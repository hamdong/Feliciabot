namespace Feliciabot.net._6._0
{
    public static class LogHelper
    {
        /// <summary>
        /// Removes logs that are at least 3 days old
        /// </summary>
        public static void ClearPreviousLogs()
        {
            foreach (string f in Directory.GetFiles(Environment.CurrentDirectory))
            {
                if (f.Contains("Feliciabot_") && f.Contains(".txt"))
                {
                    if (f.Contains("_" + DateTime.Now.Month.ToString() + "_") && (f.Contains((DateTime.Now.Day - 1).ToString() + ".")
                         || f.Contains((DateTime.Now.Day - 2).ToString() + ".") || f.Contains((DateTime.Now.Day).ToString() + ".")))
                    {
                        continue;
                    }
                    File.Delete(f);

                }
            }
        }

        /// <summary>
        /// Logs activity to a local txt file
        /// </summary>
        /// <param name="message">message to log</param>
        public static void Log(string message)
        {
            string path = GetLogFileName();

            //Create new file for the day and delete old one
            if (!File.Exists(path))
            {
                ClearPreviousLogs();
            }

            try
            {
                using StreamWriter sw = File.AppendText(path);
                sw.WriteLine(message + "_" + DateTime.Now);
                sw.Close();
            }
            catch (IOException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }

        }

        /// <summary>
        /// Gets the current log file name
        /// </summary>
        /// <returns>current log file name</returns>
        private static string GetLogFileName()
        {
            return "Feliciabot_" + DateTime.Now.Month + "_" + DateTime.Now.Day + ".txt";
        }
    }
}
