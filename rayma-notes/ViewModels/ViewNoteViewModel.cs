using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Models;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class ViewNoteViewModel : ObservableObject
    {
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        public partial Note Note { get; set; } = new();

        public ViewNoteViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        [RelayCommand]
        public async Task BackAsync()
        {
            await _navigationService.PopModalAsync();
        }

        [RelayCommand]
        public async Task EditAsync()
        {
            throw new NotImplementedException();
        }
    }
}
