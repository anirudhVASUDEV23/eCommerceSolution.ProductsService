using AutoMapper;
using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using eCommerce.DataAccessLayer.Entities;
using eCommerce.DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using System.Linq.Expressions;

namespace BusinessLogicLayer.Services
{
    public class ProductsService : IProductsService
    {
        private readonly IValidator<ProductAddRequest> _productAddValidator;
        private readonly IValidator<ProductUpdateRequest> _productUpdateValidator;

        private readonly IMapper _mapper;
        private readonly IProductsRepository _productsRepository;

        public ProductsService(IValidator<ProductAddRequest> productAddValidator, IValidator<ProductUpdateRequest> productUpdateValidator, IMapper mapper, IProductsRepository productsRepository)
        {
            _productAddValidator = productAddValidator;
            _productUpdateValidator = productUpdateValidator;
            _mapper = mapper;
            _productsRepository = productsRepository;
        }

        public async Task<ProductResponse?> AddProduct(ProductAddRequest productRequest)
        {
            if(productRequest==null)
            {
                throw new ArgumentNullException(nameof(productRequest));
            }

            // Validate the productRequest using FluentValidation
            ValidationResult validationResult = await _productAddValidator.ValidateAsync(productRequest);
            if(!validationResult.IsValid)
            {
                string errors= string.Join(",", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            Product productInput = _mapper.Map<Product>(productRequest);
            Product? addedProduct= await _productsRepository.AddProduct(productInput);
            if (addedProduct == null) return null;
            return _mapper.Map<ProductResponse>(addedProduct);
        }

        public async Task<bool> DeleProduct(Guid productID)
        {
            Product? product = await _productsRepository.GetProductByCondition(temp => temp.ProductID == productID);
            if(product == null) return false;
            return await _productsRepository.DeleteProduct(product.ProductID);
        }

        public async Task<ProductResponse?> GetProductByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            Product? product = await _productsRepository.GetProductByCondition(conditionExpression);
            return product == null ? null : _mapper.Map<ProductResponse>(product);  
        }

        public async Task<List<ProductResponse?>> GetProducts()
        {
            IEnumerable<Product?> products = await _productsRepository.GetProducts();
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
            return productResponses.ToList();
        }

        public async Task<List<ProductResponse?>> GetProductsByCondition(Expression<Func<Product, bool>> conditionExpression)
        {
            IEnumerable<Product?> products = await _productsRepository.GetProductsByCondition(conditionExpression);
            IEnumerable<ProductResponse?> productResponses = _mapper.Map<IEnumerable<ProductResponse?>>(products);
            return productResponses.ToList();
        }

        public async Task<ProductResponse?> UpdateProduct(ProductUpdateRequest productRequest)
        {
            Product? existingProduct = await _productsRepository.GetProductByCondition(temp => temp.ProductID == productRequest.ProductID);
            if(existingProduct == null)
            {
                throw new ArgumentException($"Product with ID {productRequest.ProductID} does not exist.");
            }
            ValidationResult validationResult = await _productUpdateValidator.ValidateAsync(productRequest);
            if(!validationResult.IsValid)
            {
                string errors= string.Join(",", validationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }
            //Map the updated fields from the request to the existing product entity
            Product product= _mapper.Map<Product>(productRequest);
            Product? updatedProduct = await _productsRepository.UpdateProduct(product);
            return updatedProduct == null ? null : _mapper.Map<ProductResponse>(updatedProduct);
        }
    }
}
