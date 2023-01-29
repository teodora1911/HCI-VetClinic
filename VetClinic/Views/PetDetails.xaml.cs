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
    public partial class PetDetails : Window
    {
        private TranslationUtils Translation;
        private Pet Pet;
        private IPetDao PetDao = DaoFactory.Instance(DaoType.MySql).Pets;
        //        private IPetOwnerDao OwnerDao = DaoFactory.Instance(DaoType.MySql).PetOwners;

        private string SelectedGender;

        /*        private Species SelectedSpecies = null;
                private Breed SelectedBreed = null;*/

        public PetDetails(TranslationUtils translation, Pet pet, PetOwner owner, bool read)
        {
            InitializeComponent();
            this.Translation = translation;
            this.Pet = pet;
            this.Pet.Owner = owner;
            DataContext = translation.Language;
            SetComboBoxes();
            SetTextAreas();

            if (read)
                DisableControls();
            else
                OkButton.Visibility = Visibility.Hidden;
        }

        private void DisableControls()
        {
            NameTextBox.IsReadOnly = true;
            AgeTextBox.IsReadOnly = true;
            WeightTextBox.IsReadOnly = true;
            GenderTextBox.IsReadOnly = true;

            FemaleButton.Visibility = Visibility.Hidden;
            MaleButton.Visibility = Visibility.Hidden;

            HealthConditionTextBox.IsReadOnly = true;
            DiagnosisTextBox.IsReadOnly = true;
            SpeciesTextBox.IsReadOnly = true;
            BreedTextBox.IsReadOnly = true;

            SpeciesComboBox.Visibility = Visibility.Hidden;
            BreedComboBox.Visibility = Visibility.Hidden;
            ConfirmButton.Visibility = Visibility.Hidden;
            RejectButton.Visibility = Visibility.Hidden;
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

        private void SetTextAreas()
        {
            NameTextBox.Text = Pet.Name;
            //BirthdateDatePicker.SelectedDate = Pet.Birthdate;
            /*                if(Pet.EstimatedAge > 0)
                                AgeTextBox.Text = Pet.EstimatedAge.ToString();*/
            HealthConditionTextBox.Text = string.IsNullOrEmpty(Pet.HealthCondition) ? null : Pet.HealthCondition;
            DiagnosisTextBox.Text = string.IsNullOrEmpty(Pet.Diagnosis) ? null : Pet.Diagnosis;
            SpeciesTextBox.Text = Pet.Species.Name;
            if (Pet.Breeds.Count > 0)
                BreedTextBox.Text = Pet.Breeds.First<Breed>().Name;

            SpeciesComboBox.SelectedItem = Pet.Species;
            BreedComboBox.SelectedItem = Pet.Breeds.First<Breed>();

            WeightTextBox.Text = Pet.Weight > 0 ? Pet.Weight.ToString() : null;
            if (Pet.Gender.StartsWith("M"))
                GenderTextBox.Text = Translation.Language.GenderMale;
            else
                GenderTextBox.Text = Translation.Language.GenderFemale;
        }

        private void SubmitForm()
        {
            int response = ValidateForm();
            if (response == 0)
            {
                Pet.Name = NameTextBox.Text;
//                Pet.Birthdate = BirthdateDatePicker.SelectedDate;
                Pet.Age = int.Parse(AgeTextBox.Text);
                Pet.Weight = decimal.Parse(WeightTextBox.Text);
                Pet.Gender = SelectedGender;
                Pet.HealthCondition = (HealthConditionTextBox.Text != null) ? HealthConditionTextBox.Text : "";
                Pet.Diagnosis = (DiagnosisTextBox.Text != null) ? DiagnosisTextBox.Text : "";
                Pet.Species = (Species)SpeciesComboBox.SelectedItem;
                Pet.Breeds.Clear();
                Pet.Breeds.Add((Breed)BreedComboBox.SelectedItem);

                if (PetDao.Update(Pet) == true)
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
                BannerLabel.Content = Translation.Language.DecimalStringFormatException;
                BannerLabel.Visibility = Visibility.Visible;
            }
        }

        // Returns 0 if everything is okay
        // 1 if name field is empty
        // 2 if parsing is unsuccessful
        private int ValidateForm()
        {
            if (string.IsNullOrEmpty(NameTextBox.Text))
            {
                NameTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                return 1;
            }
            try
            {
/*                if (!BirthdateDatePicker.SelectedDate.HasValue)
                {
                    BirthdateDatePicker.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 1;
                }*/
                if(!string.IsNullOrEmpty(AgeTextBox.Text) && int.Parse(AgeTextBox.Text) <= 0)
                {
                    AgeTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 2;
                }
                if(!string.IsNullOrEmpty(AgeTextBox.Text) && decimal.Parse(WeightTextBox.Text) <= 0)
                {
                    WeightTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 2;
                }
                if(string.IsNullOrEmpty(GenderTextBox.Text))
                {
                    GenderTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 1;
                }
                if (string.IsNullOrEmpty(SpeciesTextBox.Text))
                {
                    SpeciesTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 1;
                }
                if (string.IsNullOrEmpty(BreedTextBox.Text))
                {
                    BreedTextBox.BorderBrush = Application.Current.Resources["ErrorColor"] as SolidColorBrush;
                    return 1;
                }

                return 0;
            } 
            catch (Exception) { return 2; }
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

        private void OkButtonClick(object sender, RoutedEventArgs e) => FinishEditing(false);
    }
}
