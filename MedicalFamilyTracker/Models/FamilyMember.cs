using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MedicalFamilyTracker.Models
{
    public class FamilyMember : INotifyPropertyChanged
    {
        public int Id { get; set; }

        string? nameMember;

        public string? NameMember
        {
            get { return nameMember; } 
            set
            {
                nameMember = value;
                OnPropertyChanged(nameof(NameMember));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string? prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
