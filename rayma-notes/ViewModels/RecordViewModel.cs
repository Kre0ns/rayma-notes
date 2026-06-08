using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace rayma_notes.ViewModels
{
    public partial class RecordViewModel : ObservableObject
    {
        [ObservableProperty]
        public partial bool IsRecording { get; set; }

        [ObservableProperty]
        public partial bool IsPaused { get; set; }

        [RelayCommand]
        private void BeginRecording()
        {
            IsRecording = true;
        }

        [RelayCommand]
        private void TogglePauseRecording()
        {
            IsPaused = !IsPaused;
        }

        [RelayCommand]
        private void EndRecording()
        {  
            IsRecording = false;
            IsPaused = false;
        }
    }
}
