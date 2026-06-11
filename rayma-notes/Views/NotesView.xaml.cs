using rayma_notes.ViewModels;

namespace rayma_notes.Views;

public partial class NotesView : ContentPage
{
	public NotesView()
	{
		InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is NotesViewModel viewModel)
        {
            await viewModel.LoadNotesAsync();
        }
    }
}