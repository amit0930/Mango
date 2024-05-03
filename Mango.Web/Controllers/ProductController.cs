using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
                _productService = productService;
        }
        public async Task<IActionResult> ProductIndex()
        {
            List<ProductDto?> productsList=new();

            ResponseDto? response = await _productService.GetAllProductsAsync();

            if(response!=null && response.IsSuccess)
            {
                productsList = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"]=response?.Message;
            }

            return View(productsList);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> ProductCreate(ProductDto product)
		{
            if(ModelState.IsValid)
            {
                ResponseDto? response=await _productService.AddProductAsync(product);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"]="Product created successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
				else
				{
					TempData["error"] = response?.Message;
				}
			}
			return View();
		}

		public async Task<IActionResult> ProductEdit(int id)
		{
			ResponseDto? response = await _productService.GetProductByIdAsync(id);
			if (response != null && response.IsSuccess)
			{
				ProductDto? model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
				return View(model);
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return NotFound();
		}

		[HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto product)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.UpdateProductAsync(product);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View();
        }

        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto? response = await _productService.GetProductByIdAsync(id);
            if(response!=null && response.IsSuccess)
            {
                ProductDto? model=JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
           return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto product)
        {
           
                ResponseDto? response = await _productService.DeleteProductAsync(product.ProductId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product deleted successfully!";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            
            return View(product);
        }
    }
}
