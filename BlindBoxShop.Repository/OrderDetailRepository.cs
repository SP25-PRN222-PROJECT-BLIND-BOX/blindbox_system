using BlindBoxShop.Entities.Models;
using BlindBoxShop.Repository.Contract;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BlindBoxShop.Repository
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId, bool trackChanges)
        {
            Console.WriteLine($"Fetching order details for Order ID: {orderId}");
            
            var orderDetails = await FindByCondition(od => od.OrderId == orderId, trackChanges)
                .Include(od => od.Order)
                .Include(od => od.BlindBoxPriceHistory)
                    .ThenInclude(pbh => pbh.BlindBox)
                        .ThenInclude(bb => bb.BlindBoxImages)
                .AsSplitQuery() // Sử dụng AsSplitQuery để tránh vấn đề Cartesian Explosion khi join nhiều bảng
                .ToListAsync();

            // Log information about BlindBoxImages
            foreach (var detail in orderDetails)
            {
                var blindBox = detail.BlindBoxPriceHistory?.BlindBox;
                var images = blindBox?.BlindBoxImages;
                Console.WriteLine($"OrderDetail ID: {detail.Id}");
                Console.WriteLine($"BlindBox ID: {blindBox?.Id}, Name: {blindBox?.Name}");
                Console.WriteLine($"Images count: {images?.Count() ?? 0}");
                
                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        Console.WriteLine($"Image ID: {image.Id}, URL: '{image.ImageUrl}'");
                    }
                }
                else
                {
                    // Nếu không có hình ảnh, in ra thông tin BlindBoxPriceHistory để debug
                    Console.WriteLine($"No images found for BlindBox ID: {blindBox?.Id}");
                    Console.WriteLine($"BlindBoxPriceHistory ID: {detail.BlindBoxPriceHistoryId}");
                    
                    if (blindBox != null)
                    {
                        // Kiểm tra xem BlindBox có được load đầy đủ không
                        Console.WriteLine($"BlindBox is loaded: {RepositoryContext.Entry(blindBox).State != EntityState.Detached}");
                        Console.WriteLine($"BlindBoxImages navigation property is loaded: {RepositoryContext.Entry(blindBox).Collection(b => b.BlindBoxImages).IsLoaded}");
                    }
                }
                Console.WriteLine("-----------------------");
            }

            return orderDetails;
        }

        public async Task<OrderDetail> GetOrderDetailWithImagesAsync(Guid id, bool trackChanges)
        {
            Console.WriteLine($"Fetching order detail with ID: {id}");
            
            var orderDetail = await FindByCondition(od => od.Id == id, trackChanges)
                .Include(od => od.Order)
                .Include(od => od.BlindBoxPriceHistory)
                    .ThenInclude(pbh => pbh.BlindBox)
                        .ThenInclude(bb => bb.BlindBoxImages)
                .AsSplitQuery() // Sử dụng AsSplitQuery để tránh vấn đề Cartesian Explosion
                .SingleOrDefaultAsync();

            // Log information about BlindBoxImages
            if (orderDetail != null)
            {
                var blindBox = orderDetail.BlindBoxPriceHistory?.BlindBox;
                var images = blindBox?.BlindBoxImages;
                Console.WriteLine($"OrderDetail ID: {orderDetail.Id}");
                Console.WriteLine($"BlindBox ID: {blindBox?.Id}, Name: {blindBox?.Name}");
                Console.WriteLine($"Images count: {images?.Count() ?? 0}");
                
                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        Console.WriteLine($"Image ID: {image.Id}, URL: '{image.ImageUrl}'");
                    }
                }
                else
                {
                    // Nếu không có hình ảnh, in ra thông tin BlindBoxPriceHistory để debug
                    Console.WriteLine($"No images found for BlindBox ID: {blindBox?.Id}");
                    Console.WriteLine($"BlindBoxPriceHistory ID: {orderDetail.BlindBoxPriceHistoryId}");
                    
                    if (blindBox != null)
                    {
                        // Kiểm tra xem BlindBox có được load đầy đủ không
                        Console.WriteLine($"BlindBox is loaded: {RepositoryContext.Entry(blindBox).State != EntityState.Detached}");
                        Console.WriteLine($"BlindBoxImages navigation property is loaded: {RepositoryContext.Entry(blindBox).Collection(b => b.BlindBoxImages).IsLoaded}");
                    }
                }
            }

            return orderDetail;
        }
        
        // Thêm phương thức mới để lấy thông tin hình ảnh cho các order details
        public async Task<Dictionary<Guid, string>> GetImageUrlsForOrderDetailsAsync(List<Guid> orderDetailIds)
        {
            Console.WriteLine($"Fetching image URLs for {orderDetailIds.Count} order details");
            
            var imageUrlsDictionary = new Dictionary<Guid, string>();
            
            var blindBoxImages = await RepositoryContext.OrderDetails
                .Where(od => orderDetailIds.Contains(od.Id))
                .Select(od => new 
                {
                    OrderDetailId = od.Id,
                    ImageUrl = od.BlindBoxPriceHistory.BlindBox.BlindBoxImages
                        .OrderBy(i => i.Id)
                        .Select(i => i.ImageUrl)
                        .FirstOrDefault() ?? "/images/box-placeholder.jpg"
                })
                .ToListAsync();
                
            foreach (var item in blindBoxImages)
            {
                imageUrlsDictionary[item.OrderDetailId] = item.ImageUrl;
                Console.WriteLine($"Mapped OrderDetail ID: {item.OrderDetailId} to Image URL: '{item.ImageUrl}'");
            }
            
            return imageUrlsDictionary;
        }
    }
}
