using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия для NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var authWin = new AuthWindow();
            authWin.Show();
            Close();
        }

        private void ControlButton_Click(object sender, RoutedEventArgs e)
        {
            var controller = new TatamiMatchController();
            var competitionWin = new CompetitionWindow(controller);
            var controlWin = new ControlWindow(controller);

            controlWin.Show();
            competitionWin.Show();

            controlWin.Closed += (_, __) =>
            {
                if (competitionWin.IsVisible)
                {
                    competitionWin.Close();
                }
            };

            Close();
        }

        private void PartisipantsButton_Click(object sender, RoutedEventArgs e)
        {
            var partsWin = new PartisipantsWindow();
            partsWin.ShowDialog();
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            var groupsWin = new GroupsWindow();
            groupsWin.ShowDialog();
        }

        private void TatamiButton_Click(object sender, RoutedEventArgs e)
        {
            var tatamiWin = new TatamiesWindow();
            tatamiWin.ShowDialog();
        }
    }
}
