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

namespace Northville_Library.StudentWindow_Pages
{
    /// <summary>
    /// Interaction logic for StudentTransaction.xaml
    /// </summary>
    public partial class StudentTransaction : Page
    {
        DataClasses1DataContext db = new DataClasses1DataContext(Properties.Settings.Default.NorthvilleConnectionString);
        private string localStudentUID;
        public StudentTransaction(string studentUID)
        {
            localStudentUID = studentUID;
            InitializeComponent();
            LoadMyTransactions();
            LoadMyFines();
        }

        private void LoadMyTransactions()
        {
            var transactions = from t in db.Transactions
                               where t.Student_ID == localStudentUID
                               select new
                               {
                                   t.Transaction_ID,
                                   t.Book_ID,
                                   t.Borrow_Date,
                                   t.Due_Date,
                                   t.Return_Date,
                                   t.Transaction_Status
                               };

            transactionDataGrid.ItemsSource = transactions;

            if (transactionDataGrid == null)
            {
                transactionsresultsLBL.Content = "0 Transactions Found";
            }
            else
            {
                transactionsresultsLBL.Content = $"{transactionDataGrid.Items.Count} Transaction/s Found";
            }
        } // Loads All Users' Transactions
        private void LoadMyFines() // Loads All Users' Fines
        {
            List<object> finesList = new List<object>();

            var fines = from f in db.Fines
                               select new
                               {
                                   f.Fines_ID,
                                   f.Transaction_ID,
                                   f.Fine_Amount,
                                   f.Days_OverDue,
                                   f.Fines_Status
                               };

            var transactions = from t in db.Transactions
                               where t.Student_ID == localStudentUID
                               select t;

            foreach (var fine in fines)
            {
                foreach (var transaction in transactions)
                {
                    if (fine.Transaction_ID == transaction.Transaction_ID)
                    {
                        finesList.Add(fine);
                        break;
                    }
                }
            }

            finesDataGrid.ItemsSource = finesList;

            if (finesDataGrid == null)
            {
                finesresultsLBL.Content = "0 Fines Found";
            }
            else
            {
                finesresultsLBL.Content = $"{finesDataGrid.Items.Count} Fine/s Found";
            }
        }
    }
}
