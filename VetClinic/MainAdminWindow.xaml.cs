using System;
using System.Windows;
using VetClinic.Utils;
using VetClinic.Dao;
using VetClinic.Views;
using VetClinic.Dialogs;

namespace VetClinic
{
    public partial class MainAdminWindow : Window
    {

        private Models.Entities.Administrator Administrator;
        private TranslationUtils Translation;
        private IAdministratorDao AdministratorDao;

        public MainAdminWindow(Models.Entities.Administrator administrator, TranslationUtils translation)
        {
            InitializeComponent();
            this.Administrator = administrator;
            this.Translation = translation;
            this.AdministratorDao = DaoFactory.Instance(DaoType.MySql).Administrators;

            DataContext = this.Translation.Language;
        }

        // Language And Theme Settings
        private void ChangeLanguageToEnglish(object sender, RoutedEventArgs e) => ChangeLanguage("en");
        private void ChangeLanguageToSerbian(object sender, RoutedEventArgs e) => ChangeLanguage("sr");

        private void ChangeLanguage(string lang)
        {
            // Change Language In Application
            Lang l = Lang.SerbianLang;
            if (lang.ToLower().Equals("en"))
                l = Lang.EnglishLang;
            this.Translation = new TranslationUtils() { Language = l };
            DataContext = this.Translation.Language;

            // Change Language In Database
            Administrator.User.Language = lang;
            if (!AdministratorDao.Update(this.Administrator))
                new CustomMessageBox(Translation.Language.InternalServerError).Show(); 
        }

        private void ChangeThemeToDark(object sender, RoutedEventArgs e) => ChangeTheme("dark");
        private void ChangeThemeToLight(object sender, RoutedEventArgs e) => ChangeTheme("light");
        private void ChangeThemeToBlue(object sender, RoutedEventArgs e) => ChangeTheme("blue");

        private void ChangeTheme(string theme)
        {
            // Change Theme In Application
            ResourceDictionary dictionary = new ResourceDictionary();
            if (theme.ToLower().Equals("dark"))
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Dark.xaml");
            else if (theme.ToLower().Equals("light"))
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Light.xaml");
            else
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Blue.xaml");

            Application.Current.Resources.MergedDictionaries.Add(dictionary);

            // Change Theme In Database
            Administrator.User.Theme = theme;
            if (!AdministratorDao.Update(this.Administrator))
                new CustomMessageBox(Translation.Language.InternalServerError).Show();
        }


        private void OpenAppointmentsView(object sender, RoutedEventArgs e) => new Appointments(Translation, null).Show();

        private void OpenMedicineView(object sender, RoutedEventArgs e) => new Medicine(Translation).Show();

        private void OpenServicesView(object sender, RoutedEventArgs e) => new Services(Translation).Show();

        private void OpenPetsAndOwnersView(object sender, RoutedEventArgs e) => new PetsAndOwners(Translation).Show();

        private void OpenVeterinariansView(object sender, RoutedEventArgs e) => new Veterinarians(Translation).Show();

        private void Logout(object sender, RoutedEventArgs e)
        {
            new Login().Show();
            Close();
        }
    }
}
