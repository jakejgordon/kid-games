using System;
using System.Collections.Generic;
using CoolDuel.ViewModels;

namespace CoolDuel
{
    public class SkillUpOptions
    {
        private static readonly Random Random = new Random();

        private readonly List<SkillUpOption> _availableOptions = new List<SkillUpOption>
        {
            new SkillUpOption
            {
                DisplayText = $"Plus {CharacterViewModel.AttributeToAttackDamageRatio} Attack Damage",
                SkillUpKey = SkillUpOptionEnum.IncreaseAttackDamage
            },
            new SkillUpOption
            {
                DisplayText = $"Plus {CharacterViewModel.AttributeToAttackRollRatio}  Attack Roll",
                SkillUpKey = SkillUpOptionEnum.IncreaseAttackRoll
            },
            new SkillUpOption
            {
                DisplayText = $"Plus {CharacterViewModel.AttributeToDefenseRollRatio} Defense Roll",
                SkillUpKey = SkillUpOptionEnum.IncreaseDefenseRoll
            },
            new SkillUpOption
            {
                DisplayText = $"Plus {CharacterViewModel.AttributeToCounterattackDamageRatio} Counterattack Damage",
                SkillUpKey = SkillUpOptionEnum.IncreaseCounterattackDamage
            },
            new SkillUpOption
            {
                DisplayText = $"Plus {CharacterViewModel.AttributeToHitPointRatio} Hit Points",
                SkillUpKey = SkillUpOptionEnum.IncreaseHealthPoints
            }
        };

        //private Dictionary<string, >
        public SkillUpOptions()
        {
            int option1Index = Random.Next(1, _availableOptions.Count);
            var option1 = _availableOptions[option1Index];
            Option1Text = option1.DisplayText;
            Option1SkillUp = option1.SkillUpKey;
            //--get the next sequential value, but roll around if going above the max index
            var option2Index = ++option1Index % _availableOptions.Count;
            var option2 = _availableOptions[option2Index];
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
                case SkillUpOptionEnum.IncreaseAttackRoll:
                    character.BonusAttackRoll += CharacterViewModel.AttributeToAttackRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseDefenseRoll:
                    character.BonusDefenseRoll += CharacterViewModel.AttributeToDefenseRollRatio;
                    break;
                case SkillUpOptionEnum.IncreaseCounterattackDamage:
                    character.BonusCounterattackDamage += CharacterViewModel.AttributeToCounterattackDamageRatio;
                    break;
                case SkillUpOptionEnum.IncreaseHealthPoints:
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
        IncreaseHealthPoints,
        IncreaseAttackDamage,
        IncreaseAttackRoll,
        IncreaseDefenseRoll,
        IncreaseCounterattackDamage
    }
}