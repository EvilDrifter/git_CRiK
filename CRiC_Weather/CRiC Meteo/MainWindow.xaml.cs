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
using CRiC_Meteo.Presenters;
using System.Windows.Media.Animation;

namespace CRiC_Meteo
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        PresenterDataBase Presenter_DB;
        PresenterBassein Presenter_Bassein;
        public MainWindow()
        {
            InitializeComponent();

            btnLeftMenuHide.Click += (s, e) => { MenuButtonHide(); };
            btnLeftMenuShow.Click += (s, e) => { MenuButtonShow(); };

            Presenter_DB = new PresenterDataBase();
            Presenter_Bassein = new PresenterBassein();
            btn_MySQL.Click += (s, e) => { Presenter_DB.ShowUserControl(stP_MyControls); };
            btn_Bassein.Click += (s, e) => { Presenter_Bassein.ShowUserControl(stP_MyControls); };
        }

        #region МенюСлева
        public void MenuButtonHide()
        {
            ShowHideMenu("sbHideLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
        }
        public void MenuButtonShow()
        {
            ShowHideMenu("sbShowLeftMenu", btnLeftMenuHide, btnLeftMenuShow, pnlLeftMenu);
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
    }
}
