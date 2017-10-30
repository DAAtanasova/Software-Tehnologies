using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;

namespace Blog.Controllers
{
    public class ArticleController : Controller
    {

        // GET: Article
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        //Get: Article/List
        public ActionResult List()
        {
            using (var database = new BlogDbContex())
            {
                var articles = database.Articles
                    .Include(a => a.Author).ToList();
                return View(articles);
            }
        }

        //GET: Article/Details
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContex())
            {
                var article = database.Articles
                    .Where(a => a.Id == id)
                    .Include(a => a.Author)
                    .First();

                if (article == null)
                {
                    return HttpNotFound();
                }
                return View(article);
            }
        }

        //GET:Article/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        //POST:Article/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContex())
                {
                    //get author Name
                    var author = database.Users
                        .Where(u => u.UserName == this.User.Identity.Name)
                        .FirstOrDefault();
                    //set article author
                    article.Author = author;

                    database.Articles.Add(article);
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(article);
        }

        //GET: Article/Delete
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContex())
            {
                var article = database.Articles
                            .Where(a => a.Id == id)
                            .Include(a => a.Author)
                            .First();
                if (!IsUserAutorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }
                if (article == null)
                {
                    return HttpNotFound();
                }
                return View(article);
            }
        }

        //POST: Article/Delete
        [HttpPost]
        [ActionName("Delete")]
        public ActionResult DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContex())
            {
                var article = database.Articles
                            .Where(a => a.Id == id)
                            .Include(a => a.Author)
                            .First();

                //check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                database.Articles.Remove(article);
                database.SaveChanges();

                return RedirectToAction("Index");
            }
        }

        //GET:Article/Edit
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var database = new BlogDbContex())
            {
                var article = database.Articles
                            .Where(a => a.Id == id)
                            .First();

                //check if the user is logged and is able to edit the article
                if (!IsUserAutorizedToEdit(article))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
                }

                //check if article exists
                if (article == null)
                {
                    return HttpNotFound();
                }

                var model = new ArticleViewModel();
                model.Id = article.Id;
                model.Title = article.Title;
                model.Content = article.Content;

                return View(model);
            }
        }

        //POST:Article/Edit
        [HttpPost]
        public ActionResult Edit(ArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var database = new BlogDbContex())
                {
                    var article = database.Articles
                        .FirstOrDefault(a => a.Id == model.Id);

                    article.Title = model.Title;
                    article.Content = model.Content;

                    //save article state in db
                    database.Entry(article).State = EntityState.Modified;
                    database.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        private bool IsUserAutorizedToEdit(Article article)
        {
            bool isAdmin = this.User.IsInRole("Admin");
            bool isAuthor = article.IsAuthor(this.User.Identity.Name);

            return isAdmin || isAuthor;
        }
    }
}