using Domain.Responses;
using Microsoft.AspNetCore.Http;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;

namespace Infrastructure.Interfaces;

public interface IProductService
{
    Task<PagedResponse<List<GetProductDto>>> GetAllProductsAsync(ProductFilter filter);
    Task<Response<GetProductDto>> GetProductByIdAsync(int id);
    Task<Response<GetProductDto>> AddProductAsync(CreateProductDto productDto);
    Task<Response<GetProductDto>> UpdateProductAsync(int id, UpdateProductDto productDto);
    Task<Response<string>> DeleteProductAsync(int id);
    Task<Response<string>> UploadProductImageAsync(int productId, IFormFile file);
    Task<Response<string>> DeleteProductImageAsync(int productId);
    // Task<Response<GetProductDto>> GetProductByBarcodeAsync(string barcode);
    // Task<Response<bool>> IsBarcodeTakenAsync(string barcode);
}