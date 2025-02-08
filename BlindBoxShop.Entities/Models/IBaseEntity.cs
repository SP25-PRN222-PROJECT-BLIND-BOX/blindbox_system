using System.ComponentModel.DataAnnotations.Schema;

namespace BlindBoxShop.Entities.Models
{
    public interface IBaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }

}
