using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface IProductService
    {
       Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> GetProductByIdAsync(int id);

        Task<ResponseDto?> UpdateProductAsync(ProductDto product);
        Task<ResponseDto?> AddProductAsync(ProductDto product);
        Task<ResponseDto?> DeleteProductAsync(int id);
    }
}
