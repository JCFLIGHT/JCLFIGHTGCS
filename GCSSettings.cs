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
        public static int GCSFrequency = 50;
        public static int GCSRate = 20;
        public static int GCSSpeech = 0;
        public static int GCSRebootBoard = 0;
        public static int GCSAutoWP = 0;
        public static int GCSTrackLength = 200;
        public static int GCSAirPorts = 1;
        public static int GCSTrackSize = 5;

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
            XMLWrite.WriteStartElement("GCSSpeech Valor = \"" + GCSSpeech + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSRebootBoard Valor = \"" + GCSRebootBoard + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSAutoWP Valor = \"" + GCSAutoWP + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSTrackLength Valor = \"" + GCSTrackLength + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSAirPorts Valor = \"" + GCSAirPorts + "\""); XMLWrite.WriteEndElement();
            XMLWrite.WriteStartElement("GCSTrackSize Valor = \"" + GCSTrackSize + "\""); XMLWrite.WriteEndElement();

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

                            if (String.Compare(XMLReader.Name, "GCSSpeech", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSSpeech = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSRebootBoard", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSRebootBoard = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSAutoWP", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSAutoWP = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSTrackLength", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSTrackLength = Convert.ToInt32(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSAirPorts", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSAirPorts = Convert.ToByte(XMLReader.GetAttribute("Valor"));
                            }

                            if (String.Compare(XMLReader.Name, "GCSTrackSize", true) == 0 && XMLReader.HasAttributes)
                            {
                                GCSTrackSize = Convert.ToByte(XMLReader.GetAttribute("Valor"));
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
