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

namespace VetClinic.Views
{
    public partial class Examinations : Window
    {
        private ListViewDataContext<Examination> ExaminationViewModel;
        private TranslationUtils Translation;
        private Veterinarian Veterinarian;

        private IExaminationDao ExaminationDao = DaoFactory.Instance(DaoType.MySql).Examinations;

        public Examinations(TranslationUtils translation, Veterinarian veterinarian)
        {
            InitializeComponent();
            Translation = translation;
            Veterinarian = veterinarian;

            SetDataGridHeaders();
            SetStatusComboBox();
            UpdateDataGridContext();
        }

        private void SetDataGridHeaders()
        {
            PetColumn.Header = Translation.Language.PetAppExamHeader;
            OwnerColumn.Header = Translation.Language.OwnerAppExamHeader;
            DateTimeColumn.Header = Translation.Language.DateTimeHeader;
            AddressColumn.Header = Translation.Language.AddressHeader;
        }

        private void SetStatusComboBox()
        {
            StatusComboBox.SelectionChanged += OnSelectionChanged;
            StatusComboBox.Items.Add(Translation.Language.NonCompletedString);
            StatusComboBox.Items.Add(Translation.Language.CompletedString);
            StatusComboBox.SelectedIndex = 0;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is null)
                StatusComboBox.SelectedIndex = 0;
            Search();
        }

        private void UpdateDataGridContext() => Search();

        private void Search()
        {
            string query = string.IsNullOrEmpty(PetSearchQueryTextBox.Text) ? "" : PetSearchQueryTextBox.Text;
            bool completed = (StatusComboBox.SelectedIndex != 0);

            ExaminationViewModel = new ListViewDataContext<Examination>()
            {
                Items = new ObservableCollection<Examination>(ExaminationDao.GetAllFromVetAndSearchPet(Veterinarian.User.Id, query, completed)),
                Language = Translation.Language
            };
            DataContext = ExaminationViewModel;
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
            StatusComboBox.SelectedIndex = 0;
            Search();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void ExaminationsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ExaminationUpdateClick(object sender, RoutedEventArgs e)
        {

        }

        private void AddExaminationClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
