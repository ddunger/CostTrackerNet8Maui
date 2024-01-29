namespace Miljokaz.Views;

public partial class AllItems : ContentPage
{
	public AllItems()
	{
		InitializeComponent();
        this.BindingContext = App.SharedMainPageViewModel;

    }
}