using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace JudoApp
{
    /// <summary>
    /// Строит расписание боёв внутри групп и формирует данные для отображения в окнах.
    /// </summary>
    public class FightScheduleBuilder
    {
        private const int SoftSplitThreshold = 5;
        private const int MaxParticipantsPerGroup = 8;

        /// <summary>
        /// Формирует расписание по всем группам турнира.
        /// </summary>
        public List<GroupSchedule> BuildSchedules()
        {
            using (var db = new JudoDBEntities())
            {
                var groups = db.Groups
                    .Include(g => g.Participants.Select(p => p.Sportsclubs))
                    .Include(g => g.Tatamy)
                    .OrderBy(g => g.BaseAge)
                    .ThenBy(g => g.BaseWeight)
                    .ToList();

                var schedules = new List<GroupSchedule>();

                foreach (var group in groups)
                {
                    var participants = group.Participants
                        .Select(CreateSnapshot)
                        .Where(p => p != null)
                        .OrderBy(p => p.LastName)
                        .ThenBy(p => p.FirstName)
                        .ToList();

                    if (participants.Count < 2)
                    {
                        continue;
                    }

                    var sections = BuildSections(participants, group);
                    if (!sections.Any())
                    {
                        continue;
                    }

                    schedules.Add(new GroupSchedule
                    {
                        GroupId = group.Id,
                        BaseAge = group.BaseAge,
                        BaseWeight = group.BaseWeight,
                        Gender = group.Gender,
                        TatamiId = group.TatamiId,
                        TatamiName = group.Tatamy?.Name,
                        Sections = sections
                    });
                }

                return schedules;
            }
        }

        private ParticipantSnapshot CreateSnapshot(Participant participant)
        {
            if (participant == null || string.IsNullOrWhiteSpace(participant.FIO))
            {
                return null;
            }

            var fioParts = participant.FIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var firstName = fioParts.Length > 0 ? fioParts[0] : participant.FIO;
            var lastName = fioParts.Length > 1 ? fioParts[1] : string.Empty;
            var club = participant.Sportsclubs.FirstOrDefault()?.Name ?? string.Empty;

            return new ParticipantSnapshot
            {
                ParticipantId = participant.Id,
                FirstName = firstName,
                LastName = lastName,
                Club = club
            };
        }

        private List<SubgroupSchedule> BuildSections(List<ParticipantSnapshot> participants, Group group)
        {
            var sections = new List<SubgroupSchedule>();

            var splitted = SplitParticipants(participants);
            for (int i = 0; i < splitted.Count; i++)
            {
                var sectionName = splitted.Count == 1 ? "Группа" : $"Группа {(char)('A' + i)}";
                sections.Add(new SubgroupSchedule
                {
                    Name = sectionName,
                    IsPlayoff = false,
                    Fights = BuildRoundRobin(splitted[i], group, sectionName)
                });
            }

            if (sections.Count >= 2)
            {
                var finals = BuildFinals(sections, group);
                if (finals.Any())
                {
                    sections.Add(new SubgroupSchedule
                    {
                        Name = "Финал",
                        IsPlayoff = true,
                        Fights = finals
                    });
                }
            }

            return sections;
        }

        private List<List<ParticipantSnapshot>> SplitParticipants(List<ParticipantSnapshot> participants)
        {
            if (participants.Count <= SoftSplitThreshold)
            {
                return new List<List<ParticipantSnapshot>> { participants };
            }

            var groupCount = participants.Count > MaxParticipantsPerGroup
                ? (int)Math.Ceiling(participants.Count / (double)MaxParticipantsPerGroup)
                : 2;

            var buckets = Enumerable.Range(0, groupCount).Select(_ => new List<ParticipantSnapshot>()).ToList();

            for (int index = 0; index < participants.Count; index++)
            {
                buckets[index % groupCount].Add(participants[index]);
            }

            return buckets.Where(bucket => bucket.Any()).ToList();
        }

        private List<FightDisplayModel> BuildRoundRobin(List<ParticipantSnapshot> participants, Group group, string stageLabel)
        {
            var fights = new List<FightDisplayModel>();
            var tatamiNumber = group.TatamiId ?? -1;

            for (int i = 0; i < participants.Count; i++)
            {
                for (int j = i + 1; j < participants.Count; j++)
                {
                    var first = participants[i];
                    var second = participants[j];
                    var swap = ((i + j) % 2 == 0);

                    var red = swap ? first : second;
                    var white = swap ? second : first;

                    fights.Add(new FightDisplayModel
                    {
                        RedParticipantId = red.ParticipantId,
                        WhiteParticipantId = white.ParticipantId,
                        StageLabel = stageLabel,
                        AgeCategory = group.BaseAge.ToString(),
                        Gender = group.Gender,
                        WeightCategory = group.BaseWeight.ToString(),
                        RedFirstName = red.FirstName,
                        RedLastName = red.LastName,
                        RedClub = red.Club,
                        WhiteFirstName = white.FirstName,
                        WhiteLastName = white.LastName,
                        WhiteClub = white.Club,
                        TatamiNumber = tatamiNumber
                    });
                }
            }

            return fights;
        }

        private List<FightDisplayModel> BuildFinals(IEnumerable<SubgroupSchedule> sections, Group group)
        {
            var winners = sections
                .Where(s => !s.IsPlayoff)
                .Select(s => new ParticipantSnapshot
                {
                    FirstName = "Победитель",
                    LastName = s.Name,
                    Club = string.Empty
                })
                .ToList();

            if (winners.Count < 2)
            {
                return new List<FightDisplayModel>();
            }

            return BuildRoundRobin(winners, group, "Финал");
        }

        private class ParticipantSnapshot
        {
            public int ParticipantId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Club { get; set; }
        }
    }

    public class GroupSchedule
    {
        public int GroupId { get; set; }
        public int BaseAge { get; set; }
        public int BaseWeight { get; set; }
        public string Gender { get; set; }
        public int? TatamiId { get; set; }
        public string TatamiName { get; set; }
        public List<SubgroupSchedule> Sections { get; set; } = new List<SubgroupSchedule>();

        public string DisplayTitle => $"Возраст: {BaseAge}, Вес: {BaseWeight}, Пол: {Gender}";

        public IEnumerable<FightDisplayModel> AllFights => Sections.SelectMany(s => s.Fights);
    }

    public class SubgroupSchedule
    {
        public string Name { get; set; }
        public bool IsPlayoff { get; set; }
        public List<FightDisplayModel> Fights { get; set; } = new List<FightDisplayModel>();
    }
}

