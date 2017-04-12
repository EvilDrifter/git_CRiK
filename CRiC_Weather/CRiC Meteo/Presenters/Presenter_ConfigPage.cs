using CRiC_Meteo.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CRiC_Meteo.Presenters
{
    class Presenter_ConfigPage
    {
        public void ShowUserControl(ContentControl cc)
        {
            cc.Content = null;
            cc.Content = new ConfigPage();
        }
    }
}
