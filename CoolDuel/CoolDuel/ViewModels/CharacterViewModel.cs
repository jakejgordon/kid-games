using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using CoolDuel.Annotations;

namespace CoolDuel.ViewModels
{
    public class CharacterViewModel : INotifyPropertyChanged
    {
        public const int StartingHitPoints = 20;
        public const int StartingAttackDamage = 2;
        public const int StartingAttributePoints = 3;
        public const int StartingMaxAttackRoll = 6;
        public const int StartingMaxDefenseRoll = 6;
        public const int StartingCounterattackDamage = 2;
        public const int AttributeToHitPointRatio = 5;
        public const int AttributeToAttackDamageRatio = 1;

        private int _maxHitPoints = StartingHitPoints;
        private int _hitPoints = StartingHitPoints;
        private int _maxAttackRoll = StartingMaxAttackRoll;
        private int _maxDefenseRoll = StartingMaxDefenseRoll;
        private string _name;
        private int _availableAttributePoints = StartingAttributePoints;
        private int _attackDamage = StartingAttackDamage;

        private readonly Random _random = new Random();
        private int _counterattackDamage = StartingCounterattackDamage;
        private Weapon _equippedWeapon;
        private readonly bool _character1;

        public CharacterViewModel(bool character1, Weapon weapon)
        {
            _character1 = character1;
            //--use the property so the fancy setter stuff runs
            EquippedWeapon = weapon;
            CharacterImage = character1
                ? new BitmapImage(new Uri("ms-appx:///Assets/BattleIcons/c1_knight.png"))
                : new BitmapImage(new Uri("ms-appx:///Assets/BattleIcons/c2_knight.png"));
        }

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

        public int MaxAttackRoll
        {
            get => _maxAttackRoll;
            set
            {
                _maxAttackRoll = value;
                OnPropertyChanged();
            }
        }

        public int MaxDefenseRoll
        {
            get => _maxDefenseRoll;
            set
            {
                _maxDefenseRoll = value;
                OnPropertyChanged();
            }
        }

        public int CounterattackDamage
        {
            get => _counterattackDamage;
            set
            {
                _counterattackDamage = value;
                OnPropertyChanged();
            }
        }

        public Weapon EquippedWeapon
        {
            get => _equippedWeapon;
            set
            {
                _equippedWeapon = value;

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

        public BitmapImage CharacterImage { get; set; }

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

        public BasicAttack MakeBasicAttack(CharacterViewModel defendingCharacter)
        {
            var attackRoll = GetBasicAttackRoll();
            return new BasicAttack
            {
                AttackingCharacter = this,
                DefendingCharacter = defendingCharacter,
                AttackRoll = attackRoll
            };
        }


        public int GetDefenseRoll()
        {
            return _random.Next(1, _maxDefenseRoll);
        }

        public int GetBasicAttackRoll()
        {
            return _random.Next(1, _maxAttackRoll);
        }

        public int TakeDamage(CharacterViewModel attackingCharacter)
        {
            var totalDamage = attackingCharacter.AttackDamage + attackingCharacter.ConsumeOneTimeBonusDamage();
            HitPoints -= totalDamage;
 
            return totalDamage;
        }

        private int ConsumeOneTimeBonusDamage()
        {
            int bonusDamage = 0;
            while (NextAttackBonusDamageStack.Count > 0)
            {
                bonusDamage += NextAttackBonusDamageStack.Pop().Damage;
            }

            return bonusDamage;
        }

        public void AddNextAttackBonusDamage(BonusDamage bonusDamage)
        {
            NextAttackBonusDamageStack.Push(bonusDamage);
        }

        public Stack<BonusDamage> NextAttackBonusDamageStack { get; set; } = new Stack<BonusDamage>();
        public bool Dead => HitPoints <= 0;

        public ImageSource WeaponImage
        {
            get
            {
                if (_character1)
                {
                    return EquippedWeapon.ImageSourceFlowDirectionLeftToRight;
                }

                return EquippedWeapon.ImageSourceFlowDirectionRightToLeft;
            }
        }
    }
}
