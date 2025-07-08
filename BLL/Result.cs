namespace BLL
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Ok(T data) =>
            new() { Success = true, Data = data };

        public static Result<T> Fail(params string[] errors) =>
            new() { Success = false, Errors = errors.ToList() };
    }
}
