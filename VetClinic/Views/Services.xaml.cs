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
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic.Views
{
    public partial class Services : Window
    {

        private ListViewDataContext<Service> ServiceViewModel;
        private TranslationUtils Translation;
        private IServiceDao ServiceDao;

        public Services(TranslationUtils translation)
        {
            InitializeComponent();
            Translation = translation;
            ServiceDao = DaoFactory.Instance(DaoType.MySql).Services;
            UpdateDataContext();
            SetDataGridHeaders();
        }

        private void UpdateDataContext() 
        {
            ServiceViewModel = new ListViewDataContext<Service>()
            {
                Items = new ObservableCollection<Service>(ServiceDao.GetAll()),
                Language = Translation.Language
            };
            DataContext = ServiceViewModel;
        }

        private void SetDataGridHeaders()
        {
            NameColumn.Header = Translation.Language.NonPersonNameHeader;
            CostColumn.Header = Translation.Language.CostHeader;
            DescriptionColumn.Header = Translation.Language.DescriptionHeader;
        }

        private void Search()
        {
            if(string.IsNullOrEmpty(ServiceSearchQueryTextBox.Text))
            {
                UpdateDataContext();
                return;
            }

            ServiceViewModel = new ListViewDataContext<Service>()
            {
                Items = new ObservableCollection<Service>(ServiceDao.GetBySearchQuery(ServiceSearchQueryTextBox.Text)),
                Language = Translation.Language
            };
            DataContext = ServiceViewModel;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Search();
        }

        private void SearchButtonClick(object sender, RoutedEventArgs e) => Search();

        private void ClearSearchQueryClick(object sender, RoutedEventArgs e)
        {
            ServiceSearchQueryTextBox.Text = null;
            UpdateDataContext();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e) => this.Close();

        private void ServicesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenServiceDetailsWindow(ServiceViewModel.SelectedItem);

        private void DeleteServiceClick(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItem is Service)
            {
                Service? SelectedItem = (Service)ServicesDataGrid.CurrentItem;
                if (SelectedItem != null)
                {
                    YesNo YesNoDialog = new YesNo(Translation.Language.DeleteConfirmationString, Translation.Language.YesNoDialogConfirmationString, Translation.Language.YesNoDialogRejectionString);
                    if (YesNoDialog.ShowDialog() == true)
                    {
                        if (ServiceDao.DeleteById(SelectedItem.Id))
                            Search();
                        else new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    }
                }
            }
        }

        private void AddServiceClick(object sender, RoutedEventArgs e) => OpenServiceDetailsWindow(null);

        private void OpenServiceDetailsWindow(Service? service)
        {
            ServiceDetails Details = new ServiceDetails(this.Translation, this.ServiceDao, service);
            if (Details.ShowDialog() == true)
                Search();
        }

        private void ServiceUpdateClick(object sender, RoutedEventArgs e) => OpenServiceDetailsWindow(ServiceViewModel.SelectedItem);
    }
}
