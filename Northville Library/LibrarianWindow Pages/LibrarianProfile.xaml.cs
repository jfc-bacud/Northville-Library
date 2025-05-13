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
    /// Interaction logic for LibrarianProfile.xaml
    /// </summary>
    public partial class LibrarianProfile : Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

        private string localLibrarianUID;
        private string localFirstName;
        private string localLastName;
        private string localPassword;
        private string localEmail;
        private string localContact;
        public LibrarianProfile(string librarianUID)
        {
            localLibrarianUID = librarianUID;
            InitializeComponent();
            retrieveUserInformation();
        }
        public void retrieveUserInformation()
        {
            var staff = (from s in db.Staffs
                         where s.Staff_ID == localLibrarianUID
                         select s).FirstOrDefault();

            displayIDTB.Text = staff.Staff_ID.ToString();
            editFirstNameTB.Text = staff.Staff_FirstName.ToString();
            editLastNameTB.Text = staff.Staff_LastName.ToString();
            editPasswordTB.Text = staff.Staff_Password.ToString();
            editContactTB.Text = staff.Staff_ContactNum.ToString();
            editEmailTB.Text = staff.Staff_Email.ToString();
            displayRoleTB.Text = staff.Role_ID.ToString();

            saveLocalChangeable();
        } // Retrieve's current user's information
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
        private void saveBT_Click(object sender, RoutedEventArgs e)
        {
            int errorState;
            if (hasChangesNotBlank(out errorState))
            {
                MessageBoxResult result = MessageBox.Show("Are you sure of your changes?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var staff = (from s in db.Staffs
                                 where s.Staff_ID == localLibrarianUID
                                 select s).FirstOrDefault();

                    staff.Staff_FirstName = editFirstNameTB.Text;
                    staff.Staff_LastName = editLastNameTB.Text;
                    staff.Staff_Password = editPasswordTB.Text;
                    staff.Staff_Email = editEmailTB.Text;
                    staff.Staff_ContactNum = editContactTB.Text;

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
        } // Save any changes done
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

        } // Updates the main window for any changes. Mainly used if the First Name has been changed so that changes reflect IMMEDIATELY
        private void deleteBT_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your account?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var studentToDelete = db.Students.SingleOrDefault(s => s.Student_ID == localLibrarianUID);

                db.Students.DeleteOnSubmit(studentToDelete);
                db.SubmitChanges();
                db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);

                MessageBox.Show($"Deleted User: {localLibrarianUID}, returning to login page.", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                onDelete();
            }
        } // Delete's user from table
        private void onDelete()
        {
            if (Window.GetWindow(this) is LibrarianWindow librarianWindow)
            {
                librarianWindow.deleteClose();
            }
        } // Leads to the delete event in StudentWindow that closes the current window
    }
}
