using MedicalFamilyTracker.Commands;
using System.Windows.Input;
using System.Windows;
using MedicalFamilyTracker.Models;

namespace MedicalFamilyTracker.ViewModels
{
    public class DelaySheduleItemViewModel : ViewModelBase
    {
        public ApplicationContext _context;
        public SheduleEntity _sheduleEntity;
        public DelaySheduleItemViewModel(ApplicationContext context, SheduleEntity sheduleEntity)
        {
            _context = context;
            _sheduleEntity = sheduleEntity;
        }

        public DateTime newDateTime = DateTime.Now;
        public DateTime NewDateTime
        {
            get { return newDateTime; }
            set
            {
                newDateTime = value;
                OnPropertyChanged(nameof(NewDateTime));
            }
        }

        private DelegateCommand saveDateTimeCommand;

        public ICommand SaveDateTimeCommand
        {
            get
            {
                saveDateTimeCommand ??= new DelegateCommand(GetItem);
                return saveDateTimeCommand;
            }
        }

        private void GetItem()
        {
            if (NewDateTime <= DateTime.Now)
            {
                MessageBox.Show("Будь ласка, заповніть поле перед додаванням запису.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
            {
                _sheduleEntity.Status = StatusNote.IsDelayed.ToString();
                _context.SheduleEntities.Update(_sheduleEntity);
                _context.SaveChanges();

                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
        }
    }
}
