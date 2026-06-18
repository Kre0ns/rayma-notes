using rayma_notes.ViewModels;

namespace rayma_notes.Views;

public partial class RecordView : ContentPage
{
	public RecordView()
	{
		InitializeComponent();
	}

    private void Record_Pressed(object sender, EventArgs e)
    {
        if (BindingContext is RecordViewModel viewModel)
        {
            viewModel.RecordPressedCommand.Execute(null);
        }
    }

    private void Record_Released(object sender, EventArgs e)
    {
        if (BindingContext is RecordViewModel viewModel)
        {
            viewModel.RecordReleasedCommand.Execute(null);
        }
    }
}