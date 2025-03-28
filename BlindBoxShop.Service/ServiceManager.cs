﻿using AutoMapper;
using BlindBoxShop.Repository.Contract;
using BlindBoxShop.Service.Contract;
using Microsoft.Extensions.Logging;

namespace BlindBoxShop.Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        private readonly ILoggerFactory _loggerFactory;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper, ILoggerFactory loggerFactory)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _loggerFactory = loggerFactory;
        }

        public IUserService UserService => new UserService(_repositoryManager, _mapper);

        public IBlindBoxService BlindBoxService => new BlindBoxService(_repositoryManager, _mapper);

        public IBlindBoxImageService BlindBoxImageService => new BlindBoxImageService(_repositoryManager, _mapper);

        public IBlindBoxPriceHistoryService BlindBoxPriceHistoryService => new BlindBoxPriceHistoryService(_repositoryManager, _mapper);

        public IBlindBoxCategoryService BlindBoxCategoryService => new BlindBoxCategoryService(_repositoryManager, _mapper);

        public IReviewsService CustomerReviewsService => new ReviewService(_repositoryManager, _mapper, _repositoryManager.Replies);

        public IOrderDetailService OrderDetailService => new OrderDetailService(_repositoryManager, _mapper);

        public IOrderService OrderService => new OrderService(_repositoryManager, _mapper, _repositoryManager.OrderDetail);

        public IPackageService PackageService => new PackageService(_repositoryManager, _mapper);

        public IVoucherService VoucherService => new VoucherService(_repositoryManager, _mapper);

        public IReplyService ReplyService => new ReplyService(_repositoryManager, _mapper);
        
        public IAuthService AuthService => new AuthService(_repositoryManager, _mapper);
        
        public IVNPayService VNPayService => new VNPayService(
            _repositoryManager, 
            _mapper, 
            _loggerFactory.CreateLogger<VNPayService>());

        public IBlindBoxItemService BlindBoxItemService => new BlindBoxItemService(_repositoryManager, _mapper);
    }
}
