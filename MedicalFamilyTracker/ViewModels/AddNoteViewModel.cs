using MedicalFamilyTracker.Commands;
using MedicalFamilyTracker.Models;
using System.Windows.Input;
using System.Windows;

namespace MedicalFamilyTracker.ViewModels
{
    public class AddNoteViewModel : ViewModelBase
    {
        private string selected;
        private SheduleEntity newSheduleNote;
        private int frequency;
        private int periodicity;
        public ApplicationContext _context;
        public List<SheduleEntity> SheduleEntities;

        public AddNoteViewModel(string selected, ApplicationContext context)
        {
            _context = context;
            Selected = selected;
            NewSheduleNote = new SheduleEntity() { DateTime= DateTime.Now};
            SheduleEntities = new List<SheduleEntity>();
            Frequency = 1;
            Periodicity = 1;
        }

        public string Selected
        {
            get { return selected; }
            set
            {
                selected = value;
                OnPropertyChanged(nameof(Selected));
            }
        }

        public SheduleEntity NewSheduleNote
        {
            get { return newSheduleNote; }
            set
            {
                newSheduleNote = value;
                OnPropertyChanged(nameof(NewSheduleNote));
            }
        }

        public int Frequency
        {
            get { return frequency; }
            set
            {
                frequency = value;
                OnPropertyChanged(nameof(Frequency));
            }
        }

        public int Periodicity
        {
            get { return periodicity; }
            set
            {
                periodicity = value;
                OnPropertyChanged(nameof(Periodicity));
            }
        }

        private void CalculateIntakeTimes()
        {
            var intakeSchedule = GenerateIntakeSchedule(NewSheduleNote.DateTime, Frequency, Periodicity);

            foreach (var time in intakeSchedule)
            {
                var newScheduleEntity = new SheduleEntity();
                newScheduleEntity.NameFamelyMember = Selected;
                newScheduleEntity.Medicine = NewSheduleNote.Medicine;
                newScheduleEntity.DateTime = time;
                newScheduleEntity.Status = StatusNote.IsExpected.ToString();

                SheduleEntities.Add(newScheduleEntity);
            }
        }


        private List<DateTime> CalculateDailyIntakeTimes(DateTime startDateTime, int timesPerDay)
        {
            List<DateTime> intakeTimes = new List<DateTime>();
            double interval = 14.0 / timesPerDay; // 14 hours from 8 AM to 10 PM

            for (int i = 0; i < timesPerDay; i++)
            {
                DateTime intakeTime = startDateTime.Date.AddHours(8).AddHours(interval * i);
                intakeTimes.Add(intakeTime);
            }

            return intakeTimes;
        }

        private List<DateTime> GenerateIntakeSchedule(DateTime startDateTime, int timesPerDay, int periodicity)
        {
            List<DateTime> intakeSchedule = new List<DateTime>();

            for (int day = 0; day < periodicity; day++)
            {
                DateTime currentDay = startDateTime.AddDays(day);
                List<DateTime> dailyIntakeTimes = CalculateDailyIntakeTimes(currentDay, timesPerDay);

                intakeSchedule.AddRange(dailyIntakeTimes);
            }

            return intakeSchedule;
        }

        private DelegateCommand addSheduleNoteCommand;
        public ICommand AddSheduleNoteCommand
        {
            get
            {
                addSheduleNoteCommand ??= new DelegateCommand(AddSheduleNote);
                return addSheduleNoteCommand;
            }
        }

        private void AddSheduleNote()
        {
            if (string.IsNullOrWhiteSpace(NewSheduleNote.Medicine) || Frequency <= 0 || Periodicity <= 0 || NewSheduleNote.DateTime == DateTime.MinValue)
            {
                MessageBox.Show("Будь ласка, заповніть коректно всі поля перед додаванням запису.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }
            else
            {
                CalculateIntakeTimes();
                _context.SheduleEntities.AddRangeAsync(SheduleEntities);
                _context.SaveChanges();

                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
        }
    }
}
