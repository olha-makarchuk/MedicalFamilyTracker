using MedicalFamilyTracker.ViewModels;
using MedicalFamilyTracker.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MedicalFamilyTracker.Models
{
	public class SheduleEntity : INotifyPropertyChanged
    {
        public int Id { get; set; }

        string? nameFamelyMember;
        string? medicine;
        DateTime dateTime;
        string? status;

        public string? NameFamelyMember
        {
            get { return nameFamelyMember; }
            set
            {
                nameFamelyMember = value;
                OnPropertyChanged(nameof(NameFamelyMember));
            }
        }

        public string? Medicine
        {
            get { return medicine; }
            set
            {
                medicine = value;
                OnPropertyChanged(nameof(Medicine));
            }
        }

        public DateTime DateTime
        {
            get { return dateTime; }
            set
            {
                dateTime = value;
                OnPropertyChanged(nameof(DateTime));
            }
        }

        public string? Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
