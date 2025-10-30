using AutoMapper;
using AutoMapper.QueryableExtensions;
using GabriniCosmetics.Areas.Admin.Models;
using GabriniCosmetics.Areas.Admin.Models.DTOs;
using GabriniCosmetics.Areas.Admin.Models.Interface;
using GabriniCosmetics.Data;
using GabriniCosmetics.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace GabriniCosmetics.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly GabriniCosmeticsContext _context;
        private readonly IProduct _productService;
        private readonly ICategory _category;
        private readonly ISubcategory _subcategory;
        private readonly ILogger<ProductsController> _logger;
        private readonly IFlagService _flagService;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IMapper _mapper;


        public ProductsController(GabriniCosmeticsContext context,IProduct productService, ILogger<ProductsController> logger, ISubcategory subcategory, ICategory category, IFlagService flagService, ICompositeViewEngine viewEngine,IMapper mapper)
        {
            _context = context;
            _productService = productService;
            _logger = logger;
            _subcategory = subcategory;
            _category = category;
            _flagService = flagService;
            _viewEngine = viewEngine;
            _mapper = mapper;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string searchTerm = "")
        {
            var products = await _productService.GetProducts();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.NameEn.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) || p.NameAr.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant())).ToList();

            }
            return View(products);
        }

        // GET: Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            var flags = await _flagService.GetAllFlagsAsync();
            ViewBag.Flags = new SelectList(flags, "Id", "Name");

            var categories = await _category.GetCategories();
            ViewBag.Categories = categories;
            //var subcategories = await _subcategory.GetSubcategories();

            //var subcategorySelectList = new List<SelectListItem>();

            //foreach (var category in categories)
            //{
            //    var group = new SelectListGroup { Name = category.NameEn };

            //    var subcategoryItems = subcategories
            //        .Where(sc => sc.CategoryId == category.Id)
            //        .Select(sc => new SelectListItem
            //        {
            //            Text = sc.NameEn,
            //            Value = sc.Id.ToString(),
            //            Group = group
            //        }).ToList();

            //    subcategorySelectList.AddRange(subcategoryItems);
            //}

            //ViewBag.Subcategories = subcategorySelectList;


            return View(new ProductForm());

        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductForm createProductDto)
        {
            int order = 0;
            foreach (var subproduct in createProductDto.Subproducts)
            {
                subproduct.Image = FileConverters.ConvertBase64ToIFormFile(subproduct?.Base64Image);
                subproduct.Order = order++;
            }

            createProductDto.Flags = new List<int>();
            
            if (createProductDto.IsSale)
            {
                var flag = await _flagService.GetFlagByNameAsync("Sale");
                createProductDto.Flags.Add(flag.Id);
            }

            if (createProductDto.FlagNames.First() != null)
            {
                // Define a class that matches the JSON structure
                var parsedFlagNames = JsonSerializer.Deserialize<List<FlagConvert>>(createProductDto.FlagNames.First());

                // Extract values
                var extractedValues = parsedFlagNames?.ConvertAll(flag => flag.value);
                foreach (var item in extractedValues)
                {
                    var flag = await _flagService.GetFlagByNameAsync(item);
                    createProductDto.Flags.Add(flag.Id);
                }
            }
            else if(!createProductDto.Subproducts.Any() || createProductDto.Subproducts.Any(sp => sp.Image is null) || createProductDto.SubcategoryId == 0)
            {
                var flags = await _flagService.GetAllFlagsAsync();
                ViewBag.Flags = new SelectList(flags, "Id", "Name");

                var categories = await _category.GetCategories();
                ViewBag.Categories = categories;

                //var subcategories = await _subcategory.GetSubcategories();

                //var subcategorySelectList = new List<SelectListItem>();

                //foreach (var category in categories)
                //{
                //    var group = new SelectListGroup { Name = category.NameEn };

                //    var subcategoryItems = subcategories
                //        .Where(sc => sc.CategoryId == category.Id)
                //        .Select(sc => new SelectListItem
                //        {
                //            Text = sc.NameEn,
                //            Value = sc.Id.ToString(),
                //            Group = group
                //        }).ToList();

                //    subcategorySelectList.AddRange(subcategoryItems);
                //}

                //ViewBag.Subcategories = subcategorySelectList;
                return View(createProductDto);
            }
            await _productService.CreateProduct(createProductDto);
            return Redirect("/Admin/Products");
        }

        // GET: Admin/Products/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var flags = await _flagService.GetAllFlagsAsync();
            ViewBag.Flags = new SelectList(flags, "Id", "Name");


            var categories = await _category.GetCategories();
            ViewBag.Categories = categories;

            var product = await _productService.Queryable()
                .Include(p => p.Subproducts).ThenInclude(sp => sp.Image)
                .Include(p => p.Subcategory)
                .Include(p => p.Flags)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var subproducts = product.Subproducts.Select(sp => new SubprodcutForm {
                Id = sp.Id,
                DescriptionAr = sp.DescriptionAr,
                DescriptionEn = sp.DescriptionEn,
                IsAvailability = sp.IsAvailability,
                ExistingImagePath = sp.Image.ImagePath,
                Image = null,
                Order = sp.Order
            }).OrderBy(sp => sp.Order).ToList();

            //foreach(SubprodcutForm subproduct in subproducts)
            //{
            //    IFormFile file = FileConverters.ConvertImageToIFormFile($"~/uploads/{subproduct.ExistingImagePath}");
            //    subproduct.Image = file;
            //    subproduct.Base64Image = FileConverters.ConvertIFormFileToBase64(file);
            //}

            var updateProductDto = new ProductForm
            {
                Id = product.Id,
                NameEn = product.NameEn,
                NameAr = product.NameAr,
                DescriptionEn = product.DescriptionEn,
                DescriptionAr = product.DescriptionAr,
                PersantageSale = product.PersantageSale,
                PriceAfterDiscount = product.PriceAfterDiscount,
                Price = product.Price,
                Subproducts = subproducts,
                Flags = product.Flags.Select(f => f.Id).ToList(),                
                _Flags = product.Flags.ToList(),
                SubcategoryId = product.SubcategoryId,
                FlagNames = product.Flags.Select(f => f.FlagType).ToList(),
                FlagNamesString = string.Join(',', product.Flags.Select(f => f.FlagType).ToList()),
                IsSale = product.IsSale,
                IsDealOfDay = product.IsDealOfDay,
            };

            return View(updateProductDto);
        }

        // POST: Admin/Products/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductForm updateProductDto)
        {
            foreach (var subproduct in updateProductDto.Subproducts.Where(sp => !sp.Base64Image.IsNullOrEmpty() && sp.ExistingImagePath.IsNullOrEmpty()))
            {
                subproduct.Image = FileConverters.ConvertBase64ToIFormFile(subproduct?.Base64Image);
            }

            for(int i = 0; i < updateProductDto.Subproducts.Count; i++)
            {
                updateProductDto.Subproducts[i].Order = i;
            }

            updateProductDto.Flags = new List<int>();

            if (updateProductDto.IsSale)
            {
                var flag = await _flagService.GetFlagByNameAsync("Sale");
                updateProductDto.Flags.Add(flag.Id);
            }
            else
            {
                updateProductDto.PersantageSale = null;
                updateProductDto.PriceAfterDiscount = null;
            }

            if (updateProductDto.FlagNames.First() != null)
            {
                // Define a class that matches the JSON structure
                var parsedFlagNames = JsonSerializer.Deserialize<List<FlagConvert>>(updateProductDto.FlagNames.First());

                // Extract values
                var extractedValues = parsedFlagNames?.ConvertAll(flag => flag.value);
                foreach (var item in extractedValues)
                {
                    var flag = await _flagService.GetFlagByNameAsync(item);
                    updateProductDto.Flags.Add(flag.Id);
                }
            }
            else
            {
                updateProductDto.FlagNames = null;
            }
            if (!updateProductDto.Subproducts.Any())
            {
                var flags = await _flagService.GetAllFlagsAsync();
                ViewBag.Flags = new SelectList(flags, "Id", "Name");

                var categories = await _category.GetCategories();
                ViewBag.Categories = categories;

                //var subcategories = await _subcategory.GetSubcategories();

                //var subcategorySelectList = new List<SelectListItem>();

                //foreach (var category in categories)
                //{
                //    var group = new SelectListGroup { Name = category.NameEn };

                //    var subcategoryItems = subcategories
                //        .Where(sc => sc.CategoryId == category.Id)
                //        .Select(sc => new SelectListItem
                //        {
                //            Text = sc.NameEn,
                //            Value = sc.Id.ToString(),
                //            Group = group
                //        }).ToList();

                //    subcategorySelectList.AddRange(subcategoryItems);
                //}

                //ViewBag.Subcategories = subcategorySelectList;
                return View(updateProductDto);
            }
            await _productService.UpdateProduct(updateProductDto);
            return Redirect("/Admin/Products");

        }

        // POST: Admin/Products/Delete/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteProduct([FromBody] int id)
        {
            var success = await _productService.DeleteProduct(id);
            if (!success)
            {
                return NotFound();
            }
            return Ok();
        }

        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductById(id);

            // Render the partial view to HTML
            var quickViewHtml = await RenderPartialViewToStringAsync("_QuickView", product);

            return Json(new { quickViewHtml });
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products.ProjectTo<ProductView>(_mapper.ConfigurationProvider).SingleOrDefaultAsync(p => p.Id == id);
            return View(product);
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSubproductAsync(List<SubprodcutForm> subproducts, SubprodcutForm subproduct)
        {
            if (subproduct != null)
            {
                // Add the new subproduct to the list
                subproducts.Add(subproduct);
            }

            // Return the view with the updated model
            var renderedPartialView = await RenderPartialViewToStringAsync("_SubproductsForm", subproducts);
            return Json(new { renderedPartialView });
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveSubproductAsync(List<SubprodcutForm> subproducts, int index)
        {
            if (index >= 0 && index < subproducts.Count)
            {
                // Remove the subproduct from the list
                subproducts.RemoveAt(index);
            }

            // Return the view with the updated model
            var renderedPartialView = await RenderPartialViewToStringAsync("_SubproductsForm", subproducts);
            return Json(new { renderedPartialView });
        }
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSubproductAsync(List<SubprodcutForm> subproducts, SubprodcutForm subproduct, int index)
        {
            if (index >= 0 && index < subproducts.Count)
            {
                // Remove the subproduct from the list
                subproducts.RemoveAt(index);
                subproducts.Add(subproduct);
            }

            // Return the view with the updated model
            var renderedPartialView = await RenderPartialViewToStringAsync("_SubproductsForm", subproducts);
            return Json(new { renderedPartialView });
        }

        private async Task<string> RenderPartialViewToStringAsync(string viewName, object model)
        {
            // Create a new ViewDataDictionary to hold the model
            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            // Create a new ViewContext
            using (var sw = new StringWriter())
            {
                var actionContext = new ActionContext(ControllerContext.HttpContext, ControllerContext.RouteData, ControllerContext.ActionDescriptor);
                var viewResult = _viewEngine.FindView(actionContext, viewName, false);

                if (!viewResult.Success)
                {
                    throw new InvalidOperationException($"Couldn't find view {viewName}");
                }

                var viewContext = new ViewContext(
                    actionContext,
                    viewResult.View,
                    viewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);
                // Get the rendered string
                var renderedContent = sw.GetStringBuilder().ToString();

                // Remove <script> tags using a regular expression
                var sanitizedContent = Regex.Replace(renderedContent, "<script[^>]*>.*?</script>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);

                return sanitizedContent;
            }
        }
    }

}
