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

namespace Northville_Library.AdminWindow_Pages
{
    /// <summary>
    /// Interaction logic for AdminStudent.xaml
    /// </summary>
    public partial class AdminStudent : Page
    {
        DataClasses1DataContext db;

        private string selectedstudentID;
        private string selectedFirstName;
        private string selectedLastName;
        private string selectedPassword;
        private string selectedCourse;
        private string selectedEmail;
        private string selectedContact;
        Student selectedStudent;

        public AdminStudent()
        {
            InitializeComponent();
            LoadStudent();
        }
        private void LoadStudent()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            studentDataGrid.ItemsSource = null;
            studentDataGrid.ItemsSource = db.Students.ToList();

            var courses = from c in db.Courses
                          select c.Course_Name;

            courseCB.ItemsSource = courses.ToList();
        } // Loads Data Grid
        private void staffDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Role" || e.PropertyName == "Course" || e.PropertyName == "Role_ID" ||
                e.PropertyName == "Transactions" || e.PropertyName == "VisitLogs")
            {
                e.Cancel = true;
            }
        } // Auto
        private void staffDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (studentDataGrid.SelectedItem != null && studentDataGrid.SelectedItem is Student _selectedItem)
            {
                selectedStudent = _selectedItem;
                saveLocalChangeables();
                studentIDTB.IsEnabled = false;
                AddEditBTN.Content = "Edit";
                deleteBTN.IsEnabled = true;
                deselectBTN.IsEnabled = true;
            }
            else
            {
                selectedStudent = null;
                removeLocalChangeables();
                studentIDTB.IsEnabled = true;
                deleteBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
                AddEditBTN.Content = "Add";
            }
        } // When checking
        private void saveLocalChangeables()
        {
            var course = (from c in db.Courses
                        where c.Course_ID == selectedStudent.Course_ID
                        select c.Course_Name).FirstOrDefault();

            selectedstudentID = selectedStudent.Student_ID;
            selectedFirstName = selectedStudent.Student_FirstName;
            selectedLastName = selectedStudent.Student_LastName;
            selectedPassword = selectedStudent.Student_Password;
            selectedEmail = selectedStudent.Student_Email;
            selectedContact = selectedStudent.Student_ContactNum;
            selectedCourse = course.ToString();

            FillFields();
        }
        private void FillFields()
        {
            studentIDTB.Text = selectedstudentID;
            courseCB.SelectedIndex = courseCB.Items.IndexOf(selectedCourse);
            studentFirstNameTB.Text = selectedFirstName;
            studentLastNameTB.Text = selectedLastName;
            studentContactTB.Text = selectedContact;
            studentEmailTB.Text = selectedEmail;
            passwordTB.Text = selectedPassword;
        }
        private void RemoveFields()
        {
            studentIDTB.Text = "";
            studentFirstNameTB.Text = "";
            studentLastNameTB.Text = "";
            studentContactTB.Text = "";
            studentEmailTB.Text = "";
            passwordTB.Text = "";
        }
        private void removeLocalChangeables()
        {
            selectedstudentID = null;
            selectedFirstName = null;
            selectedLastName = null;
            selectedPassword = null;
            selectedEmail = null;
            selectedContact = null;
            selectedCourse = null;

            RemoveFields();
        }
        private void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            int error;

            if (AddEditBTN.Content.ToString() == "Add" && studentDataGrid.SelectedItem == null)
            {
                if (!FieldVerify(out error))
                {
                    if (error == 1)
                        MessageBox.Show("Please fill all input fields!", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    else if (error == 2)
                        MessageBox.Show("There's no changes to be made!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    AddStaff();
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
                }
                else
                {
                    EditStaff();
                }
            }
        }
        private bool FieldVerify(out int error)
        {
            if (studentIDTB.Text == "" || studentFirstNameTB.Text == "" || studentLastNameTB.Text == ""
                || studentEmailTB.Text == "" || studentContactTB.Text == "" || passwordTB.Text == "")
            {
                error = 1;
                return false;
            }
            else if (studentIDTB.Text == selectedstudentID && studentFirstNameTB.Text == selectedFirstName &&
              studentLastNameTB.Text == selectedLastName && studentContactTB.Text == selectedContact
              && studentEmailTB.Text == selectedEmail && passwordTB.Text == selectedPassword && courseCB.SelectedIndex
              == courseCB.Items.IndexOf(selectedCourse))
            {
                error = 2;
                return false;
            }
            else
            {
                error = 0;
                return true;
            }
        } // Error Checking
        private void AddStaff()
        {
            string studentID = studentIDTB.Text;
            string firstName = studentFirstNameTB.Text;
            string lastName = studentLastNameTB.Text;
            string email = studentEmailTB.Text;
            string password = passwordTB.Text;
            string contact = studentContactTB.Text;
            string course = courseCB.Text;

            if (!db.Students.Any(s => s.Student_ID == studentID))
            {
                var courseDefiinition = (from c in db.Courses
                                       where c.Course_Name == course
                                       select c.Course_ID).FirstOrDefault();

                var _newStudent = new Student
                {
                    Student_ID = studentID,
                    Student_FirstName = firstName,
                    Student_LastName = lastName,
                    Student_ContactNum = contact,
                    Student_Password = password,
                    Student_Email = email,
                    Role_ID = "R02",
                    Course_ID = courseDefiinition,
                };

                try
                {
                    db.Students.InsertOnSubmit(_newStudent);
                    db.SubmitChanges();
                    MessageBox.Show($"Staff {studentID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStudent();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Students);
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadStudent();
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditStaff()
        {
            var courseDefinition = (from c in db.Courses
                                  where c.Course_Name == courseCB.Text
                                  select c.Course_ID).FirstOrDefault();

            if (selectedStudent != null)
            {
                selectedStudent.Student_FirstName = studentFirstNameTB.Text;
                selectedStudent.Student_LastName = studentLastNameTB.Text;
                selectedStudent.Student_ContactNum = studentContactTB.Text;
                selectedStudent.Student_Email = studentEmailTB.Text;
                selectedStudent.Student_Password = passwordTB.Text;
                selectedStudent.Course_ID = courseDefinition;
            }

            try
            {
                db.SubmitChanges();
                MessageBox.Show($"Staff {selectedstudentID} has been changed!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStudent();
            }
            catch (Exception ex)
            {
                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Students);
                MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadStudent();
            }
        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            studentDataGrid.UnselectAll();
        }
        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this account?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                db.Students.DeleteOnSubmit(selectedStudent);
                db.SubmitChanges();
                MessageBox.Show($"Deleted User: {selectedstudentID}", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadStudent();
                studentDataGrid.UnselectAll();
            }
        }

    }
}
