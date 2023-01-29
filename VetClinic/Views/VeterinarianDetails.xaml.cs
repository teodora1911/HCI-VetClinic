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
    public partial class VeterinarianDetails : Window
    {

        private TranslationUtils Translation;
        private IVeterinarianDao VeterinarianDao;
        private Veterinarian Veterinarian;
        private bool Updating;

        public VeterinarianDetails(TranslationUtils translation, IVeterinarianDao dao, Veterinarian? veterinarian)
        {
            InitializeComponent();
            this.Translation = translation;
            this.VeterinarianDao = dao;
            this.Veterinarian = (veterinarian == null) ? new() { User = new() } : veterinarian;
            this.Updating = (veterinarian != null);
            DataContext = translation.Language;
            SetComboBox();
            SetTextAreas();
        }

        private void SetComboBox()
        {
            TitleComboBox.SelectionChanged += OnSelectionChanged;
            List<string> titles = VeterinarianDao.GetTitles();
            foreach (string title in titles)
                TitleComboBox.Items.Add(title);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TitleComboBox.SelectedItem != null)
                TitleTextBox.Text = TitleComboBox.SelectedItem.ToString();
        }

        private void SetTextAreas()
        {
            if (!string.IsNullOrEmpty(Veterinarian.User.Name))
                NameTextBox.Text = Veterinarian.User.Name;
            if (!string.IsNullOrEmpty(Veterinarian.User.Surname))
                SurnameTextBox.Text = Veterinarian.User.Surname;
            if (!string.IsNullOrEmpty(Veterinarian.User.Username))
            {
                UsernameTextBox.Text = Veterinarian.User.Username;
                UsernameTextBox.IsReadOnly = true;
            }
            if (!string.IsNullOrEmpty(Veterinarian.User.Password))
                PasswordTextBox.Password = Veterinarian.User.Password;
            if (!string.IsNullOrEmpty(Veterinarian.User.Email))
                EmailTextBox.Text = Veterinarian.User.Email;
            if (!string.IsNullOrEmpty(Veterinarian.User.Contact))
                ContactTextBox.Text = Veterinarian.User.Contact;
            if (!string.IsNullOrEmpty(Veterinarian.Title))
            {
                TitleTextBox.Text = Veterinarian.Title;
                if (TitleComboBox.Items.Contains(Veterinarian.Title))
                    TitleComboBox.SelectedItem = Veterinarian.Title;
            }
        }

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                Veterinarian.User.Name = NameTextBox.Text;
                Veterinarian.User.Surname = SurnameTextBox.Text;
                Veterinarian.User.Username = UsernameTextBox.Text;
                Veterinarian.User.Password = PasswordTextBox.Password;
                Veterinarian.User.Email = EmailTextBox.Text;
                Veterinarian.User.Contact = ContactTextBox.Text;
                Veterinarian.Title = TitleTextBox.Text;

                if (Updating)
                {
                    if (VeterinarianDao.Update(Veterinarian) == true)
                        FinishEditing(true);
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
                else
                {
                    int response = VeterinarianDao.Create(Veterinarian);
                    if (response < 0)
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                    else if (response == 0)
                    {
                        BannerLabel.Content = Translation.Language.UsernameIsNotAvailable;
                        BannerLabel.Visibility = Visibility.Visible;
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

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                SubmitForm();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(SurnameTextBox.Text))
            {
                SurnameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(UsernameTextBox.Text))
            {
                UsernameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                PasswordTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
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
            if (string.IsNullOrEmpty(TitleTextBox.Text))
            {
                TitleTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }

            return true;
        }

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);

        private void FinishEditing(bool result)
        {
            this.DialogResult = result;
            Close();
        }
    }
}
