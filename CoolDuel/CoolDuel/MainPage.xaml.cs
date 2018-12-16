using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
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

        public Grid Character1ImageGrid { get; set; }
        public Grid Character2ImageGrid { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new DuelViewModel();
            Character1ContentControl.DataContext = ViewModel.Character1;
            Character2ContentControl.DataContext = ViewModel.Character2;
            ViewModel.Character1.AvailableWeapons[0].IsSelected = true;
            ViewModel.Character2.AvailableWeapons[0].IsSelected = true;
        }

        private async void IncreaseHitPoints_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseMaxHitPoints();
            await CheckForBattleStart();
        }

        private async void AddAttackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);

            character.IncreaseBonusAttackDamage();
            await CheckForBattleStart();
        }

        private async void IncreaseMaxAttackRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusMaxAttackRoll();
            await CheckForBattleStart();
        }

        private async void IncreaseMaximumDefenseRoll_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusMaximumDefenseRoll();
            await CheckForBattleStart();
        }

        private async void AddCounterattackDamage_Click(object sender, RoutedEventArgs e)
        {
            var character = GetActiveCharacter<Button>(sender);
            character.IncreaseBonusCounterattackDamage();
            await CheckForBattleStart();
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

        private async Task CheckForBattleStart()
        {
            if (!ViewModel.Character1.HasAttributePoints && !ViewModel.Character2.HasAttributePoints)
            {
                await StartBattleAnimation();
            }
        }

        private async Task AttackOtherPlayer()
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
                string resultTitle;
                AnnouncementType announcementType;
                if (defenseResult.DefenseResultType == DefenseResultType.AttackBlocked)
                {
                    resultTitle = "Attack Blocked!";
                    announcementType = AnnouncementType.AttackBlocked;
                }else
                {
                    resultTitle = "Hit!";
                    announcementType = AnnouncementType.AttackHit;
                }
                await ChangeAnnouncement(resultTitle, $"{defenseResult.ResultText}", announcementType);
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
               await AttackOtherPlayer();
            }
        }

        private async Task<ContentDialogResult> ShowCharacterSpecificDialog(ContentDialog dialog, bool showOnCharacter1Side)
        {
            //--add the dialog to the grid so it will show in the appropriate place on the page
            if (showOnCharacter1Side)
            {
                Character1ContentDialogGrid.Children.Add(dialog);
                HighlightCharacter1();
            }
            else
            {
                Character2ContentDialogGrid.Children.Add(dialog);
                HighlightCharacter2();
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
                    var result =
                        await CoreApplication.RequestRestartAsync(string.Empty);
                    if (result == AppRestartFailureReason.NotInForeground ||
                        result == AppRestartFailureReason.RestartPending ||
                        result == AppRestartFailureReason.Other)
                    {
                        Debug.WriteLine("RequestRestartAsync failed: {0}", result);
                    }
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
                HighlightCharacter1();
                AttackImage.Source = ViewModel.Character1.WeaponImage;
                await CheckForSkillUp();
            }
            else
            {
                HighlightCharacter2();
                ShiftAttackImagesToCharacter2();
                AttackImage.Source = ViewModel.Character2.WeaponImage;
            }
        }


        private void HighlightCharacter1()
        {
            Character1ImageGrid.BorderBrush = new SolidColorBrush(Colors.Green);
            Character2ImageGrid.BorderBrush = null;
        }

        private void HighlightCharacter2()
        {
            Character2ImageGrid.BorderBrush = new SolidColorBrush(Colors.Green);
            Character1ImageGrid.BorderBrush = null;
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

                //--skill ups only happen on Character 1's turn, so switch the highlighting back
                HighlightCharacter1();
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

            await ChangeAnnouncement($"{character.Name} Skill Up!", announcementBody, AnnouncementType.SkillUp);
        }

        private async Task StartBattleAnimation()
        {
            var announcement = $"It's battle time! Your turn first, {ViewModel.Character1.Name}!";

            await ChangeAnnouncement("Prepare For Battle!", announcement);

            //--default the first player to active once the game starts
            Character1ImageGrid.BorderBrush = new SolidColorBrush(Colors.Green);

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

        private async void Attack_Click(object sender, RoutedEventArgs e)
        {
            await AttackOtherPlayer();
        }

        private async void PrayButton_OnClick(object sender, RoutedEventArgs e)
        {
            var activeCharacter = ViewModel.Character1Turn ? ViewModel.Character1 : ViewModel.Character2;

            var randomNumberUpTo10 = new Random().Next(1, 100);
            if (randomNumberUpTo10 <= 40)
            {
                var dialogTitle = $"{activeCharacter.Name} has been blessed!";
                await ChangeAnnouncement("Blessing Bestowed!",
                    $"{activeCharacter.Name} has had a great blessing bestowed upon them!",
                    AnnouncementType.PrayerSuccess);
                await PromptForSkillUpChoice(activeCharacter, dialogTitle);
            }
            else
            {
                await ChangeAnnouncement("Unanswered Prayers", $"No blessing was bestowed upon {activeCharacter.Name}.", AnnouncementType.PrayerFailure);
            }

            await SwitchTurns();
        }

        private async Task ChangeAnnouncement(string announcementHeader, string announcementBody, AnnouncementType announcementType = AnnouncementType.None)
        {
            var task1 = AnnouncementHeader.Fade(duration: 250).StartAsync();
            var task2 = AnnouncementBody.Fade(duration: 250).StartAsync();
            var task3 = AnnouncementImage.Fade(duration: 250).StartAsync();

            await Task.WhenAll(task1, task2, task3);

            AnnouncementHeader.Text = announcementHeader;
            AnnouncementBody.Text = announcementBody;

            switch (announcementType)
            {
                case AnnouncementType.AttackHit:
                    ShowAnnouncementImage(AnnouncementImages.AttackHit);
                    break;
                case AnnouncementType.AttackBlocked:
                    ShowAnnouncementImage(AnnouncementImages.AttackBlocked);
                    break;
                case AnnouncementType.PrayerSuccess:
                    ShowAnnouncementImage(AnnouncementImages.Blessing);
                    break;
                case AnnouncementType.PrayerFailure:
                    ShowAnnouncementImage(AnnouncementImages.NoBlessing);
                    break;
                case AnnouncementType.SkillUp:
                    ShowAnnouncementImage(AnnouncementImages.SkillUp);
                    break;
                default:
                    AnnouncementImage.Source = null;
                    AnnouncementImage.Visibility = Visibility.Collapsed;
                    AnnouncementHeader.SetValue(Grid.ColumnSpanProperty, 2);
                    AnnouncementHeader.SetValue(Grid.ColumnProperty, 0);
                    AnnouncementBody.SetValue(Grid.ColumnSpanProperty, 2);
                    AnnouncementBody.SetValue(Grid.ColumnProperty, 0);
                    break;

            }

            task1 = AnnouncementHeader.Fade(value: 1F, easingMode: EasingMode.EaseIn).StartAsync();
            task2 = AnnouncementBody.Fade(value: 1F, easingMode: EasingMode.EaseIn).StartAsync();
            task3 = AnnouncementImage.Fade(value: 1F, easingMode: EasingMode.EaseIn).StartAsync();
            await Task.WhenAll(task1, task2, task3);
        }

        private void ShowAnnouncementImage(ImageSource imageToShow)
        {
            AnnouncementImage.Source = imageToShow;
            AnnouncementImage.Visibility = Visibility.Visible;
            AnnouncementHeader.SetValue(Grid.ColumnSpanProperty, 1);
            AnnouncementHeader.SetValue(Grid.ColumnProperty, 1);
            AnnouncementBody.SetValue(Grid.ColumnSpanProperty, 1);
            AnnouncementBody.SetValue(Grid.ColumnProperty, 1);
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

        private void CharacterGrid_OnLoaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as Grid;
            var activeCharacter = GetActiveCharacter(grid);
            if (activeCharacter.Character1)
            {
                grid.HorizontalAlignment = HorizontalAlignment.Left;
                Character1ImageGrid = grid;
            }
            else
            {
                grid.HorizontalAlignment = HorizontalAlignment.Right;
                Character2ImageGrid = grid;
            }
        }

    }

    public class AnnouncementImages
    {
        public static ImageSource AttackHit { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Actions/hit.png"));
        public static ImageSource AttackBlocked { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Actions/blocked.png"));
        public static ImageSource Blessing { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Actions/blessing.png"));
        public static ImageSource NoBlessing { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Actions/no_blessing.png"));
        public static ImageSource SkillUp { get; } = new BitmapImage(new Uri("ms-appx:///Assets/Actions/skill_up.png"));
    }
}
