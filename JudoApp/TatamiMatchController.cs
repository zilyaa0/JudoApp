using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace JudoApp
{
    public enum VictoryType
    {
        Ippon,
        Points,
        Penalties,
        Decision,
        Disqualification
    }

    public class TatamiMatchController : IDisposable
    {
        public readonly TatamiMatchState _state;
        private readonly FightScheduleBuilder _scheduleBuilder;
        private readonly DispatcherTimer _mainTimer;
        private readonly DispatcherTimer _holdTimer;
        private readonly DispatcherTimer _highlightTimer;
        private readonly HashSet<int> _disqualifiedParticipants = new HashSet<int>();

        private IList<FightDisplayModel> _queue = new List<FightDisplayModel>();
        private int _currentIndex = -1;
        private DateTime? _fightStartTime;
        private DateTime? _highlightUntil;

        public TatamiMatchController(TatamiMatchState state = null)
        {
            _state = state ?? new TatamiMatchState();
            _scheduleBuilder = new FightScheduleBuilder();

            _mainTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _mainTimer.Tick += (_, __) => TickMainTimer();

            _holdTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _holdTimer.Tick += (_, __) => TickHoldTimer();

            _highlightTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _highlightTimer.Tick += (_, __) => TickHighlight();
        }

        public TatamiMatchState State => _state;

        public bool TryLoadTatami(int tatamiNumber)
        {
            if (tatamiNumber <= 0)
            {
                _state.StatusMessage = "Номер татами должен быть положительным.";
                return false;
            }

            var schedules = _scheduleBuilder.BuildSchedules();

            var fights = schedules
                .SelectMany(s => s.AllFights)
                .Where(f => f.TatamiNumber == tatamiNumber)
                .ToList();

            if (!fights.Any())
            {
                _state.StatusMessage = $"Для татами №{tatamiNumber} не найдено схваток.";
                return false;
            }

            _queue = fights;
            _currentIndex = -1;
            _state.TatamiNumber = tatamiNumber;
            _disqualifiedParticipants.Clear();
            LoadNextFight();
            _state.StatusMessage = $"Загружено {_queue.Count} схваток для татами №{tatamiNumber}.";
            return true;
        }

        public void StartMainTimer()
        {
            if (_state.IsMainTimerRunning)
            {
                return;
            }

            _state.IsMainTimerRunning = true;
            if (_fightStartTime == null)
            {
                _fightStartTime = DateTime.Now;
            }
            _mainTimer.Start();
        }

        public void StopMainTimer()
        {
            _state.IsMainTimerRunning = false;
            _mainTimer.Stop();
        }

        public void StartHoldTimer()
        {
            if (_state.IsHoldTimerRunning)
            {
                return;
            }

            _state.HoldTimerSeconds = 0;
            _state.IsHoldTimerRunning = true;
            _holdTimer.Start();
        }

        public void StopHoldTimer()
        {
            _state.IsHoldTimerRunning = false;
            _holdTimer.Stop();
        }

        public void AwardIppon(BeltColor color)
        {
            var card = GetCard(color);
            if (card.Ippon >= 1)
            {
                return;
            }

            card.Ippon = 1;
            FinishFight(color, VictoryType.Ippon);
        }

        public void AwardWazaAri(BeltColor color)
        {
            var card = GetCard(color);
            card.WazaAri++;

            if (card.WazaAri >= 2)
            {
                card.Ippon = 1;
                FinishFight(color, VictoryType.Ippon);
            }
            else
            {
                _state.StatusMessage = $"{color} получает ваза-ари.";
            }
        }

        public void GivePenalty(BeltColor color)
        {
            var card = GetCard(color);
            card.Penalties++;
            _state.StatusMessage = $"{color} получает предупреждение #{card.Penalties}.";

            if (card.Penalties > 3)
            {
                card.IsDisqualified = true;
                FinishFight(GetOpposite(color), VictoryType.Disqualification, disqualifiedColor: color);
            }
        }

        public void FinishFightManual(VictoryType type = VictoryType.Decision, BeltColor? winner = null)
        {
            var resolvedWinner = winner ?? ResolveWinner();
            FinishFight(resolvedWinner, type);
        }

        public void ResetCurrentFight()
        {
            _state.ResetScores();
            _state.StatusMessage = "Очки и таймеры сброшены.";
        }

        public void SkipCurrentFight()
        {
            _state.StatusMessage = "Схватка пропущена.";
            AdvanceQueue();
        }

        private void FinishFight(BeltColor winnerColor, VictoryType victoryType, BeltColor? disqualifiedColor = null)
        {
            if (_state.CurrentFight == null)
            {
                return;
            }

            StopMainTimer();
            StopHoldTimer();

            if (disqualifiedColor.HasValue)
            {
                var dqParticipantId = disqualifiedColor == BeltColor.White
                    ? _state.CurrentFight.WhiteParticipantId
                    : _state.CurrentFight.RedParticipantId;
                _disqualifiedParticipants.Add(dqParticipantId);
            }

            PersistResult(_state.CurrentFight, winnerColor);

            _state.HighlightWhite = winnerColor == BeltColor.White;
            _state.HighlightRed = winnerColor == BeltColor.Red;
            _highlightUntil = DateTime.Now.AddSeconds(30);
            _highlightTimer.Start();

            _state.StatusMessage = $"Победитель: {winnerColor} ({victoryType}).";
            AdvanceQueue();
        }

        private void LoadNextFight()
        {
            _state.ResetScores();
            _state.UpcomingFights.Clear();

            foreach (var fight in _queue.Skip(Math.Max(_currentIndex + 1, 0)))
            {
                _state.UpcomingFights.Add(fight);
            }

            if (_queue.Count == 0 || _currentIndex + 1 >= _queue.Count)
            {
                _state.CurrentFight = null;
                _state.NextFight = null;
                _state.StatusMessage = "Бои закончились. Можно перейти к генерации дипломов.";
                return;
            }

            _currentIndex++;
            _state.CurrentFight = _queue[_currentIndex];
            _state.NextFight = _queue.ElementAtOrDefault(_currentIndex + 1);
            _state.StatusMessage = $"Текущая схватка #{_currentIndex + 1} из {_queue.Count}.";
            _state.MainTimerSeconds = TatamiMatchState.DefaultMainDurationSeconds;
            _state.HoldTimerSeconds = 0;
            _fightStartTime = null;
        }

        private void AdvanceQueue()
        {
            PruneQueue();
            LoadNextFight();
        }

        private void PruneQueue()
        {
            if (!_disqualifiedParticipants.Any())
            {
                return;
            }

            var future = _queue.Take(_currentIndex + 1).ToList();
            future.AddRange(
                _queue.Skip(_currentIndex + 1)
                      .Where(f => !_disqualifiedParticipants.Contains(f.WhiteParticipantId)
                               && !_disqualifiedParticipants.Contains(f.RedParticipantId))
            );

            _queue = future;
        }

        private void TickMainTimer()
        {
            if (_state.MainTimerSeconds <= 0)
            {
                StopMainTimer();
                FinishFight(ResolveWinner(), VictoryType.Points);
                return;
            }

            _state.MainTimerSeconds--;
        }

        private void TickHoldTimer()
        {
            if (_state.HoldTimerSeconds >= TatamiMatchState.DefaultHoldDurationSeconds)
            {
                StopHoldTimer();
                _state.StatusMessage = "Удержание завершено. Присудите оценку вручную.";
                return;
            }

            _state.HoldTimerSeconds++;
        }

        private void TickHighlight()
        {
            if (_highlightUntil.HasValue && DateTime.Now >= _highlightUntil.Value)
            {
                _state.HighlightRed = false;
                _state.HighlightWhite = false;
                _highlightTimer.Stop();
                _highlightUntil = null;
            }
        }

        private BeltColor ResolveWinner()
        {
            if (_state.WhiteScore.Ippon > _state.RedScore.Ippon)
            {
                return BeltColor.White;
            }

            if (_state.RedScore.Ippon > _state.WhiteScore.Ippon)
            {
                return BeltColor.Red;
            }

            if (_state.WhiteScore.WazaAri > _state.RedScore.WazaAri)
            {
                return BeltColor.White;
            }

            if (_state.RedScore.WazaAri > _state.WhiteScore.WazaAri)
            {
                return BeltColor.Red;
            }

            if (_state.WhiteScore.Penalties < _state.RedScore.Penalties)
            {
                return BeltColor.White;
            }

            if (_state.RedScore.Penalties < _state.WhiteScore.Penalties)
            {
                return BeltColor.Red;
            }

            return BeltColor.White;
        }

        private void PersistResult(FightDisplayModel fight, BeltColor winnerColor)
        {
            if (fight.WhiteParticipantId <= 0 || fight.RedParticipantId <= 0)
            {
                _state.StatusMessage = "Схватка завершена (результат не сохранён, отсутствуют идентификаторы участников).";
                return;
            }

            try
            {
                using (var db = new JudoDBEntities())
                {
                    var record = db.Fights.SingleOrDefault(f =>
                        f.WhiteParticipantId == fight.WhiteParticipantId &&
                        f.RedParticipantId == fight.RedParticipantId);

                    if (record == null)
                    {
                        record = new Fight
                        {
                            WhiteParticipantId = fight.WhiteParticipantId,
                            RedParticipantId = fight.RedParticipantId
                        };
                        db.Fights.Add(record);
                    }

                    record.WhitePoints = ConvertScore(_state.WhiteScore);
                    record.RedPoints = ConvertScore(_state.RedScore);
                    record.Duration = TatamiMatchState.DefaultMainDurationSeconds - _state.MainTimerSeconds;
                    record.StartTime = _fightStartTime?.TimeOfDay;

                    if (winnerColor == BeltColor.White && _state.WhiteScore.Ippon == 0 && _state.WhiteScore.WazaAri == 0)
                    {
                        record.WhitePoints = 1m;
                    }

                    if (winnerColor == BeltColor.Red && _state.RedScore.Ippon == 0 && _state.RedScore.WazaAri == 0)
                    {
                        record.RedPoints = 1m;
                    }

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _state.StatusMessage = $"Ошибка сохранения результата: {ex.Message}";
            }
        }

        private decimal? ConvertScore(ScoreCard card)
        {
            if (card.Ippon > 0)
            {
                return 1m;
            }

            if (card.WazaAri > 0)
            {
                return Math.Min(2, card.WazaAri) * 0.5m;
            }

            return 0m;
        }

        private ScoreCard GetCard(BeltColor color) => color == BeltColor.White ? _state.WhiteScore : _state.RedScore;

        private static BeltColor GetOpposite(BeltColor color) => color == BeltColor.White ? BeltColor.Red : BeltColor.White;

        public void Dispose()
        {
            _mainTimer.Stop();
            _holdTimer.Stop();
            _highlightTimer.Stop();
        }
    }
}

