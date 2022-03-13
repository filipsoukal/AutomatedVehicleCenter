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

    public delegate void WeatherUpdateHandler(Weather newWeather);

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
            WeatherTxBlk.Text = Cars.CarList[Carindex].CurrentWeather.WeatherType.ToString();

        }
	}

    public class Car : EventArgs
    {
        Random rng = new Random();
        public Car(int id, double speed, RoadTypes roadType, double routeLength, double routeProgress = 0)
        {
            ID = id;
            tempSpeed = speed;
            RoadType = roadType;
            tempLength = routeLength;
            RouteProgress = routeProgress;
            VehicleStatus = 0;
        }

        public int ID { get; set; }
        public double Speed // m/s
        {
            get { return this.Speed; }
            set { this.Speed = tempSpeed / 3.6; }
        }
        public double RouteLength // m
        {
            get { return this.RouteLength; }
            set { this.RouteLength = tempLength * 1000; }
        } 
        public double RouteProgress { get; set; } // m
        public bool LightsOn { get; set; }

        public VehicleStatusTypes VehicleStatus { get; set; }
        public RoadTypes RoadType { get; set; }
        public Weather CurrentWeather { get; set; }

        public enum RoadTypes { Normal, Tunnel, Bridge, Highway }
        public enum VehicleStatusTypes { Operational, LightAccident, HeavyAccident} // status provozu vozidla

        public event CarUpdateHandler CarAccident; // eventy pro control center, 
        public event CarUpdateHandler RoadChange;
        
        private const int tick = 1000; // Update frequency (ms)
        private int roadChangeChances = 0;

        private double tempSpeed = 0, tempLength = 0;

        public void Drive() // hlavni loop pro pohyb vozidla a aktualizace jeho stavu
        {
            bool go = true;
            do
            {
          
                RouteProgress = RouteProgress + Speed; // vyřešit převod z km na m
                go = RouteProgress >= RouteLength ? false : true;
                if (CheckCarAccident()) CarAccident(this.ID);
                if (RoadChanged()) RoadChange(this.ID);
                
                System.Threading.Thread.Sleep(tick);
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
            if (res == true) roadChangeChances = 0;
            else roadChangeChances++;

            return res;
        }

        private void SetWeather(Weather w) => this.CurrentWeather = w;    
        public void Subscribe(WeatherCenter wc) => wc.WeatherUpdate += SetWeather;

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

        public ControlCenter(List<Car> cars) => Cars = cars;

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

            activeCar.LightsOn =
                activeCar.CurrentWeather.BadLightingConditions ? true :
                activeCar.RoadType == Car.RoadTypes.Tunnel ? true : false; 
        }

        public static List<Car> GetCars(int numOfCars)
        {
            Random rng = new Random();
            List<Car> retCars = new List<Car>();
            for (int i = 0; i < numOfCars; i++)
            {
                Car newCar = new Car(i, 50, Car.RoadTypes.Normal, rng.Next(10, 151));
            }

            return retCars;
        }

        private void ResolveAccident(int id)
        {
            ActiveID = id;
            int chances = 5;
            Cars[ActiveID].VehicleStatus = rng.Next(0, chances) == 1 ? Car.VehicleStatusTypes.HeavyAccident : Car.VehicleStatusTypes.LightAccident;

        }


        public void Subscribe()
        {
            foreach (var c in Cars)
            {
                c.RoadChange += ChangeCarStats;
                c.CarAccident += ResolveAccident;
            }
        }
        
    }
    #region weather
    public class WeatherCenter
    {
        Random rng = new Random();
        public event WeatherUpdateHandler WeatherUpdate; //event pro zmenu pocasi

        public WeatherCenter() => ChangeWeather();
        
        public void ChangeWeather() // hlavni loop pro zmenu pocasi
        {
            const int chance = 150, tick = 1000;
            while (true)
            {
                if (rng.Next(0,chance) == 1)
                {
                    Weather w = GetWeather();
                    WeatherUpdate(w);
                }
                System.Threading.Thread.Sleep(tick);
            }
        }

        public Weather GetWeather() // generace noveho pocasi
        {
            Random rng = new Random();
            Weather res = new Weather();

            res.Wind = rng.Next(5, 40);
            res.WeatherType = (Weather.WeatherTypes)rng.Next(0, 3);
            res.Temperature =
                res.WeatherType == Weather.WeatherTypes.Sunny ? rng.Next(15, 41)
                : res.WeatherType == Weather.WeatherTypes.Snow ? rng.Next(-15, 3)
                : rng.Next(8, 21);
            res.BadLightingConditions = res.WeatherType != Weather.WeatherTypes.Sunny ? false : true;
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
    #endregion

    public class Cars
    {
        public static List<Car> CarList { get; set; } = ControlCenter.Cars;

        public Cars() => Subscribe();

        private void UpdateList(int id) => CarList = ControlCenter.Cars;

        public void Subscribe()
        {
            foreach (var c in CarList)
            {
                c.RoadChange += UpdateList;
                c.CarAccident += UpdateList;
            }
        }
   //     public static List<Car> GetCars()
   //     {
   //         var list = new List<Car>();
   //          Random rnd = new Random();
			//for(int i = 1;i <= 30;i++) {
   //             list.Add(new Car(i,rnd.Next(50,201),0,rnd.Next(50,201)));
			//}
   //         return list;
   //     }
    }
}
