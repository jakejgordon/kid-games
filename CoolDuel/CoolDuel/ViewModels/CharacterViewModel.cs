﻿using System;
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
        public const  int AttributeToHitPointRatio = 4;
        public const  int AttributeToAttackDamageRatio = 1;
        public const int AttributeToAttackRollRatio = 1;
        public const int AttributeToDefenseRollRatio = 1;
        public const int AttributeToCounterattackDamageRatio = 2;
        public const int PixelsPerHitPoint = 6;

        //--this was the only way I could find to get dynamic messages in UWP :(
        public string AddCounterattackDamageMessage = XamlMessages.AddCounterattackDamageMessage;
        public string AddAttackDamageMessage = XamlMessages.AddAttackDamageMessage;
        public string AddAttackRollMessage = XamlMessages.AddAttackRollMessage;
        public string AddDefenseRollMessage = XamlMessages.AddDefenseRollMessage;
        public string AddHitPointsMessage = XamlMessages.AddHitPointsMessage;
        
        private int _maxHitPoints = StartingHitPoints;
        private int _hitPoints = StartingHitPoints;
        private string _name;
        private int _availableAttributePoints = StartingAttributePoints;
        private int _bonusAttackDamage;
        private int _bonusAttackRoll;
        private int _bonusDefenseRoll;
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

        public int BonusAttackRoll
        {
            get => _bonusAttackRoll;
            set
            {
                _bonusAttackRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalAttackRoll));
            }
        }

        public int TotalAttackRoll => BonusAttackRoll + EquippedWeapon.AttackRoll;

        public int BonusDefenseRoll
        {
            get => _bonusDefenseRoll;
            set
            {
                _bonusDefenseRoll = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalDefenseRoll));
            }
        }

        public int TotalDefenseRoll => BonusDefenseRoll + EquippedWeapon.DefenseRoll;

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
                OnPropertyChanged(nameof(TotalDefenseRoll));
                OnPropertyChanged(nameof(TotalAttackRoll));
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

        public void IncreaseBonusAttackRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusAttackRoll += 1;
        }

        public void IncreaseBonusDefenseRoll()
        {
            ValidateAvailableAttributePoints();

            AvailableAttributePoints -= 1;
            BonusDefenseRoll += AttributeToDefenseRollRatio;
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
            return new BasicAttack
            {
                AttackingCharacter = this,
                DefendingCharacter = defendingCharacter,
                AttackRoll = attackRoll
            };
        }


        public int GetDefenseRoll()
        {
            return _random.Next(1, TotalDefenseRoll);
        }

        public int GetBasicAttackRoll()
        {
            return _random.Next(1, TotalAttackRoll);
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
