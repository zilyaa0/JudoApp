using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном данных о татами
    /// </summary>
    public partial class TatamiesWindow : Window
    {
        public TatamiesWindow()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
