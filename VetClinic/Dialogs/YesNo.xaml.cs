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

namespace VetClinic.Dialogs
{
    public partial class YesNo : Window
    {

        public YesNo(string message, string confirmation, string rejection)
        {
            InitializeComponent();
            MessageTextBox.Text = message;
            MessageTextBox.IsReadOnly = true;
            ConfirmationButton.Content = confirmation;
            RejectionButton.Content = rejection;
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => ReturnResultAndClose(true);

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => ReturnResultAndClose(false);

        private void ReturnResultAndClose(bool result)
        {
            this.DialogResult = result;
            Close();
        }
    }
}
