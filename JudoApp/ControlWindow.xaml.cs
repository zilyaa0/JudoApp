using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействияс окном управления
    /// </summary>
    public partial class ControlWindow : Window
    {
        private readonly TatamiMatchController _controller;
        private bool _disposed;
        private int tatamiid;

        public ControlWindow()
            : this(new TatamiMatchController())
        {
        }

        public ControlWindow(TatamiMatchController controller)
        {
            InitializeComponent();
            _controller = controller ?? new TatamiMatchController();
            DataContext = _controller.State;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            var navWin = new NavigationWindow();
            navWin.Show();
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            if (!_disposed)
            {
                _controller.Dispose();
                _disposed = true;
            }
        }

        private void LoadTatami_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tatamiNumberTextBox.Text, out var tatamiNumber))
            {
                MessageBox.Show("Введите корректный номер татами.");
                return;
            }

            if (!_controller.TryLoadTatami(tatamiNumber))
            {
                MessageBox.Show("Не удалось найти схватки для выбранного татами.");
            }
            tatamiid = tatamiNumber;
        }

        private void MainTimerStart_Click(object sender, RoutedEventArgs e) => _controller.StartMainTimer();

        private void MainTimerStop_Click(object sender, RoutedEventArgs e) => _controller.StopMainTimer();

        private void HoldTimerStart_Click(object sender, RoutedEventArgs e) => _controller.StartHoldTimer();

        private void HoldTimerStop_Click(object sender, RoutedEventArgs e) => _controller.StopHoldTimer();

        private void WhiteIppon_Click(object sender, RoutedEventArgs e) => _controller.AwardIppon(BeltColor.White);

        private void RedIppon_Click(object sender, RoutedEventArgs e) => _controller.AwardIppon(BeltColor.Red);

        private void WhiteWazaari_Click(object sender, RoutedEventArgs e) => _controller.AwardWazaAri(BeltColor.White);

        private void RedWazaari_Click(object sender, RoutedEventArgs e) => _controller.AwardWazaAri(BeltColor.Red);

        private void WhitePenalty_Click(object sender, RoutedEventArgs e) => _controller.GivePenalty(BeltColor.White);

        private void RedPenalty_Click(object sender, RoutedEventArgs e) => _controller.GivePenalty(BeltColor.Red);

        private void FinishFight_Click(object sender, RoutedEventArgs e) => _controller.FinishFightManual();

        private void ResetFight_Click(object sender, RoutedEventArgs e) => _controller.ResetCurrentFight();

        private void SkipFight_Click(object sender, RoutedEventArgs e) => _controller.SkipCurrentFight();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_controller._state.StatusMessage == "Бои закончились. Можно перейти к генерации дипломов.")
            {
                var w = new DiplomsWindow(tatamiid);
                w.ShowDialog();
            }
            else
                MessageBox.Show("Для генерации дипломов нужно чтобы прошли все бои на данном татами.");
        }
    }
}
