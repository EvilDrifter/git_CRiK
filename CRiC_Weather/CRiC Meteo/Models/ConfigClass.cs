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
    [Serializable]
    public class ConfigClass: WR_Xml
    {
        private string fileXMLName = "config.xml";
        public string FolderWithDataWay { get; set; }
        public int curYearForCalc { get; set; }

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
            base.UpdateXMLFile(mainConfig, fileXMLName);
        }

        public ConfigClass ReadXML()
        {
            ConfigClass mainConfig = new ConfigClass();
            if (!File.Exists(fileXMLName))
            {
                UpdateXML(new ConfigClass("Folder with months", 9999));
            }

            base.ReadXMLFile(ref mainConfig, fileXMLName);

            this.FolderWithDataWay = mainConfig.FolderWithDataWay;
            this.curYearForCalc = mainConfig.curYearForCalc;

            return mainConfig;
        }
    }

    [Serializable]
    public class MeteoStaionWMO_index: WR_Xml
    {
        private string fileXMLName = "bassein.xml";
        public string indexWMO;
        public string Name_meteoSta;
        public string basseinIndex;
        public string location_Xm, location_Ym;

        public MeteoStaionWMO_index(string index, string name, string bassein, string Xm, string Ym)
        {
            this.indexWMO = index;
            this.Name_meteoSta = name;
            this.basseinIndex = bassein;
            this.location_Xm = Xm;
            this.location_Ym = Ym;
        }
        public MeteoStaionWMO_index() {}

        public void UpdateXML(List<MeteoStaionWMO_index> wmoList)
        {
            base.UpdateXMLFile(wmoList, fileXMLName);
        }
        public List<MeteoStaionWMO_index> ReadXML()
        {
            List<MeteoStaionWMO_index> wmoList = new List<MeteoStaionWMO_index>();
            if (!File.Exists(fileXMLName))
            {
                wmoList = ReadCSVFile(@"d:\GeekBrains\git_CRiK\CRiC_Weather\Bassein.csv");
            }

            wmoList = new List<MeteoStaionWMO_index>();
            base.ReadXMLFile(ref wmoList, fileXMLName);

            return wmoList;
        }

        private List<MeteoStaionWMO_index> ReadCSVFile(string wayToFile)
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
    }

    [Serializable]
    public class MySQLDataBaseConfig: WR_Xml
    {
        private string fileXMLName = "db_mysql_config.xml";
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
            base.UpdateXMLFile(msConfig, fileXMLName);
        }
        public MySQLDataBaseConfig ReadXML()
        {
            MySQLDataBaseConfig msConfig = new MySQLDataBaseConfig();
            if (!File.Exists(fileXMLName))
            {
                UpdateXML(new MySQLDataBaseConfig("localhost", "root", "123456", "pogoda"));
            }

            base.ReadXMLFile(ref msConfig, fileXMLName);

            this.msql_DB_name = msConfig.msql_DB_name;
            this.msql_password = msConfig.msql_password;
            this.msql_server = msConfig.msql_server;
            this.msql_userId = msConfig.msql_userId;


            return msConfig;
        }
    }

    [Serializable]
    public class BasseinFrozingMelting: WR_Xml
    {
        private string fileXMLName = "FrozingMelting.xml";
        public int basseinIndex;
        public List<double> meltingT, meltingV;
        public List<double> frozintT, frozingPer;

        public void UpdateXML(List<BasseinFrozingMelting> frmel)
        {
            base.UpdateXMLFile(frmel, fileXMLName);
        }
        public List<BasseinFrozingMelting> ReadXML()
        {
            List<BasseinFrozingMelting> frmel = new List<BasseinFrozingMelting>();
            base.ReadXMLFile(ref frmel, fileXMLName);
            return frmel;
        }
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
