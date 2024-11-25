//using sktodo.Config;
using sktodo.Plugins;
using sktodo.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.WebUtilities;
using HandlebarsDotNet;
using Azure.AI.OpenAI; // Added for Azure OpenAI integration
using Azure.Core;
using Azure;
using sktodo.Configurations;

#pragma warning disable SKEXP0070, SKEXP0001, SKEXP0020, SKEXP0010

namespace sktodo.Services
{
    public class KernelBase : IKernelBase{
        private readonly NLPConfiguration _nlpConfiguration;

        public KernelBase(IOptions<NLPConfiguration> options){
            _nlpConfiguration = options.Value;
         }

        public Kernel CreateKernel(){
            var builder = Kernel.CreateBuilder();
            try{
                builder.Plugins.AddFromType<ToDoPlugin>();
                builder.Plugins.AddFromType<IdentifyToDoObjectPlugin>();

            // Integrating Azure OpenAI model using API Key from NLPConfiguration
                var openAIClient = new AzureOpenAIClient(new Uri("https://models.inference.ai.azure.com"), new AzureKeyCredential(_nlpConfiguration.GITHUB_KEY));
                
                builder.Services.AddSingleton(openAIClient); // Adding OpenAIClient to the DI container
            }

            catch(Exception ex){
                Console.WriteLine(ex.Message);
                
            }

            return builder.Build();

            
        }
    }
}