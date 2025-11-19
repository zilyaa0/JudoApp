using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном соревнований
    /// </summary>
    public partial class CompetitionWindow : Window
    {
        public CompetitionWindow()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var navWin = new NavigationWindow();
            navWin.Show();
            Close();
        }
    }
}
