using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using Microsoft.AspNetCore.Mvc;
using System;

namespace GabriniCosmetics.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProduct _products;
        private readonly ISubcategory _subcategory;
        private readonly ICategory _category;

        public SearchController(IProduct products, ISubcategory subcategory, ICategory category)
        {
            _products = products;
            _category = category;
            _subcategory = subcategory;
        }
        public async Task<IActionResult> Index(string searchTerm = "", string sortOrder = "")
        {
            var listOfProduct = await _products.GetProducts();

            // Apply the search based on the searchTerm parameter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                listOfProduct = listOfProduct.Where(p => p.NameEn.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) || p.NameAr.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant())).ToList();
            }

            // Pass the filtered products and searchTerm to the view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.Count = listOfProduct.Count;
            // Apply the sorting based on the sortOrder parameter
            switch (sortOrder)
            {
                case "lowToHigh":
                    listOfProduct = listOfProduct.OrderBy(p => p.Price).ToList();
                    break;
                case "highToLow":
                    listOfProduct = listOfProduct.OrderByDescending(p => p.Price).ToList();
                    break;
                case "aToZ":
                    listOfProduct = listOfProduct.OrderBy(p => p.NameEn).ToList();
                    break;
                case "zToA":
                    listOfProduct = listOfProduct.OrderByDescending(p => p.NameEn).ToList();
                    break;
                default:
                    // Handle default case or no sorting
                    break;
            }
            foreach (var item in listOfProduct)
            {
                var subcategory = await _subcategory.GetSubcategoryById(item.SubcategoryId);
                var category = await _category.GetCategoryById(subcategory.CategoryId);
                item.Subcategory = new SubcategoryView()
                {
                    NameAr = subcategory.NameAr,
                    NameEn = subcategory.NameEn,
                    CategoryId = subcategory.CategoryId,
                    Category = new CategoryView()
                    {
                        NameEn = category.NameEn,
                        NameAr = category.NameAr,
                    }
                };
            }

            return View(listOfProduct);
        }
    }
}
