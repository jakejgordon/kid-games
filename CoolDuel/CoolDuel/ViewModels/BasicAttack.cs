namespace CoolDuel.ViewModels
{
    public class BasicAttack
    {
        public int AttackRoll { get; set; }
        
        public CharacterViewModel AttackingCharacter { get; set; }
        public CharacterViewModel DefendingCharacter { get; set; }

        public BasicAttack(int attackRoll, CharacterViewModel attackingCharacter, CharacterViewModel defendingCharacter)
        {
            AttackRoll = attackRoll;
            AttackingCharacter = attackingCharacter;
            DefendingCharacter = defendingCharacter;
        }

        public DefenseResult Defend()
        {
            var defenseRoll = DefendingCharacter.GetDefenseRoll();

            DefenseResultType defenseResultType;
            string resultText;

            if (AttackRoll > defenseRoll)
            {
                defenseResultType = DefenseResultType.AttackHit;
                var damageTaken = DefendingCharacter.TakeDamage(AttackingCharacter);
                resultText =
                    $"{DefendingCharacter.Name} rolled a {defenseRoll}, but it wasn't good enough to block a {AttackRoll}. "
                + $"{DefendingCharacter.Name} suffered {damageTaken} damage! They have {DefendingCharacter.HitPoints} left.";
            }else if (AttackRoll == defenseRoll)
            {
                defenseResultType = DefenseResultType.AttackBlocked;
                resultText = $"Both the attacker and defender rolled a {AttackRoll}! Nobody was hurt.";
                AttackingCharacter.ConsumeOneTimeBonusDamage();
            }
            else
            {
                defenseResultType = DefenseResultType.AttackBlocked;
                resultText =
                    $"{DefendingCharacter.Name} rolled a {defenseRoll}, which successfully blocked a {AttackRoll}! No damage was taken.";
                AttackingCharacter.ConsumeOneTimeBonusDamage();
            }

            return new DefenseResult(defenseRoll, defenseResultType, resultText); ;
        }

        public CounterattackResult Counterattack()
        {
            var damageTaken = DefendingCharacter.TakeDamage(AttackingCharacter);
            var bonusDamage = new BonusDamage
            {
                Damage = DefendingCharacter.TotalCounterattackDamage,
                Reason = BonusDamage.BonusDamageReason.Counterattack
            };
            DefendingCharacter.AddNextAttackBonusDamage(bonusDamage);
            return new CounterattackResult
            {
                DamageTaken = damageTaken,
                ResultText = $"{DefendingCharacter.Name} chose to take {damageTaken}. On their next attack, if they hit they will do an extra {DefendingCharacter.TotalCounterattackDamage} damage!"
            };
        }
    }
}