namespace Architecture.Core
{
    public interface IRequest
    {
    }

    public interface IRequest<out TResponse> : IRequest
    {
    }
}
