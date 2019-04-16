using Projem.BLL.Repositories;
using Projem.BOL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Projem.WebUI.ViewModels;

namespace Projem.WebUI.Controllers
{
    public class ProductController : Controller
    {
        Repository<Product> repoProduct = new Repository<Product>();             
        Repository<TContent> repoTContent = new Repository<TContent>();
        public ActionResult Index()
        {
            return View(repoProduct.GetAll().ToList());
        }
        public ActionResult Detail(int ID)
        {
            Product product = repoProduct.GetAll().Include(i => i.Pictures).Where(w => w.ID == ID).FirstOrDefault();
            ViewBag.BenzerUrunler = repoProduct.GetAll().Include(i => i.Pictures).Where(w => w.CategoryID == product.CategoryID && w.ID != product.ID).Take(8);

            ViewBag.UrunDetay = repoTContent.GetAll().Where(w => w.Filter == "ÜrünDetay").FirstOrDefault();

            return View(product);

        }

       
    }
}