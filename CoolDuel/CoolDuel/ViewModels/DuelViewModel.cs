using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoolDuel.Annotations;

namespace CoolDuel.ViewModels
{
    public class DuelViewModel : INotifyPropertyChanged
    {
        private string _announcement = "Prepare For Battle!";
        public CharacterViewModel Character1 { get; set; }
        public CharacterViewModel Character2 { get; set; }
        public string Announcement
        {
            get => _announcement;
            set
            {
                _announcement = value;
                OnPropertyChanged();
            }
        }

        public DuelViewModel()
        {
            Character1 = new CharacterViewModel();
            Character2 = new CharacterViewModel();
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
