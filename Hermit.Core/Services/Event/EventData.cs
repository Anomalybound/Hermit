namespace Hermit
{
    public class EventData
    {
        public static readonly EventData Empty = new EventData();
        
        public bool Propagation { get; private set; } = true;

        public void StopPropagation()
        {
            Propagation = false;
        }
    }
}