using Projem.BOL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projem.WebUI.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public ICollection<Category> Categories { get; set; }
        
    }
}