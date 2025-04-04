﻿using System.ComponentModel.DataAnnotations;
using BlindBoxShop.Shared.Enum;

namespace BlindBoxShop.Entities.Models
{
    public class Package : BaseEntity, IBaseEntityWithUpdatedAt
    {
        public PackageType Type { get; set; }

        public string Name { get; set; } = null!;

        public string Barcode { get; set; } = null!;

        [Range(0, int.MaxValue, ErrorMessage = "TotalBlindBox must be greater than or equal to 0.")]
        public int TotalBlindBox { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "CurrentTotalBlindBox must be greater than or equal to 0.")]
        public int CurrentTotalBlindBox { get; set; }

        public DateTime? UpdatedAt { get; set; }


        public virtual ICollection<BlindBox>? BlindBoxes { get; set; }
    }

}
