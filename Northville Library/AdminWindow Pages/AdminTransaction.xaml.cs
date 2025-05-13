using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace Northville_Library.AdminWindow_Pages
{
    /// <summary>
    /// Interaction logic for AdminTransaction.xaml
    /// </summary>
    public partial class AdminTransaction : Page
    {
        DataClasses1DataContext db;
        Transaction selectedTransaction;
        string localTransactionID;
        public AdminTransaction()
        {
            InitializeComponent();
            LoadTransactions();
        }
        public void LoadTransactions()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            transactionDataGrid.ItemsSource = null;
            transactionDataGrid.ItemsSource = db.Transactions.ToList();
        }
        private void transactionDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Fines" || e.PropertyName == "Book" || e.PropertyName == "Student")
            {
                e.Cancel = true;
            }
        } // Auto
        private void transactionDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (transactionDataGrid.SelectedItem != null && transactionDataGrid.SelectedItem 
                is Transaction _selectedItem)
            {
                if (_selectedItem.Return_Date == null)
                {
                    selectedTransaction = _selectedItem;
                    returnBTN.IsEnabled = true;
                    deselectBTN.IsEnabled = true;
                }
                else
                {
                    selectedTransaction = null;
                    returnBTN.IsEnabled = false;
                    deselectBTN.IsEnabled = false;
                }
            }
            else
            {
                selectedTransaction = null;
                returnBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
            }
        }
        private void returnTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure that you want to mark it as returned?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                db.ExecuteCommand($"EXEC usp_ReturnBook @TransactionID = {selectedTransaction.Transaction_ID}");
                localTransactionID = selectedTransaction.Transaction_ID;
                selectedTransaction = null;
                CheckIfForFines();
                ReturnBook();
                localTransactionID = null;
                LoadTransactions();
            }
        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            transactionDataGrid.UnselectAll();
        }
        private void CheckIfForFines()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

            var finalizedTransaction = (from t in db.Transactions
                                       where t.Transaction_ID == localTransactionID 
                                       select t).FirstOrDefault();
                                  
            if (finalizedTransaction.Transaction_Status == "For Fines")
            {
                CreateFine(finalizedTransaction);
            }
        }
        private void CreateFine(Transaction finalizedTransaction)
        {
            DateTime dueDate = finalizedTransaction.Due_Date;
            DateTime? returnDate = finalizedTransaction.Return_Date;
            int dateDifference = (returnDate.Value - dueDate).Days;
            var lastFine = db.Fines.OrderByDescending(f => f.Fines_ID).FirstOrDefault();
            string newFineID;

            if (lastFine == null)
            {
                newFineID = "F01";
            }
            else
            {
                int lastNum = int.Parse(lastFine.Fines_ID.Substring(1));
                int newNum = lastNum + 1;
                newFineID = "F" + newNum.ToString("D2");
            }

            Fine newFine = new Fine
            {
                Fines_ID = newFineID,
                Transaction_ID = finalizedTransaction.Transaction_ID,
                Fine_Amount = 60 * dateDifference,
                Days_Overdue = dateDifference,
                Fines_Status = "Unpaid"
            };

            db.Fines.InsertOnSubmit(newFine);

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ReturnBook()
        {
            var finalizedTransaction = (from t in db.Transactions
                                        where t.Transaction_ID == localTransactionID
                                        select t).FirstOrDefault();

            var borrowedBook = (from b in db.Books
                                where b.Book_ID == finalizedTransaction.Book_ID
                                select b).FirstOrDefault();

            if (borrowedBook != null)
            {
                int tempValue = borrowedBook.Book_Quantity.Value;
                borrowedBook.Book_Quantity = tempValue + 1;
            }

            try
            {
                db.SubmitChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void finesBTN_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is AdminWindow adminWindow)
            {
                adminWindow.finesNavToPage();
            }
        }
    }
}
