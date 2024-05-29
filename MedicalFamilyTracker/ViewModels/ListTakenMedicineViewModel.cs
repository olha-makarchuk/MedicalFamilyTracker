using MedicalFamilyTracker.Models;

namespace MedicalFamilyTracker.ViewModels
{
    public class ListTakenMedicineViewModel : ViewModelBase
    {
        public ListTakenMedicineViewModel(List<SheduleEntity> shedules)
        {
            Shedules = shedules;
        }

        private List<SheduleEntity> _shedules;
        public List<SheduleEntity> Shedules
        {
            get { return _shedules; }
            set
            {
                _shedules = value;
                OnPropertyChanged(nameof(Shedules));
            }
        }
    }
}
