using CRiC_Meteo.Presenters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CRiC_Meteo.Models
{
    class WeatherDownload
    {
        string mainHTMLAddress;
        List<MeteoStaionWMO_index> lms;
        List<InfoForDownload> list_strinForDownload;
        string bday, fday, amonth, ayear;
        string folderToDownloadHTMLFiles;
        PresenterDataBase pDb;

        class InfoForDownload
        {
            public string strinForDownload { get; set; }
            public string clearIndex { get; set; }
            public InfoForDownload(string strinForDownload, string clearIndex)
            {
                this.clearIndex = clearIndex;
                this.strinForDownload = strinForDownload;
            }
        }

        public WeatherDownload(string folderToDownloadHTMLFiles, int MonthIndex, int YearIndex)
        {
            mainHTMLAddress = "http://www.pogodaiklimat.ru/weather.php?id=";
            this.folderToDownloadHTMLFiles = folderToDownloadHTMLFiles;
            lms = MeteoStaionWMO_index.ReadXML();
            bday = "1";
            fday = DateTime.DaysInMonth(YearIndex, MonthIndex).ToString();
            amonth = MonthIndex.ToString();
            ayear = YearIndex.ToString();
        }

        public void StartDownload(PresenterDataBase pDb)
        {
            this.pDb = pDb;
            this.pDb.UpdateButtonContextHTMLWork("Waiting for connection...");
            var watch = System.Diagnostics.Stopwatch.StartNew();

            fillStringForDownload();

            Task t = Task.Run(() => Parallel.For(0, list_strinForDownload.Count, delegate (int i) { DownloadEach(list_strinForDownload[i]); }));
            Task.WaitAll(t);
            watch.Stop();
            this.pDb.UpdateButtonContextHTMLWork($"Compleated in {watch.ElapsedMilliseconds} ms");
        }
        private void fillStringForDownload()
        {
            list_strinForDownload = new List<InfoForDownload>();
            string clearIndex;
            foreach (MeteoStaionWMO_index ms in lms)
            {
                clearIndex = ms.indexWMO.Replace("st_", "");
                list_strinForDownload.Add(new InfoForDownload
                                        (mainHTMLAddress + $"{clearIndex}&bday={bday}&fday={fday}&amonth={amonth}&ayear={ayear}",
                                        clearIndex )); 
            }
        }
        private void DownloadEach(InfoForDownload infoFile)
        {
            WebClient webClient = new WebClient();
            string answer = "";
            StreamWriter wr;

            answer = webClient.DownloadString(infoFile.strinForDownload);
            wr = new StreamWriter($"{folderToDownloadHTMLFiles}{infoFile.clearIndex}.txt");
            wr.WriteLine(answer);
            wr.Close();

            pDb.UpdateButtonContextHTMLWork(infoFile.clearIndex);
        }
    }
}
