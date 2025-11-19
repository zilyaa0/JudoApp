using System.Linq;
using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном авторизации
    /// </summary>
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }
        
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginBox.Text) ||
                string.IsNullOrWhiteSpace(passwordBox.Password))
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var db = new JudoDBEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Password == passwordBox.Password && x.Login == loginBox.Text);
                    if (user == null)
                    {
                        MessageBox.Show("Неверный логин или пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    var navWin = new NavigationWindow();
                    navWin.Show();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка авторизации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            var regWin = new RegWindow();
            regWin.Show();
            Close();
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            var newPasWin = new NewPasswordWindow();
            newPasWin.ShowDialog();
        }
    }
}
