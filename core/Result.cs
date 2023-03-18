namespace Glow.Core
{
    // Should be removed
    public class Result<T>
    {
        public bool IsSuccessful { get; set; }
        public T Payload { get; set; }
        public string ErrorMessage { get; set; }

        public static Result<T> Success(T payload)
        {
            return new() { IsSuccessful = true, Payload = payload };
        }

        public static Result<T> Fail(string errorMessage)
        {
            return new() { IsSuccessful = false, ErrorMessage = errorMessage };
        }
    }
}
