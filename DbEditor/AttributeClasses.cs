using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DbEditor
{
    public class RecordAttributes
    {
        private string recordName;

        public string RecordName
        {
            get { return recordName; }
            set { recordName = value; }
        }
        
        public ObservableCollection<AccessoryType> Types { get; set; }

        public RecordAttributes()
        {
            Types = new ObservableCollection<AccessoryType>();
        }

        public RecordAttributes(string name, ObservableCollection<AccessoryType> types)
        {
            RecordName = name;
            Types = types;
        }
    }

    public class AccessoryType : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //public string Name { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                }

            }
        }
        //public readonly static DependencyProperty NameProperty =
        //    DependencyProperty.Register("Name", typeof(string), typeof(AccessoryType));

        //public string Name
        //{
        //    get { return (string)GetValue(NameProperty); }
        //    set { SetValue(NameProperty, value); }
        //}
        
        public ObservableCollection<Attribute> Attributes { get; set; }
        public AccessoryType(string name, ObservableCollection<Attribute> recordAttributes)
        {
            this.Attributes = recordAttributes;
            this.Name = name;
        }

    }

    public class Attribute
    {
        public string Name { get; set; }

        public string TableField { get; set; }

        public bool IsChecked { get; set; }

        public Attribute(string name, string tableField)
        {
            Name = name;

            TableField = tableField;
        }
    }
}
