using CRiC_Meteo.Models;
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
    /// <summary>
    /// Логика взаимодействия для ConfigPage.xaml
    /// </summary>
    public partial class ConfigPage : UserControl
    {
        public ConfigPage()
        {
            InitializeComponent();
            butMeltingFrozing.Click += ButMeltingFrozing_Click;
            but_TestSnowMapCalc.Click += SnowCalcMapTest;
        }

        private void ButMeltingFrozing_Click(object sender, RoutedEventArgs e)
        {
            BasseinFrozingMelting bfm;
            List<BasseinFrozingMelting> bfmT = new List<BasseinFrozingMelting>();
            for (int i = 0; i < 9; i++)
            {
                bfm = new BasseinFrozingMelting();
                bfm.basseinIndex = i;
                bfm.basseinName = "BasseinName";
                bfm.frozingPer = new List<double>() { 0, 10, 36, 66, 80, 90, 95, 97, 100 };
                bfm.frozintT = new List<double>() { 1, 0.5, 0, -0.6, -1.4, -3.1, -5.2, -8, -15 };

                bfm.meltingV = new List<double>() { 0, 0.03, 0.08, 0.16, 0.24, 0.33, 0.51, 0.7, 0.72 };
                bfm.meltingT = new List<double>() { -1, 0, 0.5, 1, 1.5, 2, 4, 10, 15 };
                bfmT.Add(bfm);
            }

            BasseinFrozingMelting.UpdateXML(bfmT);

        }

        private void SnowCalcMapTest(object sender, RoutedEventArgs e)
        {
            MapCalc mc = new MapCalc();
        }
    }
}
