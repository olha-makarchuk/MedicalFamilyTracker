using MedicalFamilyTracker.Commands;
using MedicalFamilyTracker.Models;
using MedicalFamilyTracker.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MedicalFamilyTracker.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ApplicationContext _context;
        private readonly MainWindow _mainWindow;

        public ObservableCollection<FamilyMember> FamilyMembersList { get; set; }

        public ObservableCollection<SheduleEntity> SheduleEntityList { get; set; }

        public MainWindowViewModel(ApplicationContext context, MainWindow mainWindow)
        {
            _context = context;

            _context.FamilyMembers.Load();
            FamilyMembersList = _context.FamilyMembers.Local.ToObservableCollection();

            _context.SheduleEntities.Load();
            SheduleEntityList = new ObservableCollection<SheduleEntity>(
                _context.SheduleEntities.Local.Where(s => s.Status != StatusNote.IsTaken.ToString()));

            _mainWindow = mainWindow;
        }

        #region CommandAddFamilyMemberCommand
        private ICommand openAddFamilyMemberCommand;
        public ICommand OpenAddFamilyMemberCommand
        {
            get
            {
                openAddFamilyMemberCommand ??= new DelegateCommand(OpenAddFamilyMember);
                return openAddFamilyMemberCommand;
            }
        }

        private void OpenAddFamilyMember()
        {
            var addFamilyMemberView = new AddFamilyMemberView();
            var addFamilyMemberViewModel = new AddFamilyMemberViewModel(_context);
            addFamilyMemberView.DataContext = addFamilyMemberViewModel;
            addFamilyMemberView.ShowDialog();
        }
        #endregion

        #region CommandSelectedFamilyMember
        private FamilyMember _selectedFamilyMember;
        public FamilyMember SelectedFamilyMember
        {
            get { return _selectedFamilyMember; }
            set
            {
                _selectedFamilyMember = value;
                var addNote = new AddNoteView();
                var addNoteViewModel = new AddNoteViewModel(SelectedFamilyMember.NameMember!, _context);
                addNote.DataContext = addNoteViewModel;
                addNote.ShowDialog();

                RefreshSheduleEntityList(); 

                OnPropertyChanged(nameof(SelectedFamilyMember));
            }
        }

        #endregion

        #region CommandRefreshDataCommand
        private ICommand refreshDataCommand;
        public ICommand RefreshDataCommand
        {
            get
            {
                refreshDataCommand ??= new DelegateCommand(Change);
                return refreshDataCommand;
            }
        }

        public void Change()
        {
            DateTime currentTime = DateTime.Now;

            foreach (var book in SheduleEntityList)
            {
                if (book.DateTime < currentTime)
                {
                    book.Status = StatusNote.IsMissed.ToString();
                    _context.SheduleEntities.Update(book);
                }
            }
            _context.SaveChanges();
            RefreshSheduleEntityList();
        }

        #endregion

        #region CommandDeleteSheduleItemCommand
        private ICommand deleteSheduleItemCommand;
        public ICommand DeleteSheduleItemCommand
        {
            get
            {
                deleteSheduleItemCommand ??= new DelegateCommand<object>(DeleteItem);
                return deleteSheduleItemCommand;
            }
        }

        private void DeleteItem(object selectedItem)
        {
            if (selectedItem is SheduleEntity sheduleItem)
            {
                _context.SheduleEntities.Remove(sheduleItem);
                _context.SaveChanges();
                RefreshSheduleEntityList(); 
            }
        }
        #endregion

        #region CommandDelaySheduleItemCommand
        private ICommand delaySheduleItemCommand;
        public ICommand DelaySheduleItemCommand
        {
            get
            {
                delaySheduleItemCommand ??= new DelegateCommand<object>(MoveItem);
                return delaySheduleItemCommand;
            }
        }

        private void MoveItem(object selectedItem)
        {
            if (selectedItem is SheduleEntity sheduleItem)
            {
                var delaySheduleItem = new DelaySheduleItemView();
                var delaySheduleItemViewModel = new DelaySheduleItemViewModel(_context, sheduleItem);
                delaySheduleItem.DataContext = delaySheduleItemViewModel;
                delaySheduleItem.ShowDialog();
            }
        }
        #endregion

        #region CommandMedicineIsTakenCommand
        private ICommand medicineIsTakenCommand;
        public ICommand MedicineIsTakenCommand
        {
            get
            {
                medicineIsTakenCommand ??= new DelegateCommand<object>(MedicineIsTakenItem);
                return medicineIsTakenCommand;
            }
        }

        private void MedicineIsTakenItem(object selectedItem)
        {
            if (selectedItem is SheduleEntity sheduleItem)
            {
                sheduleItem.Status = StatusNote.IsTaken.ToString();

                _context.SheduleEntities.Update(sheduleItem);
                _context.SaveChanges();

                RefreshSheduleEntityList();
            }
        }
        #endregion

        #region CommandMedicineIsTakenCommand
        private ICommand listTakenMedicineCommand;
        public ICommand ListTakenMedicineCommand
        {
            get
            {
                listTakenMedicineCommand ??= new DelegateCommand(ListTakenMedicineItem);
                return listTakenMedicineCommand;
            }
        }

        private void ListTakenMedicineItem()
        {
            
            var listTakenMedicineView = new ListTakenMedicineView();
            var listTakenMedicineViewModel = new ListTakenMedicineViewModel(SheduleEntityList.Where(b => b.Status == StatusNote.IsTaken.ToString()).ToList());
            listTakenMedicineView.DataContext = listTakenMedicineViewModel;
            listTakenMedicineView.ShowDialog();
        }
        #endregion
        public void RefreshSheduleEntityList()
        {
            _context.SheduleEntities.Load();
            SheduleEntityList = new ObservableCollection<SheduleEntity>(
                _context.SheduleEntities.Local.Where(s => s.Status != StatusNote.IsTaken.ToString()));
            OnPropertyChanged(nameof(SheduleEntityList));
        }

    }
}
