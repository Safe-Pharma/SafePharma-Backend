using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafePharma.AI.Agent;
using SafePharma.AI.Rag;
using SafePharma.AI.Tools;

namespace SafePharma.AI
{
    public static class AIServicesExtention
    {
        public static void AddAIServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient<IMedicalSourceTool, OpenFdaTool>(client =>
            {
                client.BaseAddress = new Uri(configuration["OpenFda:BaseUrl"] ?? "https://api.fda.gov/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddHttpClient<DailyMedTool>(client =>
            {
                client.BaseAddress = new Uri(configuration["DailyMed:BaseUrl"] ?? "https://dailymed.nlm.nih.gov/dailymed/services/v2/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddSingleton(new AzureOpenAiSettings
            {
                Endpoint = configuration["AzureOpenAI:Endpoint"]!,
                ApiKey = configuration["AzureOpenAI:ApiKey"]!,
                DeploymentName = configuration["AzureOpenAI:DeploymentName"]!,
                EmbeddingDeploymentName = configuration["AzureOpenAI:EmbeddingDeploymentName"]!, 
                MaxConcurrentPatientChecks = configuration.GetValue("AzureOpenAI:MaxConcurrentPatientChecks", 5)
            });

            services.AddHttpClient<IDrugNameNormalizer, RxNormTool>(client =>
            {
                client.BaseAddress = new Uri(configuration["RxNorm:BaseUrl"] ?? "https://rxnav.nlm.nih.gov/REST/");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            services.AddScoped<IPatientSafetyAgent, PatientSafetyAgent>();
            services.AddSingleton<IEmbeddingService, AzureOpenAiEmbeddingService>();

            services.AddSingleton(new AzureSearchSettings
            {
                Endpoint = configuration["AzureSearch:Endpoint"]!,
                ApiKey = configuration["AzureSearch:ApiKey"]!,
                IndexName = configuration["AzureSearch:IndexName"]!
            });

            //services.AddSingleton<IVectorStore, AzureSearchVectorStore>();
            services.AddSingleton<AzureSearchVectorStore>();
            services.AddSingleton<IVectorStore>(sp => sp.GetRequiredService<AzureSearchVectorStore>());
        }
    }
}