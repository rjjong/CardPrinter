using NLog;

namespace CardPinter.DLogger;

static public class Log
{
    public delegate void LogMessageEventArgs(string message);
    public static event LogMessageEventArgs? MessageReceived;

    public static Logger Logger = LogManager.GetCurrentClassLogger();
    static string _previousMessage = string.Empty;

    public static void TraceLine(LogLevel logType, string message)
    {
        try
        {
            if (!Logger.IsEnabled(logType)) return;
            if (message == _previousMessage) return;

            _previousMessage = message;
            if (logType == LogLevel.Trace)
            {
                Logger.Trace(message);
            }
            else if (logType == LogLevel.Info)
            {
                Logger.Info(message);
            }
            else if (logType == LogLevel.Warn)
            {
                Logger.Warn(message);
            }
            else if (logType == LogLevel.Error)
            {
                Logger.Error(message);
            }
            else if (logType == LogLevel.Fatal)
            {
                Logger.Fatal(message);
            }
            else
            {
                Logger.Debug(message);
            }

            MessageReceived?.Invoke(message);
            LogManager.Flush();
        }
        catch
        {

        }
    }
}
