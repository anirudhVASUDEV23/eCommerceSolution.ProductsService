using eCommerce.DataAccessLayer.Entities;
using eCommerce.BusinessLogicLayer.DTO;
using System.Linq.Expressions;

namespace eCommerce.BusinessLogicLayer.ServiceContracts;

public interface IProductsService
{
    Task<List<ProductResponse?>> GetProducts();

    Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression);

    Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression);

    Task<ProductResponse?> AddProduct(ProductAddRequest productRequest);
    
    Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productRequest);

    Task<bool> DeleProduct(Guid productID);
}
