using System.ComponentModel.DataAnnotations;
using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Shared.DataTransferObject.Package
{
    public abstract class PackageForManipulation
    {
        [Required(ErrorMessage = "Type is a required field.")]
        public PackageType Type { get; set; }

        [Required(ErrorMessage = "Name is a required field.")]
        [MaxLength(50, ErrorMessage = "Name shouldn't be longer than 100 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Barcode is a required field.")]
        [MaxLength(50, ErrorMessage = "Barcode shouldn't be longer than 100 characters.")]
        public string Barcode { get; set; } = null!;


        [Range(0, int.MaxValue, ErrorMessage = "TotalBlindBox must be greater than or equal to 0.")]
        public int TotalBlindBox { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "CurrentTotalBlindBox must be greater than or equal to 0.")]
        public int CurrentTotalBlindBox { get; set; }

        //updatDate 
        public DateTime? UpdateDate { get; set; }

    }
}
