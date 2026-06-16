using rayma_notes.ViewModels;

namespace rayma_notes.Views;

public partial class ViewNoteView : ContentPage
{
	public ViewNoteView(ViewNoteViewModel viewNoteViewModel)
	{
		InitializeComponent();

		BindingContext = viewNoteViewModel;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ViewNoteViewModel viewModel)
        {
            await viewModel.ReloadNoteAsync();
        }
    }
}