using System.Runtime.InteropServices;
using Serilog;
using Serilog.Core;
using System.Reflection;
using System.IO;

namespace ConsoleApp1;

class Program
{
    static void Main()
    {
        //---------------------------------------------------------------
        // START create application folder
        //---------------------------------
        string appName = Path.GetFileNameWithoutExtension(
            Assembly.GetExecutingAssembly().Location);

        string appFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            appName
        );

        Directory.CreateDirectory(appFolder);
        //---------------------------------------------------------------
        // END create application folder
        //-------------------------------

        //---------------------------------------------------------------
        // START logging
        //---------------
        string logFileName="logs/visionapp-.log";
        string logFilePath=Path.Combine(appFolder,logFileName);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()                  // capture debug and above
            .WriteTo.Console()                     // log to console
            .WriteTo.File(
                path: logFilePath,
                rollingInterval: RollingInterval.Day,          // new file every day
                fileSizeLimitBytes: 10_000_000,                // 10 MB
                rollOnFileSizeLimit: true,                     // create new file when size exceeded
                retainedFileCountLimit: 7                      // keep only last 7 files
            )
            .CreateLogger();

        var logger=Log.Logger;
        //-------------
        // END logging        
        //---------------------------------------------------------------        

        Log.Information("Application starting");
        
        //---------------------------------------------------------------
        // START user manager
        //--------------------
        Log.Information("Start user login");

        string dbFileName="visionapp.db";
        string dbFilePath=Path.Combine(appFolder,dbFileName);

        var repo = new UserRepository(dbFilePath);
        var userManager = new UserManager(repo);

        // check if admin exist in the user database
        Log.Information("Check if admin exist");
        // if admin not exist
        // in debug mode: allow to create admin
        // in production mode: not allow to create admin
        if (!repo.IsRoleExist(UserRole.Admin))
        {
            // if in debug mode, create admin
            if (true)
            {
                Log.Information("Start creating default admin");
            
                Console.Write("Username: ");
                var usernameAdmin = Console.ReadLine()!;

                Console.Write("Password: ");
                var passwordAdmin = Console.ReadLine()!;

                // var defaultAdmin = new User
                // {
                //     Username = "admin",
                //     PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                //     Role = UserRole.Admin,
                //     CreatedAt = DateTime.UtcNow
                // };

                var defaultAdmin = new User
                {
                    Username = usernameAdmin,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(passwordAdmin),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                };

                repo.AddUser(defaultAdmin);
                Log.Information("Default admin created");
            }
            else if (false) // if in production mode, not allow to create admin
            {
                Log.Information("No valid user database. Contact administrator");
                return;
            }
        }

        Log.Information("Start logging in user");
        Console.Write("Username: ");
        var username = Console.ReadLine()!;

        Console.Write("Password: ");
        var password = Console.ReadLine()!;

        var loggedInUser = userManager.Login(username, password);

        if (loggedInUser == null)
        {
            Log.Information("Login failed. Application exit.");
            return;
        }

        Log.Information($"Valid user login. Log in as: {loggedInUser.Username} ({loggedInUser.Role})");

        // Admin adding a user example
        // if (loggedInUser.Role == UserRole.Admin)
        // {
        //     Console.WriteLine("Adding new operator...");
        //     userManager.RegisterUser(
        //         loggedInUser,
        //         "operator1",
        //         "op123",
        //         UserRole.Operator);
        // }
        //--------------------
        // END user manager        
        //---------------------------------------------------------------

        // //---------------------------------------------------------------
        // // START app config
        // //----------------------
        // foreach (var property in typeof(AppConfig).GetProperties())
        // {
        //     var value = (int)property.GetValue(config);

        //     var limit = ConfigLimits.Get(property.Name);

        //     if (limit != null)
        //     {
        //         limit.Validate(property.Name, value);
        //     }
        // }
        // //--------------------
        // // END app config        
        // //---------------------------------------------------------------

        //---------------------------------------------------------------
        // START camera manager
        //----------------------
        CameraManager manager = new CameraManager(logger);

        for (int i = 1; i <= 3; i++)
        {
            var cam = new CameraController($"Cam{i}", logger);
            cam.InspectionCompleted += (result) =>
            {
                Console.WriteLine($"{result.CameraName} → Avg: {result.AverageGray:F2}, Pass: {result.IsPass}");
            };

            manager.AddCamera(cam);
        }

        // Start live capture for all cameras
        manager.StartAll();

        Console.WriteLine("Live capture running. Press Enter to trigger manual inspection...");
        Console.ReadLine();

        // Manual inspection
        manager.InspectAll();

        Console.WriteLine("Press Enter to stop all cameras.");
        Console.ReadLine();

        manager.StopAll();
        //--------------------
        // END camera manager
        //---------------------------------------------------------------

        Log.Information("Application finished");
        Log.CloseAndFlush();
    }
}
