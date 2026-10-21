using System;
using System.Diagnostics;

namespace DVLD
{
    public static class clsEventLogger
    {
        private const string _SourceName = "DVLD";
        private const string _LogName = "Application";

        public static void LogException(Exception ex, EventLogEntryType logType = EventLogEntryType.Error)
        {
            try
            {
                if (!EventLog.SourceExists(_SourceName))
                {
                    EventLog.CreateEventSource(_SourceName, _LogName);
                }

                string errorMessage = $"Message: {ex.Message}\n" +
                                      $"Target Site: {ex.TargetSite}\n" +
                                      $"Stack Trace: {ex.StackTrace}";

                EventLog.WriteEntry(_SourceName, errorMessage, logType);
            }
            catch(Exception)
            {

            }
        }
    }
}
