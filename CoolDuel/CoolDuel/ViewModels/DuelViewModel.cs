﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoolDuel.Annotations;

namespace CoolDuel.ViewModels
{
    public class DuelViewModel : INotifyPropertyChanged
    {
        private string _announcement = "Prepare For Battle!";
        private bool _character1Turn = true;
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

        public bool Character1Turn
        {
            get => _character1Turn;
            set
            {
                _character1Turn = value;
                OnPropertyChanged();
            }
        }

        public DuelViewModel()
        {
            Character1 = new CharacterViewModel(true, Weapon.Sword);
            Character2 = new CharacterViewModel(false, Weapon.Sword);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public BasicAttack NextAttack()
        {
            if (Character1Turn)
            {
                return Character1.MakeBasicAttack(Character2);
            }

            return Character2.MakeBasicAttack(Character1);
        }
    }
}
