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
            id: 1, 
            name: "Sword",
            attackDamage: 3,
            attackRoll: 7,
            defenseRoll: 5,
            counterAttackDamage:3,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword.png")),
            rightToLeftImage:new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword_right_to_left.png")));

        public static Weapon Mace { get; } = new Weapon(
            id: 2,
            name: "Mace",
            attackDamage: 3,
            attackRoll: 8,
            defenseRoll: 4,
            counterAttackDamage: 3,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/mace.jpg")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/mace_right_to_left.jpg")));

        public static Weapon Spear { get; } = new Weapon(
            id: 3,
            name: "Spear",
            attackDamage: 3,
            attackRoll: 9,
            defenseRoll: 3,
            counterAttackDamage: 3,
             leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/spear.png")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/spear_right_to_left.png")));

        public static Weapon WarHammer { get; } = new Weapon(
            id: 4,
            name: "War Hammer",
            attackDamage: 5,
            attackRoll: 7,
            defenseRoll: 2,
            counterAttackDamage: 4,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/war_hammer.jpg")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/war_hammer_right_to_left.jpg")));

        public static Weapon OneHandedAxe { get; } = new Weapon(
            id: 5,
            name: "One-Handed Axe",
            attackDamage: 4,
            attackRoll: 6,
            defenseRoll: 4,
            counterAttackDamage: 4,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/one_handed_axe.png")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/one_handed_axe_right_to_left.png")));

        public static Weapon TwoHandedAxe { get; } = new Weapon(
            id: 6,
            name: "Two-Handed Axe",
            attackDamage: 6,
            attackRoll: 5,
            defenseRoll: 3,
            counterAttackDamage: 4,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/two_handed_axe.jpg")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/two_handed_axe_right_to_left.jpg")));

        public static Weapon BoStaff { get; } = new Weapon(
            id: 7,
            name: "Bo Staff",
            attackDamage: 2,
            attackRoll: 9,
            defenseRoll: 6,
            counterAttackDamage: 1,
            leftToRightImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/bo_staff.png")),
            rightToLeftImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/bo_staff_right_to_left.png")));

        public static ObservableCollection<Weapon> AllWeapons { get; } = new ObservableCollection<Weapon>
        {
            Sword,
            Mace,
            Spear,
            WarHammer,
            OneHandedAxe,
            TwoHandedAxe,
            BoStaff
        };

        public static Weapon GetWeapon(string selectedWeaponName)
        {
            return AllWeapons.Single(weapon => weapon.Name == selectedWeaponName);
        }
    }
}