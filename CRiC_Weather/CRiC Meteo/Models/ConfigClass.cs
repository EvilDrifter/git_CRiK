using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CRiC_Meteo.Models
{
    class ConfigClass  //https://habrahabr.ru/post/24673/
    {
        public string FolderWithDataWay { get; set; }
        public int curYearForCalc { get; set; }
        string fileXML = "config.xml";

        public ConfigClass(string way, int year)
        {
            if (way.Length > 5) { FolderWithDataWay = way; }
            else { FolderWithDataWay = "Suddenly missing an important folder"; }

            if (year > 1900) { curYearForCalc = year; }
            else { curYearForCalc = 9999; }
        }
        public ConfigClass()
        {
            FolderWithDataWay = "Suddenly missing an important folder";
            curYearForCalc = 9999;
        }

        public bool makeXMLConfigFile()
        {
            bool result = false;
            try
            {
                XDocument doc = new XDocument(new XElement("ProgrammConfig",
                new XElement("FolderWithData", FolderWithDataWay),
                new XElement("Year", curYearForCalc)));
                doc.Save(fileXML);
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }
        public bool readXMLConfigFile()
        {
            bool result = false;

            try
            {
                XDocument doc = XDocument.Load(fileXML);
                foreach (XElement el in doc.Root.Elements())
                {
                    if (el.Name == "FolderWithData")
                    {
                        FolderWithDataWay = Convert.ToString(el.Value);
                    }
                    else if (el.Name == "Year")
                    {
                        curYearForCalc = Convert.ToInt32(el.Value);
                    }

                }


                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }

    [Serializable]
    public class MeteoStaionWMO_index
    {
        public string indexWMO;
        public string Name_meteoSta;
        public string basseinIndex;
        public string location_Xm, location_Ym;
        bool hasBeenChanged;

        public MeteoStaionWMO_index(string index, string name, string bassein, string Xm, string Ym)
        {
            this.indexWMO = index;
            this.Name_meteoSta = name;
            this.basseinIndex = bassein;
            this.location_Xm = Xm;
            this.location_Ym = Ym;
            hasBeenChanged = true;
        }
        public MeteoStaionWMO_index()
        {
            hasBeenChanged = false;
        }

        public static List<MeteoStaionWMO_index> ReadCSVFile(string wayToFile)
        {
            string[] fileAll = File.ReadAllLines(wayToFile);
            string[] curLine;
            List<MeteoStaionWMO_index> wmo = new List<MeteoStaionWMO_index>();
            for (int i = 0; i < fileAll.Length; i++)
            {
                curLine = fileAll[i].Split(';');
                wmo.Add(new MeteoStaionWMO_index(curLine[0], curLine[1], curLine[2], curLine[3], curLine[4]));
            }
            return wmo;
        }

        public static void UpdateXMLFile(List<MeteoStaionWMO_index> wmoList)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<MeteoStaionWMO_index>));
            using (FileStream fs = new FileStream("bassein.xml", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, wmoList);
            }
        }
        public static List<MeteoStaionWMO_index> ReadXMLFile()
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<MeteoStaionWMO_index>));
            using (FileStream fs = new FileStream("bassein.xml", FileMode.OpenOrCreate))
            {
                List<MeteoStaionWMO_index> wmo = (List<MeteoStaionWMO_index>)formatter.Deserialize(fs);
                return wmo;
            }

        }
    }

    [Serializable]
    public class MySQLDataBaseConfig
    {
        public string msql_server;
        public string msql_userId;
        public string msql_password;
        public string msql_DB_name;

        public MySQLDataBaseConfig(string msql_server, string msql_userId, string msql_password, string msql_DB_name)
        {
            this.msql_DB_name = msql_DB_name;
            this.msql_password = msql_password;
            this.msql_server = msql_server;
            this.msql_userId = msql_userId;
        }
        public MySQLDataBaseConfig()
        {

        }

        public void ReadXMLFile()
        {
            if (!File.Exists("db_mysql_config.xml"))
            {
                UpdateXMLFIle(new MySQLDataBaseConfig("localhost", "root", "123456", "pogodat"));
                //conn_string.Server = "localhost";
                //conn_string.UserID = "root";
                //conn_string.Password = "123456";
                //conn_string.Database = DataBaseName;
            }

            XmlSerializer formatter = new XmlSerializer(typeof(MySQLDataBaseConfig));
            using (FileStream fs = new FileStream("db_mysql_config.xml", FileMode.OpenOrCreate))
            {
                MySQLDataBaseConfig wmo = (MySQLDataBaseConfig)formatter.Deserialize(fs);
                this.msql_DB_name = wmo.msql_DB_name;
                this.msql_password = wmo.msql_password;
                this.msql_server = wmo.msql_server;
                this.msql_userId = wmo.msql_userId;
            }
        }
        public void UpdateXMLFIle(MySQLDataBaseConfig msConfig)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(MySQLDataBaseConfig));
            if (msConfig!=this || !File.Exists("db_mysql_config.xml"))
            {
                using (FileStream fs = new FileStream("db_mysql_config.xml", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, msConfig);
                }
            }
        }
    }

    [Serializable]
    public class BasseinFrozingMelting: WR_Xml
    {
        public int basseinIndex;
        public List<double> meltingT, meltingV;
        public List<double> frozintT, frozingPer;
    }

    public abstract class WR_Xml
    {
        public virtual void UpdateXMLFile<T>(T fileToMakeXMLFrom, string fileNameXML)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            {
                using (FileStream fs = new FileStream(fileNameXML, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, fileToMakeXMLFrom);
                }
            }
        }

        public virtual void ReadXMLFile<T> (ref T fileToMakeXMLFrom, string fileNameXML)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(fileNameXML, FileMode.Open))
            {
                fileToMakeXMLFrom = (T)formatter.Deserialize(fs);
            }
        }
    }
}
