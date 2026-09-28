using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class UpdatePostView
{
    private readonly IPostRepository postRepository;
    private readonly IUserRepository userRepository;

    public UpdatePostView(
        IPostRepository postRepository,
        IUserRepository userRepository)
    {
        this.postRepository = postRepository;
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Enter post ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        Post? existingPost = postRepository
            .GetMany()
            .SingleOrDefault(p => p.Id == postId);

        if (existingPost is null)
        {
            Console.WriteLine(
                $"Post with ID {postId} does not exist.");
            return;
        }

        Console.Write("Enter new title: ");
        string? title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Enter new body: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Body cannot be empty.");
            return;
        }

        Console.Write("Enter user ID: ");

        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        bool userExists = userRepository
            .GetMany()
            .Any(u => u.Id == userId);

        if (!userExists)
        {
            Console.WriteLine(
                $"User with ID {userId} does not exist.");
            return;
        }

        Post updatedPost = new Post
        {
            Id = postId,
            Title = title,
            Body = body,
            UserId = userId
        };

        await postRepository.UpdateAsync(updatedPost);

        Console.WriteLine("Post updated successfully.");
    }
}