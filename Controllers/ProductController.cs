using EcommerceApi.Models.Dto.Products.Request;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
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
        model.ImageUrl);

        await _productService.Create(product);
        return Ok(product);


       
    }
    [HttpGet]
    public async Task<ActionResult> GetAsync(){
        return Ok(await _productService.GetAsync());
    }
    [HttpGet("id")]
    public async Task<ActionResult> GetByIdAsync(Guid id){
        var product = await _productService.GetByIdAsync(id);
        if(product == null){
            return NotFound("Id not Exists");
        }
        return Ok(product);
    }
    [HttpPut]
    public async Task<ActionResult> Update(Guid id, ProductRequestDto productDto){

        if (String.IsNullOrEmpty(id.ToString())){
            return BadRequest("Product not exist");
        }
        var dbproduct = await _productService.GetByIdAsync(id);
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

        await _productService.Update(dbproduct);
        return Ok();
    }

    [HttpDelete]
    public async Task<ActionResult> Delete(Guid id){
        var product = await _productService.GetByIdAsync(id);
        if(product == null){
            return NotFound("Id not Found");
        }
        await _productService.Delete(product);
        return Ok(product);
    }

}