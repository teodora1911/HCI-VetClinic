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
    public partial class Pets : Window
    {
        private ListViewDataContext<Pet> PetViewModel;
        private TranslationUtils Translation;
        private IPetDao PetDao;
        private PetOwner PetOwner;

        public Pets(TranslationUtils translation, PetOwner owner)
        {
            InitializeComponent();
            Translation = translation;
            PetOwner = owner;
            PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
            SetComboBoxesInitial();
            UpdateDataContext();
            SetComboBoxes();
            SetDataGridHeaders();
        }

        private void SetComboBoxesInitial()
        {
            SpeciesComboBox.SelectionChanged += OnSpeciesComboBoxSelectionChanged;
            BreedComboBox.SelectionChanged += OnBreedComboBoxSelectionChanges;
        }

        private void OnSpeciesComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeciesComboBox.SelectedItem != null)
            {
                if(BreedComboBox.Items.Count > 0)
                {
                    var b = (Breed)BreedComboBox.Items.GetItemAt(BreedComboBox.Items.Count - 1);
                    if (!b.Species.Equals((Species)SpeciesComboBox.SelectedItem))
                    {
                        BreedComboBox.SelectedItem = null;
                        BreedComboBox.Items.Clear();
                        List<Breed> breeds = PetDao.GetBreedsFromSpecies((Species)SpeciesComboBox.SelectedItem);
                        foreach (Breed breed in breeds)
                            BreedComboBox.Items.Add(breed);
                    }
                }
                else
                {
                    BreedComboBox.Items.Clear();
                    List<Breed> breeds = PetDao.GetBreedsFromSpecies((Species)SpeciesComboBox.SelectedItem);
                    foreach (Breed breed in breeds)
                        BreedComboBox.Items.Add(breed);
                }

                Search();
            }
        }

        private void OnBreedComboBoxSelectionChanges(object sender, SelectionChangedEventArgs e)
        {
            if(BreedComboBox.SelectedItem != null)
            {
                var b = (Breed)BreedComboBox.SelectedItem;
                var s = b.Species;
                SpeciesComboBox.SelectedItem = s;
                Search();
            }
        }

        private void UpdateDataContext()
        {
            PetViewModel = new ListViewDataContext<Pet>()
            {
                Items = new ObservableCollection<Pet>(PetDao.GetAllPetsOfOwner(PetOwner.Id)),
                Language = Translation.Language
            };
            DataContext = PetViewModel;
        }

        private void SetComboBoxes()
        {
            SpeciesComboBox.Items.Clear();
            BreedComboBox.Items.Clear();
            SpeciesComboBox.SelectedItem = null;
            BreedComboBox.SelectedItem = null;

            List<Species> species = PetDao.GetAllSpecies();
            foreach (Species spec in species)
                SpeciesComboBox.Items.Add(spec);
            List<Breed> breeds = PetDao.GetAllBreeds();
            foreach (Breed breed in breeds)
                BreedComboBox.Items.Add(breed);
        }

        private void SetDataGridHeaders()
        {
            NameColumn.Header = Translation.Language.PersonNameHeader;
            SpeciesColumn.Header = Translation.Language.SpeciesHeader;
            GenderColumn.Header = Translation.Language.GenderHeader;
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(PetSearchQueryTextBox.Text) && SpeciesComboBox.SelectedItem is null && BreedComboBox.SelectedItem is null)
            {
                UpdateDataContext();
                return;
            }

            string query = string.IsNullOrEmpty(PetSearchQueryTextBox.Text) ? "" : PetSearchQueryTextBox.Text;
            Species? s = null;
            Breed? b = null;
            if (SpeciesComboBox.SelectedItem is not null)
                s = (Species)SpeciesComboBox.SelectedItem;
            if (BreedComboBox.SelectedItem is not null)
                b = (Breed)BreedComboBox.SelectedItem;
            PetViewModel = new ListViewDataContext<Pet>()
            {
                Items = new ObservableCollection<Pet>(PetDao.GetAllPetsBySpecs(PetOwner.Id, query, s, b)),
                Language = Translation.Language
            };
            DataContext = PetViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            PetSearchQueryTextBox.Text = null;
            SetComboBoxes();
            UpdateDataContext();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void OpenPetDetailsWindow()
        {
            new PetDetails(Translation, PetViewModel.SelectedItem, PetOwner, true).ShowDialog();
        }

        private void PetsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenPetDetailsWindow();

        private void PetDetailsButtonClick(object sender, RoutedEventArgs e) => OpenPetDetailsWindow();

        private void AddNewPetClick(object sender, RoutedEventArgs e)
        {
            if (new PetInsertWindow(Translation, PetOwner).ShowDialog() == true)
                Search();
        }
    }
}
