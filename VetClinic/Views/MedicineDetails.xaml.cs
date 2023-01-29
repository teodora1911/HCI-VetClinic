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
using VetClinic.Utils;
using MedicineEntity = VetClinic.Models.Entities.Medicine;

namespace VetClinic.Views
{
    public partial class MedicineDetails : Window
    {

        private TranslationUtils Translation;
        private IMedicineDao MedicineDao;
        private MedicineEntity Medicine;
        private bool Updating;

        public MedicineDetails(TranslationUtils translation, IMedicineDao dao, MedicineEntity? medicine)
        {
            InitializeComponent();
            this.Translation = translation;
            this.MedicineDao = dao;
            this.Medicine = (medicine == null) ? new() : medicine;
            this.Updating = (medicine != null);
            DataContext = translation.Language;
            SetComboBox();
            SetTextAreas();
        }

        private void SetComboBox()
        {
            TypeComboBox.SelectionChanged += OnSelectionChanged;
            List<string> types = MedicineDao.GetTypes();
            foreach (string type in types)
                TypeComboBox.Items.Add(type);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedItem != null)
                TypeTextBox.Text = TypeComboBox.SelectedItem.ToString();
        }

        private void SetTextAreas()
        {
            if (!string.IsNullOrEmpty(this.Medicine.Name))
                NameTextBox.Text = this.Medicine.Name;
            if (!string.IsNullOrEmpty(this.Medicine.Description))
                DescriptionTextBox.Text = this.Medicine.Description;
            if (!string.IsNullOrEmpty(this.Medicine.Type))
            {
                TypeTextBox.Text = this.Medicine.Type;
                if (TypeComboBox.Items.Contains(this.Medicine.Type))
                    TypeComboBox.SelectedItem = this.Medicine.Type;
            }
        }

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                Medicine.Name = NameTextBox.Text;
                Medicine.Type = TypeTextBox.Text;
                Medicine.Description = DescriptionTextBox.Text;

                if (Updating)
                {
                    if (MedicineDao.Update(Medicine) == true)
                        FinishEditing(true);
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
                else
                {
                    int response = MedicineDao.Create(Medicine);
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
            if (string.IsNullOrEmpty(TypeTextBox.Text))
            {
                TypeTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }

            return true;
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
