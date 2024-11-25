//MySQL with Microsoft.SemanticKernel.Memory
using Azure;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
// using sktodo.Config;
using Microsoft.EntityFrameworkCore;
using Azure.AI.Inference;
using Microsoft.SemanticKernel.Memory;
using sktodo.Models;
using DotNetEnv;
using sktodo.Context;
using sktodo.Plugins;
using Light = sktodo.Models.Light;
// using CrudWithNLP.Context;

#pragma warning disable SKEXP0070, SKEXP0001, SKEXP0020, SKEXP0010, SKEXP0050, ASP0000

// Load the .env file
Env.Load(".env");
string githubKey = Env.GetString("GITHUB_KEY");
string connectionString = Env.GetString("CONNECTION_STRING");

var endpoint = new Uri("https://models.inference.ai.azure.com");
var credential = new AzureKeyCredential("GITHUB_KEY");
var model = "gpt-4o-mini";

// Initialize the chat completions client
var client = new ChatCompletionsClient(endpoint, credential);

// Create the request options for the chat completion
var requestOptions = new ChatCompletionsOptions()
{
    Messages =
    {
        new ChatRequestSystemMessage("You are a helpful assistant."),
        new ChatRequestUserMessage("What tasks do I have pending?"),
    },
    Temperature = 1.0f,
    NucleusSamplingFactor = 1.0f,
    MaxTokens = 1000,
    Model = model
};

// Initialize the Semantic Kernel
var kernelBuilder = Kernel.CreateBuilder();
kernelBuilder.Plugins.AddFromType<ToDoPlugin>("ToDoPlugin");
kernelBuilder.Plugins.AddFromType<LightPlugin>("LightPlugin");

kernelBuilder.AddAzureOpenAIChatCompletion(model, "https://models.inference.ai.azure.com", githubKey);
kernelBuilder.Services.AddDbContext<NLPContext>(opt => opt.UseMySQL(connectionString)); // Update with your MySQL connection string

// PostgreSQL Connection
// serviceCollection.AddDbContext<NLPContext>(options =>
// options.UseNpgsql("Host=localhost;Database=database_Name;Username=user_Name;Password=password;")); // Update PostgreSQL connection string

// Adding in-memory task management service or memory store
var memoryStore = new VolatileMemoryStore();
var kernel = kernelBuilder.Build();

// Chat history management
var chatHistory = new ChatHistory();

// Automatic function calling settings
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create a new instance of ServiceCollection
var serviceCollection = new ServiceCollection();

// Add DbContext to the service collection
serviceCollection.AddDbContext<NLPContext>(options =>
options.UseMySQL(connectionString));

// Dependency Injection for DbContext
var serviceProvider = serviceCollection.BuildServiceProvider();
var dbContext = serviceProvider.GetRequiredService<NLPContext>();

// Ensure database is created
dbContext.Database.EnsureCreated();

// Function to handle conversation and task management
string? userInput;
do
{
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    if (userInput is null) break;

    // Add user input to chat history
    chatHistory.AddUserMessage(userInput);

    // Request a chat completion from the model
    var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
    var result = await chatCompletionService.GetChatMessageContentAsync(
        chatHistory,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
        );

    // Print the assistant's response
    Console.WriteLine("Assistant > " + result);

    // Add assistant's response to chat history
    chatHistory.AddMessage(result.Role, result.Content ?? string.Empty);

    // Store the question and response in MySQL
    if (userInput.Contains("todo", StringComparison.OrdinalIgnoreCase))
{
    var conversation = new ToDo
    {
        UserQuestion = userInput,
        AssistantResponse = result.Content ?? string.Empty,
        //Timestamp = DateTime.UtcNow
    };
    dbContext.ToDos.Add(conversation);
}
else
{
    var conversation = new Light
    {
        Name = userInput,
        Response = result.Content ?? string.Empty,
    };
    dbContext.Lights.Add(conversation);
}

await dbContext.SaveChangesAsync();

} while (userInput is not null);