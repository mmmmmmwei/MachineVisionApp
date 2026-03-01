using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace ConsoleApp1;

public enum Permission
{
    None = 0,
    Setup = 1,
    Run = 2,
    Config = 4
}

public enum UserRole
{
    Viewer = 0,
    Operator = 1,
    Admin = 2
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool HasPermission(Permission p)
    {
        return (((int)Role & (int)p)==0? false:true);        
    }
}
