using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using rayma_notes.Models;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class RecordViewModel : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private readonly IAudioRecorder _audioRecorder;
        private readonly IAiService _aiService;
        private readonly NavigationService _navigationService;

        private bool _isHolding = false;

        [ObservableProperty]
        public partial bool IsRecording { get; set; } = false;

        [ObservableProperty]
        public partial bool IsBusy { get; set; } = false;

        public RecordViewModel(IAudioManager audioManager, IAiService aiService, NavigationService navigationService)
        {
            _audioManager = audioManager;
            _audioRecorder = _audioManager.CreateRecorder();
            _aiService = aiService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        public async Task RecordPressedAsync()
        {
            _isHolding = true;

            string? apiKey = await SecureStorage.Default.GetAsync("groq_api_key");
            if (string.IsNullOrEmpty(apiKey))
            {
                await DialogService.ShowAlertAsync("Missing API Key", "Please set your API key in the settings before recording.", "OK");

                _isHolding = false;
                return;
            }

            PermissionStatus permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                _isHolding = false;
                return;
            }

            if (!_isHolding)
            {
                return;
            }

            if (!IsRecording)
            {
                await _audioRecorder.StartAsync();

                IsRecording = true;
            }
        }

        [RelayCommand]
        public async Task RecordReleasedAsync()
        {
            _isHolding = false;

            if (IsRecording)
            {
                IsRecording = false;

                IAudioSource audioSource = await _audioRecorder.StopAsync();

                if (audioSource is FileAudioSource fileAudioSource)
                {
                    string audioPath = fileAudioSource.GetFilePath();
                    System.Diagnostics.Debug.WriteLine($"AUDIO SAVED TO: {audioPath}");

                    await ProcessRecordingAsync(audioPath);
                }
            }
        }

        private async Task ProcessRecordingAsync(string filePath)
        {
            IsBusy = true;

            try
            {
                TranscriptionResult transcriptionResult = await _aiService.TranscribeAudioAsync(filePath);

                if (transcriptionResult.Status != TranscriptionStatus.Success)
                {
                    await HandleTranscriptionErrorAsync(transcriptionResult);
                    return;
                }

                CleanResult cleanResult = await _aiService.CleanTextAsync(transcriptionResult.Text);

                if (cleanResult.Status != CleanStatus.Success)
                {
                    await HandleCleanErrorAsync(cleanResult);
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"Cleaned transcript: {cleanResult.Text}");

                Note note = new() { Body = cleanResult.Text };

                await _navigationService.PushReviewPageAsync(note);

            }
            finally
            {
                IsBusy = false;

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }

        private static async Task HandleTranscriptionErrorAsync(TranscriptionResult transcriptionResult)
        {
            switch (transcriptionResult.Status)
            {
                case TranscriptionStatus.EmptyTranscript:
                    await DialogService.ShowAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case TranscriptionStatus.RateLimitExceeded:
                    await DialogService.ShowAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case TranscriptionStatus.InvalidApiKey:
                    await DialogService.ShowAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case TranscriptionStatus.NetworkError:
                    await DialogService.ShowAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case TranscriptionStatus.SystemError:
                    await DialogService.ShowAlertAsync("System Error", $"Transcription failed: {transcriptionResult.ErrorDetails}", "OK");
                    return;
            }
        }

        private static async Task HandleCleanErrorAsync(CleanResult cleanResult)
        {
            switch (cleanResult.Status)
            {
                case CleanStatus.EmptyOutput:
                    await DialogService.ShowAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case CleanStatus.RateLimitExceeded:
                    await DialogService.ShowAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case CleanStatus.InvalidApiKey:
                    await DialogService.ShowAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case CleanStatus.NetworkError:
                    await DialogService.ShowAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case CleanStatus.SystemError:
                    await DialogService.ShowAlertAsync("System Error", $"Transcription failed: {cleanResult.ErrorDetails}", "OK");
                    return;
            }
        }
    }
}
