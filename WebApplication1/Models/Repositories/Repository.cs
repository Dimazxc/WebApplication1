using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication1.Controllers;

namespace WebApplication1.Models.Repositories
{
    public class Repository
    {
        private ApplicationContext context;
        public Repository()
        {
            this.context = new ApplicationContext();
        }

        public void Add(User user)
        {
            context.Configuration.ValidateOnSaveEnabled = false;
            context.Users.Add(user);
            context.SaveChanges();
        }

        public List<User> GetAll()
        {
            return context.Users.ToList();
        }
        public void UpdateStatus(Models.User user, UserStatus status)
        {
            using (var ctx = new ApplicationContext())
            {
                ctx.Configuration.ValidateOnSaveEnabled = false;
                user.Status = (int)status;
                ctx.Users.Add(user);
                ctx.Entry(user).State = System.Data.Entity.EntityState.Modified;
                ctx.SaveChanges();
            }
        }

        public bool CheckLogin(int id)
        {
            var user = context.Users.Where(u => u.Id == id);
            if (user.Count() > 0)
            {
                return user.FirstOrDefault().Status == (int)UserStatus.Block;
            }
            return true;
        }
    }
}