using System.Net;
using System.Text.Json.Serialization;

namespace Shared.Itau.Result
{
    public class CommandResult : ICommandResult
    {
        public string Message { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
    public class CommandResult<T> : CommandResult, ICommandResult<T>
    {
        public T Data { get; set; }
    }
}
