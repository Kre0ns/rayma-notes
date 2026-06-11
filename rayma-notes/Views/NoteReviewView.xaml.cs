using rayma_notes.ViewModels;

namespace rayma_notes.Views;

public partial class NoteReviewView : ContentPage
{
	public NoteReviewView(NoteReviewViewModel noteReviewPopupViewModel)
	{
		InitializeComponent();

		BindingContext = noteReviewPopupViewModel;
	}
}