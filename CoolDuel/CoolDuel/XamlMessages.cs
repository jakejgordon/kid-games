using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoolDuel.ViewModels;

namespace CoolDuel
{
    public static class XamlMessages
    {
        public static string AddCounterattackDamageMessage =
            $"Plus {CharacterViewModel.AttributeToCounterattackDamageRatio} Counterattack Damage";

        public static string AddAttackDamageMessage =
            $"Plus {CharacterViewModel.AttributeToAttackDamageRatio} Attack Damage";

        public static string AddAttackRollMessage =
            $"Plus {CharacterViewModel.AttributeToAttackRollRatio} Attack Roll";

        public static string AddDefenseRollMessage =
            $"Plus {CharacterViewModel.AttributeToDefenseRollRatio} Defense Roll";
        public static string AddHitPointsMessage = $"Plus {CharacterViewModel.AttributeToHitPointRatio} Hit Points";
    }
}
