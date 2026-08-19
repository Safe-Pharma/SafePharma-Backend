using Azure;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenAI;
using OpenAI.Chat;
using SafePharma.AI.Agent;
using SafePharma.AI.Contracts;
using SafePharma.AI.Rag;
using SafePharma.AI.Tools;
using SafePharma.BLL;
using System.ClientModel;
using System.Globalization;
using System.Text.Json;

namespace SafePharma.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly IMedicalSourceTool _medicalSourceTool;

        public TestController(IEmailService emailService, IMedicalSourceTool medicalSourceTool)
        {
            _emailService = emailService;
            _medicalSourceTool = medicalSourceTool;
        }

        [HttpPost("test-email")]
        public async Task<IActionResult> Send()
        {
            await _emailService.SendEmailAsync(
                "mostafa.mamdouh1002@gmail.com",
                "Brevo Test",
                "<h1>Hello from .NET 10 🚀</h1>");

            return Ok("Email sent");
        }

        [HttpGet("test-openfda")]
        public async Task<IActionResult> TestOpenFda([FromQuery] string drugName = "ibuprofen")
        {
            var result = await _medicalSourceTool.QueryAsync(drugName);
            return Ok(result);
        }

        [HttpGet("test-agent")]
        [HttpPost("test-patient-safety")]
        public async Task<IActionResult> TestPatientSafety([FromServices] IPatientSafetyAgent agent)
        {
            var request = new PatientSafetyCheckRequest
            {
                Language = "en",
                Patients =
                [
                 
                    new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-1",
                    Allergies = ["Aspirin"]
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-1",
                        TradeName = "Advil",
                        ScientificName = "Ibuprofen"
                    }
                ]
            },

      
            new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-2",
                    ChronicConditions = ["Chronic Kidney Disease"]
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-2",
                        TradeName = "Tylenol",
                        ScientificName = "Acetaminophen"
                    }
                ]
            },
            new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-3"
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-3",
                        TradeName = "Panadol",
                        ScientificName = "Paracetamol"
                    }
                ]
            }
                ]
            };

            var result = await agent.CheckAsync(request);
            return Ok(result);
        }

        [HttpPost("test-patient-safety-stream")]
        public async Task TestPatientSafetyStream([FromServices] IPatientSafetyAgent agent, CancellationToken cancellationToken)
        {
            Response.ContentType = "text/event-stream";

            var request = new PatientSafetyCheckRequest
            {
                Language = "en",
                Patients =
    [

        new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-1",
                    Allergies = ["Aspirin"]
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-1",
                        TradeName = "Advil",
                        ScientificName = "Ibuprofen"
                    }
                ]
            },


            new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-2",
                    ChronicConditions = ["Chronic Kidney Disease"]
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-2",
                        TradeName = "Tylenol",
                        ScientificName = "Acetaminophen"
                    }
                ]
            },
            new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-3"
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-3",
                        TradeName = "Panadol",
                        ScientificName = "Paracetamol"
                    }
                ]
            }
    ]
            };

            await foreach (var evt in agent.CheckStreamAsync(request, cancellationToken))
            {
                var json = JsonSerializer.Serialize(evt);
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }

        [HttpPost("test-drug-drug")]
        public async Task<IActionResult> TestDrugDrugInteraction([FromServices] IPatientSafetyAgent agent)
        {
            var request = new PatientSafetyCheckRequest
            {
                Language = "en",
                Patients =
                [
                    new PatientCheckGroup
            {
                Profile = new PatientProfileDto
                {
                    PatientRef = "test-patient-warfarin",
                    Allergies = []
                },
                DrugsToCheck =
                [
                    new DrugInfoDto
                    {
                        ClientRef = "item-1",
                        TradeName = "Coumadin",
                        ScientificName = "Warfarin"
                    },
                    new DrugInfoDto
                    {
                        ClientRef = "item-2",
                        TradeName = "Advil",
                        ScientificName = "Ibuprofen"
                    }
                ]
            }
                ]
            };

            var result = await agent.CheckAsync(request);
            return Ok(result);
        }

        [HttpGet("test-dailymed")]
        public async Task<IActionResult> TestDailyMed(
        [FromServices] DailyMedTool dailyMedTool,
        [FromQuery] string drugName = "ibuprofen")
        {
            var result = await dailyMedTool.QueryAsync(drugName);
            return Ok(result);
        }

        [HttpGet("test-embedding")]
        public async Task<IActionResult> TestEmbedding(
        [FromServices] IEmbeddingService embeddingService,
        [FromQuery] string text = "ibuprofen may cause severe allergic reactions")
        {
            var vector = await embeddingService.GetEmbeddingAsync(text);
            return Ok(new { Length = vector.Length, First5Values = vector.Take(5) });
        }

        [HttpPost("setup-vector-index")]
        public async Task<IActionResult> SetupVectorIndex([FromServices] AzureSearchVectorStore vectorStore)
        {
            await vectorStore.EnsureIndexExistsAsync();
            return Ok("Index created/updated.");
        }

        [HttpPost("seed-vector-store")]
        public async Task<IActionResult> SeedVectorStore(
    IFormFile file,
    [FromServices] IEmbeddingService embeddingService,
    [FromServices] IVectorStore vectorStore,
    [FromServices] IMedicalSourceTool openFdaTool,
    [FromServices] DailyMedTool dailyMedTool,
    [FromServices] IDrugNameNormalizer normalizer,
    [FromServices] ILogger<TestController> logger,
    CancellationToken cancellationToken)
        {
            
            var activeIngredients = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var reader = new StreamReader(file.OpenReadStream());
            var headerLine = await reader.ReadLineAsync(cancellationToken);
            var headers = headerLine!.TrimStart('\uFEFF').Split(',');
            var ingredientIndex = Array.IndexOf(headers, "active_ingredient");

            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync(cancellationToken);
                if (string.IsNullOrWhiteSpace(line)) continue;

                var fields = line.Split(',');
                if (ingredientIndex >= fields.Length) continue;

                var ingredient = fields[ingredientIndex].Trim();

               
                if (string.IsNullOrWhiteSpace(ingredient)) continue;
                if (ingredient == "." || ingredient == "0") continue;
                if (DateTime.TryParse(ingredient, CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) continue;
                if (ingredient.Contains('+')) continue; 
                if (ingredient.Length < 3) continue;

                activeIngredients.Add(ingredient);

                if (activeIngredients.Count >= 200) break; 
            }

            logger.LogInformation("[Seed] Found {Count} clean active ingredients to process", activeIngredients.Count);

            
            var processed = 0;
            var skipped = 0;

            foreach (var ingredient in activeIngredients)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    var normalized = await normalizer.NormalizeAsync(ingredient, cancellationToken);
                    var searchName = normalized.Found ? normalized.NormalizedName! : ingredient;

                    var openFdaResult = await openFdaTool.QueryAsync(searchName, cancellationToken);
                    var dailyMedResult = await dailyMedTool.QueryAsync(searchName, cancellationToken);

                    if (!openFdaResult.Found && !dailyMedResult.Found)
                    {
                        skipped++;
                        continue; 
                    }

                    if (openFdaResult.Found)
                    {
                        var embedding = await embeddingService.GetEmbeddingAsync(searchName, cancellationToken);
                        await vectorStore.UpsertAsync(new VectorStoreEntry
                        {
                            Id = $"{searchName}-openfda".Replace(" ", "-").ToLowerInvariant(),
                            DrugName = searchName,
                            Content = openFdaResult.RawText!,
                            Source = "OpenFDA",
                            Embedding = embedding
                        }, cancellationToken);
                    }

                    if (dailyMedResult.Found)
                    {
                        var embedding = await embeddingService.GetEmbeddingAsync(searchName, cancellationToken);
                        await vectorStore.UpsertAsync(new VectorStoreEntry
                        {
                            Id = $"{searchName}-dailymed".Replace(" ", "-").ToLowerInvariant(),
                            DrugName = searchName,
                            Content = dailyMedResult.RawText!,
                            Source = "DailyMed",
                            Embedding = embedding
                        }, cancellationToken);
                    }

                    processed++;
                    logger.LogInformation("[Seed] Processed {Ingredient} ({Processed}/{Total})", searchName, processed, activeIngredients.Count);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "[Seed] Failed to process {Ingredient}, skipping", ingredient);
                    skipped++;
                }
            }

            return Ok(new { TotalCandidates = activeIngredients.Count, Processed = processed, Skipped = skipped });
        }
        [HttpGet("test-vector-search")]
        public async Task<IActionResult> TestVectorSearch(
        [FromServices] IEmbeddingService embeddingService,
        [FromServices] IVectorStore vectorStore,
        [FromQuery] string query = "allergic reaction warning")
        {
            var queryEmbedding = await embeddingService.GetEmbeddingAsync(query);
            var results = await vectorStore.SearchSimilarAsync(queryEmbedding, topK: 3);
            return Ok(results);
        }
    }
}