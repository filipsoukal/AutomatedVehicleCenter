﻿using System;
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

    public class Car : EventArgs
    {
        public Car(double speed, RoadTypes roadType, double routeLength, double routeProgress = 0)
        {
            Speed = speed;
            RoadType = roadType;
            RouteLength = routeLength;
            RouteProgress = routeProgress;
            VehicleStatus = 0;
        }

        public double Speed { get; set; }
        public double RouteLength { get; set; }
        public double RouteProgress { get; set; }
        public VehicleStatusTypes VehicleStatus { get; set; }
        public RoadTypes RoadType { get; set; }
        public Weather CurrentWeather { get; set; }

        public enum RoadTypes { Normal, Tunnel, Bridge, Highway }
        public enum VehicleStatusTypes { Operational, LightAccident, HeavyAccident} // status provozu vozidla

        public event CarUpdateHandler CarUpdate; // event pro control center, 

        private const double deltaTime = 0.001;

       public void Drive() // hlavni loop pro pohyb vozidla a aktualizace jeho stavu
        {
            bool go = true;
            do
            {
                RouteProgress = RouteProgress + Speed * deltaTime;
                if (RouteProgress >= RouteLength) go = false;
            } while (go);

        }

        public void CarAccident()
        {
            
        }

        private void ChangeDrivingStyle()
        {

        }

    }
    public class TowCar : Car{
        
        public TowCar(double speed, RoadTypes roadType, double routeLength, double routeProgress = 0) : base(speed,roadType,routeLength,routeProgress)
        {
            
        }
        public void FetchCar()
        {

        }
        public void TowCarToCC()
        {

        }
    }

    public delegate void CarUpdateHandler(Car car);

    public delegate void WeatherUpdateHandler(Weather weather);

    public class ControlCenter
    {

        public static List<Car> Cars = new List<Car>();
        

        public void MonitorACar()
        {
            // subscribe
        }
    }
    public class Visualization
    {
        public void AddCarToList()
        {

        }
        public void RemoveCarFromList()
        {

        }
        private void ListItemClick()
        {

        }
        public void ChangeIconToTowCar()
        {

        }


    }

    public class WeatherCenter
    {
        public event WeatherUpdateHandler WeatherUpdate; //event pro zmenu pocasi
        public void ChangeWeather() // hlavni loop pro zmenu pocasi
        {

        }

        public static Weather GetWeather()
        {

            Weather res = new Weather();

            res.Wind = 20;
            res.Temperature = 15;
            res.BadLightingConditions = false;
            res.WeatherType = Weather.WeatherTypes.Sunny;
            return res;

        }
    }

    public class Weather : EventArgs // typ pocasi 
    {
        public double Wind { get; set; }
        public double Temperature { get; set; }
        public bool BadLightingConditions { get; set; }
        public WeatherTypes WeatherType { get; set; }
        public enum WeatherTypes { Sunny, Rain, Storm, Snow }
        
    }
}
