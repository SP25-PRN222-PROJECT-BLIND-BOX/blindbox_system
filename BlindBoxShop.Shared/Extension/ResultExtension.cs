﻿using BlindBoxShop.Shared.ResultModel;
using System.Net;

namespace BlindBoxShop.Shared.Extension
{
    public static class ResultExtension
    {
        public static TResultType GetValue<TResultType>(this Result result)
            => (result as Result<TResultType>)!.Value!;

        public static Result<TOut> Then<TIn, TOut>(this Result<TIn> result, Func<TIn, Result<TOut>> nextStep)
        {
            return result.IsSuccess ? nextStep(result.Value!) : Result<TOut>.Failure(result.Errors!);
        }

        public static Result<TOut> ThenIf<TIn, TOut>(this Result<TIn> result, bool condition, Func<TIn, Result<TOut>> nextStep)
        {
            return result.IsSuccess && condition
                ? nextStep(result.Value!)
                : Result<TOut>.Failure(result.Errors!);
        }

        public static Result<TOut> SafeExecute<TIn, TOut>(this Result<TIn> result, Func<TIn, TOut> func, List<ErrorResult> fallbackError)
        {
            try
            {
                return result.IsSuccess ?
                    Result<TOut>.Success(func(result.Value!)) : Result<TOut>.Failure(result.Errors!);
            }
            catch
            {
                return Result<TOut>.Failure(fallbackError);
            }
        }

        public static Result<TIn> Inspect<TIn>(this Result<TIn> result, Action<TIn> action)
        {
            if (result.IsSuccess)
            {
                action(result.Value!);
            }
            return result;
        }
    }
}
