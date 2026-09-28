using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class UpdateUserView
{
    private readonly IUserRepository userRepository;

    public UpdateUserView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task ShowAsync()
    {
        Console.Write("Enter user ID to update: ");

        if (!int.TryParse(Console.ReadLine(), out int userId))
        {
            Console.WriteLine("Invalid user ID.");
            return;
        }

        User? existingUser = userRepository
            .GetMany()
            .SingleOrDefault(u => u.Id == userId);

        if (existingUser is null)
        {
            Console.WriteLine(
                $"User with ID {userId} does not exist.");
            return;
        }

        Console.Write("Enter new username: ");
        string? username = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(username))
        {
            Console.WriteLine("Username cannot be empty.");
            return;
        }

        bool usernameTaken = userRepository
            .GetMany()
            .Any(u =>
                u.Id != userId &&
                u.UserName.Equals(
                    username,
                    StringComparison.OrdinalIgnoreCase));

        if (usernameTaken)
        {
            Console.WriteLine("That username is already taken.");
            return;
        }

        Console.Write("Enter new password: ");
        string? password = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Password cannot be empty.");
            return;
        }

        User updatedUser = new User
        {
            Id = userId,
            UserName = username,
            Password = password
        };

        await userRepository.UpdateAsync(updatedUser);

        Console.WriteLine("User updated successfully.");
    }
}