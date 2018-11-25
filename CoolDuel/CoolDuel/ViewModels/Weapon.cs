using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Media.Imaging;

namespace CoolDuel.ViewModels
{
    public class Weapon
    {
        public int Id { get; }
        public string Name { get; }
        public int AttackDamage { get; }
        public int AttackRoll { get; }
        public int DefenseRoll { get; }
        public int CounterAttackDamage { get; }


        public BitmapImage ImageSourceFlowDirectionLeftToRight { get; }

        public BitmapImage ImageSourceFlowDirectionRightToLeft { get; }

        public Weapon(int id, string name, int attackDamage, 
            int attackRoll, int defenseRoll, int counterAttackDamage,
            BitmapImage leftToRightImage, BitmapImage rightToLeftImage)
        {
            Id = id;
            Name = name;
            ImageSourceFlowDirectionLeftToRight = leftToRightImage;
            ImageSourceFlowDirectionRightToLeft = rightToLeftImage;
            AttackDamage = attackDamage;
            AttackRoll = attackRoll;
            DefenseRoll = defenseRoll;
            CounterAttackDamage = counterAttackDamage;
        }

        protected bool Equals(Weapon other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Weapon)obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static Weapon Sword { get; } = new Weapon(
            1, 
            "Sword",
            2,
            6,
            5,
            3,
            new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword.png")),
            new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword_right_to_left.png")));

        public static Weapon Mace { get; } = new Weapon(
            2,
            "Mace",
            2,
            7,
            4,
            4,
            new BitmapImage(new Uri("ms-appx:///Assets/Weapons/mace.jpg")),
            new BitmapImage(new Uri("ms-appx:///Assets/Weapons/mace_right_to_left.jpg")));


        public static ObservableCollection<Weapon> AllWeapons { get; } = new ObservableCollection<Weapon>
        {
            Sword,
            Mace
        };

        public static Weapon GetWeapon(string selectedWeaponName)
        {
            return AllWeapons.Single(weapon => weapon.Name == selectedWeaponName);
        }
    }
}