using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using CoolDuel.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Animations;
using TextBox = Windows.UI.Xaml.Controls.TextBox;

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

        private void IncreaseHitPoints_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseMaxHitPoints();
            CheckForBattleStart();
        }

        private void AddAttackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);

            character.IncreaseBonusAttackDamage();
            CheckForBattleStart();
        }

        private void IncreaseMaxAttackRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusMaxAttackRoll();
            CheckForBattleStart();
        }

        private void IncreaseMaximumDefenseRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusMaximumDefenseRoll();
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

        private static CharacterViewModel GetActiveCharacter<T>(T sender) where T : FrameworkElement
        {
            var parent = sender.Parent as FrameworkElement;
            return parent.DataContext as CharacterViewModel;
        }

        private async void CheckForBattleStart()
        {
            if (!ViewModel.Character1.HasAttributePoints && !ViewModel.Character2.HasAttributePoints)
            {
                await StartBattleAnimation();
            }
        }

        private async void AttackOtherPlayer()
        {
            var basicAttack = ViewModel.NextAttack();

            var defenseDialog = new ContentDialog
            {
                Title = $"{basicAttack.DefendingCharacter.Name} Is Under Attack!",
                Content =
                    $"{basicAttack.AttackingCharacter.Name} rolled a {basicAttack.AttackRoll}! Does {basicAttack.DefendingCharacter.Name} "
                    + $"want to attempt to block the attack with a maximum defense roll of {basicAttack.DefendingCharacter.TotalMaximumDefenseRoll} "
                    + $" --OR-- do they want to take the hit and {basicAttack.DefendingCharacter.TotalCounterattackDamage} counterattack damage to the next attack if it hits?",
                CloseButtonText = "Counterattack",
                PrimaryButtonText = "Block"
            };

            //--show dialog on opposite side of attacker
            var result = await ShowCharacterSpecificDialog(defenseDialog, !ViewModel.Character1Turn);
            bool isCounterattack = false;
            if (result == ContentDialogResult.Primary)
            {
                var defenseResult = basicAttack.Defend();
                string resultTitle = string.Empty;
                if (defenseResult.DefenseResultType == DefenseResultType.AttackBlocked)
                {
                    resultTitle = "Attack Blocked!";
                }
                else if (defenseResult.DefenseResultType == DefenseResultType.AttackHit)
                {
                    resultTitle = "Hit!";
                }
                await ChangeAnnouncement(resultTitle, $"{defenseResult.ResultText}");
            }else if (result == ContentDialogResult.None)
            {
                isCounterattack = true;
                var counterattackResult = basicAttack.Counterattack();
                await ChangeAnnouncement("Counterattack!", $"{counterattackResult.ResultText}");
            }
            else
            {
                throw new InvalidOperationException("Invalid ContentDialogResult detected");
            }

            await CheckForGameEnd(basicAttack);

            await SwitchTurns();

            if (isCounterattack)
            {
                AttackOtherPlayer();
            }
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

        private async Task SwitchTurns()
        {
            ViewModel.Character1Turn = !ViewModel.Character1Turn;
            if (ViewModel.Character1Turn)
            {
                ViewModel.RoundNumber++;
                ShiftAttackButtonsToCharacter1();
                AttackImage.Source = ViewModel.Character1.WeaponImage;
                await CheckForSkillUp();
            }
            else
            {
                ShiftAttackImagesToCharacter2();
                AttackImage.Source = ViewModel.Character2.WeaponImage;
            }
        }

        private readonly Thickness _actionButtonMarginCharacter1 = new Thickness(0, 50, 250, 0);
        private readonly Thickness _actionButtonMarginCharacter2 = new Thickness(250, 50, 0, 0);


        private void ShiftAttackButtonsToCharacter1()
        {
            AttackButton.Margin = _actionButtonMarginCharacter1;
            PrayButton.Margin = new Thickness(0, 200, 250, 0);
        }

        private void ShiftAttackImagesToCharacter2()
        {
            AttackButton.Margin = _actionButtonMarginCharacter2;
            PrayButton.Margin = new Thickness(250, 200, 0, 0);
        }

        private async Task<bool> CheckForSkillUp()
        {
            var randomNumberUpTo10 = new Random().Next(1, 100);
            if (randomNumberUpTo10 <= 30 + (ViewModel.RoundNumber*2))
            {
                await PromptForSkillUpChoice(ViewModel.Character1);
                await PromptForSkillUpChoice(ViewModel.Character2);
                return true;
            }

            return false;
        }

        private async Task PromptForSkillUpChoice(CharacterViewModel character, string dialogTitle = null)
        {
            var skillUpOptions = new SkillUpOptions(ViewModel.RoundNumber);

            if (dialogTitle == null)
            {
                dialogTitle = $"{character.Name} Skill Up!";
            }

            var characterSkillUpDialog = new ContentDialog
            {
                Title = dialogTitle,
                Content =
                    $"{character.Name} is getting more skilled in combat! Pick a bonus which you'll have for the rest of the duel:",
                PrimaryButtonText = skillUpOptions.Option1Text,
                CloseButtonText = skillUpOptions.Option2Text
            };

            var result = await ShowCharacterSpecificDialog(characterSkillUpDialog, character.Character1);
            string announcementBody;

            if (result == ContentDialogResult.Primary)
            {
                skillUpOptions.ApplyOption1Bonus(character);
                announcementBody = $"{character.Name} now has {skillUpOptions.Option1Text}";
            }
            else if (result == ContentDialogResult.None)
            {
                skillUpOptions.ApplyOption2Bonus(character);
                announcementBody = $"{character.Name} now has {skillUpOptions.Option2Text}";
            }
            else
            {
                throw new InvalidOperationException("Invalid ContentDialogResult detected");
            }

            await ChangeAnnouncement($"{character.Name} Skill Up!", announcementBody);
        }

        private async Task StartBattleAnimation()
        {
            var announcement = $"It's battle time! Your turn first, {ViewModel.Character1.Name}!";

            await ChangeAnnouncement("Prepare For Battle!", announcement);


            AttackButton.IsEnabled = true;
            AttackButton.Visibility = Visibility.Visible;
            ShiftAttackButtonsToCharacter1();

            PrayButton.IsEnabled = true;
            PrayButton.Visibility = Visibility.Visible;

            ViewModel.RoundNumber = 1;
            RoundNumber.Visibility = Visibility.Visible;
            Character1ContentControl.IsEnabled = false;
            Character2ContentControl.IsEnabled = false;

            var character1NameTextBox = FindElementByName<TextBox>(Character1ContentControl, "CharacterNameTextBox");
            character1NameTextBox.Visibility = Visibility.Collapsed;

            var character2NameTextBox = FindElementByName<TextBox>(Character2ContentControl, "CharacterNameTextBox");
            character2NameTextBox.Visibility = Visibility.Collapsed;

            var character1Name = FindElementByName<TextBlock>(Character1ContentControl, "CharacterName");
            character1Name.Visibility = Visibility.Visible;

            var character2Name = FindElementByName<TextBlock>(Character2ContentControl, "CharacterName");
            character2Name.Visibility = Visibility.Visible;
        }

        public T FindElementByName<T>(FrameworkElement element, string childName) where T : FrameworkElement
        {
            T childElement = null;
            var numberOfChildren = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < numberOfChildren; i++)
            {
                var child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(childName))
                {
                    childElement = (T) child;
                    break;
                }

                childElement = FindElementByName<T>(child, childName);

                if (childElement != null)
                    break;
            }

            return childElement;
        }

        private void Attack_Click(object sender, RoutedEventArgs e)
        {
            AttackOtherPlayer();
        }

        private async void PrayButton_OnClick(object sender, RoutedEventArgs e)
        {
            var activeCharacter = ViewModel.Character1Turn ? ViewModel.Character1 : ViewModel.Character2;

            var randomNumberUpTo10 = new Random().Next(1, 100);
            if (randomNumberUpTo10 <= 40)
            {
                var dialogTitle = $"{activeCharacter.Name} has been blessed!";
                await PromptForSkillUpChoice(activeCharacter, dialogTitle);
            }
            else
            {
                await ChangeAnnouncement("Unanswered Prayers", $"No blessing was bestowed upon {activeCharacter.Name}.");
            }

            await SwitchTurns();
        }

        private async Task ChangeAnnouncement(string announcementHeader, string announcementBody)
        {
            await AnnouncementHeader.Fade().StartAsync();
            
            AnnouncementHeader.Text = announcementHeader;
            AnnouncementBody.Text = announcementBody;

            await AnnouncementHeader.Fade(value: 1F, easingMode: EasingMode.EaseIn).StartAsync();
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

            character.CharacterImage = newWeapon.GetCharacterImage(character.Character1);
        }

        private void WeaponsComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var weaponsComboBox = sender as ComboBox;
            weaponsComboBox.SelectedIndex = 0;
        }

        private void HealthBar_OnLoaded(object sender, RoutedEventArgs e)
        {
            var activeCharacter = GetActiveCharacter<StackPanel>(sender);
            var stackPanel = sender as StackPanel;
            stackPanel.SetValue(Grid.ColumnProperty, activeCharacter.Character1 ? 0 : 3);
        }

        private void FirstCharacterGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var activeCharacter = GetActiveCharacter(grid);
            grid.HorizontalAlignment = activeCharacter.Character1 ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }
    }
}
