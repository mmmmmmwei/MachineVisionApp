using BCrypt.Net;

namespace ConsoleApp1;

public class UserManager
{
    private readonly UserRepository _repository;

    public UserManager(UserRepository repository)
    {
        _repository = repository;
    }

    public bool RegisterUser(
        User currentUser,
        string username,
        string password,
        UserRole role)
    {
        // Only Admin can add users
        if (currentUser.Role != UserRole.Admin)
            return false;

        if (_repository.GetUserByUsername(username) != null)
            return false;

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        var newUser = new User
        {
            Username = username,
            PasswordHash = hashedPassword,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        _repository.AddUser(newUser);
        return true;
    }

    public User? Login(string username, string password)
    {
        var user = _repository.GetUserByUsername(username);
        if (user == null)
            return null;

        bool valid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        return valid ? user : null;
    }
}