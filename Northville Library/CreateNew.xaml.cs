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
            populateCourse();
        }

        private void populateCourse()
        {
            var courses = from c in db.Courses
                          select c.Course_Name;

            courseCB.ItemsSource = courses.ToList();
        } // Populates courseCB
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
        } // Error Handling
        private bool fieldVerify()
        {
            if (idTB.Text == "" || passwordTB.Text == "" || fnameTB.Text == "" || lnameTB.Text == "" || contactTB.Text == "" || emailTB.Text == "")
                return false;

            return true;
        } // Checks if any forms are null or empty
        private void addUser()
        {
            string userID = idTB.Text.Trim();
            string password = passwordTB.Text.Trim();
            string firstName = fnameTB.Text.Trim();
            string lastName = lnameTB.Text.Trim();
            string contact = contactTB.Text.Trim();
            string email = emailTB.Text.Trim();
            string role = roleCB.Text.Trim();
            string course = courseCB.Text.Trim();

            if (roleCB.SelectedIndex == 0)
            {
                addStudent(userID, password, firstName, lastName, contact, email, role, course);
            }
            else
            {
                addStaff(userID, password, firstName, lastName, contact, email, role);
            }
        } // Checking of user of which table should they be added
        private void addStudent(string uID, string pass, string fName, string lName, string contact, string email, string role, string course)
        {
            if (!db.Students.Any(u => u.Student_ID == uID))
            {
                var roleDefinition = (from r in db.Roles
                                     where r.Role_Name == role
                                     select r.Role_ID).FirstOrDefault();

                var courseDefinition = (from c in db.Courses
                                      where c.Course_Name == course
                                      select c.Course_ID).FirstOrDefault();

                var _newStudent = new Student
                {
                    Student_ID = uID,
                    Student_FirstName = fName,
                    Student_LastName = lName,
                    Student_Password = pass,
                    Student_ContactNum = contact,
                    Student_Email = email,
                    Role_ID = roleDefinition,
                    Course_ID = courseDefinition
                };

                try
                {
                    db.Students.InsertOnSubmit(_newStudent);
                    db.SubmitChanges();
                    MessageBox.Show($"User {uID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Students);
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
        } // Adding to Student Table
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

                try
                {
                    db.Staffs.InsertOnSubmit(_newStaff);
                    db.SubmitChanges();
                    MessageBox.Show($"User {uID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    initializeClosing();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Staffs);
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    initializeClosing();
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        } // Adding to Staff Table
        private void initializeClosing()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        } // For Closing of Window
        private void roleCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courseCB == null)
            {
                return;
            }
            else
            {
                if (roleCB.SelectedIndex == 0)
                    courseCB.IsEnabled = true;
                else
                    courseCB.IsEnabled = false;
            }
        } // Checks if which index of roleCB is selected. If not Student, disable Course Combobox
    }
}
