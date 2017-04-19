using CRiC_Meteo.Elements;
using CRiC_Meteo.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CRiC_Meteo.Presenters
{
    public interface IPresenterOxyPlot
    {
        List<BasseinFrozingMelting> lfm { get; }
        List<string> tableNamesFromDataBase { get; }
        void SnowFormation();
        void LoadConfig();
    }

    class Presenter_OxyPlot: IPresenterOxyPlot
    {
        interface_UC_SnowCalc inf_SnC;
        #region реализация интерфейса IPresenterOxyPlot

        public List<BasseinFrozingMelting> lfm
        {
            get { return BasseinFrozingMelting.ReadXML();}
        }

        public List<string> tableNamesFromDataBase
        {
            get
            {               
                MySQL_Worker t = new MySQL_Worker(new MySQLDataBaseConfig().ReadXML());
                return t.GetTableList();
            }
        }

        public void SnowFormation()
        {
            //SaveConfig();
            string selectedIndexSta = inf_SnC.selectedIndexSta;
            string selectedBassein = inf_SnC.selectedBassein;
            DateTime begTime = inf_SnC.begTime;
            DateTime endTime = inf_SnC.endTime;

            

            if (inf_SnC.begTime != null && inf_SnC.endTime != null)
            {
                if (inf_SnC.selectedIndexSta != "")
                {
                    
                    DataTable dt = new MySQL_Worker(new MySQLDataBaseConfig().ReadXML()).GetDT_ByIndex(selectedIndexSta, begTime, endTime);
                    int basIndex = Convert.ToUInt16(MeteoStaionWMO_index.ReadXML().First(s => s.indexWMO == selectedIndexSta).basseinIndex);
                    BasseinFrozingMelting bfm = lfm.First(s => s.basseinIndex == basIndex);

                    alglib.spline1dinterpolant s_formation, s_melting;
                    alglib.spline1dbuildlinear(bfm.frozintT.ToArray(), bfm.frozingPer.ToArray(), out s_formation);
                    alglib.spline1dbuildlinear(bfm.meltingT.ToArray(), bfm.meltingV.ToArray(), out s_melting);

                    dt.TableName = selectedIndexSta;
                    SnowCalc sc = new SnowCalc(dt, s_formation, s_melting, bfm);
                    SnowCalc.StructForCalc c = new SnowCalc.StructForCalc();
                    c = sc.SnowCalcByIndexSta();
                    inf_SnC.DrawSnowFormation(c);
                    dt = new DataTable();
                    dt = sc.CreateDataTable(c);
                    inf_SnC.ShowToGridSnowFormation(dt);
                }
                else if (inf_SnC.selectedBassein != "")
                {
                    int i = 0;
                    SnowCalcForBassein cnB = new SnowCalcForBassein();
                    MySQLDataBaseConfig msqlC = new MySQLDataBaseConfig().ReadXML();;
                    BasseinFrozingMelting bfm = BasseinFrozingMelting.ReadXML().First(s => s.basseinName == selectedBassein);
                    List<MeteoStaionWMO_index> st = MeteoStaionWMO_index.ReadXML().Where(s => s.basseinIndex == bfm.basseinIndex).ToList();
                    List<DataTable> dt = new List<DataTable>();
                    DataTable dtt;
                    foreach (MeteoStaionWMO_index item in st)
                    {
                        dtt = new DataTable();
                        try
                        {
                            dtt = new MySQL_Worker(msqlC).GetDT_ByIndex(item.indexWMO, begTime, endTime);
                            dt.Add(dtt);
                            dt[i].TableName = item.indexWMO;
                            i++;
                        }
                        catch (Exception){}
                    }

                    cnB.SnowCalcByBassein(dt, bfm);

                    //Расчет снега для всего бассейна
                    MessageBox.Show("Расчет снега для всего бассейна - еще не написан");
                }
                else
                {
                    MessageBox.Show("Необходимо выбрать режим расчета");
                }
            }
            else { MessageBox.Show("Не выбраны даты для расчета"); }

        }

        public void LoadConfig()
        {
            if (inf_SnC!=null)
            {
                ConfigForCalc c = new ConfigForCalc().ReadXML();
                inf_SnC.begTime = c.date_start;
                inf_SnC.endTime = c.date_fin;
            }
        }

        #endregion реализация интерфейса IPresenterOxyPlot

        public void SaveConfig()
        {
            if (inf_SnC != null)
            {
                ConfigForCalc c = new ConfigForCalc();
                c.date_start = inf_SnC.begTime;
                c.date_fin = inf_SnC.endTime;
                c.UpdateXML(c);
            }
        }

        public void ShowUserControl(ContentControl cc)
        {
            cc.Content = null;
            cc.Content = new uc_SnowCalc(this, ref inf_SnC);
        }


    }
}
