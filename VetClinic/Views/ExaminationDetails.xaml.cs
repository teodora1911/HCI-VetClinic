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
using VetClinic.Utils;
using VetClinic.Models.Entities;
using MedicineEntity = VetClinic.Models.Entities.Medicine;
using System.Collections.ObjectModel;
using VetClinic.Dialogs;
using System.Reflection;

namespace VetClinic.Views
{
    public partial class ExaminationDetails : Window
    {
        private TranslationUtils Translation;
        private IExaminationDao ExamDao = DaoFactory.Instance(DaoType.MySql).Examinations;
        private IServiceDao ServiceDao = DaoFactory.Instance(DaoType.MySql).Services;
        private IMedicineDao MedicineDao = DaoFactory.Instance(DaoType.MySql).Medicine;
        private Examination Exam;

        private List<Service> AllServices;
        private List<MedicineEntity> AllMedicine;
        private List<ExaminationService> SelectedServices = new();
        private List<Prescription> Prescriptions = new();

        private ExaminationViewModelDataContext ExamViewModel;

        public ExaminationDetails(TranslationUtils translation, Examination exam)
        {
            InitializeComponent();
            Translation = translation;
            Exam = exam;

            AllServices = ServiceDao.GetAll();
            AllMedicine = MedicineDao.GetAll();

            SetLabelValues();
            SetHeaders();
            SetDataContext();
        }

        private void SetLabelValues()
        {
            AddressLabel.Content = Exam.Address;
            DateTimeLabel.Content = Exam.DateTime.ToString();
            PetDetailsButton.Content = Exam.Pet.Name;
        }

        private void SetHeaders()
        {
            PrescriptionNameColumn.Header = Translation.Language.NonPersonNameHeader;
            ExaminationServiceNameColumn.Header = Translation.Language.NonPersonNameHeader;
            MedicineNameColumn.Header = Translation.Language.NonPersonNameHeader;
            ServiceNameColumn.Header = Translation.Language.NonPersonNameHeader;
            ExaminationServiceQuantityColumn.Header = Translation.Language.QuantityHeader;
        }

        private void SetDataContext()
        {
            SelectedServices = ExamDao.GetAllSevicesFromExamination(Exam.Id);
            Prescriptions = ExamDao.GetAllPrescriptionsFromExamination(Exam.Id);

            ExamViewModel = new ExaminationViewModelDataContext()
            {
                ServiceItems = new ObservableCollection<Service>(AllServices),
                MedicineItems = new ObservableCollection<MedicineEntity>(AllMedicine),
                ExaminationServiceItems = new ObservableCollection<ExaminationService>(SelectedServices),
                PrescriptionItems = new ObservableCollection<Prescription>(Prescriptions),
                Language = Translation.Language
            };

            DataContext = ExamViewModel;
        }

        private void ShowPetDetails(object sender, RoutedEventArgs e) => new PetDetails(Translation, Exam.Pet, Exam.Pet.Owner, false).ShowDialog();

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            YesNo YesNoDialog = new YesNo(Translation.Language.CloseEditingConfirmationString, Translation.Language.YesNoDialogConfirmationString, Translation.Language.YesNoDialogRejectionString);
            if (YesNoDialog.ShowDialog() == true)
                CompleteExamination(true);
            else FinishEditing(false);
        }

        private void AddSelectedService(object sender, RoutedEventArgs e)
        {
            if (ExamViewModel.ServiceSelectedItem is not null)
            {
                var selected = ExamViewModel.ServiceSelectedItem;
                int index = SelectedServices.FindIndex(s => s.Service.Id == selected.Id);
                if (index >= 0)
                {
                    SelectedServices[index].Quantity = SelectedServices[index].Quantity + 1;
                    ExamDao.UpdateService(SelectedServices[index]);
                }
                else
                {
                    var obj = new ExaminationService()
                    {
                        Examination = Exam.Id,
                        Service = selected,
                        Quantity = 1,
                        Cost = 0
                    };
                    ExamDao.AddService(obj);
                }

                SetDataContext();
            }
        }

        private void AddSelectedMedicine(object sender, RoutedEventArgs e)
        {
            if(ExamViewModel.MedicineSelectedItem is not null)
            {
                var selected = ExamViewModel.MedicineSelectedItem;
                int index = Prescriptions.FindIndex(p => p.Medicine.Id == selected.Id);
                if (index >= 0 && (new PrescriptionDetails(Translation, Prescriptions[index], true, false).ShowDialog() == true))
                {
                    SetDataContext();
                }
                else
                {
                    Prescription prescription = new Prescription()
                    {
                        Examination = Exam.Id,
                        Medicine = selected,
                        Name = null,
                        Dose = 0,
                        Frequency = null,
                        Start = null,
                        Duration = null,
                        Instructions = null
                    };

                    if (new PrescriptionDetails(Translation, prescription, false, false).ShowDialog() == true)
                    {
                        SetDataContext();
                    }
                }
            }
        }

