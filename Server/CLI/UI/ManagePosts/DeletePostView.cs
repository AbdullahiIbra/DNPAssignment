using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class DeletePostView
{
    private readonly IPostRepository postRepository;

    public DeletePostView(IPostRepository postRepository)
    {
        this.postRepository = postRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Enter post ID to delete: ");

        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        bool postExists = postRepository
            .GetMany()
            .Any(p => p.Id == postId);

        if (!postExists)
        {
            Console.WriteLine(
                $"Post with ID {postId} does not exist.");
            return;
        }

        await postRepository.DeleteAsync(postId);

        Console.WriteLine("Post deleted successfully.");
    }
}