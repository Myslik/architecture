namespace Architecture.Core
{
    public interface IRequest : IMessage
    {
    }

    public interface IRequest<out TResponse> : IRequest
    {
    }
}
