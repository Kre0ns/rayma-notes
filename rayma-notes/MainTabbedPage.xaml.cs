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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateTitle();
    }

    protected override void OnCurrentPageChanged()
    {
        base.OnCurrentPageChanged();
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        if (Parent is NavigationPage navPage && CurrentPage != null)
        {
            navPage.Title = CurrentPage.Title;
        }
    }
}