using CRiC_Meteo.Elements;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;

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
    }
}