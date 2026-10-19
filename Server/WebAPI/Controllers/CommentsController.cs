using ApiContracts.Comments;
using Entities;
using Microsoft.AspNetCore.Mvc;
using RepositoryContracts;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class CommentsController : ControllerBase
{
    private readonly ICommentRepository commentRepository;
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;

    public CommentsController(
        ICommentRepository commentRepository,
        IUserRepository userRepository,
        IPostRepository postRepository)
    {
        this.commentRepository = commentRepository;
        this.userRepository = userRepository;
        this.postRepository = postRepository;
    }

    [HttpPost]
    public async Task<ActionResult<CommentDto>> AddComment(
        [FromBody] CreateCommentDto request)
    {
        bool userExists = userRepository
            .GetMany()
            .Any(u => u.Id == request.UserId);

        if (!userExists)
        {
            return BadRequest(
                $"User with ID {request.UserId} does not exist.");
        }

        bool postExists = postRepository
            .GetMany()
            .Any(p => p.Id == request.PostId);

        if (!postExists)
        {
            return BadRequest(
                $"Post with ID {request.PostId} does not exist.");
        }

        Comment comment = new Comment
        {
            Body = request.Body,
            UserId = request.UserId,
            PostId = request.PostId
        };

        Comment created =
            await commentRepository.AddAsync(comment);

        CommentDto dto = new CommentDto
        {
            Id = created.Id,
            Body = created.Body,
            UserId = created.UserId,
            PostId = created.PostId
        };

        return CreatedAtAction(
            nameof(GetSingle),
            new { id = dto.Id },
            dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CommentDto>> GetSingle(int id)
    {
        try
        {
            Comment comment =
                await commentRepository.GetSingleAsync(id);

            CommentDto dto = new CommentDto
            {
                Id = comment.Id,
                Body = comment.Body,
                UserId = comment.UserId,
                PostId = comment.PostId
            };

            return Ok(dto);
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Comment with ID {id} was not found.");
        }
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommentDto>> GetMany(
        [FromQuery] int? userId,
        [FromQuery] string? username,
        [FromQuery] int? postId)
    {
        IQueryable<Comment> comments =
            commentRepository.GetMany();

        if (userId.HasValue)
        {
            comments = comments.Where(c =>
                c.UserId == userId.Value);
        }

        if (postId.HasValue)
        {
            comments = comments.Where(c =>
                c.PostId == postId.Value);
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

            comments = comments.Where(c =>
                matchingUserIds.Contains(c.UserId));
        }

        IEnumerable<CommentDto> result =
            comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Body = c.Body,
                UserId = c.UserId,
                PostId = c.PostId
            });

        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateComment(
        int id,
        [FromBody] CreateCommentDto request)
    {
        try
        {
            Comment existing =
                await commentRepository.GetSingleAsync(id);

            bool userExists = userRepository
                .GetMany()
                .Any(u => u.Id == request.UserId);

            if (!userExists)
            {
                return BadRequest(
                    $"User with ID {request.UserId} does not exist.");
            }

            bool postExists = postRepository
                .GetMany()
                .Any(p => p.Id == request.PostId);

            if (!postExists)
            {
                return BadRequest(
                    $"Post with ID {request.PostId} does not exist.");
            }

            Comment updated = new Comment
            {
                Id = existing.Id,
                Body = request.Body,
                UserId = request.UserId,
                PostId = request.PostId
            };

            await commentRepository.UpdateAsync(updated);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Comment with ID {id} was not found.");
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteComment(int id)
    {
        try
        {
            await commentRepository.DeleteAsync(id);

            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound(
                $"Comment with ID {id} was not found.");
        }
    }
}