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
    public interface IPresenterOxyPlot
    {
        List<BasseinFrozingMelting> lfm { get; }
        List<string> tableNamesFromDataBase { get; }
        void SnowFormation();
    }

    class Presenter_OxyPlot: IPresenterOxyPlot
    {
        interface_UC_SnowCalc inf_SnC;
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

        public void ShowUserControl(ContentControl cc)
        {
            cc.Content = null;
            cc.Content = new uc_SnowCalc(this, ref inf_SnC);
        }

        public void SnowFormation()
        {
            SnowCalc sc = new SnowCalc();

            if (inf_SnC.begTime != null && inf_SnC.endTime != null)
            {
                if (inf_SnC.selectedIndexSta != "")
                {
                    DataTable dt = new MySQL_Worker(new MySQLDataBaseConfig().ReadXML()).GetDT_ByIndex(inf_SnC.selectedIndexSta);
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
    }
}
