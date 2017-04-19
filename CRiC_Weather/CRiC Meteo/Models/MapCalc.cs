using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRiC_Meteo.Models
{
    class MapCalc
    {
        class LocalMeteoPoint
        {
            public string indexWMO { get; set; }
            public int basseinIndex { get; set; }
            public double koordMP_X { get; set; }
            public double koordMP_Y { get; set; }
            public double distanceFromPoint { get; set; }

            public LocalMeteoPoint(string indexWMO, int basseinIndex, double koordMP_X, double koordMP_Y)
            {
                this.indexWMO = indexWMO;
                this.basseinIndex = basseinIndex;
                this.koordMP_X = koordMP_X;
                this.koordMP_Y = koordMP_Y;
                distanceFromPoint = 100;
            }

            public void CalcDistance(double koord_X, double koord_Y)
            {
                distanceFromPoint = System.Math.Sqrt((koordMP_X - koord_X) * (koordMP_X - koord_X) + (koordMP_Y - koord_Y) * (koordMP_Y - koord_Y));
            }
        }

        int ncols, nrows;
        int NODATA_value;
        double xllcorner, yllcorner;
        int cellsize;
        int[,] basseinFile;
        List<CalculatedValueInMeteoStation> scvMeteoStation;
        List<CalculatedValueInMapPoint> cMP;

        public MapCalc()
        {
            ReadMapFile(@"d:\git_CRiK\CRiC_Weather\CRiC Meteo\XML_snowCalc\Ivan.txt");
            CalcSnowFile();
        }

        private void ReadMapFile(string WayToMap)
        {
            int k = 0;
            var logFile = File.ReadAllLines(WayToMap);
            List<string> ls = new List<string>(logFile);
            string[] tmpStr;
            tmpStr = ls[0].Split('\t');
            ncols = Convert.ToInt32(tmpStr[1]);

            tmpStr = ls[1].Split('\t');
            nrows = Convert.ToInt32(tmpStr[1]);

            tmpStr = ls[4].Split('\t');
            cellsize = Convert.ToInt32(tmpStr[1]);

            tmpStr = ls[2].Split('\t');
            xllcorner = Convert.ToDouble(tmpStr[1]);

            tmpStr = ls[3].Split('\t');
            yllcorner = Convert.ToDouble(tmpStr[1]) + (nrows-1) * cellsize;

            tmpStr = ls[5].Split('\t');
            NODATA_value = Convert.ToInt32(tmpStr[1]);
            basseinFile = new int[nrows, ncols];

            for (int i = 6; i < ls.Count; i++)
            {
                tmpStr = ls[i].Split('\t');
                for (int j = 0; j < ncols; j++)
                {
                    basseinFile[k, j] = Convert.ToInt32(tmpStr[j]);
                }
                k++;
            }
        }

        private void CalcSnowFile()
        {
            int tmpIndex;
            //Получиние списка всех метеостаций из xml файла с координатами и названиями
            List<MeteoStaionWMO_index> lms = MeteoStaionWMO_index.ReadXML();
            //Расчетные данные по каждой метеостанции
            scvMeteoStation = CalculatedValueInMeteoStation.ReadXML("testCalc");
            //Пересечеие множества всех метеостанций и множества МС с расчетными данными
            List<LocalMeteoPoint> lmp = new List<LocalMeteoPoint>();
            foreach (MeteoStaionWMO_index item in lms)
            {
                tmpIndex = scvMeteoStation.FindIndex(s=>s.indexWMO_DB== item.indexWMO);
                if (tmpIndex!=-1) { lmp.Add(new LocalMeteoPoint(item.indexWMO.ToString(), item.basseinIndex, item.location_Xm, item.location_Ym));}
            }

            cMP = new List<CalculatedValueInMapPoint>();
            for (int i = 0; i < nrows; i++)
            {
                for (int j = 0; j < ncols; j++)
                {
                    if (basseinFile[i,j]!=NODATA_value)
                        cMP.Add(new CalculatedValueInMapPoint() { koordMP_Y = yllcorner - i * cellsize, koordMP_X = xllcorner + j * cellsize });
                }
            }

            //Parallel.For(0, cMP.Count, delegate (int i) { SnowValueInOnePoint(cMP[i], lmp, 10); });

            foreach (CalculatedValueInMapPoint item in cMP)
            {
                SnowValueInOnePoint(item, lmp, 10);
            }

            CalculatedValueInMapPoint.UpdateXML(cMP, "TestSnowMap");
        }

        private void SnowValueInOnePoint(CalculatedValueInMapPoint pointToCalc, List<LocalMeteoPoint> curLMP, int interpolationKoeff)
        {
            double fr_Numerator = 0, fr_Denominator = 0;
            double snowMS = 0;
            foreach (LocalMeteoPoint item in curLMP)
            {
                item.CalcDistance(pointToCalc.koordMP_X, pointToCalc.koordMP_Y);
            }
            curLMP = curLMP.OrderBy(o => o.distanceFromPoint).ToList();

            for (int i = 0; i <= interpolationKoeff; i++)
            {
                if (i== curLMP.Count-1)
                {
                    break;
                }

                snowMS = scvMeteoStation.First(s => s.indexWMO_DB == curLMP[i].indexWMO).snowValue;
                fr_Denominator += 1 / (curLMP[i].distanceFromPoint * curLMP[i].distanceFromPoint);
                fr_Numerator += snowMS / (curLMP[i].distanceFromPoint * curLMP[i].distanceFromPoint);
            }

            if (fr_Denominator != 0)
            {
                pointToCalc.snowValue = fr_Numerator / fr_Denominator;
                if (pointToCalc.snowValue>9000)
                {
                    int a = 5;
                }
            }
        }
    }
}
