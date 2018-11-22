using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CoolDuel.ViewModels;
using FocusState = Windows.UI.Xaml.FocusState;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CoolDuel
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public DuelViewModel ViewModel;

        public MainPage()
        {
            this.InitializeComponent();
            ViewModel = new DuelViewModel();
            Character1Name.IsEnabled = true;
            Character1Name.Focus(FocusState.Programmatic);
            Character1Grid.DataContext = ViewModel.Character1;
            Character2Grid.DataContext = ViewModel.Character2;
        }

        private void Character1AddHitPoints_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Character1.IncreaseMaxHitPoints();
            CheckForBattleStart();
        }

        private void Character1AttackDamageButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Character1.IncreaseAttackDamage();
            CheckForBattleStart();
        }

        private void Character2AddHitPoints_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Character2.IncreaseMaxHitPoints();
            CheckForBattleStart();
        }

        private void Character2AttackDamageButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Character2.IncreaseAttackDamage();
            CheckForBattleStart();
        }

        private void CheckForBattleStart()
        {
            if (!ViewModel.Character1.HasAttributePoints && !ViewModel.Character2.HasAttributePoints)
            {
                StartBattleAnimation();
            }
        }

        private void StartBattleAnimation()
        {
            var announcement = "The players are ready. It's battle time!";
            
            ViewModel.Announcement = announcement;
            //StartBattleText.Visibility = Visibility.Visible;
            //ViewModel.Announcement = string.Format(announcement, 2);
            //Task.Delay(1000).Wait();
            //ViewModel.Announcement = string.Format(announcement, 1);
            //Task.Delay(1000).Wait();
            //ViewModel.Announcement = "FIGHT!";
            //StartBattleText.FontSize = 40;
            //Task.Delay(500).Wait();
            //StartBattleText.Visibility = Visibility.Collapsed;
        }
    }
}
