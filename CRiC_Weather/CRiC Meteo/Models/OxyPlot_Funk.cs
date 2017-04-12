using CRiC_Meteo.Elements;
using OxyPlot;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using static CRiC_Meteo.Models.SnowCalc;

namespace CRiC_Meteo.Models
{
    class OxyPlot_Funk
    {
        public static PlotModel DrawFrMelFunk(ObservableCollection<TempValue> DataListForUpdate, GraphInstal grIn)
        {
            var tmp = new PlotModel { Title = grIn.TitleText, Subtitle = grIn.SubTitleText };
            var series = new LineSeries { Title = grIn.NameSeries, MarkerType = MarkerType.Circle };
            var linearAxisX = new LinearAxis();
            var linearAxisY = new LinearAxis();
            linearAxisX.Position = AxisPosition.Bottom;

            linearAxisX.Title = grIn.NameX;
            linearAxisY.Title = grIn.NameY;

            foreach (TempValue item in DataListForUpdate)
            {
                series.Points.Add(new DataPoint(item.Temperature, item.ValueByTemp));
            }

            tmp.Axes.Add(linearAxisX);
            tmp.Axes.Add(linearAxisY);
            tmp.Series.Add(series);
            return tmp;
        }

        public static PlotModel DrawSnowFormationStation(StructForCalc snowList)
        {
            var plotModel1 = new PlotModel();
            plotModel1.LegendSymbolLength = 24;
            plotModel1.Title = "TwoColorAreaSeries";
            var linearAxis1 = new LinearAxis();
            linearAxis1.MajorGridlineStyle = LineStyle.Solid;
            linearAxis1.MinorGridlineStyle = LineStyle.Solid;
            linearAxis1.MinorTickSize = 0;
            linearAxis1.Title = "Temperature";
            linearAxis1.Unit = "°C";
            plotModel1.Axes.Add(linearAxis1);
            var linearAxis2 = new DateTimeAxis();
            linearAxis2.MajorGridlineStyle = LineStyle.Solid;
            linearAxis2.MinorGridlineStyle = LineStyle.Solid;
            linearAxis2.Position = AxisPosition.Bottom;
            linearAxis2.Title = "Date";
            plotModel1.Axes.Add(linearAxis2);
            var linearAxis3 = new LinearAxis();
            linearAxis3.MajorGridlineStyle = LineStyle.Solid;
            linearAxis3.MinorGridlineStyle = LineStyle.Solid;
            linearAxis3.PositionTier = 1;
            linearAxis3.MinorTickSize = 0;
            linearAxis3.Title = "Water in snow";
            linearAxis3.Unit = "mm";
            linearAxis3.Position = AxisPosition.Right;
            plotModel1.Axes.Add(linearAxis3);


            var twoColorAreaSeries1 = new TwoColorAreaSeries();
            twoColorAreaSeries1.MarkerFill2 = OxyColors.LightBlue;
            twoColorAreaSeries1.Limit = 0;
            twoColorAreaSeries1.Color2 = OxyColors.LightBlue;
            twoColorAreaSeries1.Color = OxyColors.Tomato;
            twoColorAreaSeries1.MarkerFill = OxyColors.Tomato;
            twoColorAreaSeries1.MarkerType = MarkerType.None;

            twoColorAreaSeries1.Title = "Temperature at StaIndex";
            twoColorAreaSeries1.TrackerFormatString = "{4:0.0} °C";
            for (int i = 0; i < snowList.pointTime_f.Count; i++)
            {
                twoColorAreaSeries1.Points.Add(new DataPoint(snowList.pointTime_f[i].ToOADate(), snowList.temp_T[i]));
            }


            var SnowCalcSeries = new LineSeries();
            SnowCalcSeries.Title = "Water in snow";
            SnowCalcSeries.TrackerFormatString = "{4:0.0} mm";

            for (int i = 0; i < snowList.pointTime_f.Count; i++)
            {
                SnowCalcSeries.Points.Add(new DataPoint(snowList.pointTime_f[i].ToOADate(), snowList.snowData[i]));
            }

            plotModel1.Series.Add(twoColorAreaSeries1);
            plotModel1.Series.Add(SnowCalcSeries);
            return plotModel1;
        }
    }
}