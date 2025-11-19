using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdatePartisipantWindow.xaml
    /// </summary>
    public partial class AddOrUpdatePartisipantWindow : Window
    {
        public AddOrUpdatePartisipantWindow(ParticipantDisplay participantDisplay)
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
