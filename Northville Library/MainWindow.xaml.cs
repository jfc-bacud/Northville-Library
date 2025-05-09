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
using System.Xml.Linq;

namespace Northville_Library
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataClasses1DataContext db;

        public MainWindow()
        {
            InitializeComponent();
            refreshDatabase();
        }

        private void LoginBT_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBT_Verify())
            {
                if (!userExists())
                {
                    MessageBox.Show("User does not exist!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    if (!verifyPass())
                    {
                        MessageBox.Show("Password is incorrect!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        openWindow(getUserRole());
                    }
                }
            }
        }
        private bool LoginBT_Verify()
        {
            if (LoginTB.Text == "" || PasswordTB.Text == "")
            {
                MessageBox.Show("Please fill in all fields!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        private bool userExists()
        {
            var studentNames = (from u in db.Students
                          where u.Student_ID == LoginTB.Text
                          select u).FirstOrDefault();

            var staffNames = (from u in db.Staffs
                              where u.Staff_ID == LoginTB.Text
                              select u).FirstOrDefault();

            if (studentNames == null && staffNames == null)
            {
                return false;
            }

            return true; 
        }
        private bool verifyPass()
        {
            var staff = (from u in db.Staffs
                          where u.Staff_ID == LoginTB.Text
                          select u).FirstOrDefault();

            var student = (from u in db.Students
                           where u.Student_ID == LoginTB.Text
                           select u).FirstOrDefault();

            
            if (staff != null)
            {
                if (PasswordTB.Text == staff.Staff_Password)
                    return true;
            }

            if (student != null)
            {
                if (PasswordTB.Text == student.Student_Password)
                    return true;
            }
            return false;
        }
        private string getUserRole()
        {
            string uRole = "";

            var student = (from u in db.Students
                                where u.Student_ID == LoginTB.Text
                                select u).FirstOrDefault();

            var staff = (from u in db.Staffs
                              where u.Staff_ID == LoginTB.Text
                              select u).FirstOrDefault();

            if (student != null)
            {
                uRole = student.Role_ID.ToString();
                return uRole;
            }

            if (staff != null)
            {
                uRole = staff.Role_ID.ToString();
                return uRole;
            }

            return uRole;
        }
        private void openWindow(string role)
        {
            switch (role)
            {
                case "R01":
                    break;

                case "R02": // STUDENT
                    StudentWindow studentWindow = new StudentWindow(LoginTB.Text.ToString());
                    studentWindow.Show();
                    this.Close();
                    break;

                case "R03":
                    break;
            }
        }
        private void CreateBT_Click(object sender, RoutedEventArgs e)
        {
            CreateNew newUser = new CreateNew();
            newUser.Show();
            this.Close();

        }
        private void refreshDatabase()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
        }
    }
}
