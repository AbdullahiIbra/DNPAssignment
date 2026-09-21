using Entities;
using RepositoryContracts;

namespace CLI.UI;

public class CliApp
{
    private readonly IUserRepository userRepository;
    private readonly IPostRepository postRepository;
    private readonly ICommentRepository commentRepository;

    public CliApp(
        IUserRepository userRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository)
    {
        this.userRepository = userRepository;
        this.postRepository = postRepository;
        this.commentRepository = commentRepository;
    }

    public async Task StartAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== Forum CLI ===");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Create post");
            Console.WriteLine("3. Add comment");
            Console.WriteLine("4. View posts");
            Console.WriteLine("5. View specific post");
            Console.WriteLine("0. Exit");

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await CreateUser();
                    break;

                case "2":
                    await CreatePost();
                    break;

                case "3":
                    await AddComment();
                    break;

                case "4":
                    ViewPosts();
                    break;

                case "5":
                    await ViewSpecificPost();
                    break;

                case "0":
                    running = false;
                    Console.WriteLine("Goodbye!");
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
    }

    private async Task CreateUser()
    {
        Console.Write("Enter username: ");
        string? username = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        bool usernameTaken = userRepository
            .GetMany()
            .Any(u => u.UserName.Equals(
                username,
                StringComparison.OrdinalIgnoreCase));

        if (usernameTaken)
        {
            Console.WriteLine("That username is already taken.");
            return;
        }

        Console.Write("Enter password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        User user = new User
        {
            UserName = username,
            Password = password
        };

        User createdUser =
            await userRepository.AddAsync(user);

        Console.WriteLine(
            $"User created successfully with ID {createdUser.Id}");
    }

    private async Task CreatePost()
    {
        Console.Write("Enter title: ");
        string? title = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title cannot be empty.");
            return;
        }

        Console.Write("Enter body: ");
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

        bool userExists =
            userRepository.GetMany().Any(u => u.Id == userId);

        if (!userExists)
        {
            Console.WriteLine(
                $"User with ID {userId} does not exist.");
            return;
        }

        Post post = new Post
        {
            Title = title,
            Body = body,
            UserId = userId
        };

        Post createdPost =
            await postRepository.AddAsync(post);

        Console.WriteLine(
            $"Post created successfully with ID {createdPost.Id}");
    }

    private async Task AddComment()
    {
        Console.Write("Enter comment body: ");
        string? body = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(body))
        {
            Console.WriteLine("Comment cannot be empty.");
            return;
        }

        Console.Write("Enter user ID: ");

        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        Console.Write("Enter post ID: ");

        if (!int.TryParse(Console.ReadLine(), out int postId))
        {
            Console.WriteLine("Invalid post ID.");
            return;
        }

        bool userExists =
            userRepository.GetMany().Any(u => u.Id == userId);

        if (!userExists)
        {
            Console.WriteLine(
                $"User with ID {userId} does not exist.");
            return;
        }

        bool postExists =
            postRepository.GetMany().Any(p => p.Id == postId);

        if (!postExists)
        {
            Console.WriteLine(
                $"Post with ID {postId} does not exist.");
            return;
        }

        Comment comment = new Comment
        {
            Body = body,
            UserId = userId,
            PostId = postId
        };

        Comment createdComment =
            await commentRepository.AddAsync(comment);

        Console.WriteLine(
            $"Comment created successfully with ID {createdComment.Id}");
    }

    private void ViewPosts()
    {
        IQueryable<Post> posts =
            postRepository.GetMany();

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
                $"[{post.Title}, {post.Id}]");
        }
    }

    private async Task ViewSpecificPost()
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