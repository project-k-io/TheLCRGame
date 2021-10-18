namespace ProjectK.Games.LCR.Models
{
    public interface IPoint<T>
    {
        T X { get; }
        T Y { get; }
    }
}