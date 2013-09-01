using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using System.Xml;
namespace XMLUtility.Models
{
    using System.Collections.Generic;

    public class EnvironmentSettings
    {
        /// <summary>
        /// Schema definition file
        /// </summary>
        
        private string schemaPath;
        /// <summary>
        /// For future use
        /// </summary>
       
        public Dictionary<string, string> Mappings;  

        public EnvironmentSettings()
        {
           Mappings = new Dictionary<string, string>();
        }

        public EnvironmentSettings(string schema)
        {
            schemaPath = schema;
            Mappings = new Dictionary<string, string>();
        }
        //Need to test this part of code.
        //reads schema definition file and stores settings.
        public void LoadSettings()
        {
           XmlReader reader = new XmlTextReader(schemaPath);
            while (reader.Read())
            {
                string preToken = "";
                string preTokenType = "";
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (reader.Name)
                        {
                            case "Property" :
                                preTokenType = "Property";
                                break;
                            case "Map":
                                preToken = "Map";
                                break;
                        }
                        break;
                   case XmlNodeType.Text:
                        switch (preTokenType)
                        {
                            case "Property":
                                preToken = reader.Value;
                                break;
                            case "Map":
                                preTokenType = "Map";
                                Mappings[preToken] = reader.Value;
                                break;
                        }
                        break;
                }
            }
        }


    }
}
