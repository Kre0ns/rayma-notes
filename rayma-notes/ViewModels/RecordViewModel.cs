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
        private async Task ToggleRecording()
        {
            PermissionStatus permissionStatus = await Permissions.RequestAsync<Permissions.Microphone>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                return;
            }

            try
            {
                if (!IsRecording)
                {
                    await _audioRecorder.StartAsync();

                    IsRecording = true;
                }
                else
                {
                    IsRecording = false;

                    IAudioSource audioSource = await _audioRecorder.StopAsync();

                    if (audioSource is FileAudioSource fileAudioSource)
                    {
                        string audioPath = fileAudioSource.GetFilePath();
                        System.Diagnostics.Debug.WriteLine($"AUDIO SAVED TO: {audioPath}");

                        await ProcessRecording(audioPath);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to record: {ex.Message}");
            }
        }

        private async Task ProcessRecording(string filePath)
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
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case TranscriptionStatus.RateLimitExceeded:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case TranscriptionStatus.InvalidApiKey:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case TranscriptionStatus.NetworkError:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case TranscriptionStatus.SystemError:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("System Error", $"Transcription failed: {transcriptionResult.ErrorDetails}", "OK");
                    return;
            }
        }

        private static async Task HandleCleanErrorAsync(CleanResult cleanResult)
        {
            switch (cleanResult.Status)
            {
                case CleanStatus.EmptyOutput:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case CleanStatus.RateLimitExceeded:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case CleanStatus.InvalidApiKey:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case CleanStatus.NetworkError:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case CleanStatus.SystemError:
                    await Application.Current!.Windows[0].Page!.DisplayAlertAsync("System Error", $"Transcription failed: {cleanResult.ErrorDetails}", "OK");
                    return;
            }
        }
    }
}
