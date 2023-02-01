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
    public partial class CompletedExaminationView : Window
    {
        private TranslationUtils Translation;
        private Examination Exam;
        private IExaminationDao ExamDao = DaoFactory.Instance(DaoType.MySql).Examinations;
        private CompletedExaminationViewModelDataContext ViewModel;

        public CompletedExaminationView(TranslationUtils translation, Examination examination)
        {
            InitializeComponent();
            Translation = translation;
            Exam = examination;

            SetLabelValues();
            SetHeaders();
            SetDataContext();
        }

        private void SetLabelValues()
        {
            DateTimeLabel.Content = Exam.DateTime.ToString();
            AddressLabel.Content = Exam.Address;
            PetDetailsButton.Content = Exam.Pet.Name;
            ServicesLabel.Content = Translation.Language.ServicesButton;
            PrescriptionsLabel.Content = Translation.Language.PrescriptionsButton;
        }

        private void SetHeaders()
        {
            ExaminationServiceNameColumn.Header = Translation.Language.NonPersonNameHeader;
            ExaminationServiceQuantityColumn.Header = Translation.Language.QuantityHeader;
            ExaminationServiceCostColumn.Header = Translation.Language.CostHeader;
            PrescriptionNameColumn.Header = Translation.Language.NonPersonNameHeader;
        }

        private void SetDataContext()
        {
            ViewModel = new CompletedExaminationViewModelDataContext()
            {
                ExaminationServiceItems = new ObservableCollection<ExaminationService>(ExamDao.GetAllSevicesFromExamination(Exam.Id)),
                PrescriptionItems = new ObservableCollection<Prescription>(ExamDao.GetAllPrescriptionsFromExamination(Exam.Id)),
                Language = Translation.Language
            };

            DataContext = ViewModel;
        }

        private void ShowPetDetails(object sender, RoutedEventArgs e) => new PetDetails(Translation, Exam.Pet, Exam.Pet.Owner, false).ShowDialog();

        private void PrescriptionDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) => OpenPrescriptionDetails();

        private void DetailedSelectedPrescription(object sender, RoutedEventArgs e) => OpenPrescriptionDetails();

        private void FinishButtonClick(object sender, RoutedEventArgs e) => Close();

        private void OpenPrescriptionDetails()
        {
            if (ViewModel.PrescriptionSelectedItem is not null)
                new PrescriptionDetails(Translation, ViewModel.PrescriptionSelectedItem, false, true).ShowDialog();
        }

        private void ShowBillDetails(object sender, RoutedEventArgs e)
        {
            Bill? bill = ExamDao.GetBillForExamination(Exam);
            if (bill is not null)
                new BillWindow(Translation, bill, true).Show();
            else
                new CustomMessageBox(Translation.Language.NotFoundExceptionMessage).Show();
        }
    }

    public class CompletedExaminationViewModelDataContext
    {
        public IList<ExaminationService> ExaminationServiceItems { get; set; }
        public ExaminationService ExaminationServiceSelectedItem { get; set; }
        public IList<Prescription> PrescriptionItems { get; set; }
        public Prescription PrescriptionSelectedItem { get; set; }
        public Lang Language { get; set; }
    }
}
