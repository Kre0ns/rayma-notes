using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace rayma_notes.Services
{
    public enum TranscriptionStatus
    {
        Success,
        EmptyTranscript,
        RateLimitExceeded,   
        InvalidApiKey,       
        NetworkError,       
        SystemError          
    }

    public class TranscriptionResult
    {
        public TranscriptionStatus Status { get; }
        public string Text { get; }
        public string ErrorDetails { get; }

        public TranscriptionResult(TranscriptionStatus status, string text, string errorDetails)
        {
            Status = status;
            Text = text;
            ErrorDetails = errorDetails;
        }
    }

    public enum CleanStatus
    {
        Success,
        RateLimitExceeded,
        InvalidApiKey,
        NetworkError,
        SystemError
    }

    public class CleanResult
    {
        public CleanStatus Status { get; }
        public string Text { get; }
        public string ErrorDetails { get; }

        public CleanResult(CleanStatus status, string text, string errorDetails)
        {
            Status = status;
            Text = text;
            ErrorDetails = errorDetails;
        }
    }

    public enum KeyCheckStatus
    {
        Valid,
        Invalid,
        NetworkError,
        SystemError
    }

    public class KeyCheckResult
    {
        public KeyCheckStatus Status { get; }
        public string ErrorDetails { get; }

        public KeyCheckResult(KeyCheckStatus status, string errorDetails)
        {
            Status = status;
            ErrorDetails = errorDetails;
        }
    }

    public static class GroqService
    {
        private const string TranscriptionModel = "whisper-large-v3-turbo";
        private const string CleanModel = "llama-3.3-70b-versatile";

        private const string TranscriptionEndpoint = "https://api.groq.com/openai/v1/audio/transcriptions";
        private const string ChatCompletionEndpoint = "https://api.groq.com/openai/v1/chat/completions";
        private const string ModelListEndpoint = "https://api.groq.com/openai/v1/models";

        private const string SystemPrompt = "You are a professional English editors. Format and clean up the following transcribed voice note. Correct grammar and spelling errors, remove verbal fillers (like 'eh', 'um', 'ah'), and organize the text into clean paragraphs. Output ONLY the edited English text. Do not include any introductory or conversational phrases.";

        public static async Task<TranscriptionResult> TranscribeAudioAsync(string filePath)
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await SecureStorage.Default.GetAsync("groq_api_key"));

                using MultipartFormDataContent form = new MultipartFormDataContent();
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);
                ByteArrayContent fileContent = new ByteArrayContent(fileBytes);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");

                form.Add(fileContent, "file", "audio.wav");
                form.Add(new StringContent(TranscriptionModel), "model");
                form.Add(new StringContent("en"), "language");

                HttpResponseMessage response = await client.PostAsync(TranscriptionEndpoint, form);


                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    JsonElement root = doc.RootElement;
                    string transcript = root.GetProperty("text").GetString() ?? string.Empty;


                    if (string.IsNullOrEmpty(transcript))
                    {
                        return new TranscriptionResult(TranscriptionStatus.EmptyTranscript, string.Empty, string.Empty);
                    }

                    return new TranscriptionResult(TranscriptionStatus.Success, transcript, string.Empty);
                }

                string errorDetails = await response.Content.ReadAsStringAsync();

                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new TranscriptionResult(TranscriptionStatus.InvalidApiKey, string.Empty, errorDetails),
                    HttpStatusCode.TooManyRequests => new TranscriptionResult(TranscriptionStatus.RateLimitExceeded, string.Empty, errorDetails),
                    _ => new TranscriptionResult(TranscriptionStatus.SystemError, string.Empty, $"HTTP {response.StatusCode}: {errorDetails}")
                };
            }
            catch (HttpRequestException ex)
            {
                return new TranscriptionResult(TranscriptionStatus.NetworkError, string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                return new TranscriptionResult(TranscriptionStatus.SystemError, string.Empty, ex.Message);
            }
        }

        public static async Task<CleanResult> CleanTextAsync(string rawText)
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await SecureStorage.Default.GetAsync("groq_api_key"));


                var payload = new
                {
                    model = CleanModel,
                    messages = new[]
                    {
                        new {
                            role = "system",
                            content = SystemPrompt
                    },
                        new {
                            role = "user",
                            content = rawText
                            }
                    },
                    temperature = 0.3
                };

                string jsonPayload = JsonSerializer.Serialize(payload);
                using StringContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(ChatCompletionEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();

                    using JsonDocument doc = JsonDocument.Parse(jsonResponse);
                    JsonElement root = doc.RootElement;
                    string cleanText = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;

                    return new CleanResult(CleanStatus.Success, cleanText, string.Empty);
                }

                string errorDetails = await response.Content.ReadAsStringAsync();
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new CleanResult(CleanStatus.InvalidApiKey, string.Empty, errorDetails),
                    HttpStatusCode.TooManyRequests => new CleanResult(CleanStatus.RateLimitExceeded, string.Empty, errorDetails),
                    _ => new CleanResult(CleanStatus.SystemError, string.Empty, $"HTTP {response.StatusCode}: {errorDetails}")
                };
            }
            catch (HttpRequestException ex)
            {
                return new CleanResult(CleanStatus.NetworkError, string.Empty, ex.Message);
            }
            catch (Exception ex)
            {
                return new CleanResult(CleanStatus.SystemError, string.Empty, ex.Message);
            }
        }

        public static async Task<KeyCheckResult> CheckApiKey(string apiKey)
        {
            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                HttpResponseMessage response = await client.GetAsync(ModelListEndpoint);

                if (response.IsSuccessStatusCode)
                {
                    return new KeyCheckResult(KeyCheckStatus.Valid, string.Empty);
                }

                string errorDetails = await response.Content.ReadAsStringAsync();
                return response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new KeyCheckResult(KeyCheckStatus.Invalid, errorDetails),
                    _ => new KeyCheckResult(KeyCheckStatus.SystemError, $"HTTP {response.StatusCode}: {errorDetails}")
                };
            }
            catch (HttpRequestException ex)
            {
                return new KeyCheckResult(KeyCheckStatus.NetworkError, ex.Message);
            }
            catch (Exception ex)
            {
                return new KeyCheckResult(KeyCheckStatus.SystemError, ex.Message);
            }
        }
    }
}
