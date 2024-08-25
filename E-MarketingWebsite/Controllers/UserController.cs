using E_MarketingWebsite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace E_MarketingWebsite.Controllers
{
    public class UserController : Controller
    {

        dbemarketingEntities db = new dbemarketingEntities();

        // GET: User




        // home page 
        public ActionResult UserHome()
        {

            var categories = db.tbl_category.Where(c => c.status == "1").OrderByDescending(c => c.cat_id);
            return View(categories);

        }


        // register page 
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SignUp(tbl_user user, HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "please select a file";
                return View(user);
            }

            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File could not uploaded";
                return View(user);
            }

            tbl_user user1 = new tbl_user
            {
                u_name = user.u_name,
                u_email = user.u_email,
                u_password = user.u_password,
                u_image = path,

                u_contact = user.u_contact
            };
            db.tbl_user.Add(user1);
            db.SaveChanges();

            return RedirectToAction("LogIn");
        }



        // log in user
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(tbl_user user)
        {
            var user1 = db.tbl_user.Where(us => us.u_email == user.u_email && us.u_password == user.u_password).SingleOrDefault();
            if (user1 != null)
            {
                Session["user"] = user1.u_id.ToString();
                Session["UserName"] = user1.u_name;
                return RedirectToAction("UserHome");
            }
            else
            {
                ViewBag.error = "Invalid username or password";
            }
            return View();
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
                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
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



        // Display ads 

        [HttpGet]
        public ActionResult CreateAd()
        {
           if(Session["user"] == null)
            {
                return RedirectToAction("LogIn");
            }
           
                List<tbl_category> cat = db.tbl_category.ToList();
            ViewBag.categoryList = new SelectList(cat, "cat_id", "cat_name");

            return View();
        }


        // post ads
        [HttpPost]
        public ActionResult CreateAd(tbl_product pro, HttpPostedFileBase file)
        {

            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "Select a File";
                return View(pro);

            }

            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File Could not be uploaded";
                return View(pro);

            }

            tbl_product p = new tbl_product
            {
                pro_name = pro.pro_name,
                pro_des = pro.pro_des,
                pro_image = path,
                pro_price = pro.pro_price,
                pro_fk_user = Convert.ToInt32(Session["user"].ToString()),
                pro_fk_cat = pro.pro_fk_cat







            };
            db.tbl_product.Add(p);
            db.SaveChanges();
            return RedirectToAction("UserHome");
        }


        // view ads 
        [HttpGet]
        public ActionResult ViewAds(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("LogIn");
            }
            var products = db.tbl_product.Where(p => p.pro_fk_cat == id).OrderByDescending(p => p.pro_id).ToList();
            return View(products);
        }


        // log out the user
        public ActionResult LogOut()
        {

            Session.Clear();
            Session.Abandon();
            return RedirectToAction("LogIn");
        }


        // user profile 

        [HttpGet]
        public ActionResult Profile(int id)
        {
            var user = db.tbl_user.Where(u => u.u_id == id).ToList().SingleOrDefault();
            return View(user);
        }


        // user ads
        [HttpGet]
        public ActionResult MyAds(int id)
        {
            var myAds = db.tbl_product.Where(p => p.pro_fk_user == id).ToList();
            return View(myAds);
        }

        [HttpPost]
        public ActionResult DeleteAd(int id)
        {
            var product = db.tbl_product.Find(id);
            db.tbl_product.Remove(product);
            db.SaveChanges();
            ViewBag.msg = "Ad Deleted Successfully";
            return RedirectToAction("UserHome");
        }

        public ActionResult EditProfile(int id)
        {
            var user = db.tbl_user.Find(id);
            return View(user);
        }

        [HttpPost]
        public ActionResult EditProfile(tbl_user user, HttpPostedFileBase file)
        {

            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "Select a File";
                return View(user);

            }

            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File Could not be uploaded";
                return View(user);

            }

            var existingUser = db.tbl_user.Find(user.u_id);
            existingUser.u_name = user.u_name;
            existingUser.u_email = user.u_email;
            existingUser.u_contact = user.u_contact;
            existingUser.u_image = path;

            db.SaveChanges();

            return RedirectToAction("Profile", new { id = user.u_id });


        }


        [HttpGet]
        public ActionResult SearchCategoryList(string search)
        {

            if (ModelState.IsValid)
            {
                var categories = db.tbl_category.Where(c => c.cat_name.Contains(search)).ToList();
                if (categories.Any())
                {
                    return View(categories);
                }
            }
            ViewBag.nocat = "Category Not found !";
            return RedirectToAction("UserHome");
        }
        //search category by name 

        [HttpPost]
        public ActionResult SearchCategory(string search)
        {


            return RedirectToAction("SearchCategoryList", new { search });
        }


        // search Ads

        [HttpGet]
        public ActionResult SearchAdList(string search)
        {
            if (ModelState.IsValid)
            {
                var Ads = db.tbl_product.Where(p => p.pro_name.Contains(search));
                if (Ads.Any())
                {
                    return View(Ads);
                }
                else
                {
                    ViewBag.Ads = "No Ads Found ";
                    return RedirectToAction("ViewAds");
                }
            }
            return RedirectToAction("ViewAds");
        }
        [HttpPost]
        public ActionResult SearchAd(string searchQuery)
        {
            return RedirectToAction("SearchAdList", new { searchQuery });
        }

        /// delete profile 
        /// 

        [HttpPost]
        public ActionResult DeleteProfile(int id)
        {
            var user = db.tbl_user.Find(id);
            db.tbl_user.Remove(user);
            db.SaveChanges();
            return RedirectToAction("LogIn");
        }



        // update my ads

        [HttpGet]
        public ActionResult UpdateMyAds(int id)
        {
          var Ad=  db.tbl_product.Find(id);
            return View(Ad);
        }


        [HttpPost]

        public ActionResult UpdateMyAds(tbl_product product, HttpPostedFileBase file)
        {

            if (file == null || file.ContentLength == 0)
            {
                ViewBag.error = "Select a File";
                return View(product);

            }

            string path = UploadingFile(file);
            if (path == "-1")
            {
                ViewBag.error = "File Could not be uploaded";
                return View(product);

            }

            var existingUser = db.tbl_product.Find(product.pro_id);
            existingUser.pro_name = product.pro_name;
            
            
            
            existingUser.pro_image = path;

            db.SaveChanges();

            return RedirectToAction("MyAds", new { id = product.pro_fk_user });


        }

    }




}