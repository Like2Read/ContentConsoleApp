namespace DataControl.Results
{
    public class UpdateResult : IResult
    {
        public bool Success { get; }

        public string Message { get; }

        public UpdateResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
