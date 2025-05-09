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

namespace Northville_Library.StudentWindow_Pages
{
    /// <summary>
    /// Interaction logic for StudentProfile.xaml
    /// </summary>
    public partial class StudentProfile : Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

        public string localstudentUID; // Saved locally to compare and retrieve old information 
        private string localFirstName;
        private string localLastName;
        private string localPassword;
        private string localEmail;
        private string localContact;

        public StudentProfile(string studentUID)
        {
            localstudentUID = studentUID;
            InitializeComponent();
            retrieveUserInformation();
        }
        public void retrieveUserInformation()
        {
            var student = (from s in db.Students
                           where s.Student_ID == localstudentUID
                           select s).FirstOrDefault();

            displayIDTB.Text = student.Student_ID.ToString();
            editFirstNameTB.Text = student.Student_FirstName.ToString();
            editLastNameTB.Text = student.Student_LastName.ToString();
            editPasswordTB.Text = student.Student_Password.ToString();
            editContactTB.Text = student.Student_ContactNum.ToString();
            editEmailTB.Text = student.Student_Email.ToString();
            displayCourseTB.Text = student.Course_ID.ToString();
            displayRoleTB.Text = student.Role_ID.ToString();

            saveLocalChangeable();
        } // Retrieves all info and displays it sa page
        private void saveBT_Click(object sender, RoutedEventArgs e)
        {
            int errorState;
            if (hasChangesNotBlank(out errorState))
            {
                MessageBoxResult result = MessageBox.Show("Are you sure of your changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var student = (from s in db.Students
                                   where s.Student_ID == localstudentUID
                                   select s).FirstOrDefault();

                    student.Student_FirstName = editFirstNameTB.Text;
                    student.Student_LastName = editLastNameTB.Text;
                    student.Student_Password = editPasswordTB.Text;
                    student.Student_Email = editEmailTB.Text;
                    student.Student_ContactNum = editContactTB.Text;

                    db.SubmitChanges();
                    saveLocalChangeable();
                    db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
                    updateUserInfo();
                    MessageBox.Show($"Changes have been saved!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (errorState == 1)
                    MessageBox.Show("Fill all input fields!", $"Error {errorState}", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (errorState == 2)
                    MessageBox.Show("There's nothing to change!", $"Error {errorState}", MessageBoxButton.OK, MessageBoxImage.Warning);

                returnLocalChangeable();
                return;
            }
        } // Saves any changes done
        private void saveLocalChangeable()
        {
            localFirstName = editFirstNameTB.Text.ToString();
            localLastName = editLastNameTB.Text.ToString();
            localPassword = editPasswordTB.Text.ToString();
            localEmail = editEmailTB.Text.ToString();
            localContact = editContactTB.Text.ToString();
        } // Changes old local saves to current saves IF changes were done to the database
        private void returnLocalChangeable()
        {
            editFirstNameTB.Text = localFirstName;
            editLastNameTB.Text = localLastName;
            editPasswordTB.Text = localPassword;
            editEmailTB.Text = localEmail;
            editContactTB.Text = localContact;
        } // In case of failure, returns fields to their original values through old local saves
        private bool hasChangesNotBlank(out int errorState)
        {
            if (editFirstNameTB.Text != "" && editLastNameTB.Text != "" && editPasswordTB.Text != ""
                && editEmailTB.Text != "" && editContactTB.Text != "")
            {
                if (localFirstName != editFirstNameTB.Text || localLastName != editLastNameTB.Text || localPassword != editPasswordTB.Text
                    || localEmail != editEmailTB.Text || localContact != editContactTB.Text)
                {
                    errorState = -1;
                    return true;
                }
                else
                {
                    errorState = 2;
                    return false;
                }
            }
            else
            {
                errorState = 1;
                return false;
            }
        } // Input validation to check if there are changes and no blanks
        private void updateUserInfo()
        {
            if (Window.GetWindow(this) is StudentWindow studentWindow)
            {
                studentWindow.retrieveUserInformation();
            }

/*
            var mainWindow = Application.Current.MainWindow as StudentWindow;
            mainWindow.retrieveUserInformation();*/
        } // Updates the main window for any changes. Mainly used if the First Name has been changed so that changes reflect IMMEDIATELY
        private void deleteBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var studentToDelete = db.Students.SingleOrDefault(s => s.Student_ID == localstudentUID);

                db.Students.DeleteOnSubmit(studentToDelete);
                db.SubmitChanges();
                db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

                MessageBox.Show($"Deleted User: {localstudentUID}, returning to login page.", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                onDelete();
            }
        } // Deletes user from table lmao
        private void onDelete()
        {
            if (Window.GetWindow(this) is StudentWindow studentWindow)
            {
                studentWindow.deleteClose();
            }
        } // Leads to the delete event in StudentWindow that closes the current window
    }
}
