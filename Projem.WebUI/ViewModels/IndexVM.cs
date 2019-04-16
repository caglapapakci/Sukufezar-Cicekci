using Projem.BOL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projem.WebUI.ViewModels
{
    public class IndexVM
    {
        public ICollection<Slider> Sliders { get; set; }
        public ICollection<Product> NewestProducts { get; set; }
        public ICollection<Product> BestSellerProducts { get; set; }
        public ICollection<Advertisement> Advertisements { get; set; }
        public ICollection<Favorite> FavoriteProducts { get; set; }
    }
}