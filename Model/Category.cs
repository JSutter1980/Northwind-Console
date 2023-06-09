﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Northwind_Console_Net06.Model
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Category Name Required!")]
        public string CategoryName { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
