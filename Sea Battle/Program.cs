namespace Sea_Battle
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            
            while (CurrentUser.form is not null)
            {
                Form form = CurrentUser.form;
                CurrentUser.form = null;

                Application.Run(form);
            }
        }
    }
}