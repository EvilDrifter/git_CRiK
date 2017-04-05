using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;

namespace CRiC_Meteo.Models
{
    class OxyPlot_Funk
    {
        public PlotModel Model { get; private set; }

        public OxyPlot_Funk()
        {
            var plotModel1 = new PlotModel();
            plotModel1.IsLegendVisible = false;
            plotModel1.PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0);
            plotModel1.PlotMargins = new OxyThickness(60, 4, 60, 40);
            plotModel1.Title = "Average (Mean) monthly temperatures in 2003";
            var categoryAxis1 = new CategoryAxis();
            categoryAxis1.AxislineStyle = LineStyle.Solid;
            categoryAxis1.MinorStep = 1;
            //categoryAxis1.Labels.Add("Jan");
            //categoryAxis1.Labels.Add("Feb");
            //categoryAxis1.Labels.Add("Mar");
            //categoryAxis1.Labels.Add("Apr");
            //categoryAxis1.Labels.Add("May");
            //categoryAxis1.Labels.Add("Jun");
            //categoryAxis1.Labels.Add("Jul");
            //categoryAxis1.Labels.Add("Aug");
            //categoryAxis1.Labels.Add("Sep");
            //categoryAxis1.Labels.Add("Oct");
            //categoryAxis1.Labels.Add("Nov");
            //categoryAxis1.Labels.Add("Dec");
            categoryAxis1.ActualLabels.Add("Jan");
            categoryAxis1.ActualLabels.Add("Feb");
            categoryAxis1.ActualLabels.Add("Mar");
            categoryAxis1.ActualLabels.Add("Apr");
            categoryAxis1.ActualLabels.Add("May");
            categoryAxis1.ActualLabels.Add("Jun");
            categoryAxis1.ActualLabels.Add("Jul");
            categoryAxis1.ActualLabels.Add("Aug");
            categoryAxis1.ActualLabels.Add("Sep");
            categoryAxis1.ActualLabels.Add("Oct");
            categoryAxis1.ActualLabels.Add("Nov");
            categoryAxis1.ActualLabels.Add("Dec");
            plotModel1.Axes.Add(categoryAxis1);
            var linearAxis1 = new LinearAxis();
            linearAxis1.TickStyle = TickStyle.Crossing;
            linearAxis1.AxislineStyle = LineStyle.Solid;
            linearAxis1.Title = "Fahrenheit";
            plotModel1.Axes.Add(linearAxis1);
            var lineSeries1 = new LineSeries();
            lineSeries1.LineLegendPosition = LineLegendPosition.End;
            lineSeries1.Title = "Phoenix";
            lineSeries1.TrackerFormatString = "{0}: {4:0.0}ºF";
            lineSeries1.Points.Add(new DataPoint(0, 52.1));
            lineSeries1.Points.Add(new DataPoint(1, 55.1));
            lineSeries1.Points.Add(new DataPoint(2, 59.7));
            lineSeries1.Points.Add(new DataPoint(3, 67.7));
            lineSeries1.Points.Add(new DataPoint(4, 76.3));
            lineSeries1.Points.Add(new DataPoint(5, 84.6));
            lineSeries1.Points.Add(new DataPoint(6, 91.2));
            lineSeries1.Points.Add(new DataPoint(7, 89.1));
            lineSeries1.Points.Add(new DataPoint(8, 83.8));
            lineSeries1.Points.Add(new DataPoint(9, 72.2));
            lineSeries1.Points.Add(new DataPoint(10, 59.8));
            lineSeries1.Points.Add(new DataPoint(11, 52.5));
            plotModel1.Series.Add(lineSeries1);
            var lineSeries2 = new LineSeries();
            lineSeries2.LineLegendPosition = LineLegendPosition.End;
            lineSeries2.Title = "Raleigh";
            lineSeries2.TrackerFormatString = "{0}: {4:0.0}ºF";
            lineSeries2.Points.Add(new DataPoint(0, 40.5));
            lineSeries2.Points.Add(new DataPoint(1, 42.2));
            lineSeries2.Points.Add(new DataPoint(2, 49.2));
            lineSeries2.Points.Add(new DataPoint(3, 59.5));
            lineSeries2.Points.Add(new DataPoint(4, 67.4));
            lineSeries2.Points.Add(new DataPoint(5, 74.4));
            lineSeries2.Points.Add(new DataPoint(6, 77.5));
            lineSeries2.Points.Add(new DataPoint(7, 76.5));
            lineSeries2.Points.Add(new DataPoint(8, 70.6));
            lineSeries2.Points.Add(new DataPoint(9, 60.2));
            lineSeries2.Points.Add(new DataPoint(10, 50));
            lineSeries2.Points.Add(new DataPoint(11, 41.2));
            plotModel1.Series.Add(lineSeries2);
            var lineSeries3 = new LineSeries();
            lineSeries3.LineLegendPosition = LineLegendPosition.End;
            lineSeries3.Title = "Minneapolis";
            lineSeries3.TrackerFormatString = "{0}: {4:0.0}ºF";
            lineSeries3.Points.Add(new DataPoint(0, 12.2));
            lineSeries3.Points.Add(new DataPoint(1, 16.5));
            lineSeries3.Points.Add(new DataPoint(2, 28.3));
            lineSeries3.Points.Add(new DataPoint(3, 45.1));
            lineSeries3.Points.Add(new DataPoint(4, 57.1));
            lineSeries3.Points.Add(new DataPoint(5, 66.9));
            lineSeries3.Points.Add(new DataPoint(6, 71.9));
            lineSeries3.Points.Add(new DataPoint(7, 70.2));
            lineSeries3.Points.Add(new DataPoint(8, 60));
            lineSeries3.Points.Add(new DataPoint(9, 50));
            lineSeries3.Points.Add(new DataPoint(10, 32.4));
            lineSeries3.Points.Add(new DataPoint(11, 18.6));
            plotModel1.Series.Add(lineSeries3);

            this.Model = plotModel1;
        }
    }
}
