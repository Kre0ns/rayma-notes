using rayma_notes.ViewModels;

namespace rayma_notes;

public partial class MainTabbedPage : TabbedPage
{
	public MainTabbedPage(RecordViewModel recordViewModel, NotesViewModel notesViewModel, SettingsViewModel settingsViewModel)
	{ 
		InitializeComponent();

		Children[0].BindingContext = recordViewModel;
		Children[1].BindingContext = notesViewModel;
		Children[2].BindingContext = settingsViewModel;
	}
}