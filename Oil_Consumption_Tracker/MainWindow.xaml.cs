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

namespace Oil_Consumption_Tracker
{
    public class Car
    {
        public string registerNumber { get; set; }
        public List<Measurement> measurements { get; set; } = new List<Measurement>();

        public Car(string registerNumber)
        {
            this.registerNumber = registerNumber;
        }
    }

    public class Measurement
    {
        public double oilAmount { get; set; }
        public DateTime date { get; set; }
        public Car car { get; set; }

        public Measurement(double oljynMaara, Car Car)
        {
            this.oilAmount = oljynMaara;
            this.car = Car;
            this.date = DateTime.Now;
        }
    }

    public partial class MainWindow : Window
    {

        public List<Car> cars { get; set; } = new List<Car>();
        public Grid currentWindow;
        public double graphMaxHeight = 15;

        public MainWindow()
        {
            deserialize();
            InitializeComponent();
            currentWindow = mainMenuWindow;

            DataContext = this;

            TestiArvot();
        }

        void TestiArvot()
        {
            Car car1 = new Car("KJE-123");
            car1.measurements.Add(new Measurement(5.3, car1));
            car1.measurements.Add(new Measurement(4.7, car1));
            car1.measurements.Add(new Measurement(3.9, car1));
            cars.Add(car1);

            Car car2 = new Car("FIS-344");
            car2.measurements.Add(new Measurement(10.4, car2));
            car2.measurements.Add(new Measurement(8.2, car2));
            car2.measurements.Add(new Measurement(2.1, car2));
            car2.measurements.Add(new Measurement(4.2, car2));
            car2.measurements.Add(new Measurement(0.2, car2));
            cars.Add(car2);

            ShowGrahp(car2);
        }

        void deserialize()
        {

        }

        void serialize()
        {

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
