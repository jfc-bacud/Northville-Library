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

namespace Northville_Library.StudentWindow_Pages
{
    /// <summary>
    /// Interaction logic for StudentBooks.xaml
    /// </summary>
    public partial class StudentBooks : Page
    {
        DataClasses1DataContext db; // like that as I need to call another new instance to 'refresh' everything
        public string localstudentUID; // to store which user is currently doing all the shit
        Book selectedBook; // used later


        public StudentBooks(string studentUID)
        {
            localstudentUID = studentUID;
            InitializeComponent();
            LoadBooks();
        }

        private void LoadBooks()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            booksDataGrid.ItemsSource = null;
            booksDataGrid.ItemsSource = db.Books.Where(b => b.Book_Quantity > 0).ToList();

            if (booksDataGrid == null)
            {
                booksresultsLBL.Content = "0 Books Found";
            }
            else
            {
                booksresultsLBL.Content = $"{booksDataGrid.Items.Count} Book/s Found";
            }
        } // Populates the datagrid of books that have a quantity of 1+
        private void booksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                if (booksDataGrid.SelectedItem != null && booksDataGrid.SelectedItem is Book _selectedBook)
                {
                    selectedBook = _selectedBook;
                    borrowBTN.IsEnabled = true;
                }
                else
                {
                    selectedBook = null;
                    borrowBTN.IsEnabled = false;
                }
        } // Detects if a row has been selected, and if yes, enables button
        private void borrowBTN_Click(object sender, RoutedEventArgs e)
        {
            if (selectedBook != null)
            {
                db.ExecuteCommand($"EXEC usp_BorrowBook @BookID = {selectedBook.Book_ID}");
                CreateTransaction();

            }
        } // Runs an event in SQL side to decrease quantity of book in table AND creates transaction
        private void CreateTransaction()
        {
            var lastTransaction = db.Transactions.OrderByDescending(t => t.Transaction_ID).FirstOrDefault();
            string newTransactionID;

            if (lastTransaction == null)
            {
                newTransactionID = "T01";
            }
            else
            {
                int lastNum = int.Parse(lastTransaction.Transaction_ID.Substring(1));
                int newNum = lastNum + 1;
                newTransactionID = "T" + newNum.ToString("D2");
            }

            Transaction transaction = new Transaction
            {
                Transaction_ID = newTransactionID,
                Student_ID = localstudentUID,
                Book_ID = selectedBook.Book_ID,
                Borrow_Date = DateTime.Now,
                Due_Date = DateTime.Now.AddDays(7),
                Return_Date = null,
                Transaction_Status = "Borrowed"
            };

            db.Transactions.InsertOnSubmit(transaction);
            try
            {
                MessageBox.Show($"You have borrowed book: {selectedBook.Book_ID}!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                db.SubmitChanges();
                LoadBooks();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error has occured!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }


        } // Creates Transaction Here
        private void booksDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Transactions")
            {
                e.Cancel = true;
            }
        } // An event to hide an unneccessary column
    }
}
