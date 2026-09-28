using Entities;
using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ListUsersView
{
    private readonly IUserRepository userRepository;

    public ListUsersView(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public void Show()
    {
        IQueryable<User> users = userRepository.GetMany();

        Console.WriteLine();
        Console.WriteLine("=== Users ===");

        if (!users.Any())
        {
            Console.WriteLine("No users found.");
            return;
        }

        foreach (User user in users)
        {
            Console.WriteLine(
                $"ID: {user.Id}, Username: {user.UserName}");
        }
    }
}