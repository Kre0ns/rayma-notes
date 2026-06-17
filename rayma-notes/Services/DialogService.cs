namespace rayma_notes.Services
{
    public static class DialogService
    {
        private static Page? GetCurrentPage()
        {
            return Application.Current?.Windows.FirstOrDefault()?.Page;
        }

        public static async Task ShowAlertAsync(string title, string message, string cancel)
        { 
            Page? page = GetCurrentPage();
            if (page is not null)
            {
                await page.DisplayAlertAsync(title, message, cancel);
            }
        }

        public static async Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            Page? page = GetCurrentPage();
            if (page is not null)
            {
                return await page.DisplayAlertAsync(title, message, accept, cancel);
            }

            return false;
        }
    }
}
