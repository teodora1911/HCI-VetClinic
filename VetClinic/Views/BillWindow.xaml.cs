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
using VetClinic.Utils;
using VetClinic.Models.Entities;
using VetClinic.Dao;
using VetClinic.Dialogs;

namespace VetClinic.Views
{
    public partial class BillWindow : Window
    {
        private TranslationUtils Translation;
        private Bill Bill;
        private bool IsReadOnly = false;

        private IExaminationDao ExaminationDao = DaoFactory.Instance(DaoType.MySql).Examinations;

        public BillWindow(TranslationUtils translation, Bill bill, bool isReadonly)
        {
            InitializeComponent();
            Translation = translation;
            Bill = bill;
            IsReadOnly = isReadonly;

            SetControls();
            DataContext = Translation.Language;
        }

        private void SetControls()
        {
            PetOwnerTextBox.Text = Bill.Owner.FullName;
            PetOwnerTextBox.IsReadOnly = true;
            TotalPriceTextBox.Text = Bill.Price.ToString();
            TotalPriceTextBox.IsReadOnly = true;

            if(IsReadOnly)
            {
                PaymentComboBox.Visibility = Visibility.Hidden;
                PaymentLabel.Content = Bill.Payment;
                PaymentLabel.Visibility = Visibility.Visible;

                ConfirmButton.Visibility = Visibility.Hidden;
                RejectButton.Visibility = Visibility.Hidden;    
                OkButton.Visibility = Visibility.Visible;    
            }
            else
            {
                PaymentComboBox.Items.Add(Translation.Language.CashPayment);
                PaymentComboBox.Items.Add(Translation.Language.CardPayment);
                PaymentComboBox.SelectedIndex = 0;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) => SaveBill();

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SaveBill();

        private void SaveBill()
        {
            if(PaymentComboBox.SelectedItem is null)
                PaymentComboBox.SelectedIndex = 0;

            Bill.Payment = PaymentComboBox.SelectedItem.ToString();
            if (!ExaminationDao.InsertBill(Bill))
                new CustomMessageBox(Translation.Language.InternalServerError).Show();

            Close();
        }

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => Close();

        private void OkButtonClick(object sender, RoutedEventArgs e) => Close();
    }
}