        private void RemoveSelectedExaminationService(object sender, RoutedEventArgs e)
        {
            if(ExamViewModel.ExaminationServiceSelectedItem is not null)
            {
                var selected = ExamViewModel.ExaminationServiceSelectedItem;
                int index = SelectedServices.IndexOf(selected);
                if(index >= 0)
                {
                    SelectedServices[index].Quantity = SelectedServices[index].Quantity - 1;
                    if (SelectedServices[index].Quantity <= 0)
                        ExamDao.DeleteExaminationService(SelectedServices[index]);
                    else
                        ExamDao.UpdateService(SelectedServices[index]);
                    SetDataContext();
                }
            }
        }

        private void UpdateSelectedPrescription(object sender, RoutedEventArgs e)
        {
            if(ExamViewModel.PrescriptionSelectedItem is not null)
            {
                if (new PrescriptionDetails(Translation, ExamViewModel.PrescriptionSelectedItem, true, false).ShowDialog() == true)
                    SetDataContext();
            } 
        }

        private void RemoveSelectedPrescription(object sender, RoutedEventArgs e)
        {
            if(ExamViewModel.PrescriptionSelectedItem is not null)
            {
                YesNo YesNoDialog = new YesNo(Translation.Language.DeleteConfirmationString, Translation.Language.YesNoDialogConfirmationString, Translation.Language.YesNoDialogRejectionString);
                if (YesNoDialog.ShowDialog() == true)
                {
                    if (ExamDao.DeletePrescription(ExamViewModel.PrescriptionSelectedItem))
                        SetDataContext();
                    else new CustomMessageBox(Translation.Language.InternalServerError).Show();
                }
            }
        }

        private void CompleteExamination(object sender, RoutedEventArgs e) => CompleteExamination(true);

        private void CompleteExamination(bool result)
        {
            if (result)
            {
                if (ExamDao.Update(Exam))
                {
                    decimal price = 0;
                    foreach (ExaminationService item in SelectedServices)
                        price += item.Cost;
                    Bill bill = new Bill()
                    {
                        Price = price,
                        Payment = "",
                        Timestamp = DateTime.Now,
                        Examination = Exam,
                        Owner = Exam.Pet.Owner
                    };
                    new BillWindow(Translation, bill, false).Show();

                    FinishEditing(true);
                }
                else
                {
                    new CustomMessageBox(Translation.Language.InternalServerError).Show();
                    FinishEditing(false);
                }
            }
            else FinishEditing(false);
        }

        private void FinishEditing(bool result)
        {
            this.DialogResult = result;
            Close();
        }

        private void PrescriptionDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ExamViewModel.PrescriptionSelectedItem is not null)
            {
                if (new PrescriptionDetails(Translation, ExamViewModel.PrescriptionSelectedItem, true, false).ShowDialog() == true)
                    SetDataContext();
            }
        }

        private void MedicineDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ExamViewModel.MedicineSelectedItem is not null)
            {
                var selected = ExamViewModel.MedicineSelectedItem;
                int index = Prescriptions.FindIndex(p => p.Medicine.Id == selected.Id);
                if (index >= 0 && (new PrescriptionDetails(Translation, Prescriptions[index], true, false).ShowDialog() == true))
                {
                    SetDataContext();
                }
                else
                {
                    Prescription prescription = new Prescription()
                    {
                        Examination = Exam.Id,
                        Medicine = selected,
                        Name = null,
                        Dose = 0,
                        Frequency = null,
                        Start = null,
                        Duration = null,
                        Instructions = null
                    };

                    if (new PrescriptionDetails(Translation, prescription, false, false).ShowDialog() == true)
                    {
                        SetDataContext();
                    }
                }
            }
        }
    }

    public class ExaminationViewModelDataContext
    {
        public IList<Service> ServiceItems { get; set; }
        public Service ServiceSelectedItem { get; set; }
        public IList<ExaminationService> ExaminationServiceItems { get; set; }
        public ExaminationService ExaminationServiceSelectedItem { get; set; }
        public IList<MedicineEntity> MedicineItems { get; set; }
        public MedicineEntity MedicineSelectedItem { get; set; }
        public IList<Prescription> PrescriptionItems { get; set; }
        public Prescription PrescriptionSelectedItem { get; set; }
        public Lang Language { get; set; }
    }
}
