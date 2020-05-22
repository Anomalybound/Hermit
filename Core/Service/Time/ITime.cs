namespace Hermit.Service.Time
{
    public interface ITime
    {
        float LogicTime { get; }
        
        float RealTime { get; }
    }
}