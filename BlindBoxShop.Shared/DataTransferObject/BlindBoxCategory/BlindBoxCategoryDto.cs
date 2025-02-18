using System.ComponentModel.DataAnnotations;

namespace BlindBoxShop.Shared.DataTransferObject.User
{
    public record BlindBoxCategoryDto
    {
        public string? Name { get; set; }

        public string? Description { get; set; }

        public DateTime? CreatedAt { get; set; }
    }

    public record BlindBoxCategoryForManipulation
    {
        [Required(ErrorMessage = "{0} of BlindBoxCategory is required.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "{0} of BlindBoxCategory is required.")]
        public string Description { get; set; } = null!;
    }

    public record BlindBoxCategoryForCreate : BlindBoxCategoryForManipulation
    {

    }

    public record BlindBoxCategoryForUpdate : BlindBoxCategoryForManipulation
    {

    }
}
