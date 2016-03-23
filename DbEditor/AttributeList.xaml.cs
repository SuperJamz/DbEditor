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
    /// Interaction logic for AttributeList.xaml
    /// </summary>
    public partial class AttributeList : UserControl
    {
        public AttributeList()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty AttributesProperty =
            DependencyProperty.Register("AttributeValues", typeof(ObservableCollection<Attribute>), typeof(AttributeList), new FrameworkPropertyMetadata(null, OnAttributeListChanged));

        public ObservableCollection<Attribute> AttributeValues
        {
            get { return (ObservableCollection<Attribute>)GetValue(AttributesProperty); }
            set { SetValue(AttributesProperty, value); }
        }

        private static void OnAttributeListChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            
            //logic when the table name is changed.
            if (e.Property == AttributesProperty)
            {
                
                string newName = sender.ToString();
            }
        }


    }
}
