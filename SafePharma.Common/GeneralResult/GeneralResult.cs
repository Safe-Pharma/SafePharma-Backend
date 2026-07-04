using System.Text.Json.Serialization;

namespace SafePharma.Common
{
    public class GeneralResult
    {
        public GeneralResult() { }

        public bool Success { get; set; }
        public string Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, List<Error>> Errors { get; set; }

        public static GeneralResult SuccessResult(string message = "Operation completed successfully.")
        {
            return new GeneralResult { Success = true, Message = message };
        }

        public static GeneralResult NotFound(string message = "Resource not found.")
        {
            return new GeneralResult { Success = false, Message = message, Errors = null };
        }
        public static GeneralResult FailResult(string message = "Operation faild")
        {
            return new GeneralResult { Success = false, Message = message, Errors = null };
        }

        public static GeneralResult FailResult(Dictionary<string, List<Error>> errors, string message = "One or more validation errors occurred.")
        {
            return new GeneralResult { Success = false, Message = message, Errors = errors };
        }

    }
    public class GeneralResult<T> : GeneralResult
    {
        public T? Data { get; set; }
        public static GeneralResult<T> SuccessResult(string message = "Success")
      => new() { Success = true, Message = message, Errors = null };
        public static GeneralResult<T> SuccessResult(T data, string message = "Operation completed successfully.")
        {
            return new GeneralResult<T> { Success = true, Message = message, Data = data };
        }
        public new static GeneralResult<T> NotFound(string message = "Resource not found.")
        {
            return new GeneralResult<T> { Success = false, Message = message, Errors = null };
        }
        public new static GeneralResult<T> FailResult(string message = "Operation faild")
        {
            return new GeneralResult<T> { Success = false, Message = message, Errors = null };
        }
        public new static GeneralResult<T> FailResult(Dictionary<string, List<Error>> errors, string message = "One or more validation errors occurred.")
        {
            return new GeneralResult<T> { Success = false, Message = message, Errors = errors };
        }


    }
}
