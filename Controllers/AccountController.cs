using appointmentApp.Models.Entities;
using EcommerceApi.Models.Dto.UserDto.Request;
using EcommerceApi.Models.Dto.UserDto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/accounts")]
public class AccountController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<User> userManager, 
        SignInManager<User> signInManager, 
        RoleManager<IdentityRole> roleManager
        )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;

    }


    // POST: /Account/Register
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto model)
    {


            var users = await _userManager.Users.AnyAsync();

            if (!users)
            {
                var user = new User(model.Name, model.Email, model.PhoneNumber)
                {
                    UserName = model.Email
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    return Ok();

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

            }
            else
            {
                var user = new User(model.Name, model.Email, model.PhoneNumber)
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber
                };
                var result = await _userManager.CreateAsync(user, model.Password);
                await _userManager.AddToRoleAsync(user, "Cliente");
                if (result.Succeeded)
                {
                    return Ok();

                }
            
        }

        return Ok();
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto model, [FromServices] TokenService tokenService)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null){
            return BadRequest("User not found");
        }
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.ToList();
        
        if (user == null)
        {
            return BadRequest("User not Found");
        }

        if (roles == null)
        {
            return BadRequest("Role not Found");
        }

        var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded){

            return BadRequest("Invalid Password or Username");
        }
        var token = tokenService.GenerateToken(user, roles);

        return Ok(new { token });
    }

    [HttpPost("password-recovery")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PasswordRecovery([FromForm] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            // N�o expor se o e-mail est� correto ou n�o por motivos de seguran�a
            return NotFound("User not Found");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var callbackUrl = Url.Action(
            "PasswordReset",
            "Account",
            new { token = token, email = user.Email },
            protocol: Request.Scheme);


        return Ok();
    }


    [HttpPost("password-reset")]
    [AllowAnonymous]
    public IActionResult PasswordReset(string email, string token)
    {
        if (token == null || email == null)
        {
            return BadRequest("Invalid Token");
        }

        var model = new PasswordResetDto { Token = token, Email = email };
        return Ok(model);
    }

[HttpPost("pwd-reset")]
public async Task<IActionResult> PasswordReset(PasswordResetDto model)
{

    var user = await _userManager.FindByEmailAsync(model.Email);
    if (user == null)
    {
        return RedirectToAction("PasswordResetConfirmation", "Account");
    }

    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
    if (result.Succeeded)
    {
        return RedirectToAction("PasswordResetConfirmation", "Account");
    }

    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }

    return Ok(model);
}

  

    [HttpGet("get-users")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers()
    {
        var users = _userManager.Users.ToList();
        List<UserResponseDto> userWithRoles = new List<UserResponseDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            userWithRoles.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Role = roles.ToList().First()
                }
            );
        }

        return Ok(userWithRoles);
    }

    [HttpGet("get-user/serachterms")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUsers(string? searchTerm = null)
    {
        // Obter todos os usu�rios se o campo de buscar n�o for preenchido se n�o recuperar
        //de acordo com a consulta

        var users = string.IsNullOrEmpty(searchTerm)
           ? await _userManager.Users.ToListAsync() 
           : await  _userManager.Users
           .Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm))
           .ToListAsync();


        List<UserResponseDto> usersWithRoles = new List<UserResponseDto>();
        foreach(var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            roles.ToList();

            var item = new UserResponseDto
            {
                Id = user.Id.ToString(),
                Name = user.Name,
                Email = user.Email ?? throw new NullReferenceException("Email not Founded"),
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault()
            };

            usersWithRoles.Add(item);

        }
        return Ok(usersWithRoles);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usu�rio n�o encontrado.");
        }


        return Ok(user); // Exibe a p�gina de atualiza��o com os dados do usu�rio
    }


    [HttpPost("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, UserResponseDto model)
    {
        if (id != model.Id)
        {
            return BadRequest("IDs n�o correspondem.");
        }
        // Localiza o usu�rio
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usu�rio n�o encontrado.");
        }

        // Atualiza os campos
        if (!string.IsNullOrEmpty(model.Name))
        {
            user.SetName(model.Name);
        }


        if (!string.IsNullOrEmpty(model.Email))
        {
            user.Email = model.Email;
        }


        if (!string.IsNullOrEmpty(model.PhoneNumber))
        {
            user.PhoneNumber = model.PhoneNumber;
        }
        
        // Atualiza no banco
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(result); 
    }


    [HttpDelete("id")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("ID do usu�rio n�o pode ser nulo ou vazio.");
        }
        
        var userFind = await _userManager.FindByIdAsync(id);
        if (userFind == null)
        {
            return NotFound($"Usu�rio n�o encontrado. {id}");
        }

        var result = await _userManager.DeleteAsync(userFind);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Usu�rio exclu�do com sucesso!";
        }
        else
        {
            TempData["ErrorMessage"] = "Erro ao excluir o usu�rio.";
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return Ok();
    }


    // POST: /Account/Logout
    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet("roles")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRole()
    {
        var user = await _userManager.Users.ToListAsync();
        if (user == null)
        {
            return BadRequest();
        }

        return View(user);
    }

    [HttpPost("role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddRole(string id , string role )
    {
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(role))
        {
            return BadRequest($"Invalid Role");
        }

        // Localiza o usu�rio pelo ID
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("User not Found");
        }

        // Verifica se a role j� existe
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            return BadRequest("This role doesn't exits");
        }
        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return BadRequest("Erro to remove old role from the user");
        }

        // Adiciona a role ao usu�rio
        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            return BadRequest("Error to add a role to the user");
        }

        return RedirectToAction("Index"); // Retorna para a lista de usu�rios ou outra p�gina
    }

};