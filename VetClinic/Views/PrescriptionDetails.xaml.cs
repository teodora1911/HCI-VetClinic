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
    public partial class PrescriptionDetails : Window
    {
        private TranslationUtils Translation;
        private Prescription Prescription;
        private IExaminationDao ExamDao = DaoFactory.Instance(DaoType.MySql).Examinations;

        int dose;
        bool Updating;
        bool IsReadOnly;

        public PrescriptionDetails(TranslationUtils translation, Prescription prescription, bool updating, bool IsReadOnly)
        {
            InitializeComponent();
            this.Translation = translation;
            DataContext = Translation.Language;
            this.Prescription = prescription;
            this.Updating = updating;
            SetControls();

            if (IsReadOnly)
                DisableControls();
        }

        private void SetControls()
        {
            MedicineLabel.Content = Prescription.Medicine.Name;
            if (!string.IsNullOrEmpty(Prescription.Name))
                NameTextBox.Text = Prescription.Name;
            if (Prescription.Dose != 0)
                DoseTextBox.Text = Prescription.Dose.ToString();
            if (!string.IsNullOrEmpty(Prescription.Frequency))
                FrequencyTextBox.Text = Prescription.Frequency;
            if (Prescription.Start != null)
                StartDatePicker.SelectedDate = Prescription.Start;
            if (!string.IsNullOrEmpty(Prescription.Duration))
                DurationTextBox.Text = Prescription.Duration;
            if (!string.IsNullOrEmpty(Prescription.Instructions))
                InstructionsTextBox.Text = Prescription.Instructions;
        }

        private void DisableControls()
        {
            NameTextBox.IsReadOnly = true;
            InstructionsTextBox.IsReadOnly = true;
            StartDatePicker.Visibility = Visibility.Hidden;
            StartDateLabel.Content = Prescription.Start.ToString();
            StartDateLabel.Visibility = Visibility.Visible;
            DurationTextBox.IsReadOnly = true;
            DoseTextBox.IsReadOnly = true;
            FrequencyTextBox.IsReadOnly = true;

            ConfirmButton.Visibility = Visibility.Hidden;
            RejectButton.Visibility = Visibility.Hidden;
            OkButton.Visibility = Visibility.Visible;
        }

        private int ValidateForm()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }
            try
            {
                dose = int.Parse(DoseTextBox.Text);
                if (dose <= 0) throw new Exception();
            }
            catch (Exception)
            {
                DoseTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 2;
            }
            if(StartDatePicker.SelectedDate is null)
            {
                StartDatePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }
            if (string.IsNullOrEmpty(FrequencyTextBox.Text))
            {
                FrequencyTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }

            if (string.IsNullOrEmpty(DurationTextBox.Text))
            {
                DurationTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }
            if (string.IsNullOrEmpty(InstructionsTextBox.Text))
            {
                InstructionsTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }

            return 0;
        }

        private void SubmitForm()
        {
            int response = ValidateForm();
            if (response == 0)
            {
                Prescription.Name = NameTextBox.Text;
                Prescription.Dose = dose;
                Prescription.Frequency = FrequencyTextBox.Text;
                Prescription.Start = StartDatePicker.SelectedDate;
                Prescription.Duration = DurationTextBox.Text;
                Prescription.Instructions = InstructionsTextBox.Text;

                if (Updating && ExamDao.UpdatePrescription(Prescription))
                    FinishEditing(true);
                else if (!Updating && ExamDao.AddPrescription(Prescription))
                    FinishEditing(true);
                else
                {
                    new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    FinishEditing(false);
                }
            }
            else if (response == 1)
            {
                BannerLabel.Content = Translation.Language.EmptyFieldsErrorMessage;
                BannerLabel.Visibility = Visibility.Visible;
            }
            else
            {
                BannerLabel.Content = Translation.Language.NaturalNumberStringFormatException;
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

        private void OkButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);
    }
}
