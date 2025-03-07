using EcommerceApi.Models.Dto.Products.Request;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public class ProductController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<ActionResult> Create(ProductRequestDto model){
        if (model == null){
            return NotFound();
    };
        if (String.IsNullOrEmpty(model.Name))
        {
            return BadRequest("Null name is Invalid");
        }

        if (model.Price < 0)
        {
            return BadRequest("Negative Number is Invalid");
        }


        var product = new Product(
        model.Name,
        model.Description,
        model.Price,
        model.ImageUrl,
        model.Category
        );

        await _unitOfWork.ProductRepository.Create(product);
        return Ok(product);

    }
    [HttpGet]
    public async Task<ActionResult> GetAsync(string? txt=null){
        var products = await _unitOfWork.ProductRepository.GetAsync(txt);
        var productDto = new List<ProductDto>();
        foreach (var product in products)
        {
            productDto.Add(new ProductDto 
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
            });
            
        }

        return Ok(productDto);
    }
    [HttpGet("categories/")]
    public async Task<ActionResult> GetByCategoryAsync(string? txt){
        var products = await _unitOfWork.ProductRepository.GetByCategory(txt);
        var productDto = new List<ProductDto>();
        foreach (var product in products)
        {
            productDto.Add(new ProductDto 
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Category = product.Category,
                
            });
            
        }

        return Ok(productDto);
    }

    [HttpGet("id")]
    public async Task<ActionResult> GetByIdAsync(Guid id){
        if(String.IsNullOrEmpty(id.ToString())){
            return BadRequest("Id empty is inavlid");
        }

        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if(product == null){
            return NotFound("Id not Exists");
        }
        var productDto = new ProductDto{
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Category = product.Category
        };
        Console.WriteLine(productDto.Id);
        return Ok(productDto);
    }

    [HttpPut]
    public async Task<ActionResult> Update(Guid id, ProductRequestDto productDto){

        if (String.IsNullOrEmpty(id.ToString())){
            return BadRequest("Product not exist");
        }
        var dbproduct = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if (dbproduct == null)
        {
            return NotFound("Product not found");
        }
        if (!String.IsNullOrEmpty(productDto.Name))
        {
            dbproduct.SetName(productDto.Name);
        }
        if (!String.IsNullOrEmpty(productDto.Description))
        {
            dbproduct.SetDescription(productDto.Description);
        }
        try
        {
            dbproduct.SetPrice(productDto.Price);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex);
        }
        
        if (!String.IsNullOrEmpty(productDto.ImageUrl))
        {
            dbproduct.SetImage(productDto.ImageUrl);
        }
        if (!String.IsNullOrEmpty(productDto.Category))
        {
            dbproduct.SetCategory(productDto.Category);
        }

        await _unitOfWork.ProductRepository.Update(dbproduct);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id){
        var product = await _unitOfWork.ProductRepository.GetByIdAsync(id);
        if(product == null){
            return NotFound("Id not Found");
        }
        await _unitOfWork.ProductRepository.Delete(product);
        return Ok(product);
    }

}