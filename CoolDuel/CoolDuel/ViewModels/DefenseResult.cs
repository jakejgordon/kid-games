namespace CoolDuel.ViewModels
{
    public class DefenseResult
    {
        public int DefenseRoll { get; }
        public string ResultText { get; }
        public DefenseResultType DefenseResultType { get; }
        public DefenseResult(int defenseRoll, DefenseResultType defenseResultType, string resultText)
        {
            DefenseRoll = defenseRoll;
            DefenseResultType = defenseResultType;
            ResultText = resultText;
        }
    }

    public enum DefenseResultType
    {
        AttackBlocked,
        AttackHit
    }
}