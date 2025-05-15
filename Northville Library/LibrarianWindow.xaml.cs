using Northville_Library.AdminWindow_Pages;
using Northville_Library.LibrarianWindow_Pages;
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
    /// Interaction logic for LibrarianWindow.xaml
    /// </summary>
    public partial class LibrarianWindow : Window
    {
        DataClasses1DataContext db;
        LibrarianProfile profilePage;
        LibrarianBooks booksPage;
        LibrarianTransactions transactionsPage;
        LibrarianFines finesPage;

        private string localLibrarianID;
        public LibrarianWindow(string librarianID)
        {
            localLibrarianID = librarianID;
            InitializeComponent();
            retrieveUserInformation();
            displayFirstPage();
        }

        public void retrieveUserInformation()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

            var staff = (from s in db.Staffs
                         where s.Staff_ID == localLibrarianID
                         select s).FirstOrDefault();

            string roleID = staff.Role_ID.ToString();

            var role = (from r in db.Roles
                        where r.Role_ID == roleID
                        select r).FirstOrDefault();

            unameLBL.Content = $"{staff.Staff_FirstName}!";
            staffidLBL.Content = staff.Staff_ID;
            roleLBL.Content = role.Role_Name.ToString();
        }
        private void displayFirstPage()
        {
            booksPage = new LibrarianBooks();
            pageFrame.Content = booksPage;
        }
        private void booksBTN_Click(object sender, RoutedEventArgs e)
        {
            displayFirstPage();
        }
        private void logoutBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                MainWindow window = new MainWindow();
                window.Show();
                this.Close();
            }
            else
            {
                return;
            }
        }
        private void profileBTN_Click(object sender, RoutedEventArgs e)
        {
            profilePage = new LibrarianProfile(localLibrarianID);
            pageFrame.Content = profilePage;
        }
        public void deleteClose()
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        } // Method that closes the window IF user decides to delete their account
        private void transactionBTN_Click(object sender, RoutedEventArgs e)
        {
            transactionsPage = new LibrarianTransactions();
            pageFrame.Content = transactionsPage;
        }
        public void finesNavToPage()
        {
            finesPage = new LibrarianFines();
            pageFrame.Content = finesPage;
        }
        public void transactionNavToPage()
        {
            transactionsPage = new LibrarianTransactions();
            pageFrame.Content = transactionsPage;
        }

    }
}
