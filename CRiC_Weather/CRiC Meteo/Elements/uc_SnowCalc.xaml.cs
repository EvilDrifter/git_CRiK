using CRiC_Meteo.Models;
using CRiC_Meteo.Presenters;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace CRiC_Meteo.Elements
{
    enum FunkFrozingMelting { накопление, таяние };

    public interface interface_OxyPlot
    {
        void UpdateFrozingMelting();
    }

    public class TempValue
    {
        private double _t;
        private double _v;

        public double Temperature
        {
            get { return _t; }
            set { _t = value; }
        }
        public double ValueByTemp
        {
            get { return _v; }
            set { _v = value; }
        }

    }

    public struct GraphInstal
    {
        public string TitleText, SubTitleText;
        public int MaxX, MinX;
        public int MaxY, MinY;
        public string NameX, NameY;
        public string NameSeries;
    }

    public partial class uc_SnowCalc : UserControl
    {
        IPresenterOxyPlot prestnterOxyPlot;

        public ObservableCollection<TempValue> DataListForUpdate = new ObservableCollection<TempValue>();
        private GraphInstal grIn;

        public uc_SnowCalc(IPresenterOxyPlot prestnterOxyPlot)
        {
            this.prestnterOxyPlot = prestnterOxyPlot;
            InitializeComponent();
            grid_FrMelFunk.CellEditEnding += Grid_FrMelFunk_CellEditEnding;
            cmb_basseinIndex.SelectionChanged += IndexComboboxChanged;
            cmb_FrozingMelting.SelectionChanged += IndexComboboxChanged;
            grid_FrMelFunk.DataContext = DataListForUpdate;
            UpdateComboBoxes();
        }

        private void IndexComboboxChanged(object sender, SelectionChangedEventArgs e)
        {
            grIn = new GraphInstal();
            UpdateTableWithFunk();
            oxyPlot1.Model = OxyPlot_Funk.DrawFrMelFunk(DataListForUpdate, grIn);
        }

        private void Grid_FrMelFunk_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            int rowIndex = e.Row.GetIndex();
            int colIndex = e.Column.DisplayIndex;
            var el = e.EditingElement as TextBox;
            double valueAfter = Convert.ToDouble(el.Text);
        }

        private void UpdateComboBoxes()
        {
            cmb_basseinIndex.ItemsSource = prestnterOxyPlot.lfm.Select(s => s.basseinIndex);
            cmb_FrozingMelting.ItemsSource = Enum.GetValues(typeof(FunkFrozingMelting)).Cast<FunkFrozingMelting>();
            cmb_basseinIndex.SelectedItem = 0;
            cmb_FrozingMelting.SelectedItem = 0;
        }

        private void UpdateTableWithFunk()
        {
            int index = cmb_FrozingMelting.SelectedIndex;
            int index2 = Convert.ToInt16(cmb_basseinIndex.SelectedItem.ToString());
            BasseinFrozingMelting t = new BasseinFrozingMelting();
            t = prestnterOxyPlot.lfm.Find(s => s.basseinIndex == index2);

            DataListForUpdate.Clear();
            grIn.NameX = "Температура [C]";
            grIn.SubTitleText = index2.ToString();

            grid_FrMelFunk.Columns[0].Header = "Температура";
            if (index == 0)
            {
                grid_FrMelFunk.Columns[1].Header = "% от осадков";
                for (int i = 0; i < t.frozintT.Count; i++)
                {
                    DataListForUpdate.Add(new TempValue() { Temperature = t.frozintT[i], ValueByTemp = t.frozingPer[i] });
                }
                grIn.NameY = "% образовавшегося снега \n от выпавших остадков [%]";
                grIn.NameSeries = "снегонакопление";

            }
            if (index == 1)
            {
                grid_FrMelFunk.Columns[1].Header = "мм в час";
                for (int i = 0; i < t.meltingT.Count; i++)
                {
                    DataListForUpdate.Add(new TempValue() { Temperature = t.meltingT[i], ValueByTemp = t.meltingV[i] });
                }
                grIn.NameY = "скорость стаивания \n снега [мм/час]";
                grIn.NameSeries = "стаивание";
            }
        }
    }
}
