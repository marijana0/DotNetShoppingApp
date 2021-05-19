using DotNetShop.Data;
using DotNetShop.Models;
using DotNetShop.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetShop.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;



        public ProductController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> objList = _db.Products.Include(u => u.Category).Include(u=>u.ApplicationType);
            return View(objList);
        }

        //GET -UPSERT
        public IActionResult Upsert(int? id)
        {
            //IEnumerable<SelectListItem> CategoryDropdown = _db.Categories.Select(i => new SelectListItem
            //{
            //    Text = i.Name,
            //    Value = i.Id.ToString()
            //});

            //ViewBag.CategoryDropdown = CategoryDropdown;

            //Product product = new Product();
            ProductViewModel productViewModel = new()
            {
                Product = new Product(),
                CategorySelectList = _db.Categories.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };

            if (id == null) 
            {
                //create
                return View(productViewModel);
            }
            else 
            {
                productViewModel.Product = _db.Products.Find(id);
                if (productViewModel.Product == null) 
                {
                    return NotFound();
                }
                return View(productViewModel);
            }
        }

        //POST -UPSERT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductViewModel obj)
        {
            if (ModelState.IsValid) 
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if (obj.Product.Id == 0) 
                {
                    string upload = webRootPath + WebConstants.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (FileStream fileStream = new(
                        Path.Combine(upload, fileName + extension), FileMode.Create)) 
                    {
                        files[0].CopyTo(fileStream);
                    }
                    obj.Product.Image = fileName + extension;
                    _db.Products.Add(obj.Product);
                }
                else 
                {
                    var objFromDb = _db.Products.AsNoTracking().FirstOrDefault(u => u.Id == obj.Product.Id);

                    if (files.Count > 0) 
                    {
                        string upload = webRootPath + WebConstants.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);
                        var oldFile = Path.Combine(upload, objFromDb.Image);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }
                        using (FileStream fileStream = new(
                            Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }
                        obj.Product.Image = fileName + extension; 
                    }
                    else 
                    {
                        obj.Product.Image = objFromDb.Image;
                    }
                    _db.Products.Update(obj.Product);
                }
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            obj.CategorySelectList = _db.Categories.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            obj.ApplicationTypeSelectList = _db.ApplicationTypes.Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(obj);
        }

        //GET -DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product product = _db.Products.Include(u=>u.Category).Include(u => u.ApplicationType).FirstOrDefault(u=>u.Id==id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        //POST -DELETE
        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var obj = _db.Products.Find(id);
            if (obj == null)
            {
                return NotFound();
            }
            string upload = _webHostEnvironment.WebRootPath + WebConstants.ImagePath;
            var oldFile = Path.Combine(upload, obj.Image);
            if (System.IO.File.Exists(oldFile))
            {
                System.IO.File.Delete(oldFile);
            }
            _db.Products.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
