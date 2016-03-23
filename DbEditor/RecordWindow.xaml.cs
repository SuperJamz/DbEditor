using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace DbEditor
{
    /// <summary>
    /// Interaction logic for RecordWindow.xaml
    /// </summary>
    public partial class RecordWindow : UserControl
    {
        public RecordWindow()
        {
            InitializeComponent();
            
        }

        public static readonly DependencyProperty accessoryProperty =
            DependencyProperty.Register("Accessory", typeof(string), typeof(RecordWindow));

        public string Accessory
        {
            get { return (string)GetValue(accessoryProperty); }
            set { SetValue(accessoryProperty, value); }
        }

        public static readonly DependencyProperty accessoryTypesProperty =
            DependencyProperty.Register("AccessoryTypes", typeof(ObservableCollection<AccessoryType>), typeof(RecordWindow), new FrameworkPropertyMetadata(null, OnAccessoryTypeChanged));

        public ObservableCollection<AccessoryType> AccessoryTypes
        {
            get { return (ObservableCollection<AccessoryType>)GetValue(accessoryTypesProperty); }
            set { SetValue(accessoryTypesProperty, value); }
        }

        private static void OnAccessoryTypeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            //logic when the table name is changed.
            if (e.Property == accessoryTypesProperty)
            {
                string newName = sender.ToString();
            }
        }
    }
}
