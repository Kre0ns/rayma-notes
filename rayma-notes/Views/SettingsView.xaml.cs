using rayma_notes.ViewModels;

namespace rayma_notes.Views;

public partial class SettingsView : ContentPage
{
	public SettingsView()
	{
		InitializeComponent();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();

		if (BindingContext is SettingsViewModel viewModel)
		{
			await viewModel.CheckApiKeyPresenceAsync();
		}
	}
}