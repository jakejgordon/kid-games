using System;
using System.Collections.Generic;
using CoolDuel.ViewModels;

namespace CoolDuel
{
    public class SkillUpOptions
    {
        private static readonly Random Random = new Random();

        private static readonly SkillUpOption IncreaseAttackDamage = new SkillUpOption
        {
            DisplayText = XamlMessages.AddAttackDamageMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseAttackDamage
        };

        private static readonly SkillUpOption IncreaseMinimumAttackRoll = new SkillUpOption
        {
            DisplayText = XamlMessages.AddMinimumAttackRollMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseMinimumAttackRoll
        };

        private static readonly SkillUpOption IncreaseMaximumAttackRoll = new SkillUpOption
        {
            DisplayText = XamlMessages.AddMaximumAttackRollMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseMaximumAttackRoll
        };

        private static readonly SkillUpOption IncreaseMinimumDefenseRoll = new SkillUpOption
        {
            DisplayText = XamlMessages.AddMinimumDefenseRollMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseMinimumDefenseRoll
        };

        private static readonly SkillUpOption IncreaseMaximumDefenseRoll = new SkillUpOption
        {
            DisplayText = XamlMessages.AddMaximumDefenseRollMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseMaximumAttackRoll
        };

        private static readonly SkillUpOption IncreaseHitPoints = new SkillUpOption
        {
            DisplayText = XamlMessages.AddHitPointsMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseHitPoints
        };

        private static readonly SkillUpOption IncreaseCounterAttackDamage = new SkillUpOption
        {
            DisplayText = XamlMessages.AddCounterattackDamageMessage,
            SkillUpKey = SkillUpOptionEnum.IncreaseCounterattackDamage
        };

        private static readonly List<SkillUpOption> EarlyPhaseSkillUpOptions = new List<SkillUpOption>
        {
            IncreaseAttackDamage,
            IncreaseHitPoints,
            IncreaseMaximumAttackRoll,
            IncreaseMaximumDefenseRoll,
            IncreaseCounterAttackDamage
        };

        private static readonly List<SkillUpOption> LaterPhaseSkillUpOptions = new List<SkillUpOption>
        {
            IncreaseAttackDamage,
            IncreaseHitPoints,
            IncreaseMaximumAttackRoll,
            IncreaseMaximumDefenseRoll,
            IncreaseCounterAttackDamage,
            IncreaseMinimumDefenseRoll,
            IncreaseMinimumAttackRoll
        };

        //private Dictionary<string, >
        public SkillUpOptions(int roundNumber)
        {
            var optionSet = roundNumber < 15 ? EarlyPhaseSkillUpOptions : LaterPhaseSkillUpOptions;
            int option1Index = Random.Next(0, optionSet.Count - 1);
            var option1 = optionSet[option1Index];
            Option1Text = option1.DisplayText;
            Option1SkillUp = option1.SkillUpKey;

            //--this will guarantee the next skill is anything other than the current one
            int incrementValue = Random.Next(1, optionSet.Count - 1);
            //--modulus so we get a valid index (that isn't the same as the previous)
            var option2Index = (option1Index + incrementValue) % optionSet.Count;
            var option2 = optionSet[option2Index];
            Option2Text = option2.DisplayText;
            Option2SkillUp = option2.SkillUpKey;
        }

        public string Option1Text { get; set; }
        public SkillUpOptionEnum Option1SkillUp { get; set; }
        public string Option2Text { get; set; }
        public SkillUpOptionEnum Option2SkillUp { get; set; }


        public void ApplyOption1Bonus(CharacterViewModel character)
        {
            ApplySkillUp(character, Option1SkillUp);
        }

        public void ApplyOption2Bonus(CharacterViewModel character)
        {
            ApplySkillUp(character, Option2SkillUp);
        }

        private void ApplySkillUp(CharacterViewModel character, SkillUpOptionEnum skillUpOption)
        {
            switch (skillUpOption)
            {
                case SkillUpOptionEnum.IncreaseAttackDamage:
                    character.BonusAttackDamage += CharacterViewModel.AttributeToAttackDamageRatio;
                    break;
                case SkillUpOptionEnum.IncreaseMinimumAttackRoll:
                    character.BonusMinimumAttackRoll += CharacterViewModel.AttributeToMinimumAttackRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseMaximumAttackRoll:
                    character.BonusMaximumAttackRoll += CharacterViewModel.AttributeToMaximumAttackRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseMinimumDefenseRoll:
                    character.BonusMinimumDefenseRoll += CharacterViewModel.AttributeToMinimumDefenseRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseMaximumDefenseRoll:
                    character.BonusMaximumDefenseRoll += CharacterViewModel.AttributeToMaximumDefenseRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseCounterattackDamage:
                    character.BonusCounterattackDamage += CharacterViewModel.AttributeToCounterattackDamageRatio;
                    break;
                case SkillUpOptionEnum.IncreaseHitPoints:
                    character.HitPoints += CharacterViewModel.AttributeToHitPointRatio;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid bonus '{Option1SkillUp}' could not be applied.");
            }
        }
    }

    public class SkillUpOption
    {
        public SkillUpOptionEnum SkillUpKey { get; set; }
        public string DisplayText { get; set; }
    }

    public enum SkillUpOptionEnum
    {
        IncreaseHitPoints,
        IncreaseAttackDamage,
        IncreaseMinimumAttackRoll,
        IncreaseMaximumAttackRoll,
        IncreaseMinimumDefenseRoll,
        IncreaseMaximumDefenseRoll,
        IncreaseCounterattackDamage
    }
}