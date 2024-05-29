using MedicalFamilyTracker.Commands;
using MedicalFamilyTracker.Models;
using System.Windows.Input;
using System.Windows;

namespace MedicalFamilyTracker.ViewModels
{
    public class AddFamilyMemberViewModel : ViewModelBase
    {
        private FamilyMember newFamilyMember;
        public ApplicationContext _context;

        public AddFamilyMemberViewModel(ApplicationContext context)
        {
            NewFamilyMember = new FamilyMember();
            _context = context;
        }

        public FamilyMember NewFamilyMember
        {
            get { return newFamilyMember; }
            set
            {
                newFamilyMember = value;
                OnPropertyChanged(nameof(NewFamilyMember));
            }
        }

        private DelegateCommand addFamilyMemberCommand;
        public ICommand AddFamilyMemberCommand
        {
            get
            {
                addFamilyMemberCommand ??= new DelegateCommand(AddFamilyMember);
                return addFamilyMemberCommand;
            }
        }

        private void AddFamilyMember()
        {
            if (string.IsNullOrWhiteSpace(newFamilyMember.NameMember))
            {
                MessageBox.Show("Будь ласка, заповніть поле перед додаванням запису.", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return; 
            }
            else
            {
                _context.FamilyMembers.Add(newFamilyMember);
                _context.SaveChanges();
                MessageBox.Show($"New family member added: {newFamilyMember.NameMember}");
                Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
            }
        }
    }
}
