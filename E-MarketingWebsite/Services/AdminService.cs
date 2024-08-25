using E_MarketingWebsite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_MarketingWebsite.Services
{
    public class AdminService
    {
        dbemarketingEntities db = new dbemarketingEntities();

        public tbl_admin LogIn(tbl_admin ad)
        {
            var admin= db.tbl_admin.Where(m => m.ad_username == ad.ad_username && m.ad_password == ad.ad_password).SingleOrDefault();
            return admin;
        }

        public List<tbl_category> GetAllCategories()
        {
            List<tbl_category> list = new List<tbl_category>();
            list = db.tbl_category.Where(c => c.status == "1").OrderByDescending(c => c.cat_id).ToList();
            return list;
        }

        public void DeleteCategoryById(int id)
        {
            var cat = db.tbl_category.Find(id);
            db.tbl_category.Remove(cat);
            db.SaveChanges();
        }

        public List<tbl_user> GellAllUsers()
        {
            return db.tbl_user.ToList();
        }

        public void DeleteUserById(int id)
        {
            var user = db.tbl_user.Find(id);
            db.tbl_user.Remove(user);
            db.SaveChanges();
        }
    }
}