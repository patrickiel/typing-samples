using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Threading;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ExtendedCSharpSample
{
    public class DataProcessor
    {
        private const string ApiUrl = "https://api.example.com/data"; 
        private readonly HttpClient _httpClient;
        private readonly string _connectionString;

        public DataProcessor(string connectionString)
        {
            _httpClient = new HttpClient();
            _connectionString = connectionString;
        }

        public async Task<List<DataPoint>> GetDataAsync()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                // Assuming the API returns data in JSON format
                // Deserialize the JSON response into a list of DataPoint objects
                List<DataPoint> dataPoints = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DataPoint>>(jsonResponse); 

                return dataPoints;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(<span class="math-inline">"Error fetching data from API\: \{ex\.Message\}"\);
return new List<DataPoint\>\(\);
\}
catch \(Exception ex\)
\{
Console\.WriteLine\(</span>"An unexpected error occurred: {ex.Message}");
                return new List<DataPoint>();
            }
        }

        public void ProcessData(List<DataPoint> dataPoints)
        {
            // 1. Data Cleaning and Transformation
            List<DataPoint> cleanedData = dataPoints
                .Where(dp => !string.IsNullOrEmpty(dp.Value)) 
                .Select(dp => new DataPoint
                {
                    Timestamp = dp.Timestamp,
                    Value = double.TryParse(dp.Value, out double parsedValue) ? parsedValue : 0.0 
                })
                .ToList();

            // 2. Data Analysis and Aggregation
            double averageValue = cleanedData.Average(dp => dp.Value);
            double maxValue = cleanedData.Max(dp => dp.Value);
            double minValue = cleanedData.Min(dp => dp.Value);
            double standardDeviation = CalculateStandardDeviation(cleanedData);

            // 3. Data Visualization (Example: Console output)
            Console.WriteLine(<span class="math-inline">"Average Value\: \{averageValue\}"\);
Console\.WriteLine\(</span>"Maximum Value: {maxValue}");
            Console.WriteLine(<span class="math-inline">"Minimum Value\: \{minValue\}"\);
Console\.WriteLine\(</span>"Standard Deviation: {standardDeviation}");

            // 4. Data Storage (Example: Saving to a file and database)
            string filePath = "data.csv";
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (DataPoint dataPoint in cleanedData)
                    {
                        writer.WriteLine(<span class="math-inline">"\{dataPoint\.Timestamp\},\{dataPoint\.Value\}"\);
\}
\}
Console\.WriteLine\(</span>"Data saved to: {filePath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine(<span class="math-inline">"Error saving data to file\: \{ex\.<6\>Message\}"\);
\}
try
\{
using \(SqlConnection connection \= new SqlConnection\(\_connectionString\)\)
\{
connection\.Open\(\);
using \(SqlCommand command \= new</6\> SqlCommand\("INSERT INTO DataPoints \(Timestamp, Value\) VALUES \(@Timestamp, @Value\)", connection\)\)
\{
foreach \(DataPoint dataPoint in cleanedData\)
\{
command\.Parameters\.Clear\(\);
command\.Parameters\.AddWithValue\("@Timestamp", dataPoint\.Timestamp\);
command\.Parameters\.AddWithValue\("@Value", dataPoint\.Value\);
command\.ExecuteNonQuery\(\);
\}
\}
\}
Console\.WriteLine\("Data saved to database\."\);
\}
catch \(SqlException ex\)
\{
Console\.WriteLine\(</span>"Error saving data to database: {ex.Message}");
            }
        }

        private double CalculateStandardDeviation(List<DataPoint> dataPoints)
        {
            double average = dataPoints.Average(dp => dp.Value);
            return Math.Sqrt(dataPoints.Average(dp => Math.Pow(dp.Value - average, 2)));
        }
    }

    public class DataPoint
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; } 
    }

    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

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

    public partial class MainForm : Form
    {
        private MainViewModel _viewModel;

        public MainForm()
        {
            InitializeComponent(); 
            _viewModel = new MainViewModel();
// Bind the ViewModel to the UI
            this.DataContext = _viewModel; 

            // Example: Bind a DataGrid to the DataPoints collection
            dataGrid.ItemsSource = _viewModel.DataPoints; 

            // Example: Bind a TextBox to the StatusMessage property
            statusTextBox.DataBindings.Add("Text", _viewModel, "StatusMessage");

            // Example: Add event handlers for buttons
            loadDataButton.Click += async (sender, e) => await _viewModel.LoadDataAsync();
            processDataButton.Click += (sender, e) => _viewModel.ProcessData();
        }

        // Example: A helper method for displaying a message box
        private void ShowMessageBox(string message)
        {
            MessageBox.Show(this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    // Example: A custom control for displaying a chart
    public class DataChart : UserControl
    {
        private Chart _chart;

        public DataChart()
        {
            InitializeComponent(); 
            _chart = new Chart();
            this.Controls.Add(_chart);
        }

        public void BindData(List<DataPoint> dataPoints)
        {
            // Clear existing series
            _chart.Series.Clear();

            // Create a new series
            Series series = new Series("Data");
            series.ChartType = SeriesChartType.Line; 

            // Add data points to the series
            foreach (DataPoint dataPoint in dataPoints)
            {
                series.Points.AddXY(dataPoint.Timestamp, dataPoint.Value);
            }

            // Add the series to the chart
            _chart.Series.Add(series);
        }
    }

    // Example: A custom exception class
    public class DataProcessingException : Exception
    {
        public DataProcessingException(string message) : base(message) { }
    }

    // Example: A utility class with helper methods
    public static class Utility
    {
        public static string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
        }

        public static Image ResizeImage(Image image, int width, int height)
        {
            Image resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.DrawImage(image, 0, 0, width, height);
            }
            return resizedImage;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}