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
using System.Windows.Shapes;

namespace Northville_Library
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
        public string studentUser { get; set; }
        public StudentWindow()
        {
            InitializeComponent();
        }

        public void retrieveInformation()
        {

        }
    }
}
