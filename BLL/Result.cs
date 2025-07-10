namespace BLL
{
    public static class ResultExtensions
    {
        public static T? NullIfEmpty<T>(this T data)
        {
            // Проверяем, является ли объект коллекцией
            if (data is IEnumerable<object> enumerable && !enumerable.Any())
            {
                return default;
            }

            return data;
        }
    }
    public class Result<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static Result<T> Ok(T data) =>
            new() { Success = true, Data = data.NullIfEmpty() };

        public static Result<T> Fail(params string[] errors) =>
            new() { Success = false, Errors = errors.ToList() };

        public bool DataIsNull => Data == null;
        
       
    }
}
