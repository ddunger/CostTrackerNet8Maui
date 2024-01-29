using Miljokaz.Models;
using Miljokaz.Views;
using Miljokaz.Data;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using Microcharts;
using SkiaSharp;


namespace Miljokaz.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private List<DataModel> userData;
        public List<DataModel> UserData
        {
            get { return userData; }
            set
            {
                userData = value;
                OnPropertyChanged(nameof(UserData));
            }
        }

        private List<TypeModel> userTypes;
        public List<TypeModel> UserTypes
        {
            get { return userTypes; }
            set
            {
                userTypes = value;
                OnPropertyChanged(nameof(UserTypes));
            }
        }
        public List<DisplayDataModel> DisplayData { get; set; }

        private DisplayDataModel _selectedItem;
        public DisplayDataModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    Debug.WriteLine("---------------------List item selected");
                    SelectedEditItem();
                }
            }
        }
        public DateTime currentDate { get; set; }

        private string typeText;

        public string TypeText
        {
            get { return typeText; }
            set
            {
                if (typeText != value)
                {
                    typeText = value;
                    OnPropertyChanged(nameof(TypeText));
                }
            }
        }


        private float itemAmount;

        public float ItemAmount
        {
            get { return itemAmount; }
            set
            {
                if (itemAmount != value)
                {
                    itemAmount = value;
                    OnPropertyChanged(nameof(ItemAmount));
                }
            }
        }

        private TypeModel selectedTypeModel;

        public TypeModel SelectedTypeModel
        {
            get { return selectedTypeModel; }
            set
            {
                if (selectedTypeModel != value)
                {
                    selectedTypeModel = value;
                    OnPropertyChanged(nameof(SelectedTypeModel));

                }
            }
        }
        private ColorsList selectedColorItem;

        public ColorsList SelectedColorItem
        {
            get { return selectedColorItem; }
            set
            {
                if (selectedColorItem != value)
                {
                    selectedColorItem = value;
                    OnPropertyChanged(nameof(SelectedColorItem));

                    if (selectedColorItem != null)
                    {
                        SelectedTypeColor = Color.FromArgb(selectedColorItem.ColorCode);
                        Debug.WriteLine("SelectedTypeColor " + SelectedTypeColor);
                    }
                }
            }
        }

        private Color selectedTypeColor;

        public Color SelectedTypeColor
        {
            get { return selectedTypeColor; }
            set
            {
                if (selectedTypeColor != value)
                {
                    selectedTypeColor = value;
                    OnPropertyChanged(nameof(SelectedTypeColor));
                }
            }
        }
        public ICommand SelectDateRange { get; set; }
        public ICommand SelectAllItems { get; set; }
        public ICommand SelectNewItem { get; set; }
        public ICommand SaveNewTypeCommand { get; set; }
        public ICommand SaveNewItemCommand { get; set; }
        public ICommand SaveEditedTypeCommand { get; set; }
        public ICommand CancelEditItem { get; set; }
        public ICommand DeleteEditItem { get; set; }
        public ICommand BackButton { get; set; }


        private List<ChartEntry> chartData;

        public List<ChartEntry> ChartData
        {
            get { return chartData; }
            set
            {
                if (chartData != value)
                {
                    chartData = value;
                    OnPropertyChanged(nameof(ChartData));
                }
            }
        }
        private PieChart chart;

        public PieChart Chart
        {
            get { return chart; }
            set
            {
                if (chart != value)
                {
                    chart = value;
                    OnPropertyChanged(nameof(Chart));
                }
            }
        }

        private List<ColorsList> availableColors;

        public List<ColorsList> AvailableColors
        {
            get { return availableColors; }
            set
            {
                if (availableColors != value)
                {
                    availableColors = value;
                    OnPropertyChanged(nameof(AvailableColors));
                }
            }
        }

        public DateTime SelectedStartDate
        {
            get => _selectedStartDate;
            set
            {
                if (_selectedStartDate != value)
                {
                    _selectedStartDate = value;
                    OnPropertyChanged(nameof(SelectedStartDate));
                    UpdateChart();

                }
            }
        }

        public DateTime SelectedEndDate
        {
            get => _selectedEndDate;
            set
            {
                if (_selectedEndDate != value)
                {
                    _selectedEndDate = value;
                    OnPropertyChanged(nameof(SelectedEndDate));
                    UpdateChart();
                }
            }
        }

        private DateTime _selectedStartDate;
        private DateTime _selectedEndDate;

        private int _itemId;
        public int ItemId
        {
            get { return _itemId; }
            set
            {
                if (_itemId != value)
                {
                    _itemId = value;
                    OnPropertyChanged(nameof(ItemId));


                }
            }
        }
        public DateTime EditDate { get; set; }
        public float EditAmount { get; set; }


        public MainPageViewModel()
        {
            SelectAllItems = new Command(SelectedAllItemsAsync);
            SelectNewItem = new Command(SelectedNewItem);
            SaveNewTypeCommand = new Command(InsertNewType);
            SaveNewItemCommand = new Command(InsertNewItem);
            SaveEditedTypeCommand = new Command(SaveEditedType);
            CancelEditItem = new Command(CancelEdit);
            DeleteEditItem = new Command(DeleteEdit);
            BackButton = new Command(GoBack);
            try
            {
                UserData = new List<DataModel>();
                UserTypes = new List<TypeModel>();

                App.DataRepository.Init();
                UserData = App.DataRepository.GetAllData();
                UserTypes = App.DataRepository.GetAllTypes();
                ConvertToDisplayData();
                DrawChart();
                LoadColors();
                SelectedEndDate = DateTime.Now;
                SelectedStartDate = App.DataRepository.GetOldestDate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        public void EditItemById()
        {
            DataModel dataModel = UserData.FirstOrDefault(t => t.Id == ItemId);
            if (dataModel != null)
            {
                EditDate = dataModel.dateTime;
                EditAmount = dataModel.Amount;
            }

        }
        private void UpdateChart()
        {
            UserData.Clear();
            UserData = App.DataRepository.GetDataRange(SelectedStartDate, SelectedEndDate);
            DrawChart();
            ConvertToDisplayData();
        }
        private void LoadColors()
        {
            try
            {
                int count = App.DataRepository.ColorCount();
                if (count == 0)
                {
                    App.DataRepository.AddColors(Miljokaz.Models.Colors.ColorList);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }

        private void DrawChart()
        {
            try
            {
                var localEntries = new Dictionary<(SKColor Color, string Label), float>();

                foreach (var dataModel in UserData)
                {
                    SKColor color = SKColor.Parse(dataModel.dataHexColor);
                    string label = $"{dataModel.Type}";

                    var key = (color, label);
                    if (localEntries.ContainsKey(key))
                    {
                        localEntries[key] += dataModel.Amount;
                    }
                    else
                    {
                        localEntries[key] = dataModel.Amount;
                    }
                }

                var aggregatedEntries = localEntries.Select(entry => new ChartEntry(entry.Value)
                {
                    Color = entry.Key.Color,
                    ValueLabel = $"{entry.Value.ToString("F2")} €",
                    Label = entry.Key.Label
                }).ToList();

                Chart = new PieChart { Entries = aggregatedEntries, LabelMode = LabelMode.RightOnly, GraphPosition = GraphPosition.AutoFill };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }





        public void InsertNewItem()
        {
            try
            {
                string type = SelectedTypeModel.Type.ToString();
                string HexColor = SelectedTypeModel.typeHexColor;
                currentDate = DateTime.Now;

                if (ItemAmount != 0)
                {
                    App.DataRepository.AddData(new DataModel { Type = type, dateTime = currentDate, Amount = ItemAmount, dataHexColor = HexColor });

                    UserData.Clear();
                    UserData = App.DataRepository.GetAllData();
                    DisplayData.Clear();
                    ConvertToDisplayData();
                    Application.Current.MainPage.DisplayAlert("", "New expenditure added!", "OK");
                    DrawChart();
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("", "Please add amount!", "OK");

                }

            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("", "Please select type!", "OK");

                Debug.WriteLine(ex.ToString());
            }


        }
        public void ConvertToDisplayData()
        {
            try
            {
                if (DisplayData != null)
                {
                    DisplayData.Clear();

                }
                DisplayData = new List<DisplayDataModel>();
                foreach (var dataModel in UserData)
                {
                    string amountToString = dataModel.Amount.ToString();
                    DisplayData.Add(new DisplayDataModel
                    {
                        Id = dataModel.Id,
                        Type = dataModel.Type,
                        dateTimeString = dataModel.dateTime.ToString("dd/MM/yyyy"),
                        AmountString = string.Concat(amountToString + " €"),
                        DisplayColor = Color.Parse(dataModel.dataHexColor),
                    }); ; ;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }



        public async void InsertNewType()
        {
            try
            {
                if (string.IsNullOrEmpty(TypeText))
                {
                    Application.Current.MainPage.DisplayAlert("", "Please enter type!", "OK");

                }
                else
                {

                    SKColor selectedColor = SKColor.Parse(selectedColorItem.ColorCode);
                    App.DataRepository.AddType(new TypeModel { Type = TypeText, typeHexColor = selectedColorItem.ColorCode });
                    int IdToBeUpdated = selectedColorItem.Id;
                    App.DataRepository.UsedColor(IdToBeUpdated);
                    UserTypes.Clear();
                    UserTypes = App.DataRepository.GetAllTypes();
                    Application.Current.MainPage.DisplayAlert("", "New type added!", "OK");

                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("", "Please select color!", "OK");

                Debug.WriteLine(ex.ToString());
            }
        }

        public async void SelectedNewItem()
        {
            await Shell.Current.GoToAsync("//NewItem");
            AvailableColors = App.DataRepository.GetAvailableColors();
        }
        private async void SelectedAllItemsAsync()
        {
            await Shell.Current.GoToAsync("//AllItems");

        }
        private async void SelectedEditItem()
        {
            if (SelectedItem != null)
            {
                ItemId = SelectedItem.Id;
                EditItemById();

                if (UserData != null)
                {
                    DataModel matchingDataModel = UserData.FirstOrDefault(dataModel => dataModel.Id == ItemId);

                    if (matchingDataModel != null)
                    {
                        TypeModel matchingTypeModel = UserTypes.FirstOrDefault(typeModel => typeModel.Type == matchingDataModel.Type);

                        if (matchingTypeModel != null)
                        {
                            SelectedTypeModel = matchingTypeModel;
                        }
                    }
                }

                await Shell.Current.GoToAsync("//EditItem");
            }
            else
            {
                Debug.WriteLine("SelectedItem is null");
            }
        }
        public async void SaveEditedType()
        {
            try
            {
                string typeString = SelectedTypeModel.Type.ToString();

                string HexColor = SelectedTypeModel.typeHexColor;
                string newType = SelectedTypeModel.Type;
                if (EditAmount != 0)
                {
                    DataModel updatedModel = new DataModel
                    {
                        Id = ItemId,
                        Type = newType,
                        dateTime = EditDate,
                        Amount = EditAmount,
                        dataHexColor = HexColor
                    };

                    App.DataRepository.UpdateItem(ItemId, updatedModel);

                    UpdateChart();

                    await Shell.Current.GoToAsync("//AllItems");
                    Application.Current.MainPage.DisplayAlert("", "Item successfully edited!", "OK");
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("", "Please add amount!", "OK");

                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("", "Please select type!", "OK");

                Debug.WriteLine(ex.ToString());
            }
        }
        public async void CancelEdit()
        {
            await Shell.Current.GoToAsync("//AllItems");
        }

        public async void DeleteEdit()
        {
            App.DataRepository.Delete(ItemId);
            UpdateChart();
            await Shell.Current.GoToAsync("//AllItems");
            Application.Current.MainPage.DisplayAlert("", "Item successfully deleted!", "OK");

        }
        public async void GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
