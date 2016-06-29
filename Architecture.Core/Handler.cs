namespace Architecture.Core
{
    public abstract class Handler
    {
        public IBus Bus { get; internal set; }
    }
}
