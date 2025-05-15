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

namespace Northville_Library.LibrarianWindow_Pages
{
    /// <summary>
    /// Interaction logic for LibrarianFines.xaml
    /// </summary>
    public partial class LibrarianFines : Page
    {
        DataClasses1DataContext db;
        Fine selectedFine;
        public LibrarianFines()
        {
            InitializeComponent();
            LoadFines();
        }

        public void LoadFines()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            finesDataGrid.ItemsSource = null;
            finesDataGrid.ItemsSource = db.Fines.ToList();
        }
        private void paidBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure that you want to mark it as paid?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                selectedFine.Fines_Status = "Paid";

                try
                {
                    db.SubmitChanges();
                    MessageBox.Show($"Fine Status for Fine ID: {selectedFine.Fines_ID} has been changed!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    updateTransactionStatus();
                    LoadFines();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Fines);
                    MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadFines();
                }
            }
        }
        private void updateTransactionStatus()
        {
            string selectedTransactionID = selectedFine.Transaction_ID;

            var selectedTransaction = (from t in db.Transactions
                                       where t.Transaction_ID == selectedTransactionID
                                       select t).FirstOrDefault();


            if (selectedTransaction != null)
            {
                selectedTransaction.Transaction_Status = "Paid Fine";

                try
                {
                    db.SubmitChanges();
                    LoadFines();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Fines);
                    MessageBox.Show($"Error: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadFines();
                }
            }
        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            finesDataGrid.UnselectAll();
        }
        private void finesDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Transaction")
            {
                e.Cancel = true;
            }
        }
        private void finesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (finesDataGrid.SelectedItem != null && finesDataGrid.SelectedItem
               is Fine _selectedItem)
            {
                if (_selectedItem.Fines_Status != "Paid")
                {
                    selectedFine = _selectedItem;
                    paidBTN.IsEnabled = true;
                    deselectBTN.IsEnabled = true;
                }
                else
                {
                    selectedFine = null;
                    paidBTN.IsEnabled = false;
                    deselectBTN.IsEnabled = false;
                }
            }
            else
            {
                selectedFine = null;
                paidBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
            }
        }
        private void finesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is LibrarianWindow libWindow)
            {
                libWindow.transactionNavToPage();
            }
        }
    }
}
