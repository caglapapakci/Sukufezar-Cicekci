using Projem.BLL.Repositories;
using Projem.BOL.Entities;
using Projem.WebUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projem.WebUI.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        Repository<Category> repoCategory = new Repository<Category>();
        Repository<Product> repoProduct = new Repository<Product>();
        Repository<Member> repoMember = new Repository<Member>();
        public ActionResult Index()
        {
            //Favoriler için ekledik.
            if (User.Identity.IsAuthenticated)
            {
                string membermail = HttpContext.User.Identity.Name;

                Member member = repoMember.GetBy(g => g.MailAddress == membermail);
                ViewBag.memberID = member.ID;

            }


            CategoryVM categoryVM = new CategoryVM
            {
                Categories = repoCategory.GetAll().Include(i => i.Children).Include(i => i.Products).ToList(),              
                Products = repoProduct.GetAll().Include(i => i.Pictures).Include(i=>i.Favorites).ToList()
            };
            return View(categoryVM);
        }

        public ActionResult Filter(int? CatID, int? ParentID, int? BrandID, string Price)
        {
            List<Category> categories = repoCategory.GetAll().Include(i => i.Children).Include(i => i.Products).ToList();
            List<Product> products = repoProduct.GetAll().Include(i => i.Pictures).ToList();

            if (ParentID.HasValue)
            {
                Category parentCategory = repoCategory.GetAll().Include(i => i.Children).FirstOrDefault(f => f.ID == ParentID);
                if (parentCategory.Children.Any()) products = products.Where(w => parentCategory.Children.Select(s => s.ID).Contains(w.CategoryID)).ToList();
                else products = products.Where(w => w.CategoryID == ParentID).ToList();
            }
            if (CatID.HasValue)
            {
                products = products.Where(w => w.CategoryID == CatID).ToList();
            }         
            if (!string.IsNullOrEmpty(Price))
            {
                string[] prices = Price.Replace("₺", "").Replace(" ", "").Split('_');
                decimal price1 = Convert.ToDecimal(prices[0]);
                decimal price2 = Convert.ToDecimal(prices[1]);
                products = products.Where(w => w.Price >= price1 && w.Price <= price2).ToList();
            }
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.Categories = categories;
            categoryVM.Products = products;

            //CategoryVM categoryVM = new CategoryVM
            //{
            //    Categories = repoCategory.GetAll().Include(i => i.Children).Include(i => i.Products).ToList(),
            //    Brands = repoBrand.GetAll().Include(i => i.Products).ToList(),
            //    Products = repoProduct.GetAll().Where(w=>w.CategoryID==CatID).Include(i => i.Pictures).ToList()
            //};
            return View("Index", categoryVM);
        }

        //Arama için eklendi.
        [HttpPost]
        public ActionResult Ara(string search)
        {
            if (!string.IsNullOrEmpty(search))
            {
                Category category = repoCategory.GetBy(g => g.Name == search);
                List<Product> findedProducts = repoProduct.GetAll().Include(i => i.Pictures).Include(i=>i.Category).ToList();
                if (category != null) findedProducts = findedProducts.Where(w => w.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) != -1 || w.Detail.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) != -1 || w.Category.Name.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) != -1 || w.CategoryID == category.ID || w.Category.ParentID == category.ID).ToList();
                else findedProducts = findedProducts.Where(w => w.Name.IndexOf(search,StringComparison.InvariantCultureIgnoreCase) !=-1|| w.Detail.IndexOf(search, StringComparison.InvariantCultureIgnoreCase) != -1).ToList();
                                 

                CategoryVM categoryVM = new CategoryVM
                {
                    Categories = repoCategory.GetAll().Include(i => i.Children).Include(i => i.Products).ToList(),
                    Products = findedProducts
                };
                return View("Index", categoryVM);

            }
            else return RedirectToAction("Index", "Home");


        }
    }
}