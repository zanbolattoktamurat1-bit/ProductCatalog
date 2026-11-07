using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProductCatalog.Models;

namespace ProductCatalog.Controllers;

public class ProductController : Controller
{
    private static readonly List<Product> Products =
    [
        new() { Id = 1, Name = "Ноутбук", Price = 459990, Category = "Компьютерлер", IsAvailable = true },
        new() { Id = 2, Name = "Смартфон", Price = 289990, Category = "Ұялы құрылғылар", IsAvailable = true },
        new() { Id = 3, Name = "Құлаққап", Price = 34990, Category = "Аксессуарлар", IsAvailable = true },
        new() { Id = 4, Name = "Пернетақта", Price = 24990, Category = "Аксессуарлар", IsAvailable = false },
        new() { Id = 5, Name = "Тінтуір", Price = 12990, Category = "Аксессуарлар", IsAvailable = true },
        new() { Id = 6, Name = "Монитор", Price = 179990, Category = "Компьютерлер", IsAvailable = true },
        new() { Id = 7, Name = "Планшет", Price = 199990, Category = "Ұялы құрылғылар", IsAvailable = false },
        new() { Id = 8, Name = "Смарт сағат", Price = 89990, Category = "Тағылатын құрылғылар", IsAvailable = true }
    ];

    private static int _nextId = 8;

    public IActionResult Index(string? search, string? category)
    {
        List<Product> products;
        lock (Products)
        {
            products = Products.ToList();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            products = products
                .Where(product => product.Name.Contains(search.Trim(), StringComparison.CurrentCultureIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            products = products
                .Where(product => product.Category.Equals(category, StringComparison.Ordinal))
                .ToList();
        }

        ViewBag.CatalogTitle = "Тауарлар каталогы";
        ViewBag.Categories = GetCategories();
        ViewBag.SelectedCategory = category;
        ViewBag.Search = search;
        ViewData["Title"] = "Каталог";

        return View(products);
    }

    public IActionResult Details(int id)
    {
        Product? product;
        lock (Products)
        {
            product = Products.FirstOrDefault(item => item.Id == id);
        }

        if (product is null)
        {
            return NotFound();
        }

        ViewData["Title"] = product.Name;
        return View(product);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateCategoryOptions();
        ViewData["Title"] = "Жаңа тауар қосу";
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            PopulateCategoryOptions();
            ViewData["Title"] = "Жаңа тауар қосу";
            return View(product);
        }

        lock (Products)
        {
            product.Id = ++_nextId;
            Products.Add(product);
        }

        TempData["SuccessMessage"] = "Жаңа тауар каталогқа сәтті қосылды.";
        return RedirectToAction(nameof(Index));
    }

    private static List<string> GetCategories()
    {
        lock (Products)
        {
            return Products
                .Select(product => product.Category)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(category => category, StringComparer.CurrentCulture)
                .ToList();
        }
    }

    private void PopulateCategoryOptions()
    {
        ViewBag.CategoryOptions = GetCategories()
            .Select(category => new SelectListItem(category, category))
            .ToList();
    }
}
