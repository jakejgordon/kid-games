using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using CoolDuel.ViewModels;

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
            InitializeComponent();
            ViewModel = new DuelViewModel();
            Character1ContentControl.DataContext = ViewModel.Character1;
            Character2ContentControl.DataContext = ViewModel.Character2;
            ViewModel.Character1.AvailableWeapons[0].IsSelected = true;
            ViewModel.Character2.AvailableWeapons[0].IsSelected = true;
        }

        private void AddFiveHitPoints_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseMaxHitPoints();
            CheckForBattleStart();
        }

        private void AddOneAttackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);

            character.IncreaseBonusAttackDamage();
            CheckForBattleStart();
        }

        private void AddOneAttackRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusAttackRoll();
            CheckForBattleStart();
        }

        private void AddOneDefenseRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusDefenseRoll();
            CheckForBattleStart();
        }

        private void AddCounterattackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusCounterattackDamage();
            CheckForBattleStart();
        }

        private static CharacterViewModel GetActiveCharacter<T>(object sender) where T : FrameworkElement
        {
            return ((FrameworkElement)(sender as T).Parent).DataContext as CharacterViewModel;
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
                Title = $"{basicAttack.DefendingCharacter.Name} Is Under Attack!",
                Content =
                    $"{basicAttack.AttackingCharacter.Name} rolled a {basicAttack.AttackRoll}! Does {basicAttack.DefendingCharacter.Name} "
                    + $"want to attempt to block the attack with a maximum defense roll of {basicAttack.DefendingCharacter.TotalDefenseRoll} "
                    + $" --OR-- do they want to take the hit and {basicAttack.DefendingCharacter.TotalCounterattackDamage} counterattack damage to the next attack if it hits?",
                CloseButtonText = "Counterattack",
                PrimaryButtonText = "Block"
            };

            //--show dialog on opposite side of attacker
            var result = await ShowCharacterSpecificDialog(defenseDialog, !ViewModel.Character1Turn);
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

        private async Task<ContentDialogResult> ShowCharacterSpecificDialog(ContentDialog dialog, bool showOnCharacter1Side)
        {
            //--add the dialog to the grid so it will show in the appropriate place on the page
            if (showOnCharacter1Side)
            {
                Character1ContentDialogGrid.Children.Add(dialog);
            }
            else
            {
                Character2ContentDialogGrid.Children.Add(dialog);
            }

            Grid.SetColumn(dialog, 0);
            Grid.SetRow(dialog, 0);

            //--set to in place so the dialog shows centered in the parent grid 
            var result = await dialog.ShowAsync(ContentDialogPlacement.InPlace);
            return result;
        }

        private async Task CheckForGameEnd(BasicAttack basicAttack)
        {
            if (basicAttack.DefendingCharacter.Dead)
            {
                var winnerDialog = new ContentDialog
                {
                    Title = "WINNER!",
                    Content =
                        $"{basicAttack.AttackingCharacter.Name} defeated {basicAttack.DefendingCharacter.Name}! Do you want to play again?",
                    CloseButtonText = "Exit Game",
                    PrimaryButtonText = "Play Again"
                };

                var winnerDialogResult = await winnerDialog.ShowAsync();
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
            if (ViewModel.Character1Turn)
            {
                ViewModel.RoundNumber++;
                ShiftAttackButtonToCharacter1();
                AttackImage.Source = ViewModel.Character1.WeaponImage;
                CheckForSkillUp();
            }
            else
            {
                ShiftAttackImageToCharacter2();
                AttackImage.Source = ViewModel.Character2.WeaponImage;
            }

            void ShiftAttackButtonToCharacter1()
            {
                AttackButton.Margin = new Thickness(0, 50, 250, 0);
            }

            void ShiftAttackImageToCharacter2()
            {
                AttackButton.Margin = new Thickness(250, 50, 0, 0);
            }
        }

        private async void CheckForSkillUp()
        {
            var randomNumberUpTo10 = new Random().Next(1, 100);
            if (randomNumberUpTo10 <= 30 + (ViewModel.RoundNumber*3))
            {
                await PromptForSkillUpChoice(ViewModel.Character1);
                await PromptForSkillUpChoice(ViewModel.Character2);
            }
        }

        private async Task PromptForSkillUpChoice(CharacterViewModel character)
        {
            var skillUpOptions = new SkillUpOptions();

            var characterSkillUpDialog = new ContentDialog
            {
                Title = $"{character.Name} Skill Up!",
                Content =
                    $"{character.Name} is getting more skilled in combat! Pick a bonus which you'll have for the rest of the duel:",
                PrimaryButtonText = skillUpOptions.Option1Text,
                CloseButtonText = skillUpOptions.Option2Text
            };

            var result = await ShowCharacterSpecificDialog(characterSkillUpDialog, character.Character1);
            if (result == ContentDialogResult.Primary)
            {
                skillUpOptions.ApplyOption1Bonus(character);
                ViewModel.Announcement = $"{character.Name} now has {skillUpOptions.Option1Text}";
            }
            else if (result == ContentDialogResult.None)
            {
                skillUpOptions.ApplyOption2Bonus(character);
                ViewModel.Announcement = $"{character.Name} now has {skillUpOptions.Option2Text}";
            }
            else
            {
                throw new InvalidOperationException("Invalid ContentDialogResult detected");
            }
        }

        private void StartBattleAnimation()
        {
            var announcement = "The players are ready. It's battle time!";
            
            ViewModel.Announcement = announcement;
            AttackButton.IsEnabled = true;
            ViewModel.RoundNumber = 1;
            RoundNumber.Visibility = Visibility.Visible;
            Character1ContentControl.IsEnabled = false;
            Character2ContentControl.IsEnabled = false;

        }

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            BattleAsync();
        }

        private void Weapon_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selectedComboBox = sender as ComboBox;
            var selectedWeaponName = (selectedComboBox.SelectedItem as ComboBoxItem).Content.ToString();
            var newWeapon = Weapon.GetWeapon(selectedWeaponName);
            var character = GetActiveCharacter<ComboBox>(sender);
            character.EquippedWeapon = newWeapon;
            //--if it is this player's turn then update the attack weapon image
            if (character.Character1 == ViewModel.Character1Turn
                || !character.Character1 != ViewModel.Character1Turn)
            {
                AttackImage.Source = character.WeaponImage;
            }
        }

        private void WeaponsComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var weaponsComboBox = sender as ComboBox;
            weaponsComboBox.SelectedIndex = 0;
        }
    }
}
