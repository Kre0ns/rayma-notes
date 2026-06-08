using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Plugin.Maui.Audio;

namespace rayma_notes.ViewModels
{
    public partial class RecordViewModel : ObservableObject
    {
        private readonly IAudioManager _audioManager;
        private readonly IAudioRecorder _audioRecorder;

        [ObservableProperty]
        public partial bool IsRecording { get; set; }

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
                    string completedAudioPath = fileAudioSource.GetFilePath();
                    System.Diagnostics.Debug.WriteLine($"AUDIO SAVED TO: {completedAudioPath}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to stop: {ex.Message}");
            }
        }
    }
}
