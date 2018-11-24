using System;
using Windows.UI.Xaml.Media.Imaging;

namespace CoolDuel.ViewModels
{
    public class Weapon
    {
        public static Weapon Sword { get; } = new Weapon
        {
            Name = "Sword",
            ImageSourceFlowDirectionLeftToRight = new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword.png")),
            ImageSourceFlowDirectionRightToLeft = new BitmapImage(new Uri("ms-appx:///Assets/Weapons/sword_right_to_left.png"))
        };

        public BitmapImage ImageSourceFlowDirectionLeftToRight { get; set; }

        public string Name { get; set; }
        public BitmapImage ImageSourceFlowDirectionRightToLeft { get; set; }
    }
}