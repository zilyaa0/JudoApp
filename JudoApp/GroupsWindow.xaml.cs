using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном групп участников
    /// </summary>
    public partial class GroupsWindow : Window
    {
        public GroupsWindow()
        {
            InitializeComponent();
        }
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
