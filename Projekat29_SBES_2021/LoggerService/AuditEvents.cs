using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace LoggerService
{
    public enum AuditEventTypes
    {
        UserAuthenticationSuccess = 0,
        UserAuthorizationSuccess = 1,
        UserAuthorizationFailed = 2,
        DbCreationSuccess = 3,
        DbCreationFailed = 4,
        AddItemSuccess = 5,
        AddItemFailed = 6,
        DbReadSuccess = 7,
        DbReadFailed = 8,
    }
    public class AuditEvents
    {
        private static ResourceManager resourceManager = null;
        private static object resourceLock = new object();

        private static ResourceManager ResourceMgr
        {
            get
            {
                lock (resourceLock)
                {
                    if (resourceManager == null)
                    {
                        resourceManager = new ResourceManager(typeof(AuditEventFile).FullName, Assembly.GetExecutingAssembly());
                    }
                    return resourceManager;
                }
            }
        }

        public static string UserAuthenticationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthenticationSuccess.ToString());
            }
        }

        public static string UserAuthorizationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthorizationSuccess.ToString());
            }
        }

        public static string UserAuthorizationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.UserAuthorizationFailed.ToString());
            }
        }

        public static string DbCreationSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DbCreationSuccess.ToString());
            }
        }

        public static string DbCreationFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DbCreationFailed.ToString());
            }
        }

        public static string AddItemSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AddItemSuccess.ToString());
            }
        }

        public static string AddItemFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.AddItemFailed.ToString());
            }
        }

        public static string DbReadSuccess
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DbReadSuccess.ToString());
            }
        }

        public static string DbReadFailed
        {
            get
            {
                return ResourceMgr.GetString(AuditEventTypes.DbReadFailed.ToString());
            }
        }
    }
}
