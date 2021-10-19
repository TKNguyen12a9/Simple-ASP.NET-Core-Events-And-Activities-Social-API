namespace Application.Core
{
   public class Result<T> // using generic 
   {
      public bool IsSucccess { get; set; }
      public T Value { get; set; }
      public string Error { get; set; }
      public static Result<T> Success(T value) => new Result<T> { IsSucccess = true, Value = value };
      public static Result<T> Failure(string error) => new Result<T> { IsSucccess = false, Error = error };
   }
}