namespace Miljokaz.Views;

public partial class EditItem : ContentPage
{
	public EditItem()
	{
		InitializeComponent();
        this.BindingContext = App.SharedMainPageViewModel;

    }
}