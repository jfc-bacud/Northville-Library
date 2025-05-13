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
    /// Interaction logic for AdminCourse.xaml
    /// </summary>
    public partial class AdminCourse : Page
    {
        DataClasses1DataContext db;
        private string selectedCourseID;
        private string selectedCourseName;

        Course selectedCourse;
        public AdminCourse()
        {
            InitializeComponent();
            LoadCourses();
        }
        private void LoadCourses()
        {
            db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
            courseDataGrid.ItemsSource = null;
            courseDataGrid.ItemsSource = db.Courses.ToList();

        }
        private void staffDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (courseDataGrid.SelectedItem != null && courseDataGrid.SelectedItem is Course _selectedItem)
            {
                selectedCourse = _selectedItem;
                saveLocalChangeables();
                courseIDTB.IsEnabled = false;
                AddEditBTN.Content = "Edit";
                deleteBTN.IsEnabled = true;
                deselectBTN.IsEnabled = true;
            }
            else
            {
                selectedCourse = null;
                removeLocalChangeables();
                courseIDTB.IsEnabled = true;
                deleteBTN.IsEnabled = false;
                deselectBTN.IsEnabled = false;
                AddEditBTN.Content = "Add";
            }
        } // When checking
        private void staffDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Students")
            {
                e.Cancel = true;
            }
        } // Auto
        private void saveLocalChangeables()
        {
            selectedCourseID = selectedCourse.Course_ID;
            selectedCourseName = selectedCourse.Course_Name;
            FillFields();
        }
        private void FillFields()
        {
            courseIDTB.Text = selectedCourseID;
            coursenameTB.Text = selectedCourseName;
        }
        private void RemoveFields()
        {
            courseIDTB.Text = "";
            coursenameTB.Text = "";
        }
        private void removeLocalChangeables()
        {
            selectedCourseID = null;
            selectedCourseName = null;
            RemoveFields();
        }
        private void AddEditBTN_Click(object sender, RoutedEventArgs e)
        {
            int error;

            if (AddEditBTN.Content.ToString() == "Add" && courseDataGrid.SelectedItem == null)
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
                    AddCourse();
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
            if (courseIDTB.Text == "" || coursenameTB.Text == "")
            {
                error = 1;
                return false;
            }
            else if (courseIDTB.Text == selectedCourseID && coursenameTB.Text == selectedCourseName) 
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
        private void AddCourse()
        {
            string courseID = courseIDTB.Text;
            string courseName = coursenameTB.Text;

            if (!db.Courses.Any(c => c.Course_ID == courseID))
            {
                var _newCourse = new Course
                {
                    Course_ID = courseID,
                    Course_Name = courseName,
                };

                try
                {
                    db.Courses.InsertOnSubmit(_newCourse);
                    db.SubmitChanges();
                    MessageBox.Show($"Staff {courseID} has been successfully added!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCourses();
                }
                catch (Exception ex)
                {
                    db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Students);
                    MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    LoadCourses();
                }
            }
            else
            {
                MessageBox.Show("User already exists!", "Existing User", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void EditStaff()
        {
            if (selectedCourse != null)
            {
                selectedCourse.Course_Name = coursenameTB.Text;
            }

            try
            {
                db.SubmitChanges();
                MessageBox.Show($"Staff {selectedCourseID} has been changed!", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadCourses();
            }
            catch (Exception ex)
            {
                db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, db.Students);
                MessageBox.Show($"Error in adding the user: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                LoadCourses();
            }
        }
        private void deselectBTN_Click(object sender, RoutedEventArgs e)
        {
            courseDataGrid.UnselectAll();
        }
        private void deleteBTN_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this course?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                db.Courses.DeleteOnSubmit(selectedCourse);
                db.SubmitChanges();
                MessageBox.Show($"Deleted User: {selectedCourseID}", "Status Message", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadCourses();
                courseDataGrid.UnselectAll();
            }
        }
    }
}
