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

namespace Oil_Consumption_Tracker
{
    public class Auto
    {
        public string rekisteriNumero { get; set; }
        public List<Mittaus> mittaukset { get; set; } = new List<Mittaus>();

        public Auto(string rekisteriNumero)
        {
            this.rekisteriNumero = rekisteriNumero;
        }
    }

    public class Mittaus
    {
        public double oljynMaara { get; set; }
        public DateTime paivays { get; set; }
        public Auto auto { get; set; }

        public Mittaus(double oljynMaara, Auto auto)
        {
            this.oljynMaara = oljynMaara;
            this.auto = auto;
            this.paivays = DateTime.Now;
        }
    }

    public partial class MainWindow : Window
    {

        public List<Auto> autot { get; set; } = new List<Auto>();
        
        public MainWindow()
        {
            DataContext = this;

            deserialize();
            InitializeComponent();

            Auto auto1 = new Auto("KJE-123");
            autot.Add(auto1);
            auto1.mittaukset.Add(new Mittaus(5.3, auto1));
            auto1.mittaukset.Add(new Mittaus(4.7, auto1));
            auto1.mittaukset.Add(new Mittaus(3.9, auto1));

            Auto auto2 = new Auto("FIS-344");
            autot.Add(auto2);
            auto2.mittaukset.Add(new Mittaus(10.4, auto2));
            auto2.mittaukset.Add(new Mittaus(8.2, auto2));
        }

        void deserialize()
        {

        }

        void serialize()
        {

        }
    }
}
