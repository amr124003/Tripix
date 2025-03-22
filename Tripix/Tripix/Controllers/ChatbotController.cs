using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;

namespace Tripix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly string mistralApiKey = "m1Vm4FiPzq6BllvFHDHVrau9QR35fulR";
        private readonly string cohereApiKey = "c36Ea91L5KWkMg2G0nYVBJx2dFuYGPFQsjyPEi3L";

        private static readonly List<QuestionAnswer> QAList = new()
        {
            new QuestionAnswer { Question = "ما هو Tripix؟", Answer = "Tripix هو منصة متكاملة لكل ما يخص السيارات." },
            new QuestionAnswer { Question = "كيف يمكنني استئجار سيارة؟", Answer = "يمكنك استئجار سيارة عبر تطبيقنا من خلال قسم تأجير السيارات." },
            new QuestionAnswer { Question = "هل توفرون خدمة إصلاح السيارات؟", Answer = "نعم، نوفر خدمات إصلاح السيارات ضمن خدماتنا." },
            new QuestionAnswer { Question = "كيف أحصل على دعم فني؟", Answer = "يمكنك التواصل مع خدمة العملاء عبر البريد الإلكتروني أو الهاتف." }
        };

        [HttpPost("ask")]
        public async Task<IActionResult> AskChatbot ( [FromBody] ChatRequest request )
        {
            if (string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("السؤال لا يمكن أن يكون فارغًا.");
            }

            var bestMatch = await FindBestMatchingQuestion(request.Message);
            if (bestMatch != null)
            {
                return Ok(new { reply = bestMatch.Answer });
            }

            string generatedResponse = await GetResponseFromMistral(request.Message);
            return Ok(new { reply = generatedResponse });
        }

        private async Task<QuestionAnswer> FindBestMatchingQuestion ( string userQuestion )
        {
            var client = new RestClient("https://api.cohere.ai/v1/embed");
            var restRequest = new RestRequest();
            restRequest.Method = Method.Post;
            restRequest.AddHeader("Authorization", $"Bearer {cohereApiKey}");
            restRequest.AddHeader("Content-Type", "application/json");

            var requestBody = new
            {
                texts = QAList.Select(q => q.Question).Append(userQuestion).ToArray(),
                model = "embed-english-v2.0"
            };

            restRequest.AddJsonBody(requestBody);
            var response = await client.ExecuteAsync(restRequest);

            if (!response.IsSuccessful)
            {
                return null;
            }

            var jsonResponse = JsonDocument.Parse(response.Content);
            var embeddings = jsonResponse.RootElement.GetProperty("embeddings").EnumerateArray().ToList();

            if (embeddings.Count == 0)
            {
                return null;
            }

            var userEmbedding = embeddings.Last().EnumerateArray().Select(e => e.GetDouble()).ToArray();
            embeddings.RemoveAt(embeddings.Count - 1);

            double maxSimilarity = 0.0;
            QuestionAnswer bestMatch = null;

            for (int i = 0; i < QAList.Count; i++)
            {
                double similarity = CosineSimilarity(userEmbedding, embeddings[i].EnumerateArray().Select(e => e.GetDouble()).ToArray());

                if (similarity > maxSimilarity && similarity > 0.85) // رفع الحد الأدنى للتشابه
                {
                    maxSimilarity = similarity;
                    bestMatch = QAList[i];
                }
            }

            return bestMatch;
        }

        private double CosineSimilarity ( double[] vec1, double[] vec2 )
        {
            double dotProduct = 0.0, magA = 0.0, magB = 0.0;

            for (int i = 0; i < vec1.Length; i++)
            {
                dotProduct += vec1[i] * vec2[i];
                magA += Math.Pow(vec1[i], 2);
                magB += Math.Pow(vec2[i], 2);
            }

            return dotProduct / (Math.Sqrt(magA) * Math.Sqrt(magB));
        }


        private async Task<string> GetResponseFromMistral ( string userQuestion )
        {
            try
            {
                var client = new RestClient("https://api.mistral.ai/v1/chat/completions");
                var restRequest = new RestRequest();
                restRequest.Method = Method.Post;
                restRequest.AddHeader("Authorization", $"Bearer {mistralApiKey}");
                restRequest.AddHeader("Content-Type", "application/json");

                var requestBody = new
                {
                    model = "mistral-small",
                    messages = new[]
                    {
                        new { role = "user", content = userQuestion }
                    },
                    max_tokens = 200,
                    temperature = 0.7
                };

                restRequest.AddJsonBody(requestBody);
                var response = await client.ExecuteAsync(restRequest);

                if (!response.IsSuccessful)
                {
                    Console.WriteLine("❌ فشل استدعاء API لـ Mistral");
                    return "عذرًا، حدث خطأ أثناء محاولة توليد الإجابة.";
                }

                var jsonResponse = JsonDocument.Parse(response.Content);
                string generatedText = jsonResponse.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

                Console.WriteLine($"✅ إجابة Mistral: {generatedText}");
                return generatedText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ أثناء استدعاء Mistral: {ex.Message}");
                return "عذرًا، حدث خطأ أثناء محاولة توليد الإجابة.";
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }

    public class QuestionAnswer
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}