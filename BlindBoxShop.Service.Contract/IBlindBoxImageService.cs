using BlindBoxShop.Shared.DataTransferObject.BlindBox;
using BlindBoxShop.Shared.ResultModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BlindBoxShop.Service.Contract
{
    public interface IBlindBoxImageService : IDisposable
    {
        Task<Result<IEnumerable<BlindBoxImageDto>>> GetBlindBoxImagesByBlindBoxIdAsync(Guid blindBoxId);
    }
}
