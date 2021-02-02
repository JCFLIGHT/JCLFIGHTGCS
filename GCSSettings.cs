using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace JCFLIGHTGCS
{
    class GCSSettings
    {
        public static int GCSFrequency = 10;
        public static int GCSRate = 100;

        public static void Save_To_XML(string FileName)
        {
            XmlTextWriter XMLWrite = new XmlTextWriter(FileName, null);
            XMLWrite.Formatting = Formatting.Indented;
            XMLWrite.Indentation = 4;
            XMLWrite.WriteStartDocument();
            XMLWrite.WriteStartElement("GCSPARAMETERS");

            //////////////////////////////////////////////////////////////////////////////////////////////////////////////

            XMLWrite.WriteStartElement("GCSFrequency Valor = \"" + GCSFrequency + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSRate Valor = \"" + GCSRate + "\""); XMLWrite.WriteEndElement();

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////

            XMLWrite.WriteEndElement();
            XMLWrite.WriteEndDocument();
            XMLWrite.Close();
        }

        public static bool Read_From_XML(string FileName)
        {
            XmlTextReader XMLReader;

            if (!File.Exists(FileName))
            {
                return false;
            }

            XMLReader = new XmlTextReader(FileName);
            try
            {
                while (XMLReader.Read())
                {
                    switch (XMLReader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (String.Compare(XMLReader.Name, "GCSRate", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSRate = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSFrequency", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSFrequency = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }
                            break;
                    }
                }
            }
            catch
            {
                return false;

            }
            finally
            {
                if (XMLReader != null)
                {
                    XMLReader.Close();
                }
            }
            return true;
        }
    }
}
