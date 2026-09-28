using Entities;
using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ListPostsView
{
    private readonly IPostRepository postRepository;

    public ListPostsView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public void Show()
    {
        IQueryable<Post> posts = postRepository.GetMany();

        Console.WriteLine();
        Console.WriteLine("=== Posts ===");

        if (!posts.Any())
        {
            Console.WriteLine("No posts found.");
            return;
        }

        foreach (Post post in posts)
        {
            Console.WriteLine(
                $"ID: {post.Id}, Title: {post.Title}, User ID: {post.UserId}");
        }
    }
}