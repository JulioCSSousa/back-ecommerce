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
    [AllowAnonymous]
    public async Task<IActionResult> Register(UserRegisterDto model)
{
    // Verifique se já existe um usuário
    var usersExist = await _userManager.Users.AnyAsync();
    
    // Se não houver nenhum usuário, cria um com o papel Admin
    if (!usersExist)
    {
        var user = new User(model.Name, model.Email, model.PhoneNumber)
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordTooShort")
                    return BadRequest("Password is too short");
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Admin");
        if (!roleResult.Succeeded)
        {
            return BadRequest("Failed to assign role to user.");
        }

        return Ok("User created and assigned 'Admin' role.");
    }
    else
    {
        // Verifique se o usuário já existe com o e-mail fornecido
        var userExists = await _userManager.FindByEmailAsync(model.Email);
        if (userExists != null)
        {
            return BadRequest("This user already exists");
        }

        // Cria o usuário com o papel Cliente
        var newUser = new User(model.Name, model.Email, model.PhoneNumber)
        {
            UserName = model.Email,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber
        };

        var result = await _userManager.CreateAsync(newUser, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                if (error.Code == "PasswordTooShort")
                    return BadRequest("Password is too short");
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return BadRequest(ModelState);
        }

        var roleResult = await _userManager.AddToRoleAsync(newUser, "Cliente");
        if (!roleResult.Succeeded)
        {
            return BadRequest("Failed to assign role to user.");
        }

        return Ok("User created and assigned 'Cliente' role.");
    }
}



    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(UserLoginDto model, TokenService tokenService)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        var result = await _signInManager.PasswordSignInAsync(
            user, model.Password, model.RememberMe, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = tokenService.GenerateToken(user, roles);
        var logUser = new UserResponseDtoLogin
        { 
            Name = user.Name,
            Email = model.Email,
            Role = roles.FirstOrDefault(),
            Token = token 
        };

        return Ok(logUser);
    }


    [HttpPost("password-recovery")]
    [AllowAnonymous]
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


    [HttpPost("Update/{id}")]
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
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest("Invalid Id");
        }
        
        var userFind = await _userManager.FindByIdAsync(id);
        if (userFind == null)
        {
            return NotFound($"User not Found");
        }

        var result = await _userManager.DeleteAsync(userFind);

        if (result.Succeeded)
        {
            return Ok("User successfully deleted");
        }

        return BadRequest("Something wrong");
    }


    // POST: /Account/Logout
    [HttpPost("logout")]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Bye");
    }

    [HttpGet("roles")]
    public async Task<IActionResult> GetRole()
    {
        var users = await _userManager.Users.ToListAsync();
        if (users == null)
        {
            return BadRequest("No Users Yet");
        }

        var roles = new List<Dictionary<string, string>>();
        var userAndRoles = new Dictionary<string, string>();
        foreach (var user in users){
            var role = await _userManager.GetRolesAsync(user);
            userAndRoles.Add(user.Name, role.FirstOrDefault() ?? "");
            roles.Add(userAndRoles);
        }
        return Ok(userAndRoles);
    }

    [HttpPost("role")]
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