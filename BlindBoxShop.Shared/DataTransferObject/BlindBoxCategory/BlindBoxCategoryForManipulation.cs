using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.BlindBoxCategory
{
    public record BlindBoxCategoryForManipulation
    {
        [Required(ErrorMessage = "{0} of BlindBoxCategory is required.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "{0} of BlindBoxCategory is required.")]
        public string? Description { get; set; }
    }
}
