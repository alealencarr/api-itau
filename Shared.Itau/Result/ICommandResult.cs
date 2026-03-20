using System.Net;

namespace Shared.Itau.Result
{
    public interface ICommandResult
    {
        public string Message { get; set; }

        public HttpStatusCode StatusCode { get; set; }

    }
    public interface ICommandResult<T> : ICommandResult
    {
        public T Data { get; set; }
    }
}
