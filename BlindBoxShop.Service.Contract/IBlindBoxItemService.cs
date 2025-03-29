using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IBlindBoxItemService : IDisposable
    {
        /// <summary>
        /// Gets all items for a specific BlindBox by its ID
        /// </summary>
        /// <param name="blindBoxId">The ID of the BlindBox</param>
        /// <param name="trackChanges">Whether to track changes</param>
        /// <returns>A collection of BlindBoxItemDto objects</returns>
        Task<Result<IEnumerable<BlindBoxItemDto>>> GetItemsByBlindBoxIdAsync(Guid blindBoxId, bool trackChanges);
    }
} 