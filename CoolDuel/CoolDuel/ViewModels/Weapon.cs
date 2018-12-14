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
        public int MinimumAttackRoll { get; }
        public int MaximumAttackRoll { get; }
        public int MinimumDefenseRoll { get; }
        public int MaximumDefenseRoll { get; }
        public int CounterAttackDamage { get; }

        public BitmapImage Character1WeaponImage { get; }

        public BitmapImage Character2WeaponImage { get; }

        public BitmapImage Character1HoldingWeaponImage { get; set; }

        public BitmapImage Character2HoldingWeaponImage { get; set; }


        public Weapon(int id, string name, int attackDamage, int minimumAttackRoll,
            int maximumAttackRoll, int minimumDefenseRoll, int maximumDefenseRoll, int counterAttackDamage,
            BitmapImage character1WeaponCharacter1WeaponImage, 
            BitmapImage character2WeaponCharacter2WeaponImage,
            BitmapImage character1HoldingWeaponImage,
            BitmapImage character2HoldingWeaponImage)
        {
            Id = id;
            Name = name;
            Character1WeaponImage = character1WeaponCharacter1WeaponImage;
            Character2WeaponImage = character2WeaponCharacter2WeaponImage;
            AttackDamage = attackDamage;
            MinimumAttackRoll = minimumAttackRoll;
            MaximumAttackRoll = maximumAttackRoll;
            MinimumDefenseRoll = minimumDefenseRoll;
            MaximumDefenseRoll = maximumDefenseRoll;
            CounterAttackDamage = counterAttackDamage;
            Character1HoldingWeaponImage = character1HoldingWeaponImage;
            Character2HoldingWeaponImage = character2HoldingWeaponImage;
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

        //TODO see the section on "live tree examples" where it says you shouldn't set the source of an image until adding it to a live tree: https://docs.microsoft.com/en-us/windows/uwp/debug-test-perf/optimize-animations-and-media
        public static Weapon Sword { get; } = new Weapon(
            id: 1, 
            name: "Sword",
            attackDamage: 3,
            minimumAttackRoll:1,
            maximumAttackRoll: 7,
            minimumDefenseRoll:1,
            maximumDefenseRoll: 5,
            counterAttackDamage:3,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/sword.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/sword_right_to_left.png")));

        public static Weapon Flail { get; } = new Weapon(
            id: 2,
            name: "Flail",
            attackDamage: 3,
            minimumAttackRoll:1,
            maximumAttackRoll: 8,
            minimumDefenseRoll: 1,
            maximumDefenseRoll: 4,
            counterAttackDamage: 3,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/flail.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/flail_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/flail.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/flail_right_to_left.png")));

        public static Weapon TwoHandedSpear { get; } = new Weapon(
            id: 3,
            name: "Two Handed Spear",
            attackDamage: 3,
            minimumAttackRoll: 1,
            maximumAttackRoll: 10,
            minimumDefenseRoll: 0,
            maximumDefenseRoll: 3,
            counterAttackDamage: 3,
             character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/long_spear.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/long_spear_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_spear.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_spear_right_to_left.png")));

        public static Weapon TwoHandedWarHammer { get; } = new Weapon(
            id: 4,
            name: "Two Handed War Hammer",
            attackDamage: 5,
            minimumAttackRoll: 3,
            maximumAttackRoll: 6,
            minimumDefenseRoll: 0,
            maximumDefenseRoll: 2,
            counterAttackDamage: 4,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/war_hammer.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/war_hammer_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_war_hammer.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_war_hammer_right_to_left.png")));

        public static Weapon OneHandedAxe { get; } = new Weapon(
            id: 5,
            name: "One-Handed Axe",
            attackDamage: 4,
            minimumAttackRoll: 1,
            maximumAttackRoll: 6,
            minimumDefenseRoll: 1,
            maximumDefenseRoll: 4,
            counterAttackDamage: 4,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/one_handed_axe.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/one_handed_axe_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/one_handed_axe.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/one_handed_axe_right_to_left.png")));

        public static Weapon TwoHandedAxe { get; } = new Weapon(
            id: 6,
            name: "Two-Handed Axe",
            attackDamage: 6,
            minimumAttackRoll:2,
            maximumAttackRoll: 6,
            minimumDefenseRoll:0,
            maximumDefenseRoll: 2,
            counterAttackDamage: 4,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/two_handed_axe.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/two_handed_axe_right_to_left.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_axe.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/two_handed_axe_right_to_left.png")));

        public static Weapon ShortSpear { get; } = new Weapon(
            id: 7,
            name: "One Handed Short Spear",
            attackDamage: 1,
            minimumAttackRoll: 2,
            maximumAttackRoll: 9,
            minimumDefenseRoll: 1,
            maximumDefenseRoll: 6,
            counterAttackDamage: 1,
            character1WeaponCharacter1WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/short_spear.png")),
            character2WeaponCharacter2WeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Weapons/short_spear.png")),
            character1HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/one_handed_short_spear.png")),
            character2HoldingWeaponImage: new BitmapImage(new Uri("ms-appx:///Assets/Characters/Knight/one_handed_short_spear_right_to_left.png")));

        public static ObservableCollection<Weapon> AllWeapons { get; } = new ObservableCollection<Weapon>
        {
            Sword,
            Flail,
            TwoHandedSpear,
            TwoHandedWarHammer,
            OneHandedAxe,
            TwoHandedAxe,
            ShortSpear
        };

        public static Weapon GetWeapon(string selectedWeaponName)
        {
            return AllWeapons.Single(weapon => weapon.Name == selectedWeaponName);
        }

        public BitmapImage GetCharacterImage(bool character1)
        {
            if (character1)
            {
                return Character1HoldingWeaponImage;
            }

            return Character2HoldingWeaponImage;
        }
    }
}