using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия для AddOrUpdatePartisipantWindow.xaml
    /// </summary>
    public partial class AddOrUpdatePartisipantWindow : Window
    {
        Participant participant = new Participant();
        public AddOrUpdatePartisipantWindow(ParticipantDisplay participantDisplay)
        {
            InitializeComponent();

            using (var db = new JudoDBEntities())
            {
                var sportclubs = db.Sportsclubs.ToList();
                sportsclubComboBox.ItemsSource = sportclubs;
                var towns = db.Towns.ToList();
                townComboBox.ItemsSource = towns;
                birthDatePicker.DisplayDateStart = DateTime.Parse("23.12.2006");
                birthDatePicker.DisplayDateEnd = DateTime.Parse("23.12.2010");


                if (participantDisplay != null)
                {
                    participant = participantDisplay;
                    fioBox.Text = participantDisplay.FIO;
                    genderComboBox.SelectedIndex = participantDisplay.Gender == "m" ? 0 : 1;
                    birthDatePicker.SelectedDate = participantDisplay.BirthDate;
                    weightBox.Text = participantDisplay.Weight.ToString();
                    sportsclubComboBox.SelectedItem = sportclubs.FirstOrDefault(x => x.Id == participantDisplay.Sportsclubs.FirstOrDefault().Id);
                    townComboBox.SelectedItem = towns.FirstOrDefault(x => x.Index == participantDisplay.Towns.FirstOrDefault().Index);
                    streetBox.Text = participantDisplay.Street;
                }
            }

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new JudoDBEntities())
                {
                    var error = "";
                    decimal weight;
                    if (string.IsNullOrEmpty(fioBox.Text))
                        error += "ФИО должно быть заполнено\\n";
                    if (genderComboBox.SelectedIndex == -1)
                        error += "Укажите пол\\n";
                    if (!decimal.TryParse(weightBox.Text, out weight))
                    {
                        error += "Вес указан некорректно\\n";
                    }

                    if (!string.IsNullOrEmpty(error))
                    {
                        MessageBox.Show(error, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    else
                    {
                        participant.FIO = fioBox.Text;
                        participant.Gender = genderComboBox.SelectedIndex == 0 ? "m" : "f";
                        participant.BirthDate = birthDatePicker.SelectedDate;
                        participant.Weight = weight;
                        participant.Street = streetBox.Text;
                        if (sportsclubComboBox.SelectedItem is Sportsclub sportclub)
                        {
                            participant.Sportsclubs.Add(sportclub);
                        }
                        if (townComboBox.SelectedItem is Town town)
                        {
                            participant.Towns.Add(town);
                        }

                        db.Participants.AddOrUpdate(participant);
                        db.SaveChanges();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при работе с участником", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
    }
}
