namespace Hermit
{
    public interface ITime
    {
        float LogicTime { get; }
        
        float RealTime { get; }
    }
}