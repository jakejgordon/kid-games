using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using CoolDuel.Annotations;

namespace CoolDuel.ViewModels
{
    public class CharacterViewModel : INotifyPropertyChanged
    {
        public const int StartingHitPoints = 20;
        public const  int StartingAttributePoints = 3;
        public const  int AttributeToHitPointRatio = 3;
        public const  int AttributeToAttackDamageRatio = 1;
        public const int AttributeToMinimumAttackRollRatio = 1;
        public const int AttributeToMaximumAttackRollRatio = 1;
        public const int AttributeToMinimumDefenseRollRatio = 1;
        public const int AttributeToMaximumDefenseRollRatio = 1;
        public const int AttributeToCounterattackDamageRatio = 2;
        public const int PixelsPerHitPoint = 5;

        //--this was the only way I could find to get dynamic messages in UWP :(
        public string AddCounterattackDamageMessage = XamlMessages.AddCounterattackDamageMessage;
        public string AddAttackDamageMessage = XamlMessages.AddAttackDamageMessage;
        public string AddMinimumAttackRollMessage = XamlMessages.AddMinimumAttackRollMessage;
        public string AddMaximumAttackRollMessage = XamlMessages.AddMaximumAttackRollMessage;
        public string AddMinimumDefenseRollMessage = XamlMessages.AddMinimumDefenseRollMessage;
        public string AddMaximumDefenseRollMessage = XamlMessages.AddDefenseRollMessage;
        public string AddHitPointsMessage = XamlMessages.AddHitPointsMessage;
        
        private int _maxHitPoints = StartingHitPoints;
        private int _hitPoints = StartingHitPoints;
        private string _name;
        private int _availableAttributePoints = StartingAttributePoints;
        private int _bonusAttackDamage;
        private int _bonusMinimumAttackRoll;
        private int _bonusMaximumAttackRoll;
        private int _bonusMinimumDefenseRoll;
        private int _bonusMaximumDefenseRoll;
        private int _bonusCounterattackDamage;


        private readonly Random _random = new Random();
        private Weapon _equippedWeapon;
        public bool Character1 { get; }

        public CharacterViewModel(bool character1, Weapon weapon)
        {
            Character1 = character1;
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
                OnPropertyChanged(nameof(HasAttributePoints));
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
                if (value >= 0)
                {
                    _hitPoints = value;
                }
                else
                {
                    _hitPoints = 0;
                }
                OnPropertyChanged();
                OnPropertyChanged(nameof(HealthMeter));
            }
        }

        public int HealthMeter => HitPoints * PixelsPerHitPoint;

        public int BonusAttackDamage
        {
            get => _bonusAttackDamage;
            set
            {
                _bonusAttackDamage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAttackDamage));
            }
        }

        public int TotalAttackDamage => BonusAttackDamage + EquippedWeapon.AttackDamage;

        public int BonusMinimumAttackRoll
        {
            get => _bonusMinimumAttackRoll;
            set
            {
                _bonusMinimumAttackRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMinimumAttackRoll));
            }
        }

        public int TotalMinimumAttackRoll => BonusMinimumAttackRoll + EquippedWeapon.MinimumAttackRoll;

        public int BonusMaximumAttackRoll
        {
            get => _bonusMaximumAttackRoll;
            set
            {
                _bonusMaximumAttackRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMaximumAttackRoll));
            }
        }

        public int TotalMaximumAttackRoll => BonusMaximumAttackRoll + EquippedWeapon.MaximumAttackRoll;

        public int BonusMinimumDefenseRoll
        {
            get => _bonusMinimumDefenseRoll;
            set
            {
                _bonusMinimumDefenseRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMinimumDefenseRoll));
            }
        }

        public int TotalMinimumDefenseRoll => BonusMinimumDefenseRoll + EquippedWeapon.MinimumAttackRoll;

        public int BonusMaximumDefenseRoll
        {
            get => _bonusMaximumDefenseRoll;
            set
            {
                _bonusMaximumDefenseRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalMaximumDefenseRoll));
            }
        }

        public int TotalMaximumDefenseRoll => BonusMaximumDefenseRoll + EquippedWeapon.MaximumDefenseRoll;

        public int BonusCounterattackDamage
        {
            get => _bonusCounterattackDamage;
            set
            {
                _bonusCounterattackDamage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalCounterattackDamage));
            }
        }

        public int TotalCounterattackDamage => BonusCounterattackDamage + EquippedWeapon.CounterAttackDamage;

        public Weapon EquippedWeapon
        {
            get => _equippedWeapon;
            set
            {
                _equippedWeapon = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAttackDamage));
                OnPropertyChanged(nameof(TotalMaximumDefenseRoll));
                OnPropertyChanged(nameof(TotalMaximumAttackRoll));
                OnPropertyChanged(nameof(TotalCounterattackDamage));
                OnPropertyChanged(nameof(WeaponImage));
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

        private void ValidateAvailableAttributePoints()
        {
            if (AvailableAttributePoints <= 0)
            {
                throw new InvalidOperationException(
                    "Attempted to apply an attribute point, but no attribute points were available!");
            }
        }

        public void IncreaseMaxHitPoints()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            MaxHitPoints += AttributeToHitPointRatio;
            HitPoints += AttributeToHitPointRatio;
        }

        public bool HasAttributePoints => AvailableAttributePoints > 0;

        public void IncreaseBonusAttackDamage()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusAttackDamage += AttributeToAttackDamageRatio;
        }

        public void IncreaseBonusMinimumAttackRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusMinimumAttackRoll += AttributeToMinimumAttackRollRatio;
        }

        public void IncreaseBonusMaxAttackRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusMaximumAttackRoll += AttributeToMaximumAttackRollRatio;
        }

        public void IncreaseBonusMinimumDefenseRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusMinimumDefenseRoll += AttributeToMinimumDefenseRollRatio;
        }

        public void IncreaseBonusMaximumDefenseRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusMaximumDefenseRoll += AttributeToMaximumDefenseRollRatio;
        }

        public void IncreaseBonusCounterattackDamage()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusCounterattackDamage += AttributeToCounterattackDamageRatio;
        }

        public BasicAttack MakeBasicAttack(CharacterViewModel defendingCharacter)
        {
            var attackRoll = GetBasicAttackRoll();
            return new BasicAttack(attackRoll, this, defendingCharacter);
        }

        public int GetDefenseRoll()
        {
            var maxDefenseRoll = TotalMinimumDefenseRoll < TotalMaximumDefenseRoll
                ? TotalMaximumDefenseRoll
                : TotalMinimumDefenseRoll;
            return _random.Next(TotalMinimumDefenseRoll, maxDefenseRoll);
        }

        public int GetBasicAttackRoll()
        {
            var maxAttackRoll = TotalMinimumAttackRoll < TotalMaximumAttackRoll
                ? TotalMaximumAttackRoll
                : TotalMinimumAttackRoll;
            return _random.Next(TotalMinimumAttackRoll, maxAttackRoll);
        }

        public int TakeDamage(CharacterViewModel attackingCharacter)
        {
            var totalDamage = attackingCharacter.TotalAttackDamage + attackingCharacter.ConsumeOneTimeBonusDamage();
            HitPoints -= totalDamage;
 
            return totalDamage;
        }

        public int ConsumeOneTimeBonusDamage()
        {
            int bonusDamage = 0;
            while (NextAttackBonusDamageStack.Count > 0)
            {
                bonusDamage += NextAttackBonusDamageStack.Pop().Damage;
            }

            OnPropertyChanged(nameof(PendingCounterattackDamage));

            return bonusDamage;
        }

        public void AddNextAttackBonusDamage(BonusDamage bonusDamage)
        {
            NextAttackBonusDamageStack.Push(bonusDamage);
            OnPropertyChanged(nameof(PendingCounterattackDamage));
        }

        public Stack<BonusDamage> NextAttackBonusDamageStack { get; set; } = new Stack<BonusDamage>();
        public bool Dead => HitPoints <= 0;

        public int PendingCounterattackDamage
        {
            get
            {
                int pendingDamage = 0;
                foreach (var damage in NextAttackBonusDamageStack)
                {
                    pendingDamage += damage.Damage;
                }

                return pendingDamage;
            }
        }

        public ImageSource WeaponImage
        {
            get
            {
                if (Character1)
                {
                    return EquippedWeapon.ImageSourceFlowDirectionLeftToRight;
                }

                return EquippedWeapon.ImageSourceFlowDirectionRightToLeft;
            }
        }

        public List<ComboBoxItem> AvailableWeapons = Weapon.AllWeapons.Select(weapon => new ComboBoxItem
        {
            Content = weapon.Name
        }).ToList();
    }
}
