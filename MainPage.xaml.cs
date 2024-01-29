using Microcharts;
using Miljokaz.ViewModels;
using SkiaSharp;

namespace Miljokaz
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = App.SharedMainPageViewModel;

        }
    
    }

}
