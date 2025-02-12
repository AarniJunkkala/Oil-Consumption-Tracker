using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Oil_Consumption_Tracker
{
    class Auto
    {
        string rekisteriNumero = "";
        List<Mittaus> mittaukset = new List<Mittaus>();
    }

    class Mittaus
    {
        double oljynMaara = 0;
        DateTime paivays;

        Mittaus(double oljynMaara = 0)
        {
            this.oljynMaara = oljynMaara;
        }
    }

    public partial class MainWindow : Window
    {
        List<Auto> autot = new List<Auto>();
        public MainWindow()
        {
            deserialize();
            InitializeComponent();
        }

        void deserialize()
        {

        }

        void serialize()
        {

        }
    }
}
