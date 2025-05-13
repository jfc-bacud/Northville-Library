using Northville_Library.AdminWindow_Pages;
using Northville_Library.StudentWindow_Pages;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        DataClasses1DataContext db;
        AdminProfile profilePage;
        AdminTransaction transactionPage;
        AdminStaff staffPage;
        AdminStudent studentPage;
        AdminCourse coursePage;
        AdminFines finesPage;

        private string localadminUID;

        public AdminWindow(string adminUID)
        {
            localadminUID = adminUID;
            InitializeComponent();
            retrieveUserInformation();
            displayFirstPage();
        }

        public void retrieveUserInformation()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

            var student = (from s in db.Staffs
                           where s.Staff_ID == localadminUID
                           select s).FirstOrDefault();

            string roleID = student.Role_ID.ToString();

            var role = (from r in db.Roles
                        where r.Role_ID == roleID
                        select r).FirstOrDefault();

            unameLBL.Content = $"{student.Staff_FirstName}!";
            staffidLBL.Content = student.Staff_ID;
            roleLBL.Content = role.Role_Name;

        } // Retrieve User Information for UI displays (labels) at Window
        private void profileBTN_Click(object sender, RoutedEventArgs e)
        {
            displayFirstPage();
        }
        private void displayFirstPage()
        {
            profilePage = new AdminProfile(localadminUID);
            pageFrame.Content = profilePage;
        }
        public void deleteClose()
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
        } // Method that closes the window IF user decides to delete their account
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
        private void transactionBTN_Click(object sender, RoutedEventArgs e)
        {
            transactionPage = new AdminTransaction();
            pageFrame.Content = transactionPage;
        }
        private void stafflistBTN_Click(object sender, RoutedEventArgs e)
        {
            staffPage = new AdminStaff(localadminUID);
            pageFrame.Content = staffPage;
        }
        private void studentlistBTN_Click(object sender, RoutedEventArgs e)
        {
            studentPage = new AdminStudent();
            pageFrame.Content = studentPage;
        }
        private void courseBTN_Click(object sender, RoutedEventArgs e)
        {
            coursePage = new AdminCourse();
            pageFrame.Content = coursePage;
        }
        public void finesNavToPage()
        {
            finesPage = new AdminFines();
            pageFrame.Content = finesPage;
        }
        public void transactionNavToPage()
        {
            transactionPage = new AdminTransaction();
            pageFrame.Content = transactionPage;
        }
    }
}
