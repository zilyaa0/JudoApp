using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном смены пароля
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        public NewPasswordWindow()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
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
                    var user = db.Users.FirstOrDefault(x => x.Login == loginBox.Text);
                    if (user == null)
                    {
                        MessageBox.Show("Пользователя с таким логином не существует!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    user.Password = passwordBox.Password;
                    db.Users.AddOrUpdate(user);
                    db.SaveChanges();
                    MessageBox.Show("Смена пароля прошла успешно", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
            catch
            {
                MessageBox.Show("Ошибка смены пароля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
