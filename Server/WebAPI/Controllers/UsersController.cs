using ApiContracts.Users;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository userRepository;

    public UsersController(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> AddUser(
        [FromBody] CreateUserDto request)
    {
        bool usernameTaken = userRepository
            .GetMany()
            .Any(u => u.UserName.Equals(
                request.UserName,
                StringComparison.OrdinalIgnoreCase));

        if (usernameTaken)
        {
            return BadRequest("Username is already taken.");
        }

        User user = new User
        {
            UserName = request.UserName,
            Password = request.Password
        };

        User created = await userRepository.AddAsync(user);

        UserDto dto = new UserDto
        {
            Id = created.Id,
            UserName = created.UserName
        };

        return CreatedAtAction(
            nameof(GetSingle),
            new { id = dto.Id },
            dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetSingle(int id)
    {
        try
        {
            User user = await userRepository.GetSingleAsync(id);

            UserDto dto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName
            };

            return Ok(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"User with ID {id} was not found.");
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserDto>> GetMany(
        [FromQuery] string? username)
    {
        IQueryable<User> users = userRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(username))
        {
            users = users.Where(u =>
                u.UserName.Contains(
                    username,
                    StringComparison.OrdinalIgnoreCase));
        }

        IEnumerable<UserDto> result = users.Select(u =>
            new UserDto
            {
                Id = u.Id,
                UserName = u.UserName
            });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateUser(
        int id,
        [FromBody] CreateUserDto request)
    {
        try
        {
            User existing =
                await userRepository.GetSingleAsync(id);

            bool usernameTaken = userRepository
                .GetMany()
                .Any(u =>
                    u.Id != id &&
                    u.UserName.Equals(
                        request.UserName,
                        StringComparison.OrdinalIgnoreCase));

            if (usernameTaken)
            {
                return BadRequest(
                    "Username is already taken.");
            }

            User updated = new User
            {
                Id = existing.Id,
                UserName = request.UserName,
                Password = request.Password
            };

            await userRepository.UpdateAsync(updated);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"User with ID {id} was not found.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await userRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"User with ID {id} was not found.");
        }
    }
}