using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class SinglePostView
{
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public SinglePostView(
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Enter post ID: ");

        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        try
        {
            Post post =
                await postRepository.GetSingleAsync(postId);

            Console.WriteLine();
            Console.WriteLine("=== Post ===");
            Console.WriteLine($"Title: {post.Title}");
            Console.WriteLine($"Body: {post.Body}");
            Console.WriteLine($"User ID: {post.UserId}");

            Console.WriteLine();
            Console.WriteLine("=== Comments ===");

            var comments = commentRepository
                .GetMany()
                .Where(c => c.PostId == postId);

            if (!comments.Any())
            {
                Console.WriteLine("No comments on this post.");
                return;
            }

            foreach (Comment comment in comments)
            {
                Console.WriteLine(
                    $"[{comment.Id}] {comment.Body} - User ID: {comment.UserId}");
            }
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}