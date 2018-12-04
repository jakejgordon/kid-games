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

        public static string AddMinimumAttackRollMessage =
            $"Plus {CharacterViewModel.AttributeToMaximumAttackRollRatio} Minimum Attack Roll";

        public static string AddMaximumAttackRollMessage =
            $"Plus {CharacterViewModel.AttributeToMaximumAttackRollRatio} Maximum Attack Roll";

        public static string AddMinimumDefenseRollMessage = $"Plus {CharacterViewModel.AttributeToMinimumDefenseRollRatio} Minimum Defense Roll";


        public static string AddDefenseRollMessage =
            $"Plus {CharacterViewModel.AttributeToMaximumDefenseRollRatio} Maximum Defense Roll";

        public static string AddHitPointsMessage = $"Plus {CharacterViewModel.AttributeToHitPointRatio} Hit Points";
    }
}
