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
    public partial class PetOwnerDetails : Window
    {
        private TranslationUtils Translation;
        private IPetOwnerDao PetOwnerDao;
        private PetOwner PetOwner;
        private bool Updating;

        public PetOwnerDetails(TranslationUtils translation, IPetOwnerDao dao, PetOwner? owner)
        {
            InitializeComponent();
            this.Translation = translation;
            this.PetOwnerDao = dao;
            this.PetOwner = (owner == null) ? new(): owner;
            this.Updating = (owner != null);
            DataContext = translation.Language;
            SetTextAreas();
        }

        private void SetTextAreas()
        {
            if (!string.IsNullOrEmpty(PetOwner.FullName))
                NameTextBox.Text = PetOwner.FullName;
            if(!string.IsNullOrEmpty(PetOwner.Email))
                EmailTextBox.Text = PetOwner.Email;
            if(!string.IsNullOrEmpty(PetOwner.Contact))
                ContactTextBox.Text = PetOwner.Contact;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SubmitForm();
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                PetOwner.FullName = NameTextBox.Text;
                PetOwner.Email = EmailTextBox.Text;
                PetOwner.Contact = ContactTextBox.Text;

                if (Updating)
                {
                    if (PetOwnerDao.Update(PetOwner))
                        FinishEditing(true);
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
                else
                {
                    int response = PetOwnerDao.Create(PetOwner);
                    if (response <= 0)
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                    else FinishEditing(true);
                }
            }
            else
            {
                BannerLabel.Content = Translation.Language.EmptyFieldsErrorMessage;
                BannerLabel.Visibility = Visibility.Visible;
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(ContactTextBox.Text))
            {
                ContactTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }

            return true;
        }

        private void FinishEditing(bool success)
        {
            this.DialogResult = success;
            Close();
        }
    }
}
