using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel.Connectors.Google;
using Microsoft.SemanticKernel.Planning.Handlebars;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;
using DotNetEnv;
using sktodo.Services.Interface;

#pragma warning disable SKEXP0070, SKEXP0001, SKEXP0020, SKEXP0010,SKEXP0003, SKEXP0011, SKEXP0052, SKEXP0060

namespace sktodo.Controllers
{
    [Route("[controller]/[action]")]
    public class AIController : ControllerBase
    {
        private readonly IKernelBase _kernelBase;
        public AIController(IKernelBase kernelBase)
        {
            _kernelBase = kernelBase;
        }

        [HttpPost]
        public async Task<IActionResult> NLPChat(string input)
        {
            string result = "";

            Env.Load(".env");
            string azureOpenAIAPIKey = Env.GetString("GITHUB_KEY");

            var kernel = _kernelBase.CreateKernel();
            var arguments = new KernelArguments();

            // Enable auto function calling
            GeminiPromptExecutionSettings geminiPromptExecutionSettings = new()
            {
                ToolCallBehavior = GeminiToolCallBehavior.AutoInvokeKernelFunctions
            };


            try
            {
                var planner = new HandlebarsPlanner(new HandlebarsPlannerOptions(allowLoops: true));

                arguments["input"] = input;

                var originalPlan = await planner.CreatePlanAsync(kernel, input);

                Console.WriteLine(originalPlan);
                result = await originalPlan.InvokeAsync(kernel, arguments);
                Console.WriteLine(originalPlan);
                return Ok(result);

            }
            catch (JsonReaderException ex)
            {
                Console.WriteLine($"JSON Reader Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return Ok(result);

        }
    }
}