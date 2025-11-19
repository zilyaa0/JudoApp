using System.Linq;
using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном регистрации
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var authWin = new AuthWindow();
            authWin.Show();
            Close();
        }

        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(loginBox.Text) ||
                string.IsNullOrWhiteSpace(passwordBox.Password) ||
                string.IsNullOrWhiteSpace(fioBox.Text) )
                {
                    MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                using (var db = new JudoDBEntities())
                {
                    var user = db.Users.FirstOrDefault(x => x.Login == loginBox.Text);
                    if (user != null)
                    {
                        MessageBox.Show("Пользователь с таком логином уже существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    db.Users.Add(new User()
                    {
                        Login = loginBox.Text,
                        Password = passwordBox.Password,
                        FIO = fioBox.Text
                    });
                    db.SaveChanges();
                    MessageBox.Show("Регистрация прошла успешно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    var navWin = new NavigationWindow();
                    navWin.Show();
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка регистрации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
