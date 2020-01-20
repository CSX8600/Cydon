using System;
using System.Xml.Linq;

namespace Cydon.Data.Base
{
    internal class Config
    {
        private static string path;
        public static string ConfigPath
        {
            set { path = value; }
        }

        private static Config instance;
        public static Config INSTANCE
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                }

                return instance;
            }
        }
        public string ConnectionString { get; private set; }

        private Config()
        {
            if (path == null)
            {
                path = AppContext.BaseDirectory;
                if (!path.EndsWith("\\"))
                {
                    path += "\\";
                }

                path += "CydonConfig.xml";
            }

            XDocument xDocument = XDocument.Load(path);
            ConnectionString = xDocument.Root.Element("connectionString").Value;
        }
    }
}
