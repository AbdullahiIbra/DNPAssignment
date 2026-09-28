using RepositoryContracts;

namespace CLI.UI.ManagePosts;

public class ManagePostsView
{
    private readonly CreatePostView createPostView;
    private readonly ListPostsView listPostsView;
    private readonly SinglePostView singlePostView;
    private readonly UpdatePostView updatePostView;
    private readonly DeletePostView deletePostView;

    public ManagePostsView(
        IPostRepository postRepository,
        IUserRepository userRepository,
        ICommentRepository commentRepository)
    {
        createPostView =
            new CreatePostView(postRepository, userRepository);

        listPostsView =
            new ListPostsView(postRepository);

        singlePostView =
            new SinglePostView(
                postRepository,
                commentRepository);

        updatePostView =
            new UpdatePostView(
                postRepository,
                userRepository);

        deletePostView =
            new DeletePostView(postRepository);
    }

    public async Task ShowAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== Manage Posts ===");
            Console.WriteLine("1. Create post");
            Console.WriteLine("2. List posts");
            Console.WriteLine("3. View specific post");
            Console.WriteLine("4. Update post");
            Console.WriteLine("5. Delete post");
            Console.WriteLine("0. Back");

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await createPostView.ShowAsync();
                    break;

                case "2":
                    listPostsView.Show();
                    break;

                case "3":
                    await singlePostView.ShowAsync();
                    break;

                case "4":
                    await updatePostView.ShowAsync();
                    break;

                case "5":
                    await deletePostView.ShowAsync();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }
}