using BlindBoxShop.Shared.ResultModel;

namespace BlindBoxShop.Shared.Constant.ErrorConstant
{
    public static class OrderErrors
    {
        public static ErrorResult GetOrderNotFoundError(Guid orderId)
        {
            return new ErrorResult
            {
                Code = "OrderNotFound",
                Description = $"Order with ID {orderId} not found."
            };
        }

        public static ErrorResult GetNoOrdersFoundForUserError(Guid userId)
        {
            return new ErrorResult
            {
                Code = "NoOrdersFound",
                Description = $"No orders found for user with ID {userId}. Please check the user ID or try again later."
            };
        }

        public static ErrorResult GetOrderDeleteNotAllowedError(Guid orderId)
        {
            return new ErrorResult
            {
                Code = "OrderDeleteNotAllowed",
                Description = $"Order with ID {orderId} cannot be deleted."
            };
        }

        public static ErrorResult GetOrderUpdateNotAllowedError(Guid orderId)
        {
            return new ErrorResult
            {
                Code = "OrderUpdateNotAllowed",
                Description = $"Order with ID {orderId} cannot be updated after it is confirmed."
            };
        }

        public static ErrorResult GetOrderAlreadyProcessedError(Guid orderId)
        {
            return new ErrorResult
            {
                Code = "OrderAlreadyProcessed",
                Description = $"Order with ID {orderId} has already been processed and cannot be modified."
            };
        }

        public static ErrorResult GetInvalidOrderStatusError(string status)
        {
            return new ErrorResult
            {
                Code = "InvalidOrderStatus",
                Description = $"The status '{status}' is invalid for the order."
            };
        }

        public static ErrorResult GetOrderCreationFailedError()
        {
            return new ErrorResult
            {
                Code = "OrderCreationFailed",
                Description = "An error occurred while creating the order."
            };
        }

        public static ErrorResult GetOrderUpdateFailedError()
        {
            return new ErrorResult
            {
                Code = "OrderUpdateFailed",
                Description = "An error occurred while updating the order."
            };
        }

        public static ErrorResult GetOrderNotFoundForUserError(Guid userId)
        {
            return new ErrorResult
            {
                Code = "NoOrderFoundForUser",
                Description = $"No orders found for the user with ID {userId}."
            };
        }

        public static ErrorResult GetOrderNotFoundForVoucherError(Guid voucherId)
        {
            return new ErrorResult
            {
                Code = "NoOrderFoundForVoucher",
                Description = $"No orders found for the voucher with ID {voucherId}."
            };
        }
    }
}
