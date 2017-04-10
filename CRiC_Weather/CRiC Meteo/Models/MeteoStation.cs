using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRiC_Meteo.Models
{

    class MeteoStation
    {
        #region Data
        meteoData Info_Struct;
        DataTable Info_DataTable;
        DataConstructor curDataConstructor;
        static int noData = -999;
        public static int emptyValue { get {return noData; }  }

        public struct meteoData
        {
            public List<DateTime> PointTime;
            public List<string> wind_Direction;
            public List<int> wind_Speed;
            public List<string> visibility, phenomena, cloud;
            public List<double> temp_T, temp_Td;
            public List<int> humidity;
            public List<double> temp_Te, temp_Tes;
            public List<string> comfort;
            public List<double> pressure_P, pressure_P0, temp_Tmin, temp_Tmax, precipitation;
            public List<int> snow_width;
        }
        #endregion

        public meteoData GetInfoAboutMeteoStaAs_Struct { get { return Info_Struct;} }            //Метод возвращает структуру из List всех метеоданных
        public DataTable GetInfoAboutMeteoStaAs_DataTable { get { return Info_DataTable; } }     //Метод возвращает DataTable с метеоданными
        public meteoData GetInfoAboutMeteoStaAs_StructFromDataTable(DataTable dt)
        {
            setMeteoDataFields();
            foreach (DataRow d_row in dt.Rows)
            {
                Info_Struct.PointTime.Add(Convert.ToDateTime(d_row["Дата"]));
                Info_Struct.temp_T.Add(Convert.ToDouble(d_row["T - Температура воздуха (C)"]));
                Info_Struct.precipitation.Add(Convert.ToDouble(d_row["R - Количество осадков (мм)"]));
            }
            return Info_Struct;
        }

        public MeteoStation() { }
        public MeteoStation(DataConstructor curDC)
        {
            this.curDataConstructor = curDC;
            if (isWothToDoFile())
            {
                setMeteoDataFields();
                readStringWithData();
                MakeDataTableFromList();
            }
        }

        private bool isWothToDoFile()                       //Иногда попадаются пусте файлы с метеоданными, если таблица пустая объект не добавиться в список с "хорошими" станциями
        {
            if (curDataConstructor.HTMLFile.Length > 420)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void setMeteoDataFields()                   //Создание полей в структуре для их записи из TXT файла с погодой
        {
            Info_Struct = new meteoData();

            Info_Struct.PointTime = new List<DateTime>();
            Info_Struct.wind_Direction = new List<string>();
            Info_Struct.wind_Speed = new List<int>();
            Info_Struct.visibility = new List<string>();
            Info_Struct.phenomena = new List<string>();
            Info_Struct.cloud = new List<string>();
            Info_Struct.temp_T = new List<double>();
            Info_Struct.temp_Td = new List<double>();
            Info_Struct.humidity = new List<int>();
            Info_Struct.temp_Te = new List<double>();
            Info_Struct.temp_Tes = new List<double>();
            Info_Struct.comfort = new List<string>();
            Info_Struct.pressure_P = new List<double>();
            Info_Struct.pressure_P0 = new List<double>();
            Info_Struct.temp_Tmin = new List<double>();
            Info_Struct.temp_Tmax = new List<double>();
            Info_Struct.precipitation = new List<double>();
            Info_Struct.snow_width = new List<int>();
        }               

        private void readStringWithData()
        {
            int start = 0;
            string curHour, curDayYear;
            for (int i = 100; i < 130; i++)
            {
                if (curDataConstructor.HTMLFile[i].IndexOf("<td class") != -1)
                {
                    start = i;
                    break;
                }
            }


            if (start != 0)
            {
                for (int i = start; i < curDataConstructor.HTMLFile.Length; i = i + 21)
                {
                    if (curDataConstructor.HTMLFile[i] == "")
                    {
                        break;
                    }
                    curHour = readcurString("<b>", "</b>", curDataConstructor.HTMLFile[i]);
                    curDayYear = readcurString("<b>", "</b>", curDataConstructor.HTMLFile[i + 1]);

                    //Если удачно получилось получить дату, то считывание продолжается, если нет, то пропускается 21 строка
                    if (convertToDate(curHour, curDayYear, curDataConstructor.YearOf))
                    {
                        Info_Struct.wind_Direction.Add(readcurString(">", "</td>", curDataConstructor.HTMLFile[i + 2]));

                        intDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 3], ref Info_Struct.wind_Speed);

                        Info_Struct.visibility.Add(readcurString(">", "</td>", curDataConstructor.HTMLFile[i + 4]));
                        Info_Struct.phenomena.Add(readcurString(">", "</td>", curDataConstructor.HTMLFile[i + 5]));
                        Info_Struct.cloud.Add(readcurString(">", "<br>", curDataConstructor.HTMLFile[i + 6]));

                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 7], ref Info_Struct.temp_T);
                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 8], ref Info_Struct.temp_Td);
                        intDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 9], ref Info_Struct.humidity);
                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 10], ref Info_Struct.temp_Te);
                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 11], ref Info_Struct.temp_Tes);

                        Info_Struct.comfort.Add(readcurString(">", "</td>", curDataConstructor.HTMLFile[i + 12]));

                        doubleDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 13], ref Info_Struct.pressure_P);
                        doubleDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 14], ref Info_Struct.pressure_P0);
                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 15], ref Info_Struct.temp_Tmin);
                        doubleDataFromString("<nobr>", "</nobr>", curDataConstructor.HTMLFile[i + 16], ref Info_Struct.temp_Tmax);
                        doubleDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 17], ref Info_Struct.precipitation);
                        intDataFromString(">", "</td>", curDataConstructor.HTMLFile[i + 18], ref Info_Struct.snow_width);
                    }
                    else
                    {
                        //MessageBox.Show("Возникла ошибка\n" + curDataConstructor.MeteoNameFile);
                    }
                }

            }

            MakeDataTableFromList();


        }               //Основной метод для чтения HTML скаченного файла с последующим созданием DataTable для обновления SQL

        private bool convertToDate(string curHour, string curDayYear, int curYear)
        {
            string[] dayMonth = curDayYear.Split('.');
            try
            {
                Info_Struct.PointTime.Add(new DateTime(curYear, Convert.ToInt32(dayMonth[1]), Convert.ToInt32(dayMonth[0]), Convert.ToInt32(curHour), 0, 0));
                return true;
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                return false;
            }
        }           //Перевод сткрововой даты в формат даты
        private string readcurString(string str_beg, string str_fin, string curString)
        {
            string outString = "";
            int firstCharacter = curString.IndexOf(str_beg);
            int lastCharacter = curString.IndexOf(str_fin);

            if (lastCharacter - firstCharacter + str_beg.Length > 1)
            {
                outString = curString.Substring(firstCharacter + str_beg.Length, lastCharacter - firstCharacter - str_beg.Length);
            }

            return outString;
        }       //Получение данных из определнной стркоки

        private void doubleDataFromString(string str_beg, string str_fin, string curString, ref List<double> ls)
        {
            string answer = readcurString(str_beg, str_fin, curString);
            try
            {
                ls.Add(Convert.ToDouble(answer));
            }
            catch (Exception)
            {

                ls.Add(noData);
            }
        }   //Преобразование строки в double
        private void intDataFromString(string str_beg, string str_fin, string curString, ref List<int> ls)
        {
            string answer = readcurString(str_beg, str_fin, curString);
            try
            {
                ls.Add(Convert.ToInt32(answer));
            }
            catch (Exception)
            {

                ls.Add(noData);
            }
        }         //Преобразование строки в int

        private void MakeDataTableFromList()
        {
            string index = System.IO.Path.GetFileNameWithoutExtension(curDataConstructor.FileName);
            DataColumn[] d_col = new DataColumn[17];
            Info_DataTable = new DataTable("st_" + index);
            DataRow d_row;
            d_col[0] = new DataColumn("Дата", typeof(System.DateTime));
            d_col[1] = new DataColumn("Ветер направление", typeof(System.String));
            d_col[2] = new DataColumn("Ветер скорость (м/с)", typeof(System.Int32));
            d_col[3] = new DataColumn("Видимость", typeof(System.String));
            d_col[4] = new DataColumn("Явления", typeof(System.String));
            d_col[5] = new DataColumn("Облачность", typeof(System.String));
            d_col[6] = new DataColumn("T - Температура воздуха (C)", typeof(System.Decimal));
            d_col[7] = new DataColumn("Td - Температура точки росы  (C)", typeof(System.Decimal));
            d_col[8] = new DataColumn("f - Относительная влажность воздуха (%)", typeof(System.Int32));
            d_col[9] = new DataColumn("Te - Эффективная температура (C)", typeof(System.Decimal));
            d_col[10] = new DataColumn("Tes - Эффективная температура на солнце (C)", typeof(System.Decimal));
            d_col[11] = new DataColumn("P - Атмосферное давление (гПа)", typeof(System.Decimal));
            d_col[12] = new DataColumn("Po - Атмосферное давление (гПа)", typeof(System.Decimal));
            d_col[13] = new DataColumn("Tmin - Минимальная температура (C)", typeof(System.Decimal));
            d_col[14] = new DataColumn("Tmax - Максимальная температура (C)", typeof(System.Decimal));
            d_col[15] = new DataColumn("R - Количество осадков (мм)", typeof(System.Decimal));
            d_col[16] = new DataColumn("S - Снежный покров (см)", typeof(System.Int32));
            for (int i = 0; i < d_col.Length; i++)
            {
                Info_DataTable.Columns.Add(d_col[i]);
            }

            //Info_DataTable.Columns["Дата"].Unique = true;


            for (int i = 0; i < Info_Struct.PointTime.Count; i++)
            {
                d_row = Info_DataTable.NewRow();
                d_row["Дата"] = Info_Struct.PointTime[i];
                d_row["Ветер направление"] = Info_Struct.wind_Direction[i];
                d_row["Ветер скорость (м/с)"] = Info_Struct.wind_Speed[i];
                d_row["Видимость"] = Info_Struct.visibility[i];
                d_row["Явления"] = Info_Struct.phenomena[i];
                d_row["Облачность"] = Info_Struct.cloud[i];
                d_row["T - Температура воздуха (C)"] = Info_Struct.temp_T[i];
                d_row["Td - Температура точки росы  (C)"] = Info_Struct.temp_Td[i];
                d_row["f - Относительная влажность воздуха (%)"] = Info_Struct.humidity[i];
                d_row["Te - Эффективная температура (C)"] = Info_Struct.temp_Te[i];
                d_row["Tes - Эффективная температура на солнце (C)"] = Info_Struct.temp_Tes[i];
                d_row["P - Атмосферное давление (гПа)"] = Info_Struct.pressure_P[i];
                d_row["Po - Атмосферное давление (гПа)"] = Info_Struct.pressure_P0[i];
                d_row["Tmin - Минимальная температура (C)"] = Info_Struct.temp_Tmin[i];
                d_row["Tmax - Максимальная температура (C)"] = Info_Struct.temp_Tmax[i];
                d_row["R - Количество осадков (мм)"] = Info_Struct.precipitation[i];
                d_row["S - Снежный покров (см)"] = Info_Struct.snow_width[i];

                Info_DataTable.Rows.Add(d_row);
            }
        }             //Создание из List => DataTable
    }
}
