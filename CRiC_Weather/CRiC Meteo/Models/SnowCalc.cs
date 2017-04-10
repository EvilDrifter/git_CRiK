using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRiC_Meteo.Models
{
    struct StructForCalc
    {
        public List<DateTime> pointTime;
        public List<double> temp_T;
        public List<double> precipitation;
    }

    class SnowCalc
    {
        MeteoStation.meteoData meteoDataAsList;
        public void SnowCalcByIndexSta(DataTable dt)
        {
            //PointTime     - "Дата"
            //temp_T        - "T - Температура воздуха (C)"
            //precipitation - "R - Количество осадков (мм)"
            meteoDataAsList = new MeteoStation().GetInfoAboutMeteoStaAs_StructFromDataTable(dt);

            StructForCalc str3_15, str6_18;
            str3_15 = new StructForCalc();
            str6_18 = new StructForCalc();
            SetFieldsInStruct(ref str3_15);
            SetFieldsInStruct(ref str6_18);

            SeparateDataBy_12_hours(ref str3_15, 3, 15);
            SeparateDataBy_12_hours(ref str6_18, 6, 18);


        }

        private void SetFieldsInStruct(ref StructForCalc str)
        {
            str.pointTime = new List<DateTime>();
            str.precipitation = new List<double>();
            str.temp_T = new List<double>();
        }
        private void SeparateDataBy_12_hours(ref StructForCalc str, int h1, int h2)
        {
            int k = 0; //Переменная для обратного счета на 12 часов
            int index12=-1;
            double numerator=0, denominator=0; //числитель и знаменатель для определения средневзвешенной температуры за период;

            for (int i = 0; i < meteoDataAsList.PointTime.Count; i++)
            {
                if (meteoDataAsList.PointTime[i].Hour == h1 || meteoDataAsList.PointTime[i].Hour == h2)
                {
                    str.pointTime.Add(meteoDataAsList.PointTime[i].AddHours(-12));
                    index12++;
                    str.pointTime.Add(meteoDataAsList.PointTime[i]);
                    k = i;

                    if (meteoDataAsList.precipitation[i]!=MeteoStation.emptyValue || meteoDataAsList.precipitation[i]>30)
                    {
                        str.precipitation.Add(meteoDataAsList.precipitation[i]);
                    }
                    else { str.precipitation.Add(0); }
                }

                if (k != 0)
                {
                    numerator = 0;
                    denominator = 0;
                    do
                    {
                        numerator += meteoDataAsList.temp_T[k]*(meteoDataAsList.PointTime[k]- meteoDataAsList.PointTime[k-1]).TotalHours;
                        denominator += (meteoDataAsList.PointTime[k] - meteoDataAsList.PointTime[k - 1]).TotalHours;
                        k--;
                    } while (meteoDataAsList.PointTime[k]>= str.pointTime[index12] || k==-1);
                }

                if (denominator!=0)
                {
                    str.temp_T.Add(numerator / denominator);
                }
                else
                {
                    str.temp_T.Add(meteoDataAsList.temp_T[i]);
                }
            }
        }
    }
}
