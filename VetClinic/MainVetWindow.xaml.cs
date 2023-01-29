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
using VetClinic.Models.Entities;
using VetClinic.Utils;

namespace VetClinic
{
    public partial class MainVetWindow : Window
    {

        private Veterinarian Veterinarian;
        private TranslationUtils Translation;
        private IVeterinarianDao VeterinarianDao;

        public MainVetWindow(Veterinarian veterinarian, TranslationUtils translation)
        {
            InitializeComponent();
            this.Veterinarian = veterinarian;
            this.Translation = translation;
            this.VeterinarianDao = DaoFactory.Instance(DaoType.MySql).Veterinarians;

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
            Veterinarian.User.Language = lang;
            if (!VeterinarianDao.Update(this.Veterinarian))
            {
                // Show Error Message
            }
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
            Veterinarian.User.Theme = theme;
            if (!VeterinarianDao.Update(this.Veterinarian))
            {
                // Error Message
            }
        }
    }
}
