using System;
using System.Xml.Linq;

namespace Web.Base
{
    public class Config
    {
        private static Config _instance;
        public static Config INSTANCE
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config();
                    _instance.ReadConfig();
                }

                return _instance;
            }
        }

        public string UnauthenticatedRedirect { get; private set; }
        public string CookieDomain { get; private set; }

        private void ReadConfig()
        {
            XDocument xDocument = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "\\WebConfig.xml");
            XElement root = xDocument.Root;
            UnauthenticatedRedirect = root.Element("unauthenticatedRedirect")?.Value;
            CookieDomain = root.Element("cookieDomain")?.Value;
        }
    }
}