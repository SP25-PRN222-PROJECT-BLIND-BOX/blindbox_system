﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlindBoxShop.Shared.Features
{

    public class BlindBoxCategoryParameter : RequestParameters
    {
        public string? SearchByName { get; set; }

        public BlindBoxCategoryParameter()
        {
            OrderBy = "Name";
        }
    }
}
