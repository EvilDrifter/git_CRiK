﻿using CRiC_Meteo.Elements;
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
    public interface IPresenterOxyPlot
    {
        List<BasseinFrozingMelting> lfm { get; }
        List<string> tableNamesFromDataBase { get; }
        void SnowFormation();
        void LoadConfig();
    }

    class Presenter_OxyPlot: IPresenterOxyPlot
    {
        interface_UC_SnowCalc inf_SnC;
        #region реализация интерфейса IPresenterOxyPlot

        public List<BasseinFrozingMelting> lfm
        {
            get {
                BasseinFrozingMelting t = new BasseinFrozingMelting();
                return t.ReadXML();
                }
        }

        public List<string> tableNamesFromDataBase
        {
            get
            {               
                MySQL_Worker t = new MySQL_Worker(new MySQLDataBaseConfig().ReadXML());
                return t.GetTableList();
            }
        }

        public void SnowFormation()
        {
            //SaveConfig();
            SnowCalc sc = new SnowCalc();

            if (inf_SnC.begTime != null && inf_SnC.endTime != null)
            {
                if (inf_SnC.selectedIndexSta != "")
                {
                    DataTable dt = new MySQL_Worker(new MySQLDataBaseConfig().ReadXML()).GetDT_ByIndex(inf_SnC.selectedIndexSta, inf_SnC.begTime, inf_SnC.endTime);
                    sc.SnowCalcByIndexSta(dt);
                }
                else if (inf_SnC.selectedBassein != "")
                {
                    //Расчет снега для всего бассейна
                }
                else
                {
                    MessageBox.Show("Необходимо выбрать режим расчета");
                }
            }
            else { MessageBox.Show("Не выбраны даты для расчета"); }

        }

        public void LoadConfig()
        {
            if (inf_SnC!=null)
            {
                ConfigForCalc c = new ConfigForCalc().ReadXML();
                inf_SnC.begTime = c.date_start;
                inf_SnC.endTime = c.date_fin;
            }
        }

        #endregion реализация интерфейса IPresenterOxyPlot

        public void SaveConfig()
        {
            if (inf_SnC != null)
            {
                ConfigForCalc c = new ConfigForCalc();
                c.date_start = inf_SnC.begTime;
                c.date_fin = inf_SnC.endTime;
                c.UpdateXML(c);
            }
        }

        public void ShowUserControl(ContentControl cc)
        {
            cc.Content = null;
            cc.Content = new uc_SnowCalc(this, ref inf_SnC);
        }


    }
}
