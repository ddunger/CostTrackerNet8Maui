using Miljokaz.Views;

namespace Miljokaz
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AllItems), typeof(AllItems));
            Routing.RegisterRoute(nameof(EditItem), typeof(EditItem));
            Routing.RegisterRoute(nameof(NewItem), typeof(NewItem));


        }
    }
}
