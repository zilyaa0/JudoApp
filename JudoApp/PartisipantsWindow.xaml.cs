using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace JudoApp
{
    /// <summary>
    /// Логика взаимодействия с окном участников
    /// </summary>
    public partial class PartisipantsWindow : Window
    {
        private List<ParticipantDisplay> allParticipants;
        public PartisipantsWindow()
        {
            InitializeComponent();
            try
            {
                using (var db = new JudoDBEntities())
                {
                    allParticipants = db.Participants
                       .Include(p => p.Sportsclubs)
                       .Include(p => p.Towns)
                       .Include(p => p.Groups)
                       .ToList()
                       .Select(p => new ParticipantDisplay
                       {
                           Id = p.Id,
                           FIO = p.FIO,
                           Gender = p.Gender,
                           Weight = p.Weight,
                           BirthDate = p.BirthDate,
                           Street = p.Street,
                           Sportsclubs = p.Sportsclubs,
                           Towns = p.Towns,
                           Groups = p.Groups
                       })
                       .ToList();

                    var clubs = db.Sportsclubs.ToList();
                    clubs.Insert(0, new Sportsclub { Id = 0, Name = "Все клубы" });
                    clubComboBox.ItemsSource = clubs;
                    clubComboBox.SelectedIndex = 0;

                    var ageList = new List<string>
                    {
                        "Все категории"
                    };

                    var agesFromDb = db.Groups
                        .Select(g => g.BaseAge)         
                        .Distinct()
                        .OrderBy(a => a)                
                        .Select(a => a.ToString())       
                        .ToList();

                    ageList.AddRange(agesFromDb);
                    ageGroupComboBox.ItemsSource = ageList;
                    ageGroupComboBox.SelectedIndex = 0;

                    var weightList = new List<string>
                    {
                        "Все категории"
                    };
                    var weightsFromDb = db.Groups
                        .Select(g => g.BaseWeight)
                        .Distinct()
                        .OrderBy(w => w)
                        .Select(w => w.ToString())
                        .ToList();

                    weightList.AddRange(weightsFromDb);
                    weightGroupComboBox.ItemsSource = weightList;
                    weightGroupComboBox.SelectedIndex = 0;
                }
                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            var result = allParticipants.ToList();

            if (!string.IsNullOrWhiteSpace(searchTextBox.Text))
            {
                string searchText = searchTextBox.Text.ToLower();
                result = result.Where(p => p.FIO.ToLower().Contains(searchText)).ToList();
            }

            var selectedClub = clubComboBox.SelectedItem as Sportsclub;
            if (selectedClub != null && selectedClub.Name != "Все клубы")
            {
                result = result.Where(p => p.Sportsclubs.Any(club => club.Name == selectedClub.Name)).ToList();
            }

            string selectedAge = ageGroupComboBox.SelectedItem as string;
            if (selectedAge != null && selectedAge != "Все категории")
            {
                int ageValue = int.Parse(selectedAge); 
                result = result.Where(p => p.Groups.Any(g => g.BaseAge == ageValue)).ToList();
            }

            string selectedWeight = weightGroupComboBox.SelectedItem as string;
            if (selectedWeight != null && selectedWeight != "Все категории")
            {
                int weightValue = int.Parse(selectedWeight);
                result = result.Where(p => p.Groups.Any(g => g.BaseWeight == weightValue)).ToList();
            }

            if (genderComboBox.SelectedIndex == 1) 
            {
                result = result.Where(p => p.Gender == "m").ToList();
            }
            else if (genderComboBox.SelectedIndex == 2) 
            {
                result = result.Where(p => p.Gender == "f").ToList();
            }

            participantsDataGrid.ItemsSource = result;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var addPartsWin = new AddOrUpdatePartisipantWindow(null);
            addPartsWin.ShowDialog();
        }

        private void participantsDataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (participantsDataGrid.SelectedItem is ParticipantDisplay selectedParticipant)
            {
                var editWindow = new AddOrUpdatePartisipantWindow(selectedParticipant);
                if (editWindow.ShowDialog() == true)
                {
                    
                }
            }
        }
    }

    public class ParticipantDisplay : Participant
    {
        public string SportsclubName => Sportsclubs?.FirstOrDefault()?.Name ?? string.Empty;
        public string TownName => Towns?.FirstOrDefault()?.Name ?? string.Empty;
    }
}
