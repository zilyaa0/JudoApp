using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
