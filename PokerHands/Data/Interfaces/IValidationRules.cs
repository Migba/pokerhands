namespace PokerHands.Data.Interfaces
{
    public interface IValidationRules<T>
    {
        bool IsValid(T typeToValidate);
    }
}