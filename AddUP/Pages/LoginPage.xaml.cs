using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AddUP.Pages
{
    public partial class LoginPage : Page
    {
        private readonly Entities db = new Entities();

        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль!", "Ошибка ввода",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = db.Users.FirstOrDefault(u => u.user_login == login && u.user_password == password);

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль.", "Ошибка авторизации",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            App.CurrentUser = user;

            MessageBox.Show($"Добро пожаловать, {user.user_login}!", "Успешный вход",
                MessageBoxButton.OK, MessageBoxImage.Information);

            NavigationService.Navigate(new UserAdsPage());
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}