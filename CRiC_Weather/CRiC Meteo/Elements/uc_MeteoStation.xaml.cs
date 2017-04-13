using CRiC_Meteo.Models;
using CRiC_Meteo.Presenters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRiC_Meteo.Elements
{

    public interface interface_UC_MeteoStation
    {
        void UpdateMeteoStationInfoOnForm(MeteoStaionWMO_index ms, DataTable dt_ms);
    }

    public partial class uc_MeteoStation : UserControl, interface_UC_MeteoStation
    {
        IPresenterBassein inf_bassein;
        MeteoStaionWMO_index curMS_info;
        DataTable curMS_DT;

        public uc_MeteoStation(IPresenterBassein inf_bassein, List<MeteoStaionWMO_index> lms,  ref interface_UC_MeteoStation k)
        {
            InitializeComponent();
            UpdateListView(lms);
            this.inf_bassein = inf_bassein;
            lv_allStation.SelectionChanged += (s, e) => { LvItemHasBeenChanged(); };
            k = this;
        }

        public void UpdateListView(List<MeteoStaionWMO_index> lms)
        {
            lv_allStation.ItemsSource = lms.Select(s => s.Name_meteoSta);
        }

        public void UpdateMeteoStationInfoOnForm(MeteoStaionWMO_index ms, DataTable dt_ms)
        {
            curMS_info = ms;
            curMS_DT = dt_ms;
            txbms_name.Text = ms.Name_meteoSta;
            txbms_wmo.Text = ms.indexWMO;
            txbms_bassein.Text = ms.basseinIndex.ToString();
            txbms_Xm.Text = ms.location_Xm;
            txbms_Ym.Text = ms.location_Ym;

            if (curMS_DT!=null) {dg_OneMeteoStationInfo.ItemsSource = curMS_DT.DefaultView;}
            else {dg_OneMeteoStationInfo.Items.Clear();}
        }

        void LvItemHasBeenChanged()
        {
            if (lv_allStation.SelectedItems.Count>0)
            {
                inf_bassein.GetInfoByName(lv_allStation.SelectedValue.ToString());
            }
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    List<BasseinFrozingMelting> frmelList = new List<BasseinFrozingMelting>();

        //    BasseinFrozingMelting brm = new BasseinFrozingMelting();
        //    brm.UpdateXMLFile(frmelList, "FrozingMelting.xml");
        //    List<BasseinFrozingMelting> frmelList3 = new List<BasseinFrozingMelting>();
        //    brm.ReadXMLFile(ref frmelList3, "FrozingMelting.xml");
        //}
    }
}
