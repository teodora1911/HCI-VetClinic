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
    public partial class PetInsertWindow : Window
    {
        private TranslationUtils Translation;
        private Pet Pet = new() { Species = new(), Breeds = new() };
        private IPetDao PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
        private string SelectedGender = "F";

        public PetInsertWindow(TranslationUtils translation, PetOwner owner)
        {
            InitializeComponent();
            this.Translation = translation;
            this.Pet.Owner = owner;
            DataContext = translation.Language;
            SetComboBoxes();
            GenderTextBox.Text = Translation.Language.GenderFemale;
        }

        private void SetComboBoxes()
        {
            SpeciesComboBox.SelectionChanged += OnSpeciesComboBoxSelectionChanged;
            BreedComboBox.SelectionChanged += OnBreedComboBoxSelectionChanged;
            List<Species> species = PetDao.GetAllSpecies();
            foreach (Species spec in species)
                SpeciesComboBox.Items.Add(spec);
        }

        private void OnSpeciesComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SpeciesComboBox.SelectedItem != null)
            {
                SpeciesTextBox.Text = SpeciesComboBox.SelectedItem.ToString();
                BreedComboBox.Items.Clear();
                BreedTextBox.Text = null;
                List<Breed> breeds = PetDao.GetBreedsFromSpecies((Species)SpeciesComboBox.SelectedItem);
                foreach (Breed breed in breeds)
                    BreedComboBox.Items.Add(breed);
            }
        }

        private void OnBreedComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BreedComboBox.SelectedItem != null)
                BreedTextBox.Text = BreedComboBox.SelectedItem.ToString();
        }

        private void SubmitForm()
        {
            if (ValidateForm())
            {
                Pet.Name = NameTextBox.Text;
                Pet.Gender = SelectedGender;

                if (SpeciesComboBox.SelectedItem is null)
                {
                    Pet.Species.Id = 0;
                    Pet.Species.Name = SpeciesTextBox.Text;
                }
                else Pet.Species = (Species)SpeciesComboBox.SelectedItem;

                if(BreedComboBox.SelectedItem is null)
                    Pet.Breeds.Add(new Breed()
                    {
                        Id = 0,
                        Name = BreedTextBox.Text,
                        Species = Pet.Species
                    });
                else Pet.Breeds.Add((Breed)BreedComboBox.SelectedItem);

                Pet.Diagnosis = "";
                Pet.HealthCondition = "";

                if (PetDao.Create(Pet) <= 0)
                {
                    new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    FinishEditing(false);
                }
                else FinishEditing(true);
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
            if (string.IsNullOrEmpty(SpeciesTextBox.Text))
            {
                SpeciesTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return false;
            }
            if (string.IsNullOrEmpty(BreedTextBox.Text))
            {
                BreedTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
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

        private void ChangeGenterToFemale(object sender, RoutedEventArgs e)
        {
            SelectedGender = "F";
            GenderTextBox.Text = Translation.Language.GenderFemale;
        }

        private void ChangeGenterToMale(object sender, RoutedEventArgs e)
        {
            SelectedGender = "M";
            GenderTextBox.Text = Translation.Language.GenderMale;
        }

        private void ConfirmButtonClick(object sender, RoutedEventArgs e) => SubmitForm();

        private void RejectionButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);
    }
}
