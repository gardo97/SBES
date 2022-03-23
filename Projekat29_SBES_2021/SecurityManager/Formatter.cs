        
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityManager
{
    public class Formatter
    {

		public static string ParseGroup(string name)
		{
			string group = "";

			// Dobije se OU=Admins,
			group = name.Substring(name.IndexOf("OU=")).Split(' ')[0];

			// Dobije se Admins, 
			group = group.Substring(group.IndexOf("=") + 1);
           
            if(group.Contains("Admins"))
            {
                group = group.Remove(group.Length - 2);
            }
            else
            {
                group = group.Remove(group.Length - 1);
            }
            // I samo izbrisemo zarez na kraju


            return group;

		}
        public static string ParseName(string winLogonName)
        {
            string[] parts = new string[] { };

            if (winLogonName.Contains("@"))
            {
                ///UPN format
                parts = winLogonName.Split('@');
                return parts[0];
            }
            else if (winLogonName.Contains("\\"))
            {
                /// SPN format
                parts = winLogonName.Split('\\');
                return parts[1];
            }
            else if (winLogonName.Contains("CN"))
            {
                // sertifikati, name je formiran kao CN=imeKorisnika;
                int startIndex = winLogonName.IndexOf("=") + 1;
                int endIndex = winLogonName.IndexOf(";");
                string s = winLogonName.Substring(startIndex, endIndex - startIndex);
                return s;
            }
            else
            {
                return winLogonName;
            }
        }
    }
}
