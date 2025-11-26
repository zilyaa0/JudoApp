using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном соревнований
    /// </summary>
    public partial class CompetitionWindow : Window
    {
        private readonly TatamiMatchController _controller;

        public CompetitionWindow()
            : this(new TatamiMatchController())
        {
        }

        public CompetitionWindow(TatamiMatchController controller)
        {
            InitializeComponent();
            _controller = controller ?? new TatamiMatchController();
            DataContext = _controller.State;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
