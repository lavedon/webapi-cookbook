using Microsoft.AspNetCore.SignalR.Client;
using System.CommandLine;
using System.CommandLine.Invocation;

class Program
{
    private static string CurrentMethodName = "BroadcastMessage";

    static async Task<int> Main(string[] args)
    {
        var urlOption = new Option<string>(
            "--url",
            "The URL of the SignalR hub");

        var methodOption = new Option<string>(
            "--method",
            () => "BroadcastMessage",
            "The name of the hub method to call");

        var messageOption = new Option<string>(
            "--message",
            "The message to send");

        var interactiveOption = new Option<bool>(
            "--interactive",
            () => false,
            "Run in interactive mode");

        var rootCommand = new RootCommand("SignalR Client Console Application")
        {
            urlOption,
            methodOption,
            messageOption,
            interactiveOption
        };

        rootCommand.SetHandler(async (InvocationContext context) =>
        {
            var url = context.ParseResult.GetValueForOption(urlOption);
            CurrentMethodName = context.ParseResult.GetValueForOption(methodOption) ?? "BroadcastMessage";
            var message = context.ParseResult.GetValueForOption(messageOption);
            var interactive = context.ParseResult.GetValueForOption(interactiveOption);

            if (args.Length == 0 || interactive)
            {
                await RunInteractiveMode();
            }
            else if (url != null && message != null)
            {
                await SendMessageFromCommandLine(url, CurrentMethodName, message);
            }
            else
            {
                Console.WriteLine("Please provide --url and --message, or use --interactive");
            }
        });

        return await rootCommand.InvokeAsync(args);
    }

    static async Task RunInteractiveMode()
    {
        Console.WriteLine("Please specify the URL of SignalR Hub:");
        var url = Console.ReadLine();
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(url!)
            .Build();

        hubConnection.On<string>("ReceiveMessage", 
            message => Console.WriteLine($"SignalR Hub Message: {message}"));

        try
        {
            await hubConnection.StartAsync();
            Console.WriteLine("Connected to the hub.");

            while (true)
            {
                Console.WriteLine("\nPlease specify the action:");
                Console.WriteLine("0 - Send a message");

                if (!string.IsNullOrEmpty(CurrentMethodName))
                {
                    Console.WriteLine($"1 - Change MethodName (Current: {CurrentMethodName})");
                }

                Console.WriteLine("exit - Exit the program");

                var action = Console.ReadLine();

                switch (action)
                {
                    case "0":
                        await SendMessage(hubConnection);
                        break;
                    case "1":
                        if (!string.IsNullOrEmpty(CurrentMethodName))
                        {
                            ChangeMethodName();
                        }
                        break;
                    case "exit":
                        return;
                    default:
                        Console.WriteLine("Invalid action. Please try again.");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static void ChangeMethodName()
    {
        Console.WriteLine("Enter the new hub method name:");
        CurrentMethodName = Console.ReadLine();
        Console.WriteLine($"Method name changed to: {CurrentMethodName}");
    }

    static async Task SendMessage(HubConnection hubConnection)
    {
        Console.WriteLine("Please specify the message:");
        var message = Console.ReadLine();

        await hubConnection.SendAsync(CurrentMethodName, message);
        Console.WriteLine("Message sent.");
    }

    static async Task SendMessageFromCommandLine(string url, string methodName, string message)
    {
        var hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

        try
        {
            await hubConnection.StartAsync();
            await hubConnection.SendAsync(methodName, message);
            Console.WriteLine($"Sent message '{message}' using method '{methodName}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            await hubConnection.StopAsync();
        }
    }
}
