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
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PerformLogin();
        }

        private void LoginUser(object sender, RoutedEventArgs e)
        {
            BannerLoginLabel.Visibility = Visibility.Hidden;
            PerformLogin();
        }

        private void PerformLogin()
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrEmpty(username))
            {
                BannerLoginLabel.Content = "Please enter some value in username field.";
                UsernameTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                BannerLoginLabel.Visibility = Visibility.Visible;
            } else if(string.IsNullOrEmpty(password))
            {
                BannerLoginLabel.Content = "Please enter some value in password field.";
                PasswordTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                BannerLoginLabel.Visibility = Visibility.Visible;
            } else
            {
                Administrator admin = DaoFactory.Instance(DaoType.MySql).Administrators.GetByUsernameAndPassword(username, password);
                if (admin == null)
                {
                    // Check if Veterinarian is logged in
                    Veterinarian vet = DaoFactory.Instance(DaoType.MySql).Veterinarians.GetByUsernameAndPassword(username, password);
                    if(vet == null)
                    {
                        // Error while loging
                        PasswordTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        UsernameTextBox.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                        BannerLoginLabel.Content = "Invalid username or password! Please try again.";
                        BannerLoginLabel.Visibility = Visibility.Visible;

                    } else
                    {
                        TranslationUtils translation = SetThemeAndLanguage(vet.User.Theme, vet.User.Language);
                        new MainVetWindow(vet, translation).Show();
                        Close();
                        return;
                    }
                }
                else
                {
                    TranslationUtils translation = SetThemeAndLanguage(admin.User.Theme, admin.User.Language);
                    new MainAdminWindow(admin, translation).Show();
                    Close();
                    return;
                }
            }
        }

        private TranslationUtils SetThemeAndLanguage(string theme, string language)
        {
            ResourceDictionary dictionary = new ResourceDictionary();
            if(theme.ToLower().Equals("dark"))
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Dark.xaml");
            else if(theme.ToLower().Equals("light"))
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Light.xaml");
            else
                dictionary.Source = new Uri("pack://application:,,,/Resources/Themes/Blue.xaml");

            Application.Current.Resources.MergedDictionaries.Add(dictionary);

            TranslationUtils translation = new TranslationUtils() { Language = Lang.EnglishLang };
            if (language.ToLower().Equals("sr"))
                translation = new TranslationUtils() { Language = Lang.SerbianLang };

            return translation;
        }
    }
}
