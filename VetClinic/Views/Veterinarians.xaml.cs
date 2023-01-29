using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using VetClinic.Dao;
using VetClinic.Dialogs;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Views
{
    public partial class Veterinarians : Window
    {

        private ListViewDataContext<Veterinarian> VeterinarianViewModel;
        private TranslationUtils Translation;
        private IVeterinarianDao VeterinarianDao;

        public Veterinarians(TranslationUtils translation)
        {
            InitializeComponent();
            Translation = translation;
            VeterinarianDao = DaoFactory.Instance(DaoType.MySql).Veterinarians;
            UpdateDataContext();
            SetDataGridHeaders();
        }

        private void UpdateDataContext()
        {
            VeterinarianViewModel = new ListViewDataContext<Veterinarian>()
            {
                Items = new ObservableCollection<Veterinarian>(VeterinarianDao.GetAll()),
                Language = Translation.Language
            };
            DataContext = VeterinarianViewModel;
        }

        private void SetDataGridHeaders()
        {
            //IdColumn.Header = Translation.Language.IdHeader;
            NameColumn.Header = Translation.Language.PersonNameHeader;
            SurnameColumn.Header = Translation.Language.PersonSurnameHeader;
            UsernameColumn.Header = Translation.Language.PersonUsernameHeader;
            //PasswordColumn.Header = Translation.Language.PersonPasswordHeader;
            EmailColumn.Header = Translation.Language.PersonEmailHeader;
            ContactColumn.Header = Translation.Language.PersonContactHeader;
            TitleColumn.Header = Translation.Language.VeterinarianTitleHeader;
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(VetSearchQueryTextBox.Text))
            {
                UpdateDataContext();
                return;
            }

            VeterinarianViewModel = new ListViewDataContext<Veterinarian>()
            {
                Items = new ObservableCollection<Veterinarian>(VeterinarianDao.GetBySearchQuery(VetSearchQueryTextBox.Text)),
                Language = Translation.Language
            };
            DataContext = VeterinarianViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            VetSearchQueryTextBox.Text = null;
            UpdateDataContext();
        }

        private void AddNewVetAccountClick(object sender, RoutedEventArgs e) => OpenVeterinariansDetalsWindow(null);

        private void DeleteVeterinarianClick(object sender, RoutedEventArgs e)
        {
            if(VeterinariansDataGrid.SelectedItem is Veterinarian)
            {
                Veterinarian? SelectedItem = (Veterinarian)VeterinariansDataGrid.CurrentItem;
                if(SelectedItem != null)
                {
                    YesNo YesNoDialog = new YesNo(Translation.Language.DeleteConfirmationString, Translation.Language.YesNoDialogConfirmationString, Translation.Language.YesNoDialogRejectionString);
                    if (YesNoDialog.ShowDialog() == true)
                    {
                        if (VeterinarianDao.DeleteById(SelectedItem.User.Id))
                            Search();
                        else new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    }
                }
            }
        }

        private void VeterinariansDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenVeterinariansDetalsWindow(VeterinarianViewModel.SelectedItem);

        private void OpenVeterinariansDetalsWindow(Veterinarian? veterinarian)
        {
            VeterinarianDetails Details = new VeterinarianDetails(this.Translation, this.VeterinarianDao, veterinarian);
            if (Details.ShowDialog() == true)
                Search();
        }

        private void VetUpdateClick(object sender, RoutedEventArgs e) => OpenVeterinariansDetalsWindow(VeterinarianViewModel.SelectedItem);
    }
}
