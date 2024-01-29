namespace Miljokaz.Views;

public partial class NewItem : ContentPage
{
	public NewItem()
	{
		InitializeComponent();
        this.BindingContext = App.SharedMainPageViewModel;

    }
  
}