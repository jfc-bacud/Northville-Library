using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for AdminStaff.xaml
    /// </summary>
    public partial class AdminStaff : Page
    {
        DataClasses1DataContext db;
        private string localadminUID;

        private string selectedadminID;
        private string selectedFirstName;
        private string selectedLastName;
        private string selectedPassword;
        private string selectedRole;
        private string selectedEmail;
        private string selectedContact;
        Staff selectedStaff;
        public AdminStaff(string adminUID)
        {
            localadminUID = adminUID;
            InitializeComponent();
            LoadStaff();
        }
        private void LoadStaff()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            staffDataGrid.ItemsSource = null;
            staffDataGrid.ItemsSource = db.Staffs.Where(st => st.Staff_ID != localadminUID).ToList();

            var role = from r in db.Roles
                       where r.Role_Name != "Student"
                       select r.Role_Name;

            roleCB.ItemsSource = role.ToList();
        } // Loads Data Grid
        private void staffDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Role")
            {
                e.Cancel = true;
            }
        } // Auto
        private void staffDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (staffDataGrid.SelectedItem != null && staffDataGrid.SelectedItem is Staff _selectedItem)
            {
                selectedStaff = _selectedItem;
                saveLocalChangeables();
                staffIDTB.IsEnabled = false;
                AddEditBTN.Content = "Edit";
                deleteBTN.IsEnabled = true;
                deselectBTN.IsEnabled = true;
            }
            else
            {
                selectedStaff = null;
                removeLocalChangeables();
                staffIDTB.IsEnabled = true;
                deleteBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
                AddEditBTN.Content = "Add";
            }
        } // When checking
        private void saveLocalChangeables()
        {
            var role = (from r in db.Roles
                        where r.Role_ID == selectedStaff.Role_ID
                        select r.Role_Name).FirstOrDefault();

            selectedadminID = selectedStaff.Staff_ID;
            selectedFirstName = selectedStaff.Staff_FirstName;
            selectedLastName = selectedStaff.Staff_LastName;
            selectedPassword = selectedStaff.Staff_Password;
            selectedEmail = selectedStaff.Staff_Email;
            selectedContact = selectedStaff.Staff_ContactNum;
            selectedRole = role.ToString();

            FillFields();
        }
        private void FillFields()
        {
            staffIDTB.Text = selectedadminID;
            roleCB.SelectedIndex = roleCB.Items.IndexOf(selectedRole);
            staffFirstNameTB.Text = selectedFirstName;
            staffLastNameTB.Text = selectedLastName;
            staffContactTB.Text = selectedContact;
            staffEmailTB.Text = selectedEmail;
            passwordTB.Text = selectedPassword;
        }
        private void RemoveFields()
        {
            staffIDTB.Text = "";
            staffFirstNameTB.Text = "";
            staffLastNameTB.Text = "";
            staffContactTB.Text = "";
            staffEmailTB.Text = "";
            passwordTB.Text = "";
        }
        private void removeLocalChangeables()
        {
            selectedadminID = null;
            selectedFirstName = null;
            selectedLastName = null;
            selectedPassword = null;
            selectedEmail = null;
            selectedContact = null;
            selectedRole = null;

            RemoveFields();
        }
        private void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            int error;

            if (AddEditBTN.Content.ToString() == "Add" && staffDataGrid.SelectedItem == null)
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
            if (staffIDTB.Text == "" || staffFirstNameTB.Text == "" || staffLastNameTB.Text == ""
                || staffEmailTB.Text == "" || staffContactTB.Text == "" || passwordTB.Text == "")
            {
                error = 1;
                return false;
            }
            else if (staffIDTB.Text == selectedadminID && staffFirstNameTB.Text == selectedFirstName &&
              staffLastNameTB.Text == selectedLastName && staffContactTB.Text == selectedContact
              && staffEmailTB.Text == selectedEmail && passwordTB.Text == selectedPassword && roleCB.SelectedIndex
              == roleCB.Items.IndexOf(selectedRole))
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
            string adminID = staffIDTB.Text;
            string firstName = staffFirstNameTB.Text;
            string lastName = staffLastNameTB.Text;
            string email = staffEmailTB.Text;
            string password = passwordTB.Text;
            string contact = staffContactTB.Text;
            string role = roleCB.Text;

            if (!db.Staffs.Any(s => s.Staff_ID == adminID))
            {
                var roleDefiinition = (from r in db.Roles
                                       where r.Role_Name == role
                                       select r.Role_ID).FirstOrDefault();

                var _newStaff = new Staff
                {
                    Staff_ID = adminID,
                    Staff_FirstName = firstName,
                    Staff_LastName = lastName,
                    Staff_ContactNum = contact,
                    Staff_Password = password,
                    Staff_Email = email,
                    Role_ID = roleDefiinition,
                };

                try
                {
                    db.Staffs.InsertOnSubmit(_newStaff);
                    db.SubmitChanges();
                    MessageBox.Show($"Staff {adminID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadStaff();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Staffs);
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadStaff();
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditStaff()
        {
            var roleDefinition = (from r in db.Roles
                                   where r.Role_Name == roleCB.Text
                                   select r.Role_ID).FirstOrDefault();

            if (selectedStaff != null)
            {
                selectedStaff.Staff_FirstName = staffFirstNameTB.Text;
                selectedStaff.Staff_LastName = staffLastNameTB.Text;
                selectedStaff.Staff_ContactNum = staffContactTB.Text;
                selectedStaff.Staff_Email = staffEmailTB.Text;
                selectedStaff.Staff_Password = passwordTB.Text;
                selectedStaff.Role_ID = roleDefinition;
            }

            try
            {
                db.SubmitChanges();
                MessageBox.Show($"Staff {selectedadminID} has been changed!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadStaff();
            }
            catch (Exception ex)
            {
                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Staffs);
                MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadStaff();
            }

        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            staffDataGrid.UnselectAll();
        }
        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this account?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                db.Staffs.DeleteOnSubmit(selectedStaff);
                db.SubmitChanges();
                MessageBox.Show($"Deleted User: {selectedadminID}", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadStaff();
                staffDataGrid.UnselectAll();
            }
        }
    }
}
