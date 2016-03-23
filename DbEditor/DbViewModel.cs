using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DbEditor
{
    public class DbViewModel : INotifyPropertyChanged
    {
        #region Properties
        public event PropertyChangedEventHandler PropertyChanged;

        List<DataRow> modifiedRecords = new List<DataRow>();

        private DataView displayView;

        public DataView DisplayView
        {
            get { return displayView; }
            set
            {
                displayView = value;

                //string thing = DisplayView.ToString();
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DisplayView"));
                }

            }
        }



        private List<string> AccesoriesTypes = new List<string> { 
            "beards", "clothing", "glasses", "hair", "hats", "jewellery",
            "logos", "skineffects", "thumbnails" 
        };

        private ObservableCollection<ComboBoxItem> accessoryTables;

        public ObservableCollection<ComboBoxItem> AccessoryTables
        {
            get { return accessoryTables; }
            set
            {
                accessoryTables = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("accessoryTables"));
                }
            }
        }

        private ComboBoxItem selectedTable;

        public ComboBoxItem SelectedTable
        {
            get { return selectedTable; }
            set
            {
                selectedTable = value;
                //change the table
                TableName = selectedTable.Content.ToString();
                GrabDbRecords(DbFilePath);
            }
        }

        private string tableName;

        public string TableName
        {
            get { return tableName; }
            set
            {
                tableName = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("tableName"));
                }
            }
        }

        string DbFilePath = "";

        private object selectedRow;

        public object SelectedRow
        {
            get
            {
                return selectedRow;
            }
            set
            {
                selectedRow = value;
                if (selectedRow != null)
                {
                    if (selectedRow.GetType() == typeof(DataRowView)) // when the selected row is changed
                    {
                        grabSqlImage((DataRowView)SelectedRow, DbFilePath);
                        ItemAttributes = grabRecordAttributes((DataRowView)SelectedRow, DbFilePath);
                    }
                }
            }
        }

        private ImageSource displayedAccessory;

        public ImageSource DisplayedAccessory
        {
            get { return displayedAccessory; }
            set
            {
                displayedAccessory = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("displayedAccessory"));
                }
            }
        }

        //private RecordAttributes itemAttributes;

        //public RecordAttributes ItemAttributes
        //{
        //    get { return itemAttributes; }
        //    set { itemAttributes = value; }
        //}

        private ObservableCollection<AccessoryType> itemAttributes;

        public ObservableCollection<AccessoryType> ItemAttributes
        {
            get { return itemAttributes; }
            set
            {
                itemAttributes = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("itemAttributes"));
                }
            }
        }


        #endregion

        public DbViewModel()
        {
            changeDbFilePath();
            //fill_TableDropdown(DbFilePath);
            //fill_TableDropdown("C:\\Users\\James\\vm\\DbEditor\\db files\\accessories.db");
            ////make a connection to the database and add table to the view
            //GrabDbRecords("beards");
        }

        #region Methods

        private void fill_TableDropdown(string dbPath)
        {
            List<string> tableList = new List<string>();
            using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + dbPath + ";"))
            {
                dbConnect.Open();
                using (SQLiteCommand cmd = dbConnect.CreateCommand())
                {
                    string commandString = "SELECT accesory FROM combo_titles; ";

                    cmd.CommandText = commandString;
                    DataTable tb = new DataTable();
                    SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                    adapter.Fill(tb);
                    foreach (DataRow record in tb.Rows)
                    {
                        if (!tableList.Contains(record.ItemArray[0].ToString()))
                        {
                            tableList.Add(record.ItemArray[0].ToString());
                        }

                    }
                }
            }

            accessoryTables = new ObservableCollection<ComboBoxItem>();
            //add the list of tables to the combobox
            for (int i = 0; i < tableList.Count; i++)
            {
                AccessoryTables.Add(new ComboBoxItem() { Content = tableList[i] });
            }

        }

        private void GrabDbRecords(string dbPath)
        {
            if (TableName != "")
            {
                using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + dbPath + ";"))
                {
                    dbConnect.Open();
                    using (SQLiteCommand cmd = dbConnect.CreateCommand())
                    {
                        string commandString = "SELECT name, ";
                        int columnCount = DbMetadata.Classifiers[TableName].Length;

                        //build the request for the classifier columns using the dictionary.
                        for (int i = 0; i < columnCount - 1; i++)
                        {
                            commandString += DbMetadata.Classifiers[TableName][i] + ", ";
                        }
                        //add the final column without the comma.
                        commandString += DbMetadata.Classifiers[TableName][columnCount - 1];

                        commandString += " FROM " + TableName;
                        cmd.CommandText = commandString;

                        DataTable dbData = new DataTable();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(dbData);

                        //copy the table, but not its data
                        DataTable dtCloned = dbData.Clone();
                        //change the name column to read-only
                        dtCloned.Columns[0].ReadOnly = true;

                        //change the column types for the rest of the table to bool
                        for (int i = 1; i < columnCount + 1; i++)
                        {
                            dtCloned.Columns[i].DataType = typeof(bool);
                        }
                        //import the data from the original table
                        foreach (DataRow row in dbData.Rows)
                        {
                            dtCloned.ImportRow(row);
                        }
                        //change the display table property so the user will see the table.
                        DisplayView = dtCloned.DefaultView;

                    }
                }
            }
            else { DisplayView = null; }
        }

        public void saveTable(string dbPath)
        {
            DataTable currentTable = DisplayView.Table;

            DataTable updatedTable = currentTable.Clone();
            //change the attribute columns back to INT for the SQlite database
            for (int i = 1; i < updatedTable.Columns.Count; i++)
            {
                updatedTable.Columns[i].DataType = typeof(int);
            }
            //Copy the displayed rows into the converted table
            foreach (DataRow row in currentTable.Rows)
            {
                updatedTable.ImportRow(row);
            }
            //check for modified records and add them to the list
            foreach (DataRow record in updatedTable.Rows)
            {
                if (record.RowState == DataRowState.Modified)
                {
                    modifiedRecords.Add(record);
                }
            }

            using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + dbPath + ";"))
            {
                dbConnect.Open();
                using (SQLiteCommand cmd = dbConnect.CreateCommand())
                {
                    //build the update statement using the table name, column names, and corresponding values in each item array.

                    for (int i = 0; i < modifiedRecords.Count; i++)
                    {
                        int tableLength = modifiedRecords[i].ItemArray.Length;

                        string commandString = "UPDATE " + TableName + " SET ";
                        for (int j = 1; j < tableLength - 1; j++)
                        {
                            commandString += updatedTable.Columns[j].ColumnName + " = " + modifiedRecords[i].ItemArray[j].ToString() + ", ";
                        }
                        //add final column without adding the comma (THIS IS PAINFUL)
                        commandString += updatedTable.Columns[tableLength - 1].ColumnName + " = " + modifiedRecords[i].ItemArray[tableLength - 1].ToString();
                        //add the condition updating the record where its name is, then execute the command
                        commandString += " WHERE " + updatedTable.Columns[0].ColumnName + " = " + " '" + modifiedRecords[i].ItemArray[0].ToString() + "'" + " ;";
                        cmd.CommandText = commandString;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            //accept changed records in the displayed table, and clear the list.
            DisplayView.Table.AcceptChanges();
            modifiedRecords.Clear();
        }

        private void changeDbFilePath()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.ShowDialog();
            DbFilePath = dlg.FileName;
            if (DbFilePath != "")
            {
                TableName = "";
                fill_TableDropdown(DbFilePath);
                GrabDbRecords(DbFilePath);
            }
        }

        private void grabSqlImage(DataRowView currentRow, string dbPath)
        {
            using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + dbPath + ";"))
            {
                dbConnect.Open();
                using (SQLiteCommand cmd = dbConnect.CreateCommand())
                {
                    if (currentRow != null)
                    {
                        string commandString = "SELECT bytes from '" + TableName + "' WHERE name = '" + currentRow.Row[0] + "';";
                        cmd.CommandText = commandString;
                        DataSet imgData = new DataSet();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(imgData);

                        if (imgData.Tables[0].Rows.Count == 1)
                        {
                            Byte[] data = new Byte[0];
                            data = (Byte[])(imgData.Tables[0].Rows[0]["bytes"]);
                            MemoryStream mem = new MemoryStream(data);
                            BitmapImage img = new BitmapImage();
                            img.BeginInit(); img.StreamSource = mem; img.EndInit();
                            DisplayedAccessory = img;
                        }
                    }

                }
            }
        }

        //private RecordAttributes grabRecordAttributes(DataRowView currentRow)
        //{
        //    ObservableCollection<AccessoryType> typeList = new ObservableCollection<AccessoryType>();

        //    using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + DbFilePath + ";"))
        //    {
        //        dbConnect.Open();
        //        using (SQLiteCommand cmd = dbConnect.CreateCommand())
        //        {
        //            if (currentRow != null)
        //            {
        //                //get the list of titles and foreign keys for the accessory table
        //                string commandString = "SELECT title, combo from combo_titles WHERE accesory = '" + TableName + "';";
        //                cmd.CommandText = commandString;
        //                DataTable typeTitles = new DataTable();
        //                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
        //                adapter.Fill(typeTitles);
        //                // for each type (or row) in the requested table, grab their attributes in the combo_values table, 
        //                // and add it to its attributes list.
        //                foreach (DataRow typeRow in typeTitles.Rows)
        //                {
        //                    //get the attribute titles and corresponding field names in the main table.
        //                    commandString = "SELECT title, field from combo_values where ";

        //                    commandString += "accesory = '" + TableName + "' and combo = '" + typeRow[1].ToString() + "'";
        //                    cmd.CommandText = commandString;
        //                    DataTable attribTitles = new DataTable();
        //                    adapter.Fill(attribTitles);

        //                    //now grab the attribute titles for each specific type.
        //                    ObservableCollection<Attribute> attribList = new ObservableCollection<Attribute>();
        //                    foreach (DataRow name in attribTitles.Rows)
        //                    {
        //                        attribList.Add(new Attribute(name[0].ToString()));
        //                    }

        //                    //grab the bool values from the main table using the 'field' column values
        //                    commandString = "SELECT ";
        //                    // build up the sqlite command by using the 'field' column names
        //                    for (int i = 0; i < attribTitles.Rows.Count - 1; i++)
        //                    {
        //                        commandString += attribTitles.Rows[i][1].ToString() + ", ";
        //                    }

        //                    commandString += attribTitles.Rows[attribList.Count - 1][1].ToString() + " from " + TableName + " where name = '" + currentRow.Row.ItemArray[0] + "'";

        //                    cmd.CommandText = commandString;

        //                    DataTable attribValues = new DataTable();
        //                    adapter.Fill(attribValues);

        //                    for (int i = 0; i < attribValues.Columns.Count; i++)
        //                    {
        //                        foreach (Attribute value in attribList)
        //                        {
        //                            //if (value.Name == attribValues.Columns[i].ColumnName &&
        //                            //    (int)attribValues.Rows[0].ItemArray[0] == 1)
        //                            if (attribTitles.Rows[i][0].ToString() == value.Name && attribTitles.Rows[i][1].ToString() == attribValues.Columns[i].ColumnName &&
        //                                (int)attribValues.Rows[0].ItemArray[i] == 1)
        //                            {
        //                                value.IsChecked = true;
        //                                break;
        //                            }
        //                        }
        //                    }

        //                    string typeName = typeRow[0].ToString();
        //                    typeList.Add(new AccessoryType(typeName, attribList));
        //                }
        //            }
        //        }
        //    }
        //    return new RecordAttributes(typeList);
        //}

        private ObservableCollection<AccessoryType> grabRecordAttributes(DataRowView currentRow, string dbPath)
        {
            ObservableCollection<AccessoryType> typeList = new ObservableCollection<AccessoryType>();

            using (SQLiteConnection dbConnect = new SQLiteConnection("Data Source=" + dbPath + ";"))
            {
                dbConnect.Open();
                using (SQLiteCommand cmd = dbConnect.CreateCommand())
                {
                    if (currentRow != null)
                    {
                        //get the list of titles and foreign keys for the accessory table
                        string commandString = "SELECT title, combo from combo_titles WHERE accesory = '" + TableName + "';";
                        cmd.CommandText = commandString;
                        DataTable typeTitles = new DataTable();
                        SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                        adapter.Fill(typeTitles);
                        // for each type (or row) in the requested table, grab their attributes in the combo_values table, 
                        // and add them to its attributes list.
                        foreach (DataRow typeRow in typeTitles.Rows)
                        {
                            //get the attribute titles and corresponding field names in the main table.
                            commandString = "SELECT title, field from combo_values where ";

                            commandString += "accesory = '" + TableName + "' and combo = '" + typeRow["combo"].ToString() + "'";
                            cmd.CommandText = commandString;
                            DataTable attribTitles = new DataTable();
                            adapter.Fill(attribTitles);

                            //now grab the attribute titles for each specific type.
                            ObservableCollection<Attribute> attribList = new ObservableCollection<Attribute>();
                            foreach (DataRow name in attribTitles.Rows)
                            {
                                attribList.Add(new Attribute(name["title"].ToString(), name["field"].ToString()));
                            }

                            // grab the bool values from the main table using the 'field' column values
                            commandString = "SELECT ";
                            // build up the sqlite command by using the 'field' column names
                            for (int i = 0; i < attribTitles.Rows.Count - 1; i++)
                            {
                                commandString += attribTitles.Rows[i]["field"].ToString() + ", ";
                            }

                            commandString += attribTitles.Rows[attribList.Count - 1]["field"].ToString() + " from " + TableName + " where name = '" + currentRow.Row.ItemArray[0] + "'";

                            cmd.CommandText = commandString;

                            DataTable attribValues = new DataTable();
                            adapter.Fill(attribValues);

                            for (int i = 0; i < attribValues.Columns.Count; i++)
                            {
                                foreach (Attribute value in attribList)
                                {
                                    // if the value's name is the same as the 'title' from combo_values,
                                    // and the column name is the same as the 'field' in the same row,
                                    // then you have the correct attribute field for that record.
                                    // change to true if it is 1.
                                    if (attribTitles.Rows[i]["title"].ToString() == value.Name &&
                                        attribTitles.Rows[i]["field"].ToString() == attribValues.Columns[i].ColumnName &&
                                        (int)attribValues.Rows[0].ItemArray[i] == 1)
                                    {
                                        value.IsChecked = true;
                                        break;
                                    }
                                }
                            }

                            string typeName = typeRow["title"].ToString();
                            typeList.Add(new AccessoryType(typeName, attribList));
                        }
                    }
                }
            }
            return typeList;
        }

        #endregion

        #region Commands

        private ICommand _updateDbCommand;

        public ICommand UpdateDbCommand
        {
            get
            {
                if (_updateDbCommand == null)
                {
                    _updateDbCommand = new RelayCommand(param => this.saveTable(DbFilePath));
                }
                return _updateDbCommand;
            }

        }

        private ICommand _changeDbFilePathCommand;

        public ICommand ChangeDbFilePathCommand
        {
            get
            {
                if (_changeDbFilePathCommand == null)
                {
                    _changeDbFilePathCommand = new RelayCommand(param => this.changeDbFilePath());
                }
                return _changeDbFilePathCommand;
            }

        }


        #endregion
    }
}
