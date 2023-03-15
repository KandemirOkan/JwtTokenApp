using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtTokenApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        public List<string> productList = new List<string>() { "Phone", "Desktop Computer", "Tablet", "Notebook", "Accessory" };
        [HttpGet("[Action]")]
        public List<string> GetProductList()
        {
            try
            {
                return productList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
