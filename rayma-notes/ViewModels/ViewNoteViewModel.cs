using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class ViewNoteViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly NavigationService _navigationService;
         
        [ObservableProperty]
        public partial string TitleText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string BodyText { get; set; } = string.Empty;

        public ViewNoteViewModel(IDatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
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
