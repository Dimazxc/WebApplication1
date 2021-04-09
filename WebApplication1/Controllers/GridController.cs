using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public enum UserStatus
    {
        Unblock = 0,
        Block = 1
    }

    public class GridController : Controller
    {
        private ApplicationContext context = new ApplicationContext();
        private Repository usersRepository = new Repository();
        public ActionResult Index()
        {
            return View();
        }

        private bool CheckBan()
        {
            int currutenID = (int)Session["Id"];
            if (usersRepository.CheckLogin(currutenID))
            {
                Session.Clear();
                return false;
            }
            return true;
        }

        public void Delete(string[] values)
        {
           if (!CheckBan()) return;
           context.Configuration.ValidateOnSaveEnabled = false;
           var deleteUsers = context.Users.Where(u => values.Contains(u.Id.ToString())).ToList().ToList();
           context.Users.RemoveRange(deleteUsers);
           context.SaveChanges();
        }

        public void Block(string[] values)
        {
            if (!CheckBan()) return;
            var unblockUsers = context.Users.Where(u => values.Contains(u.Id.ToString()) && u.Status == (int)UserStatus.Unblock);
            foreach(var i in unblockUsers)
            {
                usersRepository.UpdateStatus(i, UserStatus.Block);
            }
        }

        public void Unblock(string[] values)
        {
            if (!CheckBan()) return;
            var blockUsers = context.Users.Where(u => values.Contains(u.Id.ToString()) && u.Status == (int)UserStatus.Block);
            foreach (var i in blockUsers)
            {
                usersRepository.UpdateStatus(i, UserStatus.Unblock);
            }
        }
    }
}