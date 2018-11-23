﻿using System;
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
        public CharacterViewModel Character1;
        public CharacterViewModel Character2;

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

                
                BattleAsync();
                
            }
        }

        private async void BattleAsync()
        {
            var basicAttack = ViewModel.NextAttack();

            var defenseDialog = new ContentDialog
            {
                Title = $"{basicAttack.AttackingCharacter.Name} is attacking {basicAttack.DefendingCharacter.Name}!",
                Content =
                    $"{basicAttack.AttackingCharacter.Name} rolled a {basicAttack.AttackRoll}! Does {basicAttack.DefendingCharacter.Name} "
                    + $"want to attempt to block the attack with a maximum defense roll of {basicAttack.DefendingCharacter.MaxDefenseRoll}"
                    + $"or do they want to take the hit and counterattack with a bonus of {basicAttack.DefendingCharacter.CounterattackDamage} damage?",
                CloseButtonText = "Counterattack",
                PrimaryButtonText = "Block"
            };

             ContentDialogResult result = await defenseDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var defenseResult = basicAttack.Defend();
                ViewModel.Announcement = defenseResult.ResultText;
            }else if (result == ContentDialogResult.None)
            {
                var counterattackResult = basicAttack.Counterattack();
                ViewModel.Announcement = counterattackResult.ResultText;
            }
            else
            {
                throw new InvalidOperationException("Invalid ContentDialogResult detected");
            }

            ViewModel.Character1Turn = !ViewModel.Character1Turn;
        }

        private void StartBattleAnimation()
        {
            var announcement = "The players are ready. It's battle time!";
            
            ViewModel.Announcement = announcement;
        }
    }
}
