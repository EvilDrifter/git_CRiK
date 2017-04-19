using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CRiC_Meteo.Models
{
    [Serializable]
    public class ConfigClass
    {
        private string fileXMLName = "../../XML_configFile/config.xml";
        public string FolderWithDataWay { get; set; }
        public int curYearForCalc { get; set; }
        public string FolderToDownloadHTML { get; set; }
        public int YearToDownloadHTML { get; set; }

        public ConfigClass(string way, int year)
        {
            FolderWithDataWay = way;
            curYearForCalc = year;
        }
        public ConfigClass()
        {
            FolderWithDataWay = "Suddenly missing an important folder";
            curYearForCalc = 9999;
        }

        public void UpdateXML(ConfigClass mainConfig)
        {
            WR_Xml.UpdateXMLFile(mainConfig, fileXMLName);
        }

        public ConfigClass ReadXML()
        {
            ConfigClass mainConfig = new ConfigClass();
            if (!File.Exists(fileXMLName))
            {
                UpdateXML(new ConfigClass("Folder with months", 9999));
            }

            WR_Xml.ReadXMLFile(ref mainConfig, fileXMLName);

            this.FolderWithDataWay = mainConfig.FolderWithDataWay;
            this.curYearForCalc = mainConfig.curYearForCalc;

            return mainConfig;
        }
    }

    [Serializable]
    public class MeteoStaionWMO_index
    {
        public string indexWMO;
        public string Name_meteoSta;
        public int basseinIndex;
        public double location_Xm, location_Ym;

        public MeteoStaionWMO_index(string index, string name, int bassein, double Xm, double Ym)
        {
            this.indexWMO = index;
            this.Name_meteoSta = name;
            this.basseinIndex = bassein;
            this.location_Xm = Xm;
            this.location_Ym = Ym;
        }
        public MeteoStaionWMO_index() {}

        public static void UpdateXML(List<MeteoStaionWMO_index> wmoList)
        {
            WR_Xml.UpdateXMLFile(wmoList, "../../XML_configFile/bassein.xml");
        }
        public static List<MeteoStaionWMO_index> ReadXML()
        {
            List<MeteoStaionWMO_index> wmoList = new List<MeteoStaionWMO_index>();
            if (!File.Exists("../../XML_configFile/bassein.xml"))
            {
                MessageBox.Show("File bassein.xml is missing.\nIt has to be in root folder XML_configFile");
            }

            wmoList = new List<MeteoStaionWMO_index>();
            WR_Xml.ReadXMLFile(ref wmoList, "../../XML_configFile/bassein.xml");

            return wmoList;
        }
    }

    [Serializable]
    public class MySQLDataBaseConfig
    {
        private string fileXMLName = "../../XML_configFile/db_mysql_config.xml";
        public string msql_server;      //localhost
        public string msql_userId;      //root
        public string msql_password;    //123456
        public string msql_DB_name;     //pogodat

        public MySQLDataBaseConfig(string msql_server, string msql_userId, string msql_password, string msql_DB_name)
        {
            this.msql_DB_name = msql_DB_name;
            this.msql_password = msql_password;
            this.msql_server = msql_server;
            this.msql_userId = msql_userId;
        }
        public MySQLDataBaseConfig() {}


        public void UpdateXML(MySQLDataBaseConfig msConfig)
        {
            WR_Xml.UpdateXMLFile(msConfig, fileXMLName);
        }
        public MySQLDataBaseConfig ReadXML()
        {
            MySQLDataBaseConfig msConfig = new MySQLDataBaseConfig();
            if (!File.Exists(fileXMLName))
            {
                UpdateXML(new MySQLDataBaseConfig("localhost", "root", "123456", "pogoda"));
            }

            WR_Xml.ReadXMLFile(ref msConfig, fileXMLName);

            this.msql_DB_name = msConfig.msql_DB_name;
            this.msql_password = msConfig.msql_password;
            this.msql_server = msConfig.msql_server;
            this.msql_userId = msConfig.msql_userId;


            return msConfig;
        }
    }

    [Serializable]
    public class BasseinFrozingMelting
    {
        public int basseinIndex;
        public string basseinName;
        public List<double> meltingT, meltingV;
        public List<double> frozintT, frozingPer;

        public static void UpdateXML(List<BasseinFrozingMelting> frmel)
        {
            WR_Xml.UpdateXMLFile(frmel, "../../XML_configFile/FrozingMelting.xml");
        }
        public static List<BasseinFrozingMelting> ReadXML()
        {
            List<BasseinFrozingMelting> frmel = new List<BasseinFrozingMelting>();
            WR_Xml.ReadXMLFile(ref frmel, "../../XML_configFile/FrozingMelting.xml");
            return frmel;
        }
    }

    [Serializable]
    public class ConfigForCalc
    {
        private string fileXMLName = "../../XML_configFile/config_forcalc.xml";
        //public bool rejim_meteostation, rejim_bassein;
        public DateTime date_start, date_fin;
        //public string comboboxValue;

        public ConfigForCalc() {}
        public ConfigForCalc(DateTime date_start, DateTime date_fin)
        {
            //this.rejim_bassein = rejim_bassein;
            //this.rejim_meteostation = rejim_meteostation;
            //this.comboboxValue = comboboxValue;
            this.date_start = date_start;
            this.date_fin = date_fin;
        }

        public void UpdateXML(ConfigForCalc calcConfig)
        {
            WR_Xml.UpdateXMLFile(calcConfig, fileXMLName);
        }
        public ConfigForCalc ReadXML()
        {
            ConfigForCalc calcConfig = new ConfigForCalc();
            if (!File.Exists(fileXMLName))
            {
                UpdateXML(new ConfigForCalc(new DateTime(2016, 1, 1), new DateTime(2016, 4, 1)));
            }
            WR_Xml.ReadXMLFile(ref calcConfig, fileXMLName);

            //this.rejim_bassein = calcConfig.rejim_bassein;
            //this.rejim_meteostation = calcConfig.rejim_meteostation;
            //this.comboboxValue = calcConfig.comboboxValue;
            this.date_start = calcConfig.date_start;
            this.date_fin = calcConfig.date_fin;

            return calcConfig;
        }
    }

    public static class WR_Xml
    {
        public static void UpdateXMLFile<T>(T fileToMakeXMLFrom, string fileNameXML)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            {
                using (FileStream fs = new FileStream(fileNameXML, FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, fileToMakeXMLFrom);
                }
            }
        }

        public static void ReadXMLFile<T>(ref T fileToMakeXMLFrom, string fileNameXML)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(T));
            using (FileStream fs = new FileStream(fileNameXML, FileMode.Open))
            {
                fileToMakeXMLFrom = (T)formatter.Deserialize(fs);
            }
        }
    }
}
