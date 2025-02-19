using BlindBoxShop.Shared.Features;
using System.Net;

namespace BlindBoxShop.Shared.ResultModel
{
    public class Result
    {
        public bool IsSuccess => Errors is null;

        public List<ErrorResult>? Errors { get; private set; }

        protected Result(List<ErrorResult> errors)
        {
            Errors = errors;
        }

        protected Result()
        {
        }

        public static Result Success()
            => new();

        public static Result Failure(List<ErrorResult> errors)
                   => new(errors);

        public static Result Failure(ErrorResult errors)
                   => new([errors]);

        public static implicit operator Result(List<ErrorResult> errors)
            => Failure(errors);

        public static implicit operator Result(ErrorResult errors)
            => Failure(errors);
    }

    public class Result<T> : Result
    {
        public T? Value { get; private set; } = default;

        public MetaData? Paging { get; private set; } = null;

        protected Result(T value, MetaData? paging = null) : base()
        {
            Value = value;
            Paging = paging;
        }

        protected Result(List<ErrorResult> errors) : base(errors) { }

        public static Result<T> Success(T value, MetaData? paging = null)
            => new Result<T>(value, paging);

        public static new Result<T> Failure(List<ErrorResult> errors)
           => new Result<T>(errors);

        public static new Result<T> Failure(ErrorResult errors)
           => new Result<T>([errors]);

        public static implicit operator Result<T>(List<ErrorResult> errors)
            => Failure(errors);

        public static implicit operator Result<T>(ErrorResult errors)
            => Failure(errors);

        public static implicit operator Result<T>(T value)
            => Success(value);

        public static implicit operator Result<T>((T value, MetaData metaData) param)
            => Success(param.value, param.metaData);
    }
}
