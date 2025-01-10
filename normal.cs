public class MainViewModel : ObservableObject
{
    private List<DataPoint> _dataPoints;
    private string _statusMessage;

    public List<DataPoint> DataPoints
    {
        get { return _dataPoints; }
        set 
        {
            _dataPoints = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get { return _statusMessage; }
        set 
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadDataAsync()
    {
        try
        {
            StatusMessage = "Loading data...";
            DataProcessor processor = new DataProcessor("your_connection_string"); 
            DataPoints = await processor.GetDataAsync();
            StatusMessage = "Data loaded successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading data: {ex.Message}";
        }
    }

    public void ProcessData()
    {
        try
        {
            StatusMessage = "Processing data...";
            DataProcessor processor = new DataProcessor("your_connection_string"); 
            processor.ProcessData(DataPoints);
            StatusMessage = "Data processed successfully.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error processing data: {ex.Message}";
        }
    }
}
