using System;
using System.Collections.Generic;
using System.Data;


namespace CRiC_Meteo.Models
{
    class SnowCalc
    {
        public class StructForCalc
        {
            public List<DateTime> pointTime_b, pointTime_f;
            public List<double> temp_T;
            public List<double> precipitation;
            public List<double> precipitationByMonth;
            public List<string> monthIndex;
            public List<double> snowData;
            public StructForCalc()
            {
                pointTime_b = new List<DateTime>();
                pointTime_f = new List<DateTime>();
                temp_T = new List<double>();
                precipitation = new List<double>();
                precipitationByMonth = new List<double>();
                monthIndex = new List<string>();
            }
        }
        StructForCalc str3_15, str6_18, str_fin;
        public StructForCalc MeteoCalcBy12Hours { get { return str_fin; } }

        MeteoStation.meteoData meteoDataAsList;
        public void SnowCalcByIndexSta(DataTable dt, BasseinFrozingMelting bfm)
        {
            //PointTime     - "Дата"
            //temp_T        - "T - Температура воздуха (C)"
            //precipitation - "R - Количество осадков (мм)"
            meteoDataAsList = new MeteoStation().GetInfoAboutMeteoStaAs_StructFromDataTable(dt);

            str3_15 = new StructForCalc();
            str6_18 = new StructForCalc();
            str_fin = new StructForCalc();

            SeparateDataBy_12_hours(ref str3_15, 3, 15);
            SeparateDataBy_12_hours(ref str6_18, 6, 18);
            CombineStruct(str3_15, str6_18, ref str_fin);
        }

        private void SeparateDataBy_12_hours(ref StructForCalc str, int h1, int h2)
        {
            int monthIndex = 0;
            int k = 0; //Переменная для обратного счета на 12 часов
            int index12=-1;
            double numerator=0, denominator=0; //числитель и знаменатель для определения средневзвешенной температуры за период;

            for (int i = 0; i < meteoDataAsList.PointTime.Count; i++)
            {
                if (meteoDataAsList.PointTime[i].Hour == h1 || meteoDataAsList.PointTime[i].Hour == h2)
                {
                    str.pointTime_b.Add(meteoDataAsList.PointTime[i].AddHours(-12));
                    index12++;
                    str.pointTime_f.Add(meteoDataAsList.PointTime[i]);
                    k = i;

                    if (meteoDataAsList.precipitation[i]!=MeteoStation.emptyValue || meteoDataAsList.precipitation[i]>30)
                    {
                        str.precipitation.Add(meteoDataAsList.precipitation[i]);
                    }
                    else { str.precipitation.Add(0); }

                    if (monthIndex == meteoDataAsList.PointTime[i].Month)
                    {
                        str.precipitationByMonth[str.precipitationByMonth.Count - 1] += str.precipitation[str.precipitation.Count - 1];
                    }
                    else
                    {
                        str.precipitationByMonth.Add(str.precipitation[str.precipitation.Count - 1]);
                        monthIndex = meteoDataAsList.PointTime[i].Month;
                        str.monthIndex.Add($"{meteoDataAsList.PointTime[i].Year}_{monthIndex.ToString("00")}");
                    }
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
                    } while ((meteoDataAsList.PointTime[k]>= str.pointTime_b[index12]) && k>0);

                    if (denominator != 0) {str.temp_T.Add(numerator / denominator); }
                    else {str.temp_T.Add(meteoDataAsList.temp_T[i]); }
                    k = 0;
                }


            }
        }
        private void CombineStruct(StructForCalc str3_15, StructForCalc str6_18, ref StructForCalc str_f)
        {
            int index3_15=0, index6_18=0, i=0;
            List<string> allMonth = new List<string>();
            foreach (string item in str3_15.monthIndex) {allMonth.Add(item);}
            foreach (string item in str6_18.monthIndex) {allMonth.Add(item);}
            allMonth.Sort();

            do
            {
                if (allMonth[i+1] == allMonth[i])
                {
                    allMonth.RemoveAt(i+1);
                }
                i++;
            } while (i<allMonth.Count);

            foreach (string item in allMonth)
            {
                index3_15 = str3_15.monthIndex.FindIndex(k => k == item);
                index6_18 = str6_18.monthIndex.FindIndex(k => k == item);

                if (index3_15!=-1 && index6_18!=-1)
                {
                    if (str3_15.precipitationByMonth[index3_15] > str6_18.precipitationByMonth[index6_18])
                    {
                        MergeTwoStruct(str3_15, ref str_f, str3_15.monthIndex[index3_15]);
                        str_f.monthIndex.Add(str3_15.monthIndex[index3_15]);
                        str_f.precipitationByMonth.Add(str3_15.precipitationByMonth[index3_15]);
                    }
                    else
                    {
                        MergeTwoStruct(str6_18, ref str_f, str6_18.monthIndex[index6_18]);
                        str_f.monthIndex.Add(str6_18.monthIndex[index6_18]);
                        str_f.precipitationByMonth.Add(str6_18.precipitationByMonth[index6_18]);
                    }
                }
                else if (index3_15 != -1 && index6_18 == -1)
                {
                    MergeTwoStruct(str3_15, ref str_f, str3_15.monthIndex[index3_15]);
                    str_f.monthIndex.Add(str3_15.monthIndex[index3_15]);
                    str_f.precipitationByMonth.Add(str3_15.precipitationByMonth[index3_15]);
                }
                else
                {
                    MergeTwoStruct(str6_18, ref str_f, str6_18.monthIndex[index6_18]);
                    str_f.monthIndex.Add(str6_18.monthIndex[index6_18]);
                    str_f.precipitationByMonth.Add(str6_18.precipitationByMonth[index6_18]);
                }
            }
        }
        private void CalcSnowFormation(ref StructForCalc str_f, BasseinFrozingMelting bfm)
        {
            str_f.snowData = new List<double>();
            alglib.spline1dinterpolant s_formation, s_melting;
            alglib.spline1dbuildlinear(bfm.frozintT.ToArray(), bfm.frozingPer.ToArray(), out s_formation);
            alglib.spline1dbuildlinear(bfm.meltingT.ToArray(), bfm.meltingV.ToArray(), out s_melting);

            double frSn=0, melSn=0;
            for (int i = 0; i < str_f.pointTime_f.Count; i++)
            {
                if (str_f.temp_T[i]<=bfm.frozintT[0])
                {
                    frSn = 1;
                }
                if (str_f.temp_T[i]>=bfm.meltingT[0])
                {
                    melSn = 1;
                }

                if (i>1) { str_f.snowData.Add(str_f.snowData[i - 1] + frSn - melSn); }
                else { str_f.snowData.Add(frSn - melSn); }

                if (str_f.snowData[i]<0) { str_f.snowData[i] = 0; }
            }
        }

        private void MergeTwoStruct(StructForCalc str, ref StructForCalc str_f, string year_month)
        {
            string[] t = year_month.Split('_');
            int yearC = Convert.ToInt16(t[0]);
            int monthC = Convert.ToInt16(t[1]);

            for (int i = 0; i < str.pointTime_f.Count; i++)
            {
                if (str.pointTime_f[i].Month==monthC && str.pointTime_f[i].Year==yearC)
                {
                    str_f.pointTime_b.Add(str.pointTime_b[i]);
                    str_f.pointTime_f.Add(str.pointTime_f[i]);

                    str_f.temp_T.Add(str.temp_T[i]);
                    str_f.precipitation.Add(str.precipitation[i]);
                }
                if (str.pointTime_f[i].Year > yearC)
                {
                    break;
                }
            }

        }
    }
}
