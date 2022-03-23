using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class CustomPrincipal : IPrincipal
    {
        private GenericIdentity identity = null;
        //private WindowsIdentity identity = null;
        private string group = string.Empty;

        public CustomPrincipal(GenericIdentity genericIdentity)
        {
            this.identity = genericIdentity;

            Group = Formatter.ParseGroup(identity.Name);
        }

        public IIdentity Identity
        {
            get { return this.identity; }
        }
        public string Group{get;set;}
        public bool IsInRole(string permission)
        {
            string[] permissions;

            if (RolesConfig.GetPermissions(Group, out permissions))
            {
                foreach (string permision in permissions)
                {
                    if (permision.Equals(permission))
                        return true;
                }
            }
            return false;
           
        }
        public bool IsInRole2(string permission)
        {
            string[] permissions;

            if (RolesConfig.GetPermissions(Group, out permissions))
             {
                 foreach (string permision in permissions)
                  {
                     if (permision.Equals(permission))
                            return true;
                  }
             }
             return false;


        }
    }
}
