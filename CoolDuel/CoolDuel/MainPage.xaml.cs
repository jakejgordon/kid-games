﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

        public ObservableCollection<Weapon> Weapons = Weapon.AllWeapons;

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new DuelViewModel();
            Character1ContentControl.DataContext = ViewModel.Character1;
            Character2ContentControl.DataContext = ViewModel.Character2;
            ViewModel.Character1.AvailableWeapons[0].IsSelected = true;
            ViewModel.Character2.AvailableWeapons[0].IsSelected = true;
        }

        private void AddFiveHitPoints_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter(sender);
            character.IncreaseMaxHitPoints();
            CheckForBattleStart();
        }

        private void AddOneAttackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter(sender);

            character.IncreaseAttackDamage();
            CheckForBattleStart();
        }

        private void AddOneAttackRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter(sender);
            character.IncreaseAttackRoll();
            CheckForBattleStart();
        }

        private void AddOneDefenseRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter(sender);
            character.IncreaseDefenseRoll();
            CheckForBattleStart();
        }

        private void AddTwoCounterattackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter(sender);
            character.IncreaseCounterattackDamageByTwo();
            CheckForBattleStart();
        }

        private static CharacterViewModel GetActiveCharacter(object sender)
        {
            return ((FrameworkElement)(sender as Button).Parent).DataContext as CharacterViewModel;
        }

        private void CheckForBattleStart()
        {
            if (!ViewModel.Character1.HasAttributePoints && !ViewModel.Character2.HasAttributePoints)
            {
                StartBattleAnimation();
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
                    + $"want to attempt to block the attack with a maximum defense roll of {basicAttack.DefendingCharacter.MaxDefenseRoll} "
                    + $" --OR-- do they want to take the hit and {basicAttack.DefendingCharacter.CounterattackDamage} counterattack damage to the next attack if it hits?",
                CloseButtonText = "Counterattack",
                PrimaryButtonText = "Block"
            };

            //--add teh dialog to the grid so it will show in the appropriate place on the page
            if (ViewModel.Character1Turn)
            {
                Character2ContentDialogGrid.Children.Add(defenseDialog);
            }else
            {
                Character1ContentDialogGrid.Children.Add(defenseDialog);
            }

            Grid.SetColumn(defenseDialog, 0);
            Grid.SetRow(defenseDialog, 0);
            
            //--set to in place so the dialog shows centered in the parent grid 
            ContentDialogResult result = await defenseDialog.ShowAsync(ContentDialogPlacement.InPlace);
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

            await CheckForGameEnd(basicAttack);

            SwitchTurns();
        }

        private async Task CheckForGameEnd(BasicAttack basicAttack)
        {
            if (basicAttack.DefendingCharacter.Dead)
            {
                ContentDialog winnerDialog = new ContentDialog
                {
                    Title = "WINNER!",
                    Content =
                        $"{basicAttack.AttackingCharacter.Name} defeated {basicAttack.DefendingCharacter.Name}! Do you want to play again?",
                    CloseButtonText = "Exit Game",
                    PrimaryButtonText = "Play Again"
                };

                ContentDialogResult winnerDialogResult = await winnerDialog.ShowAsync();
                if (winnerDialogResult == ContentDialogResult.Primary)
                {
                    //--TODO THIS DOESN'T WORK
                    InitializeComponent();
                    ViewModel = new DuelViewModel();
                }
                else
                {
                    Application.Current.Exit();
                }
            }
        }

        private void SwitchTurns()
        {
            ViewModel.Character1Turn = !ViewModel.Character1Turn;

            AttackImage.Source = ViewModel.Character1Turn ? ViewModel.Character1.WeaponImage 
                : ViewModel.Character2.WeaponImage;
        }

        private void StartBattleAnimation()
        {
            var announcement = "The players are ready. It's battle time!";
            
            ViewModel.Announcement = announcement;
            AttackButton.IsEnabled = true;
        }

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            BattleAsync();
        }

        private void ChangeWeapon_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void WeaponsComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var weaponsComboBox = sender as ComboBox;
            weaponsComboBox.SelectedIndex = 0;
        }
    }
}
