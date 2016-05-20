namespace Architecture.Core
{
    public interface IBus
    {
        void Send(IMessage message);
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }
}
