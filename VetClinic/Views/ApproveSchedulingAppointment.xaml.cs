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
using VetClinic.Utils;
using VetClinic.Models.Entities;
using VetClinic.Dialogs;

namespace VetClinic.Views
{
    public partial class ApproveSchedulingAppointment : Window
    {
        private TranslationUtils Translation;

        private IAppointmentDao AppointmentDao = DaoFactory.Instance(DaoType.MySql).Appointments;
        private Appointment Appointment;
        private string AddressDefault = "Adresa Veterinarske ambulante";

        public ApproveSchedulingAppointment(TranslationUtils translation, Appointment appointment)
        {
            InitializeComponent();
            this.Translation = translation;
            this.Appointment = appointment;

            SetValues();
            DataContext = translation.Language;
        }

        private void SetValues()
        {
            AppointmentDatePicker.SelectedDate = Appointment.DateTime;
            AppointmentTimePicker.SelectedTime = Appointment.DateTime;
            ReasonTextBox.Text = Appointment.Reason;
            ReasonTextBox.IsReadOnly = true;
            OwnerLabel.Content = Appointment.Pet.Owner.FullName;
            PetLabel.Content = Appointment.Pet.Name;
            GenderLabel.Content = Appointment.Pet.Gender;
            SpeciesLabel.Content = Appointment.Pet.Species.Name;
        }

        private bool ValidateForm()
        {
            if (AppointmentDatePicker.SelectedDate is null)
            {
                AppointmentDatePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (AppointmentTimePicker.SelectedTime is null)
            {
                AppointmentTimePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }

            return true;
        }

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                Appointment.DateTime = AppointmentDatePicker.SelectedDate.GetValueOrDefault().Date + AppointmentTimePicker.SelectedTime.GetValueOrDefault().TimeOfDay;
                string address = string.IsNullOrEmpty(AddressTextBox.Text) ? AddressDefault : AddressTextBox.Text;
                if (AppointmentDao.Schedule(Appointment, address))
                    FinishEditing(true);
                else
                {
                    new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    FinishEditing(false);
                }
            }
            else
            {
                BannerLabel.Content = Translation.Language.EmptyFieldsErrorMessage;
                BannerLabel.Visibility = Visibility.Visible;
            }
        }

        private void FinishEditing(bool result)
        {
            this.DialogResult = result;
            Close();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) => SubmitForm();

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);
    }
}
