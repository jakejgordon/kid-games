using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CoolDuel.Annotations;

namespace CoolDuel.ViewModels
{
    public class CharacterViewModel : INotifyPropertyChanged
    {
        public const int StartingHitPoints = 20;
        public const int StartingAttackDamage = 2;
        public const int StartingAttributePoints = 3;
        public const int AttributeToHitPointRatio = 5;
        public const int AttributeToAttackDamageRatio = 1;

        private int _maxHitPoints = StartingHitPoints;
        private int _hitPoints = StartingHitPoints;
        private string _name;
        private int _availableAttributePoints = StartingAttributePoints;
        private int _attackDamage = StartingAttackDamage;

        public int AvailableAttributePoints
        {
            get => _availableAttributePoints;
            set
            {
                _availableAttributePoints = value;
                OnPropertyChanged();
            }
        }

        public int MaxHitPoints
        {
            get => _maxHitPoints;
            set
            {
                _maxHitPoints = value;
                OnPropertyChanged();
            } 
        }

        public int HitPoints
        {
            get => _hitPoints;
            set
            {
                _hitPoints = value;
                OnPropertyChanged();
            }
        }

        public int AttackDamage
        {
            get => _attackDamage;
            set
            {
                _attackDamage = value;
                OnPropertyChanged();
            }
        }


        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void IncreaseMaxHitPoints()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            MaxHitPoints += AttributeToHitPointRatio;
            HitPoints += AttributeToHitPointRatio;
            OnPropertyChanged(nameof(HasAttributePoints));
            OnPropertyChanged(nameof(AvailableAttributePoints));
            OnPropertyChanged(nameof(MaxHitPoints));
        }

        private void ValidateAvailableAttributePoints()
        {
            if (AvailableAttributePoints <= 0)
            {
                throw new InvalidOperationException(
                    "Attempted to apply an attribute point, but no attribute points were available!");
            }
        }

        public bool HasAttributePoints =>AvailableAttributePoints > 0;

        public void IncreaseAttackDamage()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            AttackDamage += AttributeToAttackDamageRatio;
            OnPropertyChanged(nameof(HasAttributePoints));
            OnPropertyChanged(nameof(AvailableAttributePoints));
            OnPropertyChanged(nameof(AttackDamage));
        }

    }
}
