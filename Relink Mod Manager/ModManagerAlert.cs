using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public class ModManagerAlert
    {
        public string MaxVersionShown { get; set; }
        public string Alert {  get; set; }
        public string Link { get; set; }

        public ModManagerAlert()
        {
            MaxVersionShown = "";
            Alert = "";
            Link = "";
        }

        public ModManagerAlert(string maxVersionShown, string alert, string link)
        {
            MaxVersionShown = maxVersionShown;
            Alert = alert;
            Link = link;
        }
    }
}
