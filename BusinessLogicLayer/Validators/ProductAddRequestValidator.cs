
using eCommerce.BusinessLogicLayer.DTO;
using FluentValidation;

namespace eCommerce.BusinessLogicLayer.Validators;

public class ProductAddRequestValidator:AbstractValidator<ProductAddRequest>
{
    public ProductAddRequestValidator()
    {
        //ProductName
        RuleFor(temp=>temp.ProductName).NotEmpty().WithMessage("Product name is required")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

        //Category
        RuleFor(temp=>temp.Category).IsInEnum().WithMessage("Invalid category");

        //UnitPrice
        RuleFor(temp => temp.UnitPrice).InclusiveBetween(0,double.MaxValue).WithMessage("Unit price must be a non-negative value");

        //QuantityInStock
        RuleFor(temp=>temp.QuantityInStock).InclusiveBetween(0, int.MaxValue).WithMessage("Quantity in stock must be a non-negative value");
    }
}
