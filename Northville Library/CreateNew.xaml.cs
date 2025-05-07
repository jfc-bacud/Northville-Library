using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;
using System.Xml.Linq;

namespace Northville_Library
{
    /// <summary>
    /// Interaction logic for CreateNew.xaml
    /// </summary>
    public partial class CreateNew : Window
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
        public CreateNew()
        {
            InitializeComponent();
        }

        private void newuserBT_Click(object sender, RoutedEventArgs e)
        {
            if (!fieldVerify())
            {
                MessageBox.Show("Please fill all input fields!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                addUser();
            }
        }

        private bool fieldVerify()
        {
            if (idTB.Text == "" || passwordTB.Text == "" || fnameTB.Text == "" || lnameTB.Text == "" || contactTB.Text == "" || emailTB.Text == "")
                return false;

            return true;
        }

        private void addUser()
        {
            string userID = idTB.Text.Trim();
            string password = passwordTB.Text.Trim();
            string firstName = fnameTB.Text.Trim();
            string lastName = lnameTB.Text.Trim();
            string contact = contactTB.Text.Trim();
            string email = emailTB.Text.Trim();
            string role = roleCB.Text.Trim();

            if (roleCB.SelectedIndex == 0)
            {
                addStudent(userID, password, firstName, lastName, contact, email, role);
            }
            else
            {
                addStaff(userID, password, firstName, lastName, contact, email, role);
            }
        }

        private void addStudent(string uID, string pass, string fName, string lName, string contact, string email, string role)
        {
            if (!db.Students.Any(u => u.Student_ID == uID))
            {
                var roleDefinition = (from r in db.Roles
                                     where r.Role_Name == role
                                     select r.Role_ID).FirstOrDefault();

                var _newStudent = new Student
                {
                    Student_ID = uID,
                    Student_FirstName = fName,
                    Student_LastName = lName,
                    Student_Password = pass,
                    Student_ContactNum = contact,
                    Student_Email = email,
                    Role_ID = roleDefinition
                };

                db.Students.InsertOnSubmit(_newStudent);

                try
                {
                    db.SubmitChanges();
                    MessageBox.Show($"User {uID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    initializeClosing();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void addStaff(string uID, string pass, string fName, string lName, string contact, string email, string role)
        {
            if (!db.Staffs.Any(u => u.Staff_ID == uID))
            {
                var roleDefinition = (from r in db.Roles
                                      where r.Role_Name == role
                                      select r.Role_ID).FirstOrDefault();

                var _newStaff = new Staff
                {
                    Staff_ID = uID,
                    Staff_FirstName = fName,
                    Staff_LastName = lName,
                    Staff_Password = pass,
                    Staff_ContactNum = contact,
                    Staff_Email = email,
                    Role_ID = roleDefinition
                };

                db.Staffs.InsertOnSubmit(_newStaff);

                try
                {
                    db.SubmitChanges();
                    MessageBox.Show($"User {uID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    initializeClosing();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    initializeClosing();
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void initializeClosing()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }


    }
}
