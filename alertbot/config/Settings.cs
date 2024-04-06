using servicecontrolhub.storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servicecontrolhub.config
{
    public class Settings
    {

        #region properties
        public Config config { get; set; } = new();
        #endregion

        #region singletone
        private static Settings instance;
        private Settings() {
            var storage = new Storage<Config>("config", config);
            config = storage.load();
        }

        public static Settings getInstance()
        {
            if (instance == null)
                instance = new Settings();  
            return instance;
        }
        #endregion

    }

    public class receiver_settings  
    {
        public int port { get; set; } = 5050;
    }

    public class keepalive_settins
    {
        public string url { get; set; } = "";
        public int period { get; set; } = 10;
    }

    public class bot_settings
    {
        public string token { get; set; } = "";
        public string user_password { get; set; } = "4444";
        public string admin_password { get; set; } = "5555";
    }

    public class Config
    {
        public bot_settings bot { get; set; } = new();
        public receiver_settings receiver { get; set; } = new();
        public keepalive_settins keepalive { get; set; } = new();        

        public Config()
        {            
        }
    }

}
