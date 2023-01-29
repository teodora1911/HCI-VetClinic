using Google.Protobuf.WellKnownTypes;
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
using VetClinic.Dao;
using VetClinic.Dialogs;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Views
{
    public partial class ServiceDetails : Window
    {

        private TranslationUtils Translation;
        private IServiceDao ServiceDao;
        private Service Service;
        private bool Updating;

        private decimal cost;

        public ServiceDetails(TranslationUtils translation, IServiceDao dao, Service service)
        {
            InitializeComponent();
            this.Translation = translation;
            this.Service = (service == null) ? new() : service;
            this.Updating = (service != null);
            this.ServiceDao = dao;
            DataContext = translation.Language;
            SetTextAreas();
        }

        private void SetTextAreas()
        {
            if (!string.IsNullOrEmpty(this.Service.Name))
                NameTextBox.Text = this.Service.Name;
            if (!string.IsNullOrEmpty(this.Service.Description))
                DescriptionTextBox.Text = this.Service.Description;
            if (this.Service.Cost != 0)
                CostTextBox.Text = this.Service.Cost.ToString();
        }

        // Returns 0 if everything is okay
        // 1 if name field is empty
        // 2 if parsing is unsuccessful
        private int ValidateForm()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }
            try
            {
                cost = decimal.Parse(CostTextBox.Text);
                return 0;
            }
            catch (FormatException)
            {
                CostTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 2;
            }
        }

        private void SubmitForm()
        {
            int response = ValidateForm();
            if(response == 0)
            {
                Service.Name = NameTextBox.Text;
                Service.Description = DescriptionTextBox.Text;
                Service.Cost = cost;

                if (Updating)
                {
                    if(ServiceDao.Update(Service) == true)
                        FinishEditing(true);
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
                else
                {
                    int insert = ServiceDao.Create(Service);
                    if (insert <= 0)
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                    else FinishEditing(true);
                }
            } 
            else if(response == 1)
            {
                BannerLabel.Content = Translation.Language.EmptyFieldsErrorMessage;
                BannerLabel.Visibility = Visibility.Visible;
            }
            else
            {
                BannerLabel.Content = Translation.Language.DecimalStringFormatException;
                BannerLabel.Visibility = Visibility.Visible;
            }
        }

        private void FinishEditing(bool result)
        {
            this.DialogResult = result;
            Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SubmitForm();
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);
    }
}
