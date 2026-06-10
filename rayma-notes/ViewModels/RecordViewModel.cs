using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;
using rayma_notes.Services;

namespace rayma_notes.ViewModels
{
    public partial class RecordViewModel : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private readonly IAudioRecorder _audioRecorder;

        [ObservableProperty]
        public partial bool IsRecording { get; set; } = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasText))]
        public partial string NoteText {  get; set; } = string.Empty;

        public bool HasText => !string.IsNullOrWhiteSpace(NoteText);

        public RecordViewModel(IAudioManager audioManager)
        {
            _audioManager = audioManager;
            _audioRecorder = _audioManager.CreateRecorder();
        }
        
        [RelayCommand]
        private async Task BeginRecording()
        {
            PermissionStatus permissionStatus =  await Permissions.RequestAsync<Permissions.Microphone>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                return;
            }

            try
            {
                await _audioRecorder.StartAsync();
                IsRecording = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to record: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task EndRecording()
        {
            IsRecording = false;

            try
            {
                IAudioSource audioSource = await _audioRecorder.StopAsync();

                if (audioSource is FileAudioSource fileAudioSource)
                {
                    string audioPath = fileAudioSource.GetFilePath();
                    System.Diagnostics.Debug.WriteLine($"AUDIO SAVED TO: {audioPath}");

                    await ProcessRecording(audioPath);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to stop: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task SaveNote()
        {
            await AppShell.Current.DisplayAlertAsync("Note Saved", "Placeholder", "OK");
            NoteText = string.Empty;
        }

        private async Task ProcessRecording(string filePath)
        {
            TranscriptionResult transcriptionResult = await GroqService.TranscribeAudioAsync(filePath);

            switch (transcriptionResult.Status)
            {
                case TranscriptionStatus.Success:
                    System.Diagnostics.Debug.WriteLine($"Transcript: {transcriptionResult.Text}");
                    break;

                case TranscriptionStatus.EmptyTranscript:
                    await AppShell.Current.DisplayAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case TranscriptionStatus.RateLimitExceeded:
                    await AppShell.Current.DisplayAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case TranscriptionStatus.InvalidApiKey:
                    await AppShell.Current.DisplayAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case TranscriptionStatus.NetworkError:
                    await AppShell.Current.DisplayAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case TranscriptionStatus.SystemError:
                    await AppShell.Current.DisplayAlertAsync("System Error", $"Transcription failed: {transcriptionResult.ErrorDetails}", "OK");
                    return;
            }

            CleanResult cleanResult = await GroqService.CleanTextAsync(transcriptionResult.Text);

            switch (cleanResult.Status)
            {
                case CleanStatus.Success:
                    System.Diagnostics.Debug.WriteLine($"Cleaned transcript: {cleanResult.Text}");
                    NoteText = cleanResult.Text;
                    break;

                case CleanStatus.EmptyOutput:
                    await AppShell.Current.DisplayAlertAsync("Silent Recording", "We didn't hear anything. Try speaking louder or holder the phone closer.", "OK");
                    return;

                case CleanStatus.RateLimitExceeded:
                    await AppShell.Current.DisplayAlertAsync("Too Fast", "You are creating notes too quickly. Please pause for a moment.", "OK");
                    return;

                case CleanStatus.InvalidApiKey:
                    await AppShell.Current.DisplayAlertAsync("Key Error", "Your API key is expired or invalid.", "OK");
                    return;

                case CleanStatus.NetworkError:
                    await AppShell.Current.DisplayAlertAsync("No Internet", "You seem to be offline. Please check your connection.", "OK");
                    return;

                case CleanStatus.SystemError:
                    await AppShell.Current.DisplayAlertAsync("System Error", $"Transcription failed: {cleanResult.ErrorDetails}", "OK");
                    return;
            }
        }
    }
}
