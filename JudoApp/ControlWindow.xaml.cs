using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействияс окном управления
    /// </summary>
    public partial class ControlWindow : Window
    {
        public ControlWindow()
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
