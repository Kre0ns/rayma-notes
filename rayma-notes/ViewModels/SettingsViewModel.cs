using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly IAiService _aiService;

        [ObservableProperty]
        public partial string ApiKeyText {  get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool HasApiKey { get; set; } = false;

        public SettingsViewModel(IAiService aiService) 
        {
            _aiService = aiService;
        }

        [RelayCommand]
        private async Task HandleApiKey()
        {
            if (HasApiKey)
            {
                SecureStorage.Default.Remove("groq_api_key");
            }
            else
            {
                bool isValid = await IsKeyValid(ApiKeyText);

                if (!isValid)
                {
                    return;
                }

                await SecureStorage.Default.SetAsync("groq_api_key", ApiKeyText);
            }

            await CheckApiKeyPresenceAsync();
        }

        public async Task CheckApiKeyPresenceAsync()
        {
            string? key = await SecureStorage.Default.GetAsync("groq_api_key");
            HasApiKey = !string.IsNullOrWhiteSpace(key);

            ApiKeyText = HasApiKey ? new string('-', key!.Length) : String.Empty;
        }

        private async Task<bool> IsKeyValid(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return false;
            }

            bool result = false;

            KeyCheckResult checkResult = await _aiService.CheckApiKeyAsync(apiKey);

            switch (checkResult.Status)
            {
                case KeyCheckStatus.Valid:
                    System.Diagnostics.Debug.WriteLine($"Key valid");
                    result = true;
                    break;

                case KeyCheckStatus.Invalid:
                    await AppShell.Current.DisplayAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    break;

                case KeyCheckStatus.NetworkError:
                    await AppShell.Current.DisplayAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    break;

                case KeyCheckStatus.SystemError:
                    await AppShell.Current.DisplayAlertAsync("System Error", $"Key validation failed: {checkResult.ErrorDetails}", "OK");
                    break;
            }
            
            return result;
        }
    }
}
