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
using VetClinic.Dialogs;
using VetClinic.Utils;
using MedicineEntity = VetClinic.Models.Entities.Medicine;


namespace VetClinic.Views
{
    public partial class Medicine : Window
    {

        private ListViewDataContext<MedicineEntity> MedicineViewModel;
        private TranslationUtils Translation;
        private IMedicineDao MedicineDao;
        private string SelectedType = "";

        public Medicine(TranslationUtils translation)
        {
            InitializeComponent();
            Translation = translation;
            MedicineDao = DaoFactory.Instance(DaoType.MySql).Medicine;
            UpdateDataContext();
            MedicineTypeComboBox.SelectionChanged += OnSelectionChanged;
            SetComboBox();
            SetDataGridHeaders();
        }

        private void UpdateDataContext()
        {
            MedicineViewModel = new ListViewDataContext<MedicineEntity>()
            {
                Items = new ObservableCollection<MedicineEntity>(MedicineDao.GetAll()),
                Language = Translation.Language
            };
            DataContext = MedicineViewModel;
        }

        private void SetDataGridHeaders()
        {
            IdColumn.Header = Translation.Language.IdHeader;
            NameColumn.Header = Translation.Language.NonPersonNameHeader;
            TypeColumn.Header = Translation.Language.TypeHeader;
            DescriptionColumn.Header = Translation.Language.DescriptionHeader;
        }

        private void SetComboBox()
        {
            MedicineTypeComboBox.Items.Clear();
            MedicineTypeComboBox.Items.Add("*");
            List<string> types = MedicineDao.GetTypes();
            foreach (string type in types)
                MedicineTypeComboBox.Items.Add(type);

            if(!string.IsNullOrEmpty(SelectedType) && MedicineTypeComboBox.Items.Contains(SelectedType))
                MedicineTypeComboBox.SelectedItem = SelectedType;

        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MedicineTypeComboBox.SelectedItem != null)
            {
                if (!"*".Equals(MedicineTypeComboBox.SelectedItem.ToString()))
                    SelectedType = MedicineTypeComboBox.SelectedItem.ToString();
                else
                    SelectedType = "";
                Search();
            }
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(MedicineSearchQueryTextBox.Text) && string.IsNullOrEmpty(SelectedType))
            {
                UpdateDataContext();
                return;
            }

            string query = string.IsNullOrEmpty(MedicineSearchQueryTextBox.Text) ? "" : MedicineSearchQueryTextBox.Text;
            MedicineViewModel = new ListViewDataContext<MedicineEntity>()
            {
                Items = new ObservableCollection<MedicineEntity>(MedicineDao.GetByNameAndType(query, SelectedType)),
                Language = Translation.Language
            };
            DataContext = MedicineViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            MedicineSearchQueryTextBox.Text = null;
            MedicineTypeComboBox.SelectedIndex = 0;
            UpdateDataContext();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void DeleteMedicineClick(object sender, RoutedEventArgs e)
        {
            if(MedicineDataGrid.SelectedItem is MedicineEntity)
            {
                MedicineEntity? SelectedItem = (MedicineEntity)MedicineDataGrid.CurrentItem;
                if(SelectedItem != null)
                {
                    YesNo YesNoDialog = new YesNo(Translation.Language.DeleteConfirmationString, Translation.Language.YesNoDialogConfirmationString, Translation.Language.YesNoDialogRejectionString);
                    if(YesNoDialog.ShowDialog() == true)
                    {
                        if (MedicineDao.DeleteById(SelectedItem.Id))
                        {
                            SetComboBox();
                            Search();
                        } else new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    }
                }
            }
        }

        private void MedicineDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenMedicineDetailsWindow(MedicineViewModel.SelectedItem);

        private void AddNewMedicineClick(object sender, RoutedEventArgs e) => OpenMedicineDetailsWindow(null);

        private void OpenMedicineDetailsWindow(MedicineEntity? medicine)
        {
            MedicineDetails Details = new MedicineDetails(this.Translation, this.MedicineDao, medicine);
            if(Details.ShowDialog() == true)
            {
                SetComboBox();
                Search();
            }
        }

        private void MedicineUpdateClick(object sender, RoutedEventArgs e) => OpenMedicineDetailsWindow(MedicineViewModel.SelectedItem);
    }
}
