using RepositoryContracts;

namespace CLI.UI.ManageUsers;

public class ManageUsersView
{
    private readonly CreateUserView createUserView;
    private readonly ListUsersView listUsersView;
    private readonly UpdateUserView updateUserView;
    private readonly DeleteUserView deleteUserView;

    public ManageUsersView(IUserRepository userRepository)
    {
        createUserView = new CreateUserView(userRepository);
        listUsersView = new ListUsersView(userRepository);
        updateUserView = new UpdateUserView(userRepository);
        deleteUserView = new DeleteUserView(userRepository);
    }

    public async Task ShowAsync()
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine();
            Console.WriteLine("=== Manage Users ===");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. List users");
            Console.WriteLine("3. Update user");
            Console.WriteLine("4. Delete user");
            Console.WriteLine("0. Back");

            Console.Write("Choose an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await createUserView.ShowAsync();
                    break;

                case "2":
                    listUsersView.Show();
                    break;

                case "3":
                    await updateUserView.ShowAsync();
                    break;

                case "4":
                    await deleteUserView.ShowAsync();
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