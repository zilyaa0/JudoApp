using System;
using System.Collections.Generic;
using System.Linq;
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
            LoadTatamiData();
        }

        private void LoadTatamiData()
        {
            try
            {
                var builder = new FightScheduleBuilder();
                var schedules = builder.BuildSchedules();

                var tatamiData = schedules
                    .Where(s => s.AllFights.Any())
                    .GroupBy(s => s.TatamiId ?? -1)
                    .Select(group => new TatamiData
                    {
                        TatamiNumber = group.Key,
                        TatamiName = ResolveTatamiName(group.Key, group),
                        Fights = group.SelectMany(s => s.AllFights)
                                      .OrderBy(f => f.StageLabel)
                                      .ThenBy(f => f.RedLastName)
                                      .ThenBy(f => f.WhiteLastName)
                                      .ToList()
                    })
                    .OrderBy(t => t.TatamiNumber)
                    .ToList();

                if (tatamiData.Any())
                {
                    tatamiItemsControl.ItemsSource = tatamiData;
                }
                else
                {
                    ShowSampleData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                ShowSampleData();
            }
        }

        private string ResolveTatamiName(int tatamiNumber, IEnumerable<GroupSchedule> schedules)
        {
            var explicitName = schedules
                .Select(s => s.TatamiName)
                .FirstOrDefault(name => !string.IsNullOrWhiteSpace(name));

            if (!string.IsNullOrWhiteSpace(explicitName))
            {
                return explicitName;
            }

            return tatamiNumber <= 0 ? "Без назначения" : $"Татами {tatamiNumber}";
        }

        private void ShowSampleData()
        {
            var sampleFights = new List<FightDisplayModel>
            {
                new FightDisplayModel
                {
                    RedParticipantId = -1,
                    WhiteParticipantId = -2,
                    StageLabel = "Группа",
                    AgeCategory = "12 лет",
                    Gender = "М",
                    WeightCategory = "36 кг",
                    RedFirstName = "Иван",
                    RedLastName = "Петров",
                    RedClub = "Спартак",
                    WhiteFirstName = "Алексей",
                    WhiteLastName = "Сидоров",
                    WhiteClub = "Динамо"
                },
                new FightDisplayModel
                {
                    RedParticipantId = -3,
                    WhiteParticipantId = -4,
                    StageLabel = "Группа",
                    AgeCategory = "12 лет",
                    Gender = "М",
                    WeightCategory = "36 кг",
                    RedFirstName = "Дмитрий",
                    RedLastName = "Козлов",
                    RedClub = "ЦСКА",
                    WhiteFirstName = "Сергей",
                    WhiteLastName = "Иванов",
                    WhiteClub = "Локомотив"
                },
                new FightDisplayModel
                {
                    RedParticipantId = -5,
                    WhiteParticipantId = -6,
                    StageLabel = "Группа",
                    AgeCategory = "11 лет",
                    Gender = "Ж",
                    WeightCategory = "33 кг",
                    RedFirstName = "Мария",
                    RedLastName = "Смирнова",
                    RedClub = "Спартак",
                    WhiteFirstName = "Анна",
                    WhiteLastName = "Кузнецова",
                    WhiteClub = "Динамо"
                },
                new FightDisplayModel
                {
                    RedParticipantId = -7,
                    WhiteParticipantId = -8,
                    StageLabel = "Финал",
                    AgeCategory = "13 лет",
                    Gender = "М",
                    WeightCategory = "42 кг",
                    RedFirstName = "Андрей",
                    RedLastName = "Васильев",
                    RedClub = "ЦСКА",
                    WhiteFirstName = "Михаил",
                    WhiteLastName = "Попов",
                    WhiteClub = "Локомотив"
                }
            };

            tatamiItemsControl.ItemsSource = new List<TatamiData>
            {
                new TatamiData
                {
                    TatamiNumber = 1,
                    TatamiName = "Татами 1",
                    Fights = sampleFights
                }
            };
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }

    public class FightDisplayModel
    {
        public int RedParticipantId { get; set; }
        public int WhiteParticipantId { get; set; }
        public string StageLabel { get; set; }
        public string AgeCategory { get; set; }
        public string Gender { get; set; }
        public string WeightCategory { get; set; }
        public string RedFirstName { get; set; }
        public string RedLastName { get; set; }
        public string RedClub { get; set; }
        public string WhiteFirstName { get; set; }
        public string WhiteLastName { get; set; }
        public string WhiteClub { get; set; }
        public int TatamiNumber { get; set; }
        public string WhiteFullName => $"{WhiteFirstName} {WhiteLastName}".Trim();
        public string RedFullName => $"{RedFirstName} {RedLastName}".Trim();
        public string CategoryDisplay => $"U{AgeCategory} - {Gender} - {WeightCategory} kg";
    }

    public class TatamiData
    {
        public int TatamiNumber { get; set; }
        public string TatamiName { get; set; }
        public List<FightDisplayModel> Fights { get; set; } = new List<FightDisplayModel>();
    }
}
