using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/")]
public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index(){
        return Ok("Bem Vindo");
    }
}