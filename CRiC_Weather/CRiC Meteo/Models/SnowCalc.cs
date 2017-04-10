using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRiC_Meteo.Models
{
    class SnowCalc
    {
        public void SnowCalcByIndexSta(DataTable dt)
        {
            MeteoStation.meteoData md = new MeteoStation().GetInfoAboutMeteoStaAs_StructFromDataTable(dt);
        }
    }
}
