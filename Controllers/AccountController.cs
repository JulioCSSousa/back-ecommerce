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
        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.ToList();
        if (user == null)
        {
            return BadRequest();
        }

        if (roles == null)
        {
            return BadRequest("Role não encontrado");
        }

        var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            
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
            // Não expor se o e-mail está correto ou não por motivos de segurança
            return RedirectToAction("Login", "Account");
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
            return BadRequest("Token inválido.");
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
        List<UserWithRolesDto> userWithRoles = new List<UserWithRolesDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            userWithRoles.Add(new UserWithRolesDto
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
        // Obter todos os usuários se o campo de buscar não for preenchido se não recuperar
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
                Name = user.Name,
                UserName = user.UserName,
                Email = user.Email,
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
            return NotFound("Usuário não encontrado.");
        }


        return Ok(user); // Exibe a página de atualização com os dados do usuário
    }


    [HttpPost("Update/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(string id, UserResponseDto model)
    {
        if (id != model.Id)
        {
            return BadRequest("IDs não correspondem.");
        }
        // Localiza o usuário
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
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
            return BadRequest("ID do usuário não pode ser nulo ou vazio.");
        }
        
        var userFind = await _userManager.FindByIdAsync(id);
        if (userFind == null)
        {
            return NotFound($"Usuário não encontrado. {id}");
        }

        var result = await _userManager.DeleteAsync(userFind);

        if (result.Succeeded)
        {
            TempData["SuccessMessage"] = "Usuário excluído com sucesso!";
        }
        else
        {
            TempData["ErrorMessage"] = "Erro ao excluir o usuário.";
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
            return BadRequest($"Usuário ou role inválidos.");
        }

        // Localiza o usuário pelo ID
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        // Verifica se a role já existe
        var roleExists = await _roleManager.RoleExistsAsync(role);
        if (!roleExists)
        {
            return BadRequest("A role especificada não existe.");
        }
        var currentRoles = await _userManager.GetRolesAsync(user);
        var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded)
        {
            return BadRequest("Erro ao remover roles antigas do usuário.");
        }

        // Adiciona a role ao usuário
        var result = await _userManager.AddToRoleAsync(user, role);

        if (!result.Succeeded)
        {
            return BadRequest("Erro ao adicionar a role ao usuário.");
        }

        return RedirectToAction("Index"); // Retorna para a lista de usuários ou outra página
    }

}
