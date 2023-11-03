using Inveon.Web.Hubs;
using Inveon.Web.Models;
using Inveon.Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Inveon.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IHubContext<ChatHub> _chatHub;
        public HomeController(ILogger<HomeController> logger, IProductService productService, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            _logger = logger;
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto> list = new();
            var response = await _productService.GetAllProductsAsync<ResponseDto>("");
            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            return View(list);

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

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var role = User.Claims.Where(u => u.Type == "role")?.FirstOrDefault()?.Value;
            if (role == "Admin")
            {
                // return Redirect("~/Admin/Yonetici");
                return RedirectToAction("Git", "Yonetici", new { area = "Admin" });
            }
            //buradan IdentityServer daki login sayfasına gidiliyor.
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }

        [Route("Message")]
        [HttpPost]
        public IActionResult Message([FromBody] Message message)
        {
            _chatHub.Clients.All.SendAsync("lastMessage", message);
            return Accepted();
        }
    }
}