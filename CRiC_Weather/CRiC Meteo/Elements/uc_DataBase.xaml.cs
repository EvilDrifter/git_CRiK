using CRiC_Meteo.Presenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRiC_Meteo.Elements
{
    public delegate void Delegate_UpdateProgressBar(int value_max);
    public interface interface_UC_DataBase
    {
        void UpdateProgressBar_HTML(int value_max);
        void UpdateProgressBar_SQL(int value_max);
    }
    public partial class uc_DataBase : UserControl, interface_UC_DataBase
    {
        PresenterDataBase dbPresenter;
        public uc_DataBase(PresenterDataBase dbPresenter, ref interface_UC_DataBase k)
        {
            InitializeComponent();
            this.dbPresenter = dbPresenter;
            txb_MainFolderWithMonth.Text = dbPresenter.FolderWithData;
            txb_YearForMeteo.Text = dbPresenter.YearToCalc;
            bnt_StartProcessWithMySQL.Click += (s, e) => { MainUpdateProcess(); };
            bnt_ShowMonthFolders.Click += (s, e) => { ShowMonthFolder(); };
            k = this;
        }

        private void ShowMonthInMainFolder()
        {
            lv_eachMonth.Items.Clear();

            string[] listOfFiles = System.IO.Directory.GetDirectories(txb_MainFolderWithMonth.Text);
            for (int i = 0; i < listOfFiles.Length; i++)
            {
                lv_eachMonth.Items.Add(new { pathToMonth = listOfFiles[i] });
            }
        }

        private void ShowMonthFolder()
        {
            lv_eachMonth.Items.Clear();

            string[] listOfFiles = System.IO.Directory.GetDirectories(txb_MainFolderWithMonth.Text);
            for (int i = 0; i < listOfFiles.Length; i++)
            {
                lv_eachMonth.Items.Add(new { pathToMonth = listOfFiles[i] });
            }
        }
        private void MainUpdateProcess()
        {
            dbPresenter.UpdateConfigFile();

            foreach (var item in lv_eachMonth.SelectedItems)
            {
                var selectedItem = (dynamic)item;
                string[] OneMonthFiles = System.IO.Directory.GetFiles(selectedItem.pathToMonth);
                dbPresenter.view_UpdateWeatherForecast(OneMonthFiles, Convert.ToInt32(txb_YearForMeteo.Text));
            }
        }

        public void UpdateProgressBar_HTML(int valueMax)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (valueMax != prBar_readingHTML.Maximum)
                {
                    prBar_readingHTML.Maximum = valueMax;
                }
                prBar_readingHTML.Value++;
                lb_htmlProgress.Content = String.Format("Выполнено {0:F2} %", (prBar_readingHTML.Value * 100) / valueMax);
            }
            else
            {
                prBar_readingHTML.Dispatcher.Invoke(new Delegate_UpdateProgressBar(UpdateProgressBar_HTML), valueMax);
                return;
            }
        }
        public void UpdateProgressBar_SQL(int valueMax)
        {
            if (this.Dispatcher.CheckAccess())
            {
                if (valueMax != prBar_updatingMySQL.Maximum)
                {
                    prBar_updatingMySQL.Maximum = valueMax;
                }
                prBar_updatingMySQL.Value++;
                lb_dbProgress.Content = String.Format("Выполнено {0:F2} %", (prBar_updatingMySQL.Value * 100) / valueMax);
            }
            else
            {
                prBar_updatingMySQL.Dispatcher.Invoke(new Delegate_UpdateProgressBar(UpdateProgressBar_SQL), valueMax);
                return;
            }
        }
    }
}
