using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Falcon_Blog.Helpers;
using Falcon_Blog.Models;
using PagedList;
using PagedList.Mvc;

namespace Falcon_Blog.Controllers
{
    [RequireHttps]
    //I'm telling this action not to let anyone in unless they are an admin.
    [Authorize(Roles = "Admin, Moderator")]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //private SearchHelper foo = new SearchHelper();

        // GET: BlogPosts
        [AllowAnonymous]
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogList = IndexSearch(searchStr);

            int pageSize = 3; //display three blog posts at a time on this page
            int pageNumber = (page ?? 1);
            return View(blogList.OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));

            var listPosts = db.BlogPosts.AsQueryable();
            return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Contact(EmailModel model)


        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                                    p.Body.Contains(searchStr) ||
                                    p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                                    c.Author.FirstName.Contains(searchStr) ||
                                                    c.Author.LastName.Contains(searchStr) ||
                                                    c.Author.DisplayName.Contains(searchStr) ||
                                                    c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }
            return result.OrderByDescending(p => p.Created);
        }




        // GET: BlogPosts/Details/5
        /*public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }*/

        [AllowAnonymous]
        public ActionResult Details(string Slug)
        {
            if (String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }       


        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,Abstract,Body,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image) //new blogPost var for BlogPost class
        {
            if (ModelState.IsValid)
            {
                var slug = StringUtilities.URLFriendly(blogPost.Title);
                
                //I want to check for a few error conditions and if they exist I will return an error
                //First: Lets check if the slug is empty for some reason
                if (string.IsNullOrEmpty(slug))
                {
                    //This is my opportunity to display a custom error using the ValidationMessageFor
                    //I determine where the error shows by specifying the property in the first set of quotes
                    ModelState.AddModelError("Title", "Oops, the Title cannot be empty!");
                    return View(blogPost);
                }

                //Second: We have to make sure this slug has not already been used on a previous BlogPost
                if (db.BlogPosts.Any(b => b.Slug == slug))
                {
                    //I can also display custom errors using the ValidationSummary by leaving the first set of quotes empty
                    ModelState.AddModelError("", $"Oops, the Title: '{blogPost.Title}' has been used before");
                    return View(blogPost);
                }

                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }

                //I want to make sure my blogPost.Slug assignment happens inside the if statement
                //beyond both error state checks...
                blogPost.Slug = slug;
                blogPost.Created = DateTime.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");


                //Original working code.
                //if (String.IsNullOrWhiteSpace(Slug))
                //{
                //    ModelState.AddModelError("Title", "Invalid title");
                //    return View(blogPost);
                //}
                //if(db.BlogPosts.Any(p => p.Slug == Slug))
                //{
                //    ModelState.AddModelError("Title", "The title must be unique");
                //    return View(blogPost);
                //}


            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Abstract,Body,MediaUrl,Published")]  BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                blogPost.Updated = DateTime.Now;
                var bpId = blogPost.Id;
                var oldPost = db.BlogPosts.AsNoTracking().FirstOrDefault(b => b.Id == bpId);
                blogPost.Created = oldPost.Created;
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                    blogPost.MediaUrl = "/Uploads/" + fileName;
                }                          
                
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
