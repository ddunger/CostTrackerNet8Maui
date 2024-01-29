using Miljokaz.Data;
using Miljokaz.ViewModels;
namespace Miljokaz
{
    public partial class App : Application
    {
        public static MainPageViewModel SharedMainPageViewModel;

        public static DataRepository DataRepository { get; private set; }
        public App(DataRepository dataRepository)
        {
            InitializeComponent();

            MainPage = new AppShell();
            DataRepository = dataRepository;
            SharedMainPageViewModel = new MainPageViewModel();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 400;
            const int newHeight = 600;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }

    }
}
