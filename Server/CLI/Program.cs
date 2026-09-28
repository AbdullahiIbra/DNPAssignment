using CLI.UI;
using FileRepositories;
using RepositoryContracts;

IUserRepository userRepository =
    new UserFileRepository();

IPostRepository postRepository =
    new PostFileRepository();

ICommentRepository commentRepository =
    new CommentFileRepository();

CliApp cliApp = new CliApp(
    userRepository,
    postRepository,
    commentRepository);

await cliApp.StartAsync();