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

namespace AutomatedVehicle
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }



    public class Car
    {
        public double Speed { get; set; }
        public RoadTypes RoadType { get; set; }
        public double RouteLength { get; set; }

        public enum RoadTypes { Normal, Tunnel, Bridge }

       

        public void CarAccident()
        {

        }

        private void ChangeDriving()
        {

        }


    }

    public class ControlCenter
    {
        
    }

    public class WeatherCenter
    {
        public double Wind { get; set; }
        public double Temperature { get; set; }
        public bool BadLightingConditions { get; set; }
        public WeatherTypes WeatherType { get; set; }
        public enum WeatherTypes { Sunny, Rain, Storm, Snow }
        


    }
}
