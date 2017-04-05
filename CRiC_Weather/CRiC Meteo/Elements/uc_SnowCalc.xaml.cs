using CRiC_Meteo.Models;
using CRiC_Meteo.Presenters;
using System;
using System.Collections.Generic;
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
    public interface interface_OxyPlot
    {
        void UpdateFrozingMelting();
    }

    enum FunkFrozingMelting { накопление, таяние };

    public partial class uc_SnowCalc : UserControl
    {
        IPresenterOxyPlot prestnterOxyPlot;

        public List<double> TempVal { get; set; }
        public List<double> ValFromTemp { get; set; }

        public uc_SnowCalc(IPresenterOxyPlot prestnterOxyPlot)
        {
            this.prestnterOxyPlot = prestnterOxyPlot;
            InitializeComponent();
            UpdateComboBoxes();
        }

        private void UpdateComboBoxes()
        {
            cmb_basseinIndex.ItemsSource = prestnterOxyPlot.lfm.Select(s => s.basseinIndex);
            cmb_basseinIndex.SelectedIndex = 0;
            cmb_FrozingMelting.ItemsSource = Enum.GetValues(typeof(FunkFrozingMelting)).Cast<FunkFrozingMelting>();
            cmb_FrozingMelting.SelectedIndex = 1;

            UpdateTableWithFunk();
        }

        private void UpdateTableWithFunk()
        {
            int index = cmb_FrozingMelting.SelectedIndex;
            int index2 = Convert.ToInt16(cmb_basseinIndex.SelectedItem.ToString());
            BasseinFrozingMelting t = new BasseinFrozingMelting();
            t = prestnterOxyPlot.lfm.Find(s => s.basseinIndex == index2);

            grid_FrMelFunk.Columns[0].Header = "Температура";
            if (index == 0)
            {
                grid_FrMelFunk.Columns[1].Header = "% от осадков";
                TempVal = t.frozintT;
                ValFromTemp = t.frozingPer;
            }
            if (index == 1)
            {
                grid_FrMelFunk.Columns[1].Header = "мм в час";
                TempVal = t.meltingT;
                ValFromTemp = t.meltingV;
            }


        }
    }
}
