using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class CreateUserView
{
    private readonly IUserRepository userRepository;

    public CreateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
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
}