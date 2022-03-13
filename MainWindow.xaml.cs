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
using System.IO;

namespace AutomatedVehicle
{

    public delegate void CarUpdateHandler(int id);

    public delegate void WeatherUpdateHandler();

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ChangeUItoCar();
            
        }
        private Car selectedCar;
        private void listView_Click(object sender, RoutedEventArgs e) {
            var item = (sender as ListView).SelectedItem;
            if (item != null) {
                selectedCar = Cars.CarList[(sender as ListView).SelectedIndex];
                ChangeUItoCar();
            }
            Visualization Viz = new Visualization();
            
        }
        private void ChangeUItoCar() {
            int Carindex = 0;
            if(selectedCar != null) {
                Carindex = selectedCar.ID - 1;
            }
            CarNamelbl.Content = "Car " + (Carindex+1);
            SpeedTxBlk.Text = Cars.CarList[Carindex].Speed.ToString();
            StatusTxBlk.Text = Cars.CarList[Carindex].VehicleStatus.ToString();
            RoadTypeTxBlk.Text = Cars.CarList[Carindex].RoadType.ToString();
            LightsTxBlk.Text = Cars.CarList[Carindex].LightsOn.ToString();
            //WeatherTxBlk.Text = Cars.CarList[Carindex].CurrentWeather.ToString();

        }
	}

    public class Car : EventArgs
    {
        Random rng = new Random();
        public Car(int id, double speed, RoadTypes roadType, double routeLength, double routeProgress = 0)
        {
            ID = id;
            Speed = speed;
            RoadType = roadType;
            RouteLength = routeLength;
            RouteProgress = routeProgress;
            VehicleStatus = 0;
        }

        public int ID { get; set; }
        public double Speed { get; set; } // km/h
        public double RouteLength { get; set; } // km
        public double RouteProgress { get; set; } // km
        public bool LightsOn { get; set; }
        public VehicleStatusTypes VehicleStatus { get; set; }
        public RoadTypes RoadType { get; set; }
        public Weather CurrentWeather { get; set; }

        public enum RoadTypes { Normal, Tunnel, Bridge, Highway }
        public enum VehicleStatusTypes { Operational, LightAccident, HeavyAccident} // status provozu vozidla

        public event CarUpdateHandler CarAccident; // eventy pro control center, 
        public event CarUpdateHandler RoadChange;
        
        private const int deltaTime = 1000; // Update frequency (ms)
        private int roadChangeChances = 0;

        public void Drive() // hlavni loop pro pohyb vozidla a aktualizace jeho stavu
        {
            bool go = true;
            do
            {
                RouteProgress = RouteProgress + (Speed / 3.6); // vyřešit převod z km na m
                go = RouteProgress >= RouteLength ? false : true;
                if (CheckCarAccident()) CarAccident(this.ID);
                if (RoadChanged()) RoadChange(this.ID);
                
                System.Threading.Thread.Sleep(deltaTime);
            } while (go);
        }

        #region car accident
        private bool CheckCarAccident()
        {
            int chances = 0;
            bool res = false;

            switch (this.RoadType)
            {
                case RoadTypes.Normal:
                    chances = 5000;
                    chances -= WeatherCalc(false);
                    break;
                case RoadTypes.Tunnel:
                    chances = 5000;
                    break;
                case RoadTypes.Highway:
                    chances = 4500;
                    chances -= WeatherCalc(false);
                    break;
                case RoadTypes.Bridge:
                    chances = 4500;
                    chances -= WeatherCalc(true);
                    break;
            }

            int temp = chances * 100;

            res = rng.Next(0 , temp) % chances == 0 ? true : false;
            return res;
        }

        private int WeatherCalc(bool heavyImpact)
        {
            int res = 0;
            int amp = heavyImpact ? 2 : 1;
            
            res -=
                this.CurrentWeather.WeatherType == Weather.WeatherTypes.Rain ? 5 * amp :
                this.CurrentWeather.WeatherType == Weather.WeatherTypes.Storm ? 8 * amp :
                this.CurrentWeather.WeatherType == Weather.WeatherTypes.Snow ? 10 * amp : 0;
            return res;
        }
        #endregion
        private bool RoadChanged()
        {

            const int roof = 100;
            bool res = false;
            RoadTypes tempRoad = this.RoadType;

            int temp = rng.Next(0, roof);
            this.RoadType = temp <= roadChangeChances ? (RoadTypes)rng.Next(0, 4) : tempRoad;

            res = this.RoadType == tempRoad ? false : true;
            roadChangeChances += res == false ? 1 : 0;
            return res;

        }
		public override string ToString() {
			return ID.ToString();
		}
	}
    public class TowCar : Car{
        
        public TowCar(int id, double speed, RoadTypes roadType, double routeLength, double routeProgress = 0) : base(id,speed,roadType,routeLength,routeProgress)
        {
            
        }
        public void FetchCar()
        {

        }
        public void TowCarToCC()
        {

        }
    }


    public class ControlCenter
    {
        Random rng = new Random();
        public int ActiveID { get; set; }

        public static List<Car> Cars = new List<Car>();

        public ControlCenter(List<Car> cars)
        {
            Cars = cars;
        }
		//private void CreateCars() {

		//	foreach(Car item in CarList) {
		//		testbox.Text += item.RouteLength + "\n";
		//	}
		//}
		private void ChangeCarStats(int id)
        {
            ActiveID = id;
            Car activeCar = Cars[ActiveID];

            switch (activeCar.RoadType)
            {
                case Car.RoadTypes.Normal:
                    activeCar.Speed = 50;
                    break;
                case Car.RoadTypes.Tunnel:
                    activeCar.Speed = 110;
                    break;
                case Car.RoadTypes.Bridge:
                    activeCar.Speed = 130;
                    break;
                case Car.RoadTypes.Highway:
                    activeCar.Speed = 130;
                    break;
            }

            activeCar.LightsOn = activeCar.CurrentWeather.BadLightingConditions ? true : false;
        }

        private void ResolveAccident(int id)
        {
            ActiveID = id;
        }


        public void Activate()
        {
            foreach (var c in Cars)
            {
                c.RoadChange += ChangeCarStats;
                c.CarAccident += ResolveAccident;
            }
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
    public class Cars
    {
        public static List<Car> CarList { get; set; } = GetCars();
        public static List<Car> GetCars()
        {
            var list = new List<Car>();
             Random rnd = new Random();
			for(int i = 1;i <= 30;i++) {
                list.Add(new Car(i,rnd.Next(50,201),0,rnd.Next(50,201)));
			}
            return list;
        }
    }
}
