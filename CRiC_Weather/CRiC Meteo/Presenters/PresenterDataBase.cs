using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using CRiC_Meteo.Models;
using CRiC_Meteo.Elements;

namespace CRiC_Meteo.Presenters
{
    public interface IPresenterDataBase
    {
        void UpdateForm_PrBarHTMLReader(int MaxValueOfPrBar);  //Обновление Бара для чтения HTML
        void UpdateForm_PrBarSQL(int MaxValueOfPrBar);         //Обновление Бара для записи в БД
    }

    public class PresenterDataBase: IPresenterDataBase
    {
        WeatherForecast Cl_wf;
        ConfigClass configFile;
        interface_UC_DataBase interfaceUC_DB;

        #region Данные
        public string FolderWithData { get; set; }
        public string YearToCalc { get; set; }
        #endregion Данные

        public void ShowUserControl(ContentControl cc)
        {
            cc.Content = null;
            LoadConfigFile();
            cc.Content = new uc_DataBase(this, ref interfaceUC_DB);        
        }

        public void LoadConfigFile()
        {
            configFile = new ConfigClass();
            configFile = configFile.ReadXML();
            FolderWithData = configFile.FolderWithDataWay;
            YearToCalc = configFile.curYearForCalc.ToString();
        }
        public void UpdateConfigFile()
        {
            if (configFile.FolderWithDataWay != interfaceUC_DB.GetFolderWayFromUserControl || configFile.curYearForCalc.ToString() != interfaceUC_DB.GetYearToCalcFromUserControl)
            {
                configFile.FolderWithDataWay = interfaceUC_DB.GetFolderWayFromUserControl;
                configFile.curYearForCalc = Convert.ToInt32(interfaceUC_DB.GetYearToCalcFromUserControl);
                configFile.UpdateXML(configFile);
            }
        }

        public void view_UpdateWeatherForecast(string[] filesInFolder, int YearOfCalc)
        {
            Cl_wf = new WeatherForecast(filesInFolder, YearOfCalc, this);
            Task t = Task.Run(() => Cl_wf.WeatherForecastUpdate_start());
        }

        #region Обновление ProgressBar
        public void UpdateForm_PrBarHTMLReader(int MaxValueOfPrBar)
        {
            interfaceUC_DB.UpdateProgressBar_HTML(MaxValueOfPrBar);
        }
        public void UpdateForm_PrBarSQL(int MaxValueOfPrBar)
        {
            interfaceUC_DB.UpdateProgressBar_SQL(MaxValueOfPrBar);
        }
        #endregion Обновление ProgressBar
    }
}
