namespace PokerHands.Data.Interfaces
{
    public interface IConverter<TPreConversionType, TPostConversionType>
    {
        TPostConversionType Convert(TPreConversionType preConversionType);
    }
}