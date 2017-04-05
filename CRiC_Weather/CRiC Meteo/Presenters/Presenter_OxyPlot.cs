using CRiC_Meteo.Elements;
using CRiC_Meteo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CRiC_Meteo.Presenters
{
    public interface IPresenterOxyPlot
    {
        List<BasseinFrozingMelting> lfm { get; }
    }

    class Presenter_OxyPlot: IPresenterOxyPlot
    {
        public List<BasseinFrozingMelting> lfm
        {
            get {
                BasseinFrozingMelting t = new BasseinFrozingMelting();
                return t.ReadXML();
                }
        }

        public void ShowUserControl(StackPanel stPanel)
        {
            stPanel.Children.Clear();
            stPanel.Children.Add(new uc_SnowCalc(this));
        }
    }
}
