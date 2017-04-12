using CRiC_Meteo.Models;
using CRiC_Meteo.Presenters;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using System.Windows;
using System.Collections.Generic;

namespace CRiC_Meteo.Elements
{
    enum FunkFrozingMelting { накопление, таяние };

    public interface interface_UC_SnowCalc
    {
        string selectedBassein { get; }
        string selectedIndexSta { get; }
        DateTime begTime { get; set; }
        DateTime endTime { get; set; }
        void DrawSnowFormation(SnowCalc.StructForCalc t);
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

    public partial class uc_SnowCalc : UserControl, interface_UC_SnowCalc
    {
        IPresenterOxyPlot prestnterOxyPlot;

        private List<string> tableNames;
        public ObservableCollection<TempValue> DataListForUpdate = new ObservableCollection<TempValue>();
        private GraphInstal grIn;

        #region реализация интерфейса interface_OxyPlot
        public string selectedBassein
        {
            get
            {
                if (rb_bassein.IsChecked == true) { return cmb_StaBassein.SelectedValue.ToString(); }
                else { return ""; }
            }
        }

        public string selectedIndexSta
        {
            get
            {
                if (rb_meteoSta.IsChecked == true) { return cmb_StaBassein.SelectedValue.ToString(); }
                else { return ""; }
            }
        }

        public DateTime begTime
        {
            get
            {
                DateTime? date = dp_beginCalc.SelectedDate;
                if (date != null) { return date.Value; }
                else { return default(DateTime); }
            }
            set { dp_beginCalc.SelectedDate = value; }
        }

        public DateTime endTime
        {
            get {
                DateTime? date = dp_endCalc.SelectedDate;
                if (date != null) { return date.Value; }
                else { return default(DateTime); }
            }
            set { dp_endCalc.SelectedDate = value; }
        }
        #endregion реализация интерфейса interface_OxyPlot


        public uc_SnowCalc(IPresenterOxyPlot prestnterOxyPlot, ref interface_UC_SnowCalc k)
        {
            this.prestnterOxyPlot = prestnterOxyPlot;
            k = this;
            InitializeComponent();

            btnRightMenuHide.Click += (s, e) => { MenuButtonHide(); };
            btnRightMenuShow.Click += (s, e) => { MenuButtonShow(); };
            but_doCalc.Click += (s, e) => { prestnterOxyPlot.SnowFormation(); };

            grid_FrMelFunk.CellEditEnding += Grid_FrMelFunk_CellEditEnding;
            cmb_basseinIndex.SelectionChanged += IndexComboboxChanged;
            cmb_FrozingMelting.SelectionChanged += IndexComboboxChanged;

            grid_FrMelFunk.DataContext = DataListForUpdate;
            UpdateComboBoxes();
            this.prestnterOxyPlot.LoadConfig();
        }


        #region МенюСправа
        public void MenuButtonHide()
        {
            ShowHideMenu("sbHideRightMenu", btnRightMenuHide, btnRightMenuShow, infoRightMenu);
        }
        public void MenuButtonShow()
        {
            ShowHideMenu("sbShowRightMenu", btnRightMenuHide, btnRightMenuShow, infoRightMenu);
        }
        private void ShowHideMenu(string Storyboard, Button btnHide, Button btnShow, StackPanel pnl)
        {
            Storyboard sb = Resources[Storyboard] as Storyboard;
            sb.Begin(pnl);

            if (Storyboard.Contains("Show"))
            {
                btnHide.Visibility = System.Windows.Visibility.Visible;
                btnShow.Visibility = System.Windows.Visibility.Hidden;
            }
            else if (Storyboard.Contains("Hide"))
            {
                btnHide.Visibility = System.Windows.Visibility.Hidden;
                btnShow.Visibility = System.Windows.Visibility.Visible;
            }
        }
        #endregion МенюСлева


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
            cmb_basseinIndex.ItemsSource = prestnterOxyPlot.lfm.Select(s => s.basseinName);
            cmb_FrozingMelting.ItemsSource = Enum.GetValues(typeof(FunkFrozingMelting)).Cast<FunkFrozingMelting>();
            cmb_basseinIndex.SelectedItem = 0;
            cmb_FrozingMelting.SelectedItem = 0;
        }

        private void UpdateTableWithFunk()
        {
            int index = cmb_FrozingMelting.SelectedIndex;
            int index2 = prestnterOxyPlot.lfm.First(s => s.basseinName == cmb_basseinIndex.SelectedItem.ToString()).basseinIndex; 
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

        private void meteoSta_Checked(object sender, RoutedEventArgs e)
        {
            if (tableNames==null)
            {
                tableNames = new List<string>();
                tableNames = prestnterOxyPlot.tableNamesFromDataBase;
            }
            RadioButton pressed = (RadioButton)sender;
            cmb_StaBassein.ItemsSource = tableNames;
        }

        private void bassein_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton pressed = (RadioButton)sender;
            cmb_StaBassein.ItemsSource = prestnterOxyPlot.lfm.Select(s => s.basseinIndex);
        }

        void interface_UC_SnowCalc.DrawSnowFormation(SnowCalc.StructForCalc str_fin)
        {
            oxyPlot2.Model = OxyPlot_Funk.DrawSnowFormationStation(str_fin);
        }
    }
}
