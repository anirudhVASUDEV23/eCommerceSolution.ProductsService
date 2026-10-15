using eCommerce.BusinessLogicLayer.DTO;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.ProductsMicroService.API.ApiEndpoints;

public static class ProductAPIEndpoints
{
    public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
    {
        //GET /api/products
        app.MapGet("/api/products", async (IProductsService productsService) =>
        {
            List<ProductResponse?>products = await productsService.GetProducts();
            return Results.Ok(products);
        });

        //GET /api/products/search/product-id/xxxxxxxxxxxxxxxx
        app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IProductsService productsService,Guid ProductID) =>
        {
            ProductResponse? product = await productsService.GetProductByCondition(p => p.ProductID == ProductID);
            if (product == null) return Results.NotFound();
            return Results.Ok(product);
        });

        //GET /api/products/search/xxxxxxxxxxxxxxxx
        app.MapGet("/api/products/search/{SearchString}", async (IProductsService productsService, string SearchString) =>
        {
            var products = await productsService.GetProductsByCondition(p =>
           (p.ProductName != null &&
            EF.Functions.Like(p.ProductName, $"%{SearchString}%"))
           ||
           (p.Category != null &&
            EF.Functions.Like(p.Category, $"%{SearchString}%"))
   );
            return Results.Ok(products);
        });


        app.MapPost("/api/products", async (IProductsService productsService,IValidator<ProductAddRequest> validator, ProductAddRequest productRequest) =>
        {
            ValidationResult result= await validator.ValidateAsync(productRequest);
            if (!result.IsValid) 
            {
                Dictionary<string, string[]> errors = result.Errors.GroupBy(temp => temp.PropertyName).ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }
            ProductResponse? product = await productsService.AddProduct(productRequest);
            if(product == null)
            {
                return Results.Problem("There was a problem while adding the product");
            }
            return Results.Created($"/api/products/search/product-id/{product.ProductID}", product);
        });

        app.MapPut("/api/products", async (IProductsService productsService, IValidator<ProductUpdateRequest> validator, ProductUpdateRequest productRequest) =>
        {
            ValidationResult result = await validator.ValidateAsync(productRequest);
            if (!result.IsValid)
            {
                Dictionary<string, string[]> errors = result.Errors.GroupBy(temp => temp.PropertyName).ToDictionary(grp => grp.Key, grp => grp.Select(err => err.ErrorMessage).ToArray());
                return Results.ValidationProblem(errors);
            }
            ProductResponse? product = await productsService.UpdateProduct(productRequest);
            if (product == null)
            {
                return Results.Problem("There was a problem while updating the product");
            }
            return Results.Ok(product);
        });

        app.MapDelete("/api/products/{ProductID:guid}", async (IProductsService productsService,Guid ProductID) =>
        {
           bool isDeleted = await productsService.DeleProduct(ProductID);
            if (!isDeleted)
            {
                return Results.Problem("There was a problem while deleting the product");
            }
            return Results.Ok(true);
        });


        return app;
    }
}
