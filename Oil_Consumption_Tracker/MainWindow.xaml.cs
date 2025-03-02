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

namespace Oil_Consumption_Tracker
{
    public class Car
    {
        public string registerNumber { get; set; } = "";
        public string manufacturer { get; set; } = "";
        public string motorCode { get; set; } = "";
        //ObservableCollection on sama kuin lista, mutta gridin näyttäminen toimii samaan aikaan kun muuttaa sisältöä.
        public ObservableCollection<Measurement> measurements { get; set; } = new ObservableCollection<Measurement>();

        public Car(string registerNumber, string manufacture = "No manufacturer found", string motorCode = "No motorcode found")
        {
            this.registerNumber = registerNumber;
            this.manufacturer = manufacture;
            this.motorCode = motorCode;
        }
        public Car() { }
    }

    public class Measurement
    {
        public double oilAmount { get; set; } = 0;
        public DateTime date { get; set; } = DateTime.Now;

        public Measurement(double oilAmount)
        {
            this.oilAmount = oilAmount;
            this.date = DateTime.Now;
        }

        public Measurement() { }
    }

    public partial class MainWindow : Window
    {

        public ObservableCollection<Car> cars { get; set; } = new ObservableCollection<Car>();
        public Grid currentWindow; //The window that is currently visible.
        public double graphMaxHeight = 15; 

        public MainWindow()
        {
            InitializeComponent();
            currentWindow = mainMenuWindow;

            DataContext = this;
            //TestiArvot();
            Deserialize();
        }

        void TestiArvot()
        {
            cars.Add(new Car("KJE-123"));
            cars[0].measurements.Add(new Measurement(5.3));
            cars[0].measurements.Add(new Measurement(4.7));
            cars[0].measurements.Add(new Measurement(3.9));

            cars.Add(new Car("FIS-344"));
            cars[1].measurements.Add(new Measurement(10.4));
            cars[1].measurements.Add(new Measurement(8.2));
            cars[1].measurements.Add(new Measurement(2.1));
            cars[1].measurements.Add(new Measurement(4.2));
            cars[1].measurements.Add(new Measurement(0.2));
        }

        void ValidateRegisternumber(object sender, TextChangedEventArgs e)
        {
            saveCarButton.IsEnabled = !(addRegisterNumber.Text == "");
        }

        void AddCar(object sender, RoutedEventArgs e)
        {
            if(addRegisterNumber.Text == "")
            {
                return;
            }
            Application.Current.Dispatcher.Invoke(() => cars.Add(new Car(addRegisterNumber.Text, addManufacturer.Text, addMotorcode.Text)));
            Serialize();
            OpenWindow("mainMenuWindow");
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

        private void OpenWindow(string name) //Called with a code
        {
            ChangeWindowVisibility(currentWindow, false);
            currentWindow = this.FindName(name) as Grid;
            ChangeWindowVisibility(currentWindow, true);
        }

        private void OpenWindow(object sender, RoutedEventArgs e) // Called with WPF button
        {
            ChangeWindowVisibility(currentWindow, false);
            currentWindow = this.FindName((string)((Button)sender).Tag) as Grid;
            ChangeWindowVisibility(currentWindow, true);
        }

        private void ChangeWindowVisibility(Grid window, bool setVisible)
        {
            if (currentWindow != null)
            {
                if(setVisible)
                {
                    currentWindow.Visibility = Visibility.Visible;
                }
                else
                {
                    currentWindow.Visibility = Visibility.Collapsed;
                }
            }
        }

        void ShowGrahp(Car Car)
        {
            PointCollection grahpPoints = new PointCollection();
            for (int i = 0; i < Car.measurements.Count; i++)
            {
                grahpPoints.Add(new Point(i * Grahp.Width / Car.measurements.Count, Car.measurements[i].oilAmount / graphMaxHeight * Grahp.Height));
            }
            Grahp.Points = grahpPoints;
        }
    }
}
