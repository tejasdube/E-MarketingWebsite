using E_MarketingWebsite.Models;
using E_MarketingWebsite.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_MarketingWebsite.Controllers
{
    public class AdminController : Controller
    {
        dbemarketingEntities db = new dbemarketingEntities();
        AdminService adminService = new AdminService();
        // GET: Admin
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        public ActionResult AdminHome()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(tbl_admin ad)
        {
            var admin = adminService.LogIn(ad);
            if (admin != null)
            {
                Session["ad_id"] = admin.ad_id.ToString();
                Session["Name"] = admin.ad_username;
                return RedirectToAction("AdminHome");
            }
            else
            {
                ViewBag.error = "Invalid Username or Password ?";
            } 
            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("LogIn");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(tbl_category cat, HttpPostedFileBase file)
        {
            // Check if the file is null
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "Please select a file.";
                return View(cat);
            }

            // Upload file and get the file path
            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File could not be uploaded.";
                return View(cat); // Return the same view with the model
            }

            // Create new category and set its properties
            tbl_category c = new tbl_category
            {
                cat_name = cat.cat_name,
                cat_image = path, // Assuming 'cat_image' is the correct property
                status = "1",
                cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString())
            };

            // Save category to the database
            db.tbl_category.Add(c);
            db.SaveChanges();

            // Redirect to a different action or view
            return RedirectToAction("ViewCategory"); // Change 'Index' to the appropriate action
        }

        public string UploadingFile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".jfif")
                {
                    try
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random+Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                    }
                    catch (Exception ex)
                    {
                        path = "-1";
                        // Optionally log the exception
                        // LogException(ex);

                    }
                }
                else
                {
                    ViewBag.error = "Only .jpg, .jpeg, and .png files are acceptable.";
                }
            }
            else
            {
                ViewBag.error = "Please select a file.";
            }
            return path;
        }


        [HttpGet]
        public ActionResult ViewCategory()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("LogIn");
            }
            var categories = adminService.GetAllCategories();
            return View(categories);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            adminService.DeleteCategoryById(id);
            TempData["msg"] = "Category Remove succesfully !!!!!!! ";
            return RedirectToAction("ViewCategory");
        }


       [HttpGet]
       public ActionResult Users()
        {
            var users = adminService.GellAllUsers();
            return View(users);
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            adminService.DeleteUserById(id);
            TempData["msg"] = "User Deleted Successfully ! ";
            return RedirectToAction("Users");
        }


        // update the category
        [HttpGet]
        public ActionResult Update(int id)
        {
            var category = db.tbl_category.Find(id);

            return View(category);
        }

        [HttpPost]
        public ActionResult Update(tbl_category cat, HttpPostedFileBase file)
        {
            // Check if the file is null
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "Please select a file.";
                return View(cat);
            }

            // Upload file and get the file path
            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File could not be uploaded.";
                return View(cat); // Return the same view with the model
            }

            var existingcat = db.tbl_category.Find(cat.cat_id);
            existingcat.cat_name = cat.cat_name;
            existingcat.cat_image = path;

            

           
            
            db.SaveChanges();

            // Redirect to a different action or view
            return RedirectToAction("ViewCategory"); // Change 'Index' to the appropriate action
        }
    }
}
