using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;


namespace Oil_Consumption_Tracker
{
    public class Car : INotifyPropertyChanged
    {
        public string registerNumber { get; set; } = "";
        public string manufacturer { get; set; } = "";
        public string motorCode { get; set; } = "";
        public ObservableCollection<Measurement> _measurements { get; set; } = new ObservableCollection<Measurement>();

        public Car(string registerNumber, string manufacture = "No manufacturer found", string motorCode = "No motorcode found", ObservableCollection<Measurement> measurements = null)
        {
            this.registerNumber = registerNumber;
            this.manufacturer = manufacture;
            this.motorCode = motorCode;
            if(measurements != null)
            {
                this._measurements = measurements;
            }
        }
        public Car() { }

        public void ChangeData(string registerNumber, string manufacture = "No manufacturer found", string motorCode = "No motorcode found")
        {
            this.registerNumber = registerNumber;
            this.manufacturer = manufacture;
            this.motorCode = motorCode;
        }

        public ObservableCollection<Measurement> Measurements
        {
            get => _measurements;
            set
            {
                _measurements = value;
                OnPropertyChanged(nameof(Measurements));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Measurement
    {
        public double oilAmount { get; set; } = 0;
        public DateTime? date { get; set; } = DateTime.Now; //Kysymysmerkki tekee nullattavaksi

        public Measurement(double oilAmount, DateTime? date = null)
        {
            this.oilAmount = oilAmount;
            if(date != null)
            {
                this.date = date;
            }
            else
            {
                this.date = DateTime.Now;
            }
        }

        public Measurement() { }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Car> cars { get; set; } = new ObservableCollection<Car>();
        public Car _currentCar;
        public Car currentCar
        {
            get => _currentCar;
            set
            {
                _currentCar = value;
                DisplayNameButton.IsEnabled = _currentCar != null;
                EditCarButton.IsEnabled = _currentCar != null;
                RemoveCarButton.IsEnabled = _currentCar != null;
                if (_currentCar != null)
                {
                    MittausDataText.Text = "Mittausdata: " + _currentCar.registerNumber;
                    editRegisternumber.Text = _currentCar.registerNumber;
                    editManufacturer.Text = _currentCar.manufacturer;
                    editMotorcode.Text = _currentCar.motorCode;
                    SortMeasurements();
                }
                OnPropertyChanged(nameof(currentCar));
                ShowGraph(); // Update the graph when a new car is selected
            }
        }

        public Measurement _currentMeasurement;
        public Measurement currentMeasurement
        {
            get => _currentMeasurement;
            set
            {
                if (_currentMeasurement == value) return;

                _currentMeasurement = value;
                if(_currentMeasurement == null)
                {
                    EditMeasurementButton.IsEnabled = false;
                    RemoveMeasurementButton.IsEnabled = false;
                }
                
                if(_currentMeasurement != null)
                {
                    EditMeasurementButton.IsEnabled = true;
                    RemoveMeasurementButton.IsEnabled = true;
                    editOilAmount.Text = _currentMeasurement.oilAmount.ToString();
                    editDate.Text = _currentMeasurement.date != null ? _currentMeasurement.date.Value.ToString("dd:MM:yyyy") : DateTime.Now.ToString("dd:MM:yyyy");
                }
                OnPropertyChanged(nameof(currentMeasurement));
                ShowGraph(); // Update the graph when a new car is selected
            }
        }

        public Grid currentWindow;
        public double graphMaxHeight = 15;

        public MainWindow()
        {
            InitializeComponent();
            currentWindow = mainMenuWindow;
            Deserialize();
            DataContext = this; // Ensure binding works
        }

        void ValidateRegisternumber(object sender, TextChangedEventArgs e)
        {
            saveCarButton.IsEnabled = !string.IsNullOrWhiteSpace(addRegisterNumber.Text);
        }

        void ValidateOilEdit(object sender, TextChangedEventArgs e)
        {
            saveEditedMeasurement.IsEnabled = DateTime.TryParseExact(editDate.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) && double.TryParse(editOilAmount.Text, out double holder);
        }

        void ValidateOilAdd(object sender, TextChangedEventArgs e)
        {
            saveAddedMeasurement.IsEnabled = DateTime.TryParseExact(addDate.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) && double.TryParse(addOilAmount.Text, out double holder);
        }

        void RemoveCar(object sender, RoutedEventArgs e)
        {
            cars.Remove(_currentCar);
            _currentCar = null;
            Serialize();
        }

        void AddCar(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(addRegisterNumber.Text))
                return;

            Application.Current.Dispatcher.Invoke(() =>
                cars.Add(new Car(addRegisterNumber.Text, addManufacturer.Text, addMotorcode.Text))
            );

            Serialize();
            OpenWindow("mainMenuWindow");
        }

        void EditCar(object sender, RoutedEventArgs e)
        {
            cars[cars.IndexOf(_currentCar)] = new Car(editRegisternumber.Text, editManufacturer.Text, editMotorcode.Text, _currentCar.Measurements);
            OpenWindow("mainMenuWindow");
            Serialize();
        }

        void RemoveMeasurement(object sender, RoutedEventArgs e)
        {
            if (currentCar != null && _currentMeasurement != null)
            {
                currentCar.Measurements.Remove(_currentMeasurement);
                OnPropertyChanged(nameof(currentCar.Measurements)); // Notify UI
                Serialize();
                ShowGraph();
            }
        }

        void AddMeasurement(object sender, RoutedEventArgs e)
        {
            if (DateTime.TryParseExact(addDate.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) &&
                double.TryParse(addOilAmount.Text, out double oilAmount))
            {
                Measurement newMeasurement = new Measurement(oilAmount, parsedDate);

                currentCar.Measurements.Add(newMeasurement);
                OnPropertyChanged(nameof(currentCar.Measurements)); // Notify UI

                ShowGraph();
                Serialize();
            }
            OpenWindow("measurementWindow");
        }

        void EditMeasurement(object sender, RoutedEventArgs e)
        {
            //Poistaa tämänhetkisen
            if (currentCar != null && _currentMeasurement != null)
            {
                currentCar.Measurements.Remove(_currentMeasurement);
                OnPropertyChanged(nameof(currentCar.Measurements)); // Notify UI
                Serialize();
                ShowGraph();
            }

            if (DateTime.TryParseExact(editDate.Text, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate) &&
                double.TryParse(editOilAmount.Text, out double oilAmount))
            {
                Measurement newMeasurement = new Measurement(oilAmount, parsedDate);

                currentCar.Measurements.Add(newMeasurement);
                OnPropertyChanged(nameof(currentCar.Measurements)); // Notify UI

                ShowGraph();
                Serialize();
            }
            OpenWindow("measurementWindow");
        }

        public void SortMeasurements()
        {
            var sortedList = currentCar.Measurements.OrderBy(m => m.date).ToList();
            currentCar.Measurements.Clear();
            foreach (var measurement in sortedList)
            {
                currentCar.Measurements.Add(measurement);
            }
            OnPropertyChanged(nameof(currentCar.Measurements)); // Notify UI
        }

        void Deserialize()
        {
            string filePath = "autot.json";
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { IncludeFields = true };
                cars = JsonSerializer.Deserialize<ObservableCollection<Car>>(jsonString, options) ?? new ObservableCollection<Car>();
            }
        }

        void Serialize()
        {
            string filePath = "autot.json";
            string jsonString = JsonSerializer.Serialize(cars, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, jsonString);
        }

        private void OpenWindow(string name)
        {
            ChangeWindowVisibility(currentWindow, false);
            currentWindow = this.FindName(name) as Grid;
            ChangeWindowVisibility(currentWindow, true);
        }

        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            ChangeWindowVisibility(currentWindow, false);
            currentWindow = this.FindName((string)((Button)sender).Tag) as Grid;
            ChangeWindowVisibility(currentWindow, true);
        }

        private void ChangeWindowVisibility(Grid window, bool setVisible)
        {
            if (currentWindow != null)
            {
                currentWindow.Visibility = setVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        void ShowGraph()
        {
            if (currentCar == null) return;
            SortMeasurements();
            PointCollection graphPoints = new PointCollection();
            for (int i = 0; i < currentCar.Measurements.Count; i++)
            {
                graphPoints.Add(new Point(
                    i * Grahp.Width / currentCar.Measurements.Count,
                    Grahp.Height - currentCar.Measurements[i].oilAmount / graphMaxHeight * Grahp.Height
                ));
            }
            Grahp.Points = graphPoints;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
