namespace CoolDuel.ViewModels
{
    public class BasicAttack
    {
        public int AttackRoll { get; set; }
        
        public CharacterViewModel AttackingCharacter { get; set; }
        public CharacterViewModel DefendingCharacter { get; set; }

        public DefenseResult Defend()
        {
            var defenseRoll = DefendingCharacter.GetDefenseRoll();
            var defenseResult = new DefenseResult
            {
                DefenseRoll = defenseRoll
            };

            if (AttackRoll > defenseRoll)
            {
                var damageTaken = DefendingCharacter.TakeDamage(AttackingCharacter);
                defenseResult.ResultText =
                    $"{DefendingCharacter.Name} rolled a {defenseRoll}, but it wasn't good enough to block a {AttackRoll}. "
                + $"{DefendingCharacter.Name} suffered {damageTaken} damage! They have {DefendingCharacter.HitPoints} left.";
            }else if (AttackRoll == defenseRoll)
            {
                defenseResult.ResultText = $"Both the attacker and defender rolled a {AttackRoll}! Nobody was hurt.";
                AttackingCharacter.ConsumeOneTimeBonusDamage();
            }
            else
            {
                defenseResult.ResultText =
                    $"{DefendingCharacter.Name} rolled a {defenseRoll}, which successfully blocked a {AttackRoll}! No damage was taken.";
                AttackingCharacter.ConsumeOneTimeBonusDamage();
            }

            return defenseResult;
        }

        public CounterattackResult Counterattack()
        {
            var damageTaken = DefendingCharacter.TakeDamage(AttackingCharacter);
            var bonusDamage = new BonusDamage
            {
                Damage = DefendingCharacter.CounterattackDamage,
                Reason = BonusDamage.BonusDamageReason.Counterattack
            };
            DefendingCharacter.AddNextAttackBonusDamage(bonusDamage);
            return new CounterattackResult
            {
                DamageTaken = damageTaken,
                ResultText = $"{DefendingCharacter.Name} chose to take {damageTaken}. On their next attack, if they hit they will do an extra {DefendingCharacter.CounterattackDamage} damage!"
            };
        }
    }
}