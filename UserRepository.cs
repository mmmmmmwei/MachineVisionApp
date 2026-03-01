using Microsoft.Data.Sqlite;

namespace ConsoleApp1;

public class UserRepository
{
    private readonly string _connectionString;

    public UserRepository(string dbPath)
    {
        _connectionString = $"Data Source={dbPath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        CREATE TABLE IF NOT EXISTS Users (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Username TEXT NOT NULL UNIQUE,
            PasswordHash TEXT NOT NULL,
            Role INTEGER NOT NULL,
            CreatedAt TEXT NOT NULL
        );
        ";
        command.ExecuteNonQuery();
    }

    public void AddUser(User user)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        INSERT INTO Users (Username, PasswordHash, Role, CreatedAt)
        VALUES ($username, $passwordHash, $role, $createdAt);
        ";

        command.Parameters.AddWithValue("$username", user.Username);
        command.Parameters.AddWithValue("$passwordHash", user.PasswordHash);
        command.Parameters.AddWithValue("$role", (int)user.Role);
        command.Parameters.AddWithValue("$createdAt", user.CreatedAt.ToString("o"));

        command.ExecuteNonQuery();
    }

    public User? GetUserByUsername(string username)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        SELECT Id, Username, PasswordHash, Role, CreatedAt
        FROM Users
        WHERE Username = $username;
        ";

        command.Parameters.AddWithValue("$username", username);

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return null;

        return new User
        {
            Id = reader.GetInt32(0),
            Username = reader.GetString(1),
            PasswordHash = reader.GetString(2),
            Role = (UserRole)reader.GetInt32(3),
            CreatedAt = DateTime.Parse(reader.GetString(4))
        };
    }

    public bool IsRoleExist(UserRole role)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
        SELECT Id, Username, PasswordHash, Role, CreatedAt
        FROM Users
        WHERE Role = $role;
        ";

        command.Parameters.AddWithValue("$role", role);

        using var reader = command.ExecuteReader();

        return reader.Read();
    }
}