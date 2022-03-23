using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public class Audit : IDisposable
    {



        private static EventLog customLog = null;
        const string SourceName = "LoggerService.Audit";
        const string LogName = "Application";

        static Audit()
        {
            try
            {
                if (!EventLog.SourceExists(SourceName))
                {
                    EventLog.CreateEventSource(SourceName, LogName);
                }

                customLog = new EventLog(LogName, Environment.MachineName, SourceName);
            }
            catch (Exception e)
            {
                customLog = null;
                Console.WriteLine("Error while trying to create log handle. Error = {0}", e.Message);
            }
        }


        public static void AuthenticationSuccess(string userName)
        {
            //string UserAuthenticationSuccess = AuditEvents.UserAuthenticationSuccess;
            if (customLog != null)
            {
                string UserAuthenticationSuccess = AuditEvents.UserAuthenticationSuccess;
    
                string message = string.Format(UserAuthenticationSuccess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthenticationSuccess));
            }
        }

        public static void AuthorizationSuccess(string userName, string serviceName)
        {
           // string UserAuthorizationSuccess = AuditEvents.UserAuthorizationSuccess;
            if (customLog != null)
            {
                string UserAuthorizationSuccess = AuditEvents.UserAuthorizationSuccess;
                string message = string.Format(UserAuthorizationSuccess, userName, serviceName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthorizationSuccess));
            }
        }

        // Database creation logs
        public static void CreationSuccess(string userName)
        {
         //   string DbCreationSucess = AuditEvents.DbCreationSuccess;
            if (customLog != null)
            {
                string DbCreationSucess = AuditEvents.DbCreationSuccess;
                string message = string.Format(DbCreationSucess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.DbCreationSuccess));
            }
        }

        public static void CreationFailed(string userName)
        {
            string DbCreationFailed = AuditEvents.DbCreationFailed;
            if (customLog != null)
            {
                string message = string.Format(DbCreationFailed, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.DbCreationFailed));
            }
        }

        // Add item logs
        public static void AddSuccess(string userName)
        {
            string AddItemSuccess = AuditEvents.AddItemSuccess;
            if (customLog != null)
            {
                string message = string.Format(AddItemSuccess, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.AddItemSuccess));
            }
        }

        public static void AddFailed(string userName)
        {
            string AddItemFailed = AuditEvents.AddItemFailed;
            if (customLog != null)
            {
                string message = string.Format(AddItemFailed, userName);
                customLog.WriteEntry(message);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.AddItemFailed));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="serviceName"> should be read from the OperationContext as follows: OperationContext.Current.IncomingMessageHeaders.Action</param>
        /// <param name="reason">permission name</param>
        public static void AuthorizationFailed(string userName, string serviceName, string reason)
        {
            string UserAuthorizationFailed = AuditEvents.UserAuthorizationFailed;
            if (customLog != null)
            {
                string message = string.Format(UserAuthorizationFailed, userName, serviceName, reason);
                customLog.WriteEntry(message, EventLogEntryType.Error);
            }
            else
            {
                throw new ArgumentException(string.Format("Error while trying to write event (eventid = {0}) to event log.", (int)AuditEventTypes.UserAuthorizationFailed));
            }
        }

        public void Dispose()
        {
            if (customLog != null)
            {
                customLog.Dispose();
                customLog = null;
            }
        }
    }
}
