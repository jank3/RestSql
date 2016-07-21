using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RestSql.Data
{
    public class ConnectionSettings
    {
        public void ToXml(XmlWriter writer)
        {
            writer.WriteStartElement("ConnectionSettings");
            writer.WriteEndElement();
        }

        public virtual String ToXml()
        {
            StringBuilder xml = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(xml);
            ToXml(writer);
            writer.Flush();
            writer.Close();
            return xml.ToString();
        }

        public virtual void LoadXml(XmlNode node)
        {
            if(node.Name == "ConnectionSettings")
            {

            }
        }
    }
}
