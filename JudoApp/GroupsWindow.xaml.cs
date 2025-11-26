using System;
using System.Linq;
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
            LoadGroups();
        }

        private void LoadGroups()
        {
            try
            {
                var builder = new FightScheduleBuilder();
                var schedules = builder.BuildSchedules();

                if (schedules.Any())
                {
                    emptyStateTextBlock.Visibility = Visibility.Collapsed;
                    groupsItemsControl.ItemsSource = schedules;
                }
                else
                {
                    groupsItemsControl.ItemsSource = null;
                    emptyStateTextBlock.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка построения групп: {ex.Message}");
                groupsItemsControl.ItemsSource = null;
                emptyStateTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
