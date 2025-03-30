using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBox
{
    public class BlindBoxPriceHistoryDto
    {
        public Guid Id { get; set; }
        public Guid? BlindBoxId { get; set; }
        public decimal DefaultPrice { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
