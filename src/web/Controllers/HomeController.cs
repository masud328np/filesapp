using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using dynata.web.Models;
using core.models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using web.Controllers;
using core.interfaces;
using System;

namespace dynata.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IContentProvider _provider;

        public HomeController(
            ILogger<HomeController> logger,
            IContentProvider provider
            )
        {

            _logger = logger;
            _provider = provider;
        }

        public async Task<IActionResult> IndexAsync()
        {
            var items = await _provider.GetAllAsync();

            ViewBag.Model = GetTree(items.Where(x => x.ParentId == null));
            ViewBag.menuItems = GetMenu();
            return View();
        }

        private IEnumerable<object> GetTree(IEnumerable<IFsEntity> fsEntities)
        {
            return fsEntities.Select(x => new
            {
                id = x.Id,
                name = x.Name,
                hasChildren = x.Children.Any(),
                type = x.Type.ToString(),
                size = x.Size
            });
        }

        private static List<object> GetMenu()
        {
            List<object> menuItems = new List<object>();

            menuItems.Add(new
            {
                text = "Add Folder",
            });
            menuItems.Add(new
            {
                text = "Delete",
            });

            menuItems.Add(new
            {
                text = "Download",
            });

            menuItems.Add(new
            {
                text = "Upload",
            });

            menuItems.Add(new
            {
                text = "Copy",
            });

            return menuItems;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
