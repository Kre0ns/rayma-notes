namespace rayma_notes.Services.Interfaces
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
        EmptyOutput,
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

    public interface IAiService
    {

        Task<TranscriptionResult> TranscribeAudioAsync(string filePath);

        Task<CleanResult> CleanTextAsync(string rawText);

        Task<KeyCheckResult> CheckApiKeyAsync(string apiKey);
    }
}
