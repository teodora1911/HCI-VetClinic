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
using VetClinic.Dao;
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Views
{
    public partial class PetsAndOwners : Window
    {
        private ListViewDataContext<PetOwner> OwnerViewModel;
        private TranslationUtils Translation;
        private IPetOwnerDao PetOwnerDao;

        public PetsAndOwners(TranslationUtils translation)
        {
            InitializeComponent();
            Translation = translation;
            PetOwnerDao = DaoFactory.Instance(DaoType.MySql).PetOwners;
            UpdateDataContext();
            SetDataGridHeaders();
        }

        private void UpdateDataContext()
        {
            OwnerViewModel = new ListViewDataContext<PetOwner>()
            {
                Items = new ObservableCollection<PetOwner>(PetOwnerDao.GetAll()),
                Language = Translation.Language
            };
            DataContext = OwnerViewModel;
        }

        private void SetDataGridHeaders()
        {
            //IdColumn.Header = Translation.Language.IdHeader;
            NameColumn.Header = Translation.Language.PersonFullNameHeader;
            EmailColumn.Header = Translation.Language.PersonEmailHeader;
            ContactColumn.Header = Translation.Language.PersonContactHeader;
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(OwnerSearchQueryTextBox.Text))
            {
                UpdateDataContext();
                return;
            }

            OwnerViewModel = new ListViewDataContext<PetOwner>()
            {
                Items = new ObservableCollection<PetOwner>(PetOwnerDao.GetBySearchQuery(OwnerSearchQueryTextBox.Text)),
                Language = Translation.Language
            };
            DataContext = OwnerViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void CloseButtonClick(object sender, RoutedEventArgs e) => Close();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            OwnerSearchQueryTextBox.Text = null;
            UpdateDataContext();
        }

        private void PerOwnersDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenOwnerDetailsWindow(OwnerViewModel.SelectedItem);

        private void AddOwnerAccountClick(object sender, RoutedEventArgs e) => OpenOwnerDetailsWindow(null);

        private void OpenOwnerDetailsWindow(PetOwner? owner)
        {
            PetOwnerDetails Details = new PetOwnerDetails(this.Translation, this.PetOwnerDao, owner);
            if (Details.ShowDialog() == true)
                Search();
        }

        private void ShowPetOwnerPets(object sender, RoutedEventArgs e)
        {
            new Pets(Translation, OwnerViewModel.SelectedItem).Show();
        }

        private void AddNewPetForOwner(object sender, RoutedEventArgs e)
        {
            new PetInsertWindow(Translation, OwnerViewModel.SelectedItem).ShowDialog();
        }
    }
}
