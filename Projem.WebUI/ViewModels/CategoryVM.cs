using Projem.BOL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projem.WebUI.ViewModels
{
    public class CategoryVM
    {
        public ICollection<Category> Categories { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}