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
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        DataClasses1DataContext db;
        StudentProfile profilePage;
        StudentTransaction transactionPage;
        StudentBooks booksPage;

        public string localstudentUID { get; set; }
        public StudentWindow(string studentUID)
        {
            localstudentUID = studentUID;
            InitializeComponent();
            retrieveUserInformation();
            displayFirstPage();
        }
        public void displayFirstPage()
        {
            profilePage = new StudentProfile(localstudentUID);
            pageFrame.Content = profilePage;
        }
        public void retrieveUserInformation()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

            var student = (from s in db.Students
                           where s.Student_ID == localstudentUID
                           select s).FirstOrDefault();

            string roleID = student.Role_ID.ToString();

            var role = (from r in db.Roles
                       where r.Role_ID == roleID
                       select r).FirstOrDefault();

            unameLBL.Content = $"{student.Student_FirstName}!";
            studentidLBL.Content = student.Student_ID;
            roleLBL.Content = role.Role_Name;

        } // Retrieve User Information
        private void profileBTN_Click(object sender, RoutedEventArgs e)
        {
            displayFirstPage();
        }
        public void deleteClose()
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Close();
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
        private void transactionBTN_Click(object sender, RoutedEventArgs e)
        {
            transactionPage = new StudentTransaction(localstudentUID);
            pageFrame.Content = transactionPage;
        }
        private void booksBTN_Click(object sender, RoutedEventArgs e)
        {
            booksPage = new StudentBooks(localstudentUID);
            pageFrame.Content = booksPage;
        }
    }
}
