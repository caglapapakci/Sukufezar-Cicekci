using Projem.BLL.Repositories;
using Projem.BOL.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Projem.WebUI.ViewModels;
using System.Web.Mvc;
using System.IO;
using System.Text;
using System.Net.Mail;
using System.Net;

namespace Projem.WebUI.Controllers
{
    public class ContactController : Controller
    {
        Repository<Product> repoProduct = new Repository<Product>();
        Repository<Member> repoMember = new Repository<Member>();
        Repository<Favorite> repoFavorite = new Repository<Favorite>();
        Repository<Contact> repoContact = new Repository<Contact>();
        // GET: Contact
        public ActionResult Index()
        {
            //Favoriler için ekledik.          
            if (User.Identity.IsAuthenticated)
            {
                string membermail = HttpContext.User.Identity.Name;

                Member member = repoMember.GetBy(g => g.MailAddress == membermail);

                ViewBag.memberID = member.ID;
                if (repoFavorite.GetAll().Any(a => a.MemberID == member.ID))
                {  IndexVM IndexVM = new IndexVM
                   {
                    //FavoriteProducts = repoProduct.GetAll().Include(i => i.Pictures).OrderByDescending(o => o.ID).Take(4).ToList(),
                    FavoriteProducts = repoFavorite.GetAll().Include(i => i.Product).Include(i => i.Product.Pictures).Where(w => w.MemberID == member.ID).Take(4).ToList()

                   };
                   return View(IndexVM);
                }
                else
                {

                    IndexVM IndexVM = new IndexVM
                    {

                        BestSellerProducts = repoProduct.GetAll().Include(i => i.Pictures).Include(i => i.Favorites).OrderByDescending(o => o.OrderDetail.Sum(s => s.Quantity)).Take(4).ToList()
                    };

                    return View(IndexVM);

                }
                
               
            }
            else
            {
                IndexVM IndexVM = new IndexVM
                {
                    
                    BestSellerProducts = repoProduct.GetAll().Include(i => i.Pictures).Include(i => i.Favorites).OrderByDescending(o => o.OrderDetail.Sum(s => s.Quantity)).Take(4).ToList()
                };

                return View(IndexVM);
            }
               
        }
        public int addFavorite(int productID)
        {
            string membermail = HttpContext.User.Identity.Name;          
            Member member = repoMember.GetBy(g => g.MailAddress == membermail);
            Favorite favorite = repoFavorite.GetBy(g => g.MemberID == member.ID && g.ProductID == productID);
            if (favorite != null)
            {
                repoFavorite.Remove(favorite);
                return 0;
            }
            else
            {
                repoFavorite.Add(new Favorite { MemberID = member.ID, ProductID = productID });
                return 1;
            }
        }
        [HttpPost]
        public ActionResult Index(Contact contact)
        //public ActionResult Index( Contact contact, HttpPostedFileBase dosya)
        {

            ////db.Contact.Add(new Contact { Name= name, Email = email, Subject=subject ,Message=message,KayitTarih=DateTime.Now});
            ////db.SaveChanges();
            ////MailGonder();
            ////return RedirectToAction("Index");


            //dosya.SaveAs(Path.Combine(Server.MapPath("~/Content/file"), Path.GetFileName(dosya.FileName)));
            contact.SendDate = DateTime.Now;
            repoContact.Add(contact);
            MailGonder(contact);
            return RedirectToAction("Index");

        }
        void MailGonder(Contact contact)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(contact.Email);
                mail.To.Add(new MailAddress("sukufezar@gmail.com"));
                mail.Subject = contact.Subject;
                sb.Append("Aşağıda bilgileri bulunan bir ziyaretçinizden mesaj aldınız.<br/>");
                sb.Append("Adı Soyadı: " + contact.NameSurname + "<br/>");
                sb.Append("Mail Adresi: " + contact.Email+ "<br/>");
                sb.Append("Konu: " + contact.Subject);
                sb.Append("Mesajı: " + contact.Message);
                sb.Append("Tarih: " + contact.SendDate);
                mail.Body = sb.ToString();
                mail.IsBodyHtml = true;
                //SmtpClient smtpClient = new SmtpClient("mail.biltekno.com", 587);
                //GMAIL için
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("sukufezar@gmail.com", "123456");
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {

                //ScriptManager.RegisterStartupScript(this.GetType(), "key_mailGonderilemedi", "<script>alert('Mesajınız mail olarak gönderilemedi, Hata:" + ex.Message + "')</script>", false);
            }
        }

    }

}