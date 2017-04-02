using CRiC_Meteo.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRiC_Meteo.Models
{
    class DataConstructor
    {
        public string FileName { get; set; }
        public string[] HTMLFile { get; set; }
        public int YearOf { get; set; }

        public DataConstructor(string FileName, string[] HTMLFile, int YearOf)
        {
            this.FileName = FileName;
            this.HTMLFile = HTMLFile;
            this.YearOf = YearOf;
        }
    }

    class WeatherForecast
    {
        IPresenterDataBase presInterface;
        string[] filesInFolder;
        int year;
        List<MeteoStation> List_MeteoStation;
        MeteoStation tMS;
        List<DataConstructor> List_DataConstuctor;

        public WeatherForecast(string[] filesInFolder, int YearOfCalc, IPresenterDataBase presInterface)
        {
            this.filesInFolder = filesInFolder;
            this.year = YearOfCalc;
            this.presInterface = presInterface;
        }

        public void WeatherForecastUpdate_start()
        {
            //Чтение HTML файлов и создание DataTable и meteoStruct как стурктуры с полями
            List_DataConstuctor = new List<DataConstructor>();
            UpdateDataFrom_HTML_Files();

            List_MeteoStation = new List<MeteoStation>();
            Task t = Task.Run(() => UpdateDataForEachMeteoStation());
            Task.WaitAll(t);

            //Обновление MySQL Базы данных, в скобках - название БД и итерфейс для взаимодействия с главным окном
            MySQLDataBaseConfig msConfig = new MySQLDataBaseConfig();
            msConfig.ReadXMLFile();
            MySQL_Worker msqlW = new MySQL_Worker(msConfig, presInterface);
            t = Task.Run(() => msqlW.UpdateDataBaseFromMeteoStationData(List_MeteoStation, 12));    //Список метеостанций для обработки и количество потоков для работы (желательно <=12)
            Task.WaitAll(t);
        }
        
        void UpdateDataFrom_HTML_Files()
        {
            foreach (string item in filesInFolder)
            {
                List_DataConstuctor.Add(new DataConstructor(item, System.IO.File.ReadAllLines(item), year));
            }
        }
        void UpdateDataForEachMeteoStation()
        {
            presInterface.UpdateForm_PrBarHTMLReader(List_DataConstuctor.Count);
            foreach (DataConstructor item in List_DataConstuctor)
            {
                tMS = new MeteoStation(item);
                if (tMS.GetInfoAboutMeteoStaAs_DataTable.Rows.Count>0)
                {
                    List_MeteoStation.Add(tMS);
                }
                presInterface.UpdateForm_PrBarHTMLReader(List_DataConstuctor.Count);
            }  
        }
    }
}
