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
using System.Collections.ObjectModel;
using VetClinic.Dialogs;

namespace VetClinic.Views
{
    public partial class Appointments : Window
    {
        private ListViewDataContext<Appointment> AppointmentViewModel;
        private TranslationUtils Translation;
        private IAppointmentDao AppointmentDao = DaoFactory.Instance(DaoType.MySql).Appointments;
        private Veterinarian? LoggedVeterinarian;
        private bool AdminView;

        public Appointments(TranslationUtils translation, Veterinarian? logged)
        {
            InitializeComponent();
            Translation = translation;
            LoggedVeterinarian = logged;
            AdminView = (logged is null);

            SchedulingTypeComboBox.SelectionChanged += OnSelectionChanged;
            SetComboBox();
            SetDataGridHeaders();
            Search();
        }

        private void SetDataGridHeaders()
        {
            DateTimeColumn.Header = Translation.Language.DateTimeHeader;
            ReasonColumn.Header = Translation.Language.ReasonHeader;
            PetColumn.Header = Translation.Language.PetAppExamHeader;
            OwnerColumn.Header = Translation.Language.OwnerAppExamHeader;

            if (AdminView)
            {
                VetColumn.Header = Translation.Language.VetAppExamHeader;
            }
            else
            {
                VetColumn.Header = "";
                VetColumn.Width = DataGridLength.SizeToHeader;
                VetColumn.Visibility = Visibility.Hidden;
                VetSearchQueryTextBox.Text = LoggedVeterinarian.User.Name + " " + LoggedVeterinarian.User.Surname;
                VetSearchQueryTextBox.IsReadOnly = true;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e) => Search();

        private void SetComboBox()
        {
            SchedulingTypeComboBox.Items.Clear();
            SchedulingTypeComboBox.Items.Add(Translation.Language.NonScheduledString);
            SchedulingTypeComboBox.Items.Add(Translation.Language.ScheduledString);
            SchedulingTypeComboBox.SelectedItem = Translation.Language.NonScheduledString;
        }

        private void Search()
        {
            string owner = string.IsNullOrEmpty(OwnerSearchQueryTextBox.Text) ? "" : OwnerSearchQueryTextBox.Text;
            string vet = string.IsNullOrEmpty(VetSearchQueryTextBox.Text) ? "" : VetSearchQueryTextBox.Text;
            bool scheduled = SchedulingTypeComboBox.SelectedIndex != 0;
            AppointmentViewModel = new ListViewDataContext<Appointment>()
            {
                Items = new ObservableCollection<Appointment>(AppointmentDao.GetBySpecs(owner, vet, scheduled)),
                Language = Translation.Language
            };
            DataContext = AppointmentViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            OwnerSearchQueryTextBox.Text = null;
            if (AdminView)
                VetSearchQueryTextBox.Text = null;
            SchedulingTypeComboBox.SelectedIndex = 0;
            Search();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void AppointmentsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (AdminView)
            {
                OpenAppointmentDetailsWindow(AppointmentViewModel.SelectedItem);
            }
        }

        private void AppointmentUpdateClick(object sender, RoutedEventArgs e)
        {
            if (AdminView)
            {
                OpenAppointmentDetailsWindow(AppointmentViewModel.SelectedItem);
            }
        }

        private void DeleteAppointmentClick(object sender, RoutedEventArgs e)
        {
            if(AppointmentViewModel.SelectedItem is not null)
            {
                if(AppointmentViewModel.SelectedItem.IsScheduled)
                    new CustomMessageBox(Translation.Language.AppointmentIsScheduled).Show();
                else
                {
                    if (!AppointmentDao.DeleteById(AppointmentViewModel.SelectedItem.Id))
                        new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    else
                        Search();
                }
            }
        }

        private void AddAppointmentClick(object sender, RoutedEventArgs e) => OpenAppointmentDetailsWindow(null);

        private void OpenAppointmentDetailsWindow(Appointment? appointment)
        {
            AppointmentDetails Details = new AppointmentDetails(Translation, appointment);
            if (Details.ShowDialog() == true)
                Search();
        }
    }
}
