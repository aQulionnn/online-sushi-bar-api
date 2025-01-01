namespace DAL.SharedKernels
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public Error? Error { get; set; }

        private Result(bool isSucces, T data, Error? error)
        {
            IsSuccess = isSucces;
            Data = data;
            Error = error;
        }

        public static Result<T> Success(T data) => new(true, data, Error.None);

        public static Result<T> Failure(Error error) => new(false, default(T), error);
    }
}
