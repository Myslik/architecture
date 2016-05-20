namespace Architecture.Core
{
    public interface IBus
    {
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }
}
