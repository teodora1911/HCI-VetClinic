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
using MaterialDesignThemes.Wpf;
using VetClinic.Dialogs;

namespace VetClinic.Views
{
    public partial class AppointmentDetails : Window
    {
        private TranslationUtils Translation;

        private IAppointmentDao AppointmentDao = DaoFactory.Instance(DaoType.MySql).Appointments;
        private IPetOwnerDao PetOwnerDao = DaoFactory.Instance(DaoType.MySql).PetOwners;
        private IPetDao PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
        private IVeterinarianDao VetDao = DaoFactory.Instance(DaoType.MySql).Veterinarians;

        private Appointment? Appointment;

        private bool Create = false;
        private bool Read = false;
        private bool Write = false;

        public AppointmentDetails(TranslationUtils translation, Appointment? appointment)
        {
            InitializeComponent();
            this.Translation = translation;
            this.Appointment = appointment;

            if (appointment is null)
            {
                Create = true;
                Appointment = new();
                SetAllComboBoxes();
            }
            else if(appointment.IsScheduled)
            {
                Read = true;
                DisableControls();
            }
            else
            {
                Write = true;
                SetVetsComboBox();
                SetControlsValues();
            }

            DataContext = translation.Language;
        }

        private void SetAllComboBoxes()
        {
            SetOwnersComboBox();
            SetPetsComboBox();
            SetVetsComboBox();
        }

        private void SetOwnersComboBox()
        {
            OwnersComboBox.SelectionChanged += OnOwnersComboBoxSelectionChanged;
            foreach (var po in PetOwnerDao.GetAll())
                OwnersComboBox.Items.Add(po);
        }

        private void OnOwnersComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(OwnersComboBox.SelectedItem is not null)
            {
                var po = (PetOwner)OwnersComboBox.SelectedItem;
                PetsComboBox.Items.Clear();
                foreach (Pet pet in PetDao.GetAllPetsOfOwner(po.Id))
                    PetsComboBox.Items.Add(pet);
            }
        }

        private void SetPetsComboBox()
        {
            PetsComboBox.Items.Clear();
            foreach (Pet pet in PetDao.GetAll())
                PetsComboBox.Items.Add(pet);
        }

        private void SetVetsComboBox()
        {
            foreach(var vet in VetDao.GetAll())
                VetsComboBox.Items.Add(vet);
        }

        private void DisableControls()
        {
            AppointmentDatePicker.Visibility = Visibility.Hidden;
            AppointmentTimePicker.Visibility = Visibility.Hidden;
            ScheduledLabel.Content = Translation.Language.AppointmentIsScheduled + " " + Appointment?.DateTime.ToString();
            ScheduledLabel.Visibility = Visibility.Visible;

            ReasonTextBox.Text = Appointment?.Reason;
            ReasonTextBox.IsReadOnly = true;

            OwnersComboBox.Visibility = Visibility.Hidden;
            OwnerLabel.Content = Appointment?.Pet.Owner.FullName;
            OwnerLabel.Visibility = Visibility.Visible;

            PetsComboBox.Visibility = Visibility.Hidden;
            PetLabel.Content = Appointment?.Pet.Name;
            PetLabel.Visibility = Visibility.Visible;

            VetsComboBox.Visibility = Visibility.Hidden;
            VetLabel.Content = Appointment?.Vet.ToString();
            VetLabel.Visibility = Visibility.Visible;

            ConfirmButton.Visibility = Visibility.Hidden;
            RejectButton.Visibility = Visibility.Hidden;
            OkButton.Visibility = Visibility.Visible;
        }

        private void SetControlsValues()
        {
            OkButton.Visibility = Visibility.Hidden;

            AppointmentDatePicker.SelectedDate = Appointment?.DateTime;
            AppointmentTimePicker.SelectedTime = Appointment?.DateTime;

            ReasonTextBox.Text = Appointment?.Reason;

            OwnersComboBox.Visibility = Visibility.Hidden;
            OwnerLabel.Content = Appointment?.Pet.Owner.FullName;
            OwnerLabel.Visibility = Visibility.Visible;

            PetsComboBox.Visibility = Visibility.Hidden;
            PetLabel.Content = Appointment?.Pet.Name;
            PetLabel.Visibility = Visibility.Visible;

            VetsComboBox.SelectedItem = Appointment?.Vet;
        }

        private bool ValidateForm()
        {
            if(AppointmentDatePicker.SelectedDate is null)
            {
                AppointmentDatePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if(AppointmentTimePicker.SelectedTime is null)
            {
                AppointmentTimePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(ReasonTextBox.Text))
            {
                ReasonTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if(VetsComboBox.SelectedItem is null)
            {
                VetsComboBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }

            if (Create)
            {
                if (OwnersComboBox.SelectedItem is null)
                {
                    OwnersComboBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return false;
                }
                if (PetsComboBox.SelectedItem is null)
                {
                    PetsComboBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return false;
                }
            }

            return true;
        }

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                Appointment.DateTime = AppointmentDatePicker.SelectedDate.GetValueOrDefault().Date + AppointmentTimePicker.SelectedTime.GetValueOrDefault().TimeOfDay;
                Appointment.Reason = ReasonTextBox.Text;
                Appointment.Vet = (Veterinarian)VetsComboBox.SelectedItem;
                if (Create)
                {
                    Appointment.Pet = (Pet)PetsComboBox.SelectedItem;
                    if(AppointmentDao.Create(Appointment) > 0)
                    {
                        FinishEditing(true);
                    }
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
                else if (Write)
                {
                    if (AppointmentDao.Update(Appointment))
                        FinishEditing(true);
                    else
                    {
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                        FinishEditing(false);
                    }
                }
            }
            else
            {
                BannerLabel.Content = Translation.Language.EmptyFieldsErrorMessage;
                BannerLabel.Visibility = Visibility.Visible;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) => SubmitForm();

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);

        private void OkButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);

        private void FinishEditing(bool result)
        {
            this.DialogResult = result;
            Close();
        }
    }
}
