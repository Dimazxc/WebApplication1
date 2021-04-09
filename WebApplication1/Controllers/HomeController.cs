using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using Telerik.Web.Mvc.Infrastructure;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext context = new ApplicationContext();
        private Repository usersRepository = new Repository();
        public ActionResult Index()
        {
            if (Session["Id"] != null)
            {
                int currutenID = (int)Session["Id"];
                if (usersRepository.CheckLogin(currutenID))
                {
                    Session.Clear();
                    return RedirectToAction("Login");
                }
                return View(usersRepository.GetAll());
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public ActionResult Register()
        {
            return View(new User());
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User _user)
        {
            if (ModelState.IsValid)
            {
                var check = usersRepository.GetAll().FirstOrDefault(s => s.Email == _user.Email);
                if (check == null)
                {
                    usersRepository.Add(new User {Email = _user.Email, Name =_user.Name,  Password = GetMD5(_user.Password), RegDate = DateTime.Now, LoginDate = DateTime.Now, Status = (int)UserStatus.Unblock});
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error = "Email already exists";
                    return View(new User());
                }
            }
            return View(new User());
        }

        public ActionResult Login()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            if (ModelState.IsValid)
            {
                var hashPassword = GetMD5(password);
                var listUsers = usersRepository.GetAll().Where(s => s.Email.Equals(email) && s.Password.Equals(hashPassword)).ToList();
                if (listUsers.Count() > 0)
                {
                    var user = listUsers.FirstOrDefault();
                    if (user != null && user.Status != (int)UserStatus.Block)
                    {
                        user.LoginDate = DateTime.Now;
                        Session["Id"] = user.Id;
                        context.Configuration.ValidateOnSaveEnabled = false;
                        context.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }

        private string GetMD5(string inputMessage)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(inputMessage);
            byte[] targetData = md5.ComputeHash(fromData);
            string resultMessage = BitConverter.ToString(targetData);
            return resultMessage;
        }
    }
}
