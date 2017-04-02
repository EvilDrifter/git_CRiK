using CRiC_Meteo.Presenters;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRiC_Meteo.Models
{

    class MySQL_Worker
    {
        MySqlConnectionStringBuilder conn_string;
        List<string> tableNames;
        List<Task> allTasks;
        IPresenterDataBase presInterface_worker;
        public MySQL_Worker(MySQLDataBaseConfig msConfig, IPresenterDataBase presInterface)
        {
            this.presInterface_worker = presInterface;

            conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = msConfig.msql_server;
            conn_string.UserID = msConfig.msql_userId;
            conn_string.Password = msConfig.msql_password;
            conn_string.Database = msConfig.msql_DB_name;

            tableNames = new List<string>();
            tableNames = GetTableList();  //Получение списка названий всех таблиц в БД
        }
        public MySQL_Worker(MySQLDataBaseConfig msConfig)
        {
            conn_string = new MySqlConnectionStringBuilder();
            conn_string.Server = msConfig.msql_server;
            conn_string.UserID = msConfig.msql_userId;
            conn_string.Password = msConfig.msql_password;
            conn_string.Database = msConfig.msql_DB_name;
        }

        public void UpdateDataBaseFromMeteoStationData(List<MeteoStation> listMeteoStation, int NumberOfThreads)    //Обновление таблицы MySQL, указывается список метеостанций и количество потоков обращения к БД (желательно<=12)
        {
            DataTable tempDataTable;
            List<MySQL_DataBase> msqlForUpdate = new List<MySQL_DataBase>();
            allTasks = new List<Task>();
            foreach (MeteoStation item in listMeteoStation)
            {
                tempDataTable = item.GetInfoAboutMeteoStaAs_DataTable;
                msqlForUpdate.Add(new MySQL_DataBase(conn_string, tempDataTable));
            }

            foreach (MySQL_DataBase item in msqlForUpdate)
            {
                if (allTasks.Count > NumberOfThreads)        //Количество потоков для записи в MySQL БД (15+ - отваливается, 10 - стабильная работа)
                {
                    Task.WaitAny(allTasks.ToArray());
                    CheckAllTask(item, msqlForUpdate.Count);
                }
                if (allTasks.Count <= NumberOfThreads)
                {
                    allTasks.Add(Task.Factory.StartNew(() => item.UpdateDataBaseByIndex(tableNames, presInterface_worker, msqlForUpdate.Count)));
                    //MessageBox.Show(allTasks.Count + '\t' + tempDataTable.TableName);
                }
            }
        

        }
        private void CheckAllTask(MySQL_DataBase dt, int AllTableCount)
        {
            for (int i = 0; i < allTasks.Count; i++)
            {
                if (allTasks[i].Status == TaskStatus.RanToCompletion)
                {
                    //Если поток простаивает, ему назначается новое задание
                    allTasks[i] = Task.Run(() => dt.UpdateDataBaseByIndex(tableNames, presInterface_worker, AllTableCount));
                    break;
                }

            }
        }

        public DataTable GetDT_ByIndex(string index)
        {
            return new MySQL_DataBase(conn_string).GetDataTableByIndex(index);
        }
        public List<string> GetTableList()
        {
            return new MySQL_DataBase(conn_string).GetAllShemasNameIn_MySQL(); 
        }



        class MySQL_DataBase    //Класс для работы одного подключения к БД
        {
            MySqlConnection connection;
            string mySQLDB_Name;
            DataTable dt_meteoStation;
            public MySQL_DataBase(MySqlConnectionStringBuilder conn_string, DataTable dt_meteoStation)
            {
                this.dt_meteoStation = dt_meteoStation;
                mySQLDB_Name = conn_string.Database;
                connection = new MySqlConnection(conn_string.ToString());
            }
            public MySQL_DataBase(MySqlConnectionStringBuilder conn_string)
            {
                mySQLDB_Name = conn_string.Database;
                connection = new MySqlConnection(conn_string.ToString());
            }

            private bool OpenConnection()                                                       //Открывает соединение с БД (True/False)
            {
                try
                {
                    connection.Open();
                    //MessageBox.Show("Connect to server." + Convert.ToString(connection.State));
                    return true;
                }
                catch (MySqlException ex)
                {
                    //When handling errors, you can your application's response based 
                    //on the error number.
                    //The two most common error numbers when connecting are as follows:
                    //0: Cannot connect to server.
                    //1045: Invalid user name and/or password.
                    MessageBox.Show("Ошибка открытия БД");
                    switch (ex.Number)
                    {
                        case 0:
                            MessageBox.Show("Cannot connect to server.  Contact administrator");
                            break;

                        case 1045:
                            MessageBox.Show("Invalid username/password, please try again");
                            break;
                    }
                    return false;
                }
            }
            private bool CloseConnection()                                                      //Закрывает соединение с БД (True/False)
            {
                try
                {
                    connection.Close();
                    return true;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message + "\nОшибка закрытия БД");
                    return false;
                }
            }

            #region Обновление БД таблицей по индексу
            public bool UpdateDataBaseByIndex(List<string> nameOfTables, IPresenterDataBase presInterface_db, int listOfMeteoStationCount)     //Добавляет данные в БД, если нужно то создает новую таблицу
            {
                bool isUpdated = false;
                try
                {
                    if (this.OpenConnection() == true)
                    {
                        if (isNewTable(dt_meteoStation.TableName, nameOfTables)) { CreateNewTable(dt_meteoStation); }     //Если нет таблицы с таким индексом нужно создать новую в БД

                        string firstDate = Convert.ToDateTime(dt_meteoStation.Rows[0]["Дата"]).ToString("yyyy-MM-dd HH:mm:ss");
                        string lastDate = Convert.ToDateTime(dt_meteoStation.Rows[dt_meteoStation.Rows.Count - 1]["Дата"]).ToString("yyyy-MM-dd HH:mm:ss");

                        string query = String.Format("DELETE FROM `{0}`.`{1}` WHERE Дата >= '{2}' AND Дата <= '{3}' ORDER BY Дата ;", mySQLDB_Name, dt_meteoStation.TableName, firstDate, lastDate);
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        cmd.ExecuteNonQuery();

                        query = String.Format("SELECT * FROM `{0}`.`{1}` WHERE Дата >= '{2}' AND Дата <= '{3}' ORDER BY Дата ;", mySQLDB_Name, dt_meteoStation.TableName, firstDate, lastDate);
                        cmd = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        MySqlCommandBuilder cb = new MySqlCommandBuilder(adapter);

                        adapter.Update(dt_meteoStation);
                    }
                    this.CloseConnection();

                    isUpdated = true;
                    presInterface_db.UpdateForm_PrBarSQL(listOfMeteoStationCount);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + '\n' + $"Объединение таблиц HTML и MySQL {dt_meteoStation.TableName}");
                    //throw;
                }

                return isUpdated;
            }
            private bool isNewTable(string dataTabelName, List<string> nameOfTables)
            {
                bool isNew = true;
                for (int i = 0; i < nameOfTables.Count; i++)
                {
                    if (nameOfTables[i] == dataTabelName)
                    {
                        isNew = false;
                        break;
                    }
                }
                return isNew;
            }
            private void CreateNewTable(DataTable dt)                                                   //Создает новую таблицу
            {
                string SQL_commandString = string.Format(
                                        @"CREATE TABLE `{0}`.`{1}`
                                        (`Дата` DATETIME NULL,
                                        `Ветер направление` VARCHAR(128) NULL,
                                        `Ветер скорость (м/с)` INT NULL,
                                        `Видимость` VARCHAR(128) NULL,
                                        `Явления` VARCHAR(128) NULL,
                                        `Облачность` VARCHAR(128) NULL,
                                        `T - Температура воздуха (C)` DECIMAL NULL,
                                        `Td - Температура точки росы  (C)` DECIMAL NULL,
                                        `f - Относительная влажность воздуха (%)` INT NULL,
                                        `Te - Эффективная температура (C)` DECIMAL NULL,
                                        `Tes - Эффективная температура на солнце (C)` DECIMAL NULL,
                                        `P - Атмосферное давление (гПа)` DECIMAL NULL,
                                        `Po - Атмосферное давление (гПа)` DECIMAL NULL,
                                        `Tmin - Минимальная температура (C)` DECIMAL NULL,
                                        `Tmax - Максимальная температура (C)` DECIMAL NULL,
                                        `R - Количество осадков (мм)` DECIMAL NULL,
                                        `S - Снежный покров (см)` INT NULL);", mySQLDB_Name, dt.TableName);

                //MessageBox.Show(mySQLDB_Name + '\t' + dt.TableName);

                MySqlCommand command = new MySqlCommand(SQL_commandString, connection);
                try
                {
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n" + dt.TableName + "создание таблицы");
                }
            }
            #endregion

            public List<string> GetAllShemasNameIn_MySQL()                                              //Получение назваий всех таблиц из БД
            {
                List<string> l_names = new List<string>();
                if (this.OpenConnection() == true)
                {
                    string query = "SHOW TABLES FROM " + mySQLDB_Name;
                    MySqlCommand command = new MySqlCommand(query, connection);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            l_names.Add(reader.GetString(0));
                        }
                    }
                }

                return l_names;
            }

            public DataTable GetDataTableByIndex(string staIndex)                                       //Получение одного DataTable из БД
            {
                DataTable dTable = new DataTable();
                if (this.OpenConnection() == true)
                {
                    string query = String.Format("SELECT * FROM `{0}`.`{1}` ORDER BY Дата ;", mySQLDB_Name, staIndex);
                    dTable = new DataTable(staIndex);
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataSet dSet = new DataSet(dTable.TableName);
                    adapter.FillSchema(dSet, SchemaType.Source);
                    adapter.Fill(dSet);

                    dTable = dSet.Tables[0];
                }
                this.CloseConnection();
                return dTable;
            }
        }
    }
}
