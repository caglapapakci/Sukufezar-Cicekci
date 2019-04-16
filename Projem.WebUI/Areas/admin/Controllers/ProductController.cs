using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Projem.BLL.Repositories;
using Projem.BOL.Entities;
using Projem.WebUI.ViewModels;

namespace Projem.WebUI.Areas.admin.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProductController : Controller
    {
        Repository<Product> repoProduct = new Repository<Product>();
        Repository<Category> repoCategory = new Repository<Category>();
        Repository<Picture> repoPicture = new Repository<Picture>();

        public ActionResult Index()
        {
            return View(repoProduct.GetAll());
        }
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repoProduct.GetBy(g => g.ID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }
        public ActionResult Create()
        {
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                Categories = repoCategory.GetAll().ToList(),
            };
            return View(productVM);
            
        }
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public ActionResult Create(ProductVM model, List<HttpPostedFileBase> Picture)
        {
            if (ModelState.IsValid)
            {
                // minumum bir resim secildiyse(ilk resim)
                if (Picture[0] != null)
                {
                    int pindex = 1;
                    repoProduct.Add(model.Product);
                    foreach (var picture in Picture)
                    {
                        if (picture != null)
                        {
                            if (!Directory.Exists(Server.MapPath("~/Content/img/product"))) Directory.CreateDirectory(Server.MapPath("~/Content/img/product"));
                            picture.SaveAs(Server.MapPath("~/Content/img/product/" + picture.FileName));

                            repoPicture.Add(new Picture
                            {
                                FPath = "/Content/img/product/" + picture.FileName,
                                PIndex = pindex,
                                ProductID = model.Product.ID
                            });
                            pindex++;
                        }
                    }
                }

                return RedirectToAction("Index");
            }

            return View(model.Product);
            
        }

        // GET: admin/Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repoProduct.GetBy(g => g.ID == id);
            ProductVM productVM = new ProductVM
            {
                Product = product,
                Categories = repoCategory.GetAll().ToList(),
            };
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(productVM);
            
        }

        // POST: admin/Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, List<HttpPostedFileBase> Picture)
        {
            if (ModelState.IsValid)
            {
                //Resim seçilmediyse sadece Product tablosunda güncelleme yapar
                repoProduct.Update(product);

                //Minumum bir resim secili ise(ilk resim).... <<Picture[1] = ikinci resim(input file'lardan ikincisi)>>
                if (Picture[0] != null)
                {
                    IList<Picture> silinecekResimler = repoPicture.GetAll().Where(w => w.ProductID == product.ID).ToList();
                    // Dosyadan Silme
                    foreach (var picture in silinecekResimler)
                    {
                        if (System.IO.File.Exists(HttpContext.Server.MapPath(picture.FPath)))
                        {
                            System.IO.File.Delete(HttpContext.Server.MapPath(picture.FPath));
                        }
                    }
                    // Veritabanindan silme
                    repoPicture.RemoveRange(silinecekResimler);

                    // Yeni seçilen resimlelri Picture Tablosuna Ekleme 
                    int pindex = 1;
                    foreach (var picture in Picture)
                    {
                        if (picture != null)
                        {
                            if (!Directory.Exists(Server.MapPath("~/Content/img/product"))) Directory.CreateDirectory(Server.MapPath("~/Content/img/product"));
                            picture.SaveAs(Server.MapPath("~/Content/img/product/" + picture.FileName));

                            repoPicture.Add(new Picture
                            {
                                FPath = "/Content/img/product/" + picture.FileName,
                                PIndex = pindex,
                                ProductID = product.ID
                            });
                            pindex++;
                        }
                    }
                }

                return RedirectToAction("Index");
            }
            return View(product);
        }

        // GET: admin/Product/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = repoProduct.GetBy(g => g.ID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: admin/Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // Urun silinirse dosyadan ve Picture tablosundan da silinmeli
            IList<Picture> silinecekResimler = repoPicture.GetAll().Where(w => w.ProductID == id).ToList();
            // Dosyadan Silme
            foreach (var picture in silinecekResimler)
            {
                if (System.IO.File.Exists(HttpContext.Server.MapPath(picture.FPath)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(picture.FPath));
                }
            }
            // Veritabanindan silme
            repoPicture.RemoveRange(silinecekResimler);

            Product product = repoProduct.GetBy(g => g.ID == id);
            repoProduct.Remove(product);
            return RedirectToAction("Index");
        }
    }
}
