using TUSP.Client;
using TUSP.Server;

namespace TUSP;

public static class CLI
{
    static CLI()
    {
        _commands = new Dictionary<string, Func<string[], int>>(StringComparer.OrdinalIgnoreCase)
        {
            { "ping", Ping },
            { "listen", Listen }
        };
    }

    public static Dictionary<string, Func<string[], int>> _commands;

    public static int Execute(params string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("No command specified.");
            return 1;
        }

        try
        {
            var cmd = args[0];
            if (!_commands.ContainsKey(cmd))
            {
                Console.WriteLine($"Unknown command: {cmd}");
                return 1;
            }

            return _commands[cmd].Invoke(args.Skip(1).ToArray());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    public static int Listen(params string[] args)
    {
        var tuspServer = new TuspListener();
        var thread = new Thread(tuspServer.StartListening);

        Console.WriteLine("Starting TUSP server...");
        thread.Start();

        return 0;
    }

    /// <summary>
    /// Ping a TUSP server
    /// </summary>
    /// <param name="args">-h host -p port</param>
    /// <example>tusp ping -h localhost -p 5000</example>
    /// <returns>0 if success, 1 if error</returns>
    public static int Ping(params string[] args)
    {
        string host = null;
        int port = 5000; 

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-h":
                case "--host":
                    if (i + 1 < args.Length)
                    {
                        host = args[i + 1];
                        i++;
                    }
                    else
                    {
                        Console.WriteLine("Host not specified after -h");
                        return 1;
                    }
                    break;

                case "-p":
                case "--port":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int p))
                    {
                        port = p;
                        i++;
                    }
                    else
                    {
                        Console.WriteLine("Invalid port specified after -p");
                        return 1;
                    }
                    break;

                default:
                    Console.WriteLine($"Unknown argument: {args[i]}");
                    return 1;
            }
        }

        if (host == null)
        {
            Console.WriteLine("Host is required. Use -h <host>");
            return 1;
        }

        try
        {
            var client = new TuspClient();
            client.Ping(host, port);
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to {host}:{port} - {ex.Message}");
            return 1;
        }
    }
}