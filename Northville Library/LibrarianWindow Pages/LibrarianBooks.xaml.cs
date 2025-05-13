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
    /// Interaction logic for LibrarianBooks.xaml
    /// </summary>
    public partial class LibrarianBooks : Page
    {
        DataClasses1DataContext db;
        private string selectedBookID;
        private string selectedBookTitle;
        private string selectedBookAuthor;
        private string selectedBookISBN;
        private DateTime selectedBookDate;
        private string selectedBookGenre;
        private int selectedBookQuantity;
        Book selectedBook;
        public LibrarianBooks()
        {
            InitializeComponent();
            LoadBooks();
        }
        private void LoadBooks()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            bookDataGrid.ItemsSource = null;
            bookDataGrid.ItemsSource = db.Books.ToList();
        }
        private void bookDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Transactions")
            {
                e.Cancel = true;
            }
        }
        private void saveLocalChangeables()
        {
            selectedBookID = selectedBook.Book_ID;
            selectedBookTitle = selectedBook.Book_Title;
            selectedBookAuthor = selectedBook.Book_Author;
            selectedBookISBN = selectedBook.Book_ISBN;
            selectedBookDate = selectedBook.Book_PublicationDate;
            selectedBookGenre = selectedBook.Book_Genre;
            selectedBookQuantity = selectedBook.Book_Quantity.Value;

            FillFields();
        }
        private void FillFields()
        {
            bookIDTB.Text = selectedBookID;
            titleTB.Text = selectedBookTitle;
            authorTB.Text = selectedBookAuthor;
            isbnTB.Text = selectedBookISBN;
            publicationdateDPTB.SelectedDate = selectedBookDate;
            genreTB.Text = selectedBookGenre;
            quantityTB.Text = selectedBookQuantity.ToString();
        }
        private void removeLocalChangeables()
        {
            selectedBookID = null;
            selectedBookTitle = null;
            selectedBookAuthor = null;
            selectedBookISBN = null;
            selectedBookDate = DateTime.Today;
            selectedBookGenre = null;
            selectedBookQuantity = 0;

            RemoveFields();
        }
        private void RemoveFields()
        {
            bookIDTB.Text = "";
            titleTB.Text = "";
            authorTB.Text = "";
            isbnTB.Text = "";
            publicationdateDPTB.SelectedDate = null;
            genreTB.Text = "";
            quantityTB.Text = "";
        }
        private void bookDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bookDataGrid.SelectedItem != null && bookDataGrid.SelectedItem is Book _selectedItem)
            {
                selectedBook = _selectedItem;
                saveLocalChangeables();
                bookIDTB.IsEnabled = false;
                AddEditBTN.Content = "Edit";
                deleteBTN.IsEnabled = true;
                deselectBTN.IsEnabled = true;
            }
            else
            {
                selectedBook = null;
                removeLocalChangeables();
                bookIDTB.IsEnabled = true;
                deleteBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
                AddEditBTN.Content = "Add";
            }
        }
        private void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            int error;

            if (AddEditBTN.Content.ToString() == "Add" && bookDataGrid.SelectedItem == null)
            {
                if (!FieldVerify(out error))
                {
                    if (error == 1)
                        MessageBox.Show("Please fill all input fields!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (error == 2)
                        MessageBox.Show("There's no changes to be made!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (error == 3)
                        MessageBox.Show("The quantity fields only accepts numbers!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    AddBook();
                }
            }
            else
            {
                if (!FieldVerify(out error))
                {
                    if (error == 1)
                        MessageBox.Show("Please fill all input fields!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (error == 2)
                        MessageBox.Show("There's no changes to be made!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (error == 3)
                        MessageBox.Show("The quantity field only accepts numbers!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    EditBook();
                }
            }
        }
        private void AddBook()
        {
            string BookID = bookIDTB.Text;
            string Title = titleTB.Text;
            string Author = authorTB.Text;
            string ISBN = isbnTB.Text;
            string Genre = genreTB.Text;
            int Quantity = int.Parse(quantityTB.Text);
            DateTime Date = publicationdateDPTB.SelectedDate.Value;
            
            if (!db.Books.Any(b => b.Book_ID == BookID))
            {
                var _newBook = new Book
                {
                    Book_ID = BookID,
                    Book_Title = Title,
                    Book_Author = Author,
                    Book_ISBN = ISBN,
                    Book_Genre = Genre,
                    Book_PublicationDate = Date,
                    Book_Quantity = Quantity
                };

                try
                {
                    db.Books.InsertOnSubmit(_newBook);
                    db.SubmitChanges();
                    MessageBox.Show($"Staff {BookID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadBooks();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Books);
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadBooks();
                }
            }




        }    
        private void EditBook()
        {
            if (selectedBook != null)
            {
                selectedBook.Book_ID = bookIDTB.Text;
                selectedBook.Book_Title = titleTB.Text;
                selectedBook.Book_Author = authorTB.Text;
                selectedBook.Book_ISBN = isbnTB.Text;
                selectedBook.Book_Genre = genreTB.Text;
                selectedBook.Book_PublicationDate = publicationdateDPTB.SelectedDate.Value;
                selectedBook.Book_Quantity = int.Parse(quantityTB.Text);
            }
            try
            {
                db.SubmitChanges();
                MessageBox.Show($"Book {selectedBookID} has been changed!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadBooks();
            }
            catch (Exception ex)
            {
                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Books);
                MessageBox.Show($"Error in adding the book: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadBooks();
            }
        }
        private bool FieldVerify(out int error)
        {
            if (bookIDTB.Text == "" || titleTB.Text == "" || authorTB.Text == ""
                || publicationdateDPTB.SelectedDate == null || genreTB.Text == "" ||
                quantityTB.Text == "" || isbnTB.Text == "")
            {
                error = 1;
                return false;
            }
            else if (bookIDTB.Text == selectedBookID && titleTB.Text == selectedBookTitle &&
              authorTB.Text == selectedBookAuthor && publicationdateDPTB.SelectedDate == selectedBookDate
              && genreTB.Text == selectedBookGenre && quantityTB.Text == selectedBookQuantity.ToString()
              && isbnTB.Text == selectedBookISBN)
            {
                error = 2;
                return false;
            }
            else if (!int.TryParse(quantityTB.Text, out int result))
            {
                error = 3;
                return false;
            }
            else
            {
                error = 0;
                return true;
            }
        } // Error Checking
        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this book?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                db.Books.DeleteOnSubmit(selectedBook);
                db.SubmitChanges();
                MessageBox.Show($"Deleted Book: {selectedBookID}", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadBooks();
                bookDataGrid.UnselectAll();
            }
        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            bookDataGrid.UnselectAll();
        }

    }
}
