using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using FocusState = Windows.UI.Xaml.FocusState;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolDuel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Character1Name.IsEnabled = true;
            Character1Name.Focus(FocusState.Programmatic);
        }

        private void Character1Name_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Character1Name.IsEnabled = false;
        }

        private void AddHitPoints_Click(object sender, RoutedEventArgs e)
        {
            var availablePoints = int.Parse(Character1AvailablePoints.Text);
            var hitPoints = int.Parse(Character1HitPoints.Text);
            Character1HitPoints.Text = (hitPoints + 5).ToString();
            Character1AvailablePoints.Text = (availablePoints - 1).ToString();

            if (availablePoints == 0)
            {
                Character1HitPoints.Text = (hitPoints + 5).ToString();
            }
        }
    }
}
