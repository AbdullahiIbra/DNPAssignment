using ApiContracts.Posts;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public PostsController(
        IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> AddPost(
        [FromBody] CreatePostDto request)
    {
        bool userExists = userRepository
            .GetMany()
            .Any(u => u.Id == request.UserId);

        if (!userExists)
        {
            return BadRequest(
                $"User with ID {request.UserId} does not exist.");
        }

        Post post = new Post
        {
            Title = request.Title,
            Body = request.Body,
            UserId = request.UserId
        };

        Post created = await postRepository.AddAsync(post);

        PostDto dto = new PostDto
        {
            Id = created.Id,
            Title = created.Title,
            Body = created.Body,
            UserId = created.UserId
        };

        return CreatedAtAction(
            nameof(GetSingle),
            new { id = dto.Id },
            dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PostDto>> GetSingle(int id)
    {
        try
        {
            Post post =
                await postRepository.GetSingleAsync(id);

            PostDto dto = new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Body = post.Body,
                UserId = post.UserId
            };

            return Ok(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Post with ID {id} was not found.");
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<PostDto>> GetMany(
        [FromQuery] string? title,
        [FromQuery] int? userId,
        [FromQuery] string? username)
    {
        IQueryable<Post> posts =
            postRepository.GetMany();

        if (!string.IsNullOrWhiteSpace(title))
        {
            posts = posts.Where(p =>
                p.Title.Contains(
                    title,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (userId.HasValue)
        {
            posts = posts.Where(p =>
                p.UserId == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(username))
        {
            var matchingUserIds = userRepository
                .GetMany()
                .Where(u =>
                    u.UserName.Contains(
                        username,
                        StringComparison.OrdinalIgnoreCase))
                .Select(u => u.Id)
                .ToList();

            posts = posts.Where(p =>
                matchingUserIds.Contains(p.UserId));
        }

        IEnumerable<PostDto> result =
            posts.Select(p => new PostDto
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserId = p.UserId
            });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePost(
        int id,
        [FromBody] CreatePostDto request)
    {
        try
        {
            Post existing =
                await postRepository.GetSingleAsync(id);

            bool userExists = userRepository
                .GetMany()
                .Any(u => u.Id == request.UserId);

            if (!userExists)
            {
                return BadRequest(
                    $"User with ID {request.UserId} does not exist.");
            }

            Post updated = new Post
            {
                Id = existing.Id,
                Title = request.Title,
                Body = request.Body,
                UserId = request.UserId
            };

            await postRepository.UpdateAsync(updated);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Post with ID {id} was not found.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeletePost(int id)
    {
        try
        {
            await postRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Post with ID {id} was not found.");
        }
    }
}