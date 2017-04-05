using CRiC_Meteo.Elements;
using CRiC_Meteo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CRiC_Meteo.Presenters
{
    public interface IPresenterBassein
    {
        void GetInfoByName(string meteoName);
    }

    class PresenterBassein: IPresenterBassein
    {
        List<MeteoStaionWMO_index> lms { get; set; }
        interface_UC_MeteoStation inf_Ms;

        public void ShowUserControl(StackPanel stPanel)
        {
            stPanel.Children.Clear();
            openXML();
            stPanel.Children.Add(new uc_MeteoStation(this, lms, ref inf_Ms));
        }

        private List<MeteoStaionWMO_index> openXML()
        {
            lms = new List<MeteoStaionWMO_index>();
            MeteoStaionWMO_index t = new MeteoStaionWMO_index();
            lms = t.ReadXML();
            return lms;
        }

        public void GetInfoByName(string meteoName)
        {
            MeteoStaionWMO_index ms = lms.Single(s => s.Name_meteoSta == meteoName);
            MySQL_Worker msqlW;
            MySQLDataBaseConfig msConfig;
            DataTable msqlW_byIndex;
            if (ms!=null)
            {
                msConfig = new MySQLDataBaseConfig();
                msConfig = msConfig.ReadXML();
                msqlW = new MySQL_Worker(msConfig);
                msqlW_byIndex = new DataTable();
                try
                {
                    msqlW_byIndex = msqlW.GetDT_ByIndex("st_" + ms.indexWMO);
                }
                catch (Exception)
                {
                    msqlW_byIndex = null;
                }

                inf_Ms.UpdateMeteoStationInfoOnForm(ms, msqlW_byIndex);
            }
        }
    }
}
