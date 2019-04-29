using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ProjektNet
{
    public partial class MainWindow : Window
    {
        BackgroundWorker worker = new BackgroundWorker();


        private async void LoadWeatherData(object sender, RoutedEventArgs e)
        {
            string selectedCity = SelectedCity.Text;
            string responseXML;
            WeatherDataEntry result;


            while (true)
            {
                responseXML = await APIConnection.LoadDataAsync(selectedCity);
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
                {

                    result = ParseWeather_LINQ.Parse(stream);
                    var newEntry = new Pogoda();
                    newEntry.Miasto = result.City;
                    if (HumidityCheck.IsChecked == true)
                        newEntry.Wiatr = result.Humidity.ToString();
                    if (TempratureCheck.IsChecked == true)
                        newEntry.Temperatura = KelvinToCelsius.KelvToCel(result.Temperature);
                    if (PressureCheck.IsChecked == true)
                        newEntry.Ciśnienie = result.Pressure.ToString();

                    db.Pogoda.Add(newEntry);
                    db.SaveChanges();
                    if (worker.IsBusy != true)
                        worker.RunWorkerAsync();
                }
                await Task.Delay(3000);
            }

        }




        Database1Entities db = new Database1Entities();
        System.Windows.Data.CollectionViewSource categoryViewSource;
        System.Windows.Data.CollectionViewSource weatherEntitiesViewSource;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;



        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            db.Pogoda.Local.Concat(db.Pogoda.ToList());
            categoryViewSource =
             ((System.Windows.Data.CollectionViewSource)(this.FindResource("categoryViewSource")));
            weatherEntitiesViewSource =
               ((System.Windows.Data.CollectionViewSource)(this.FindResource("weatherEntitiesViewSource")));

            // After the data is loaded call the DbSet<T>.Local property
            // to use the DbSet<T> as a binding source.
            categoryViewSource.Source = db.Pogoda.Local;
            weatherEntitiesViewSource.Source = db.Pogoda.Local;






            db.SaveChanges();
            // Refresh the grids so the database generated values show up.
            this.weatherEntryDataGrid.Items.Refresh();

        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string selectedCity = CityTextBox.Text;
            string responseXML;
            WeatherDataEntry result;
            responseXML = await APIConnection.LoadDataAsync(CityTextBox.Text);
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseXML)))
            {

                result = ParseWeather_LINQ.Parse(stream);
                var newEntry = new Pogoda();
                newEntry.Miasto = result.City;
                if (HumidityCheck.IsChecked == true)
                {
                    newEntry.Wiatr = result.Humidity.ToString();


                }
                if (TempratureCheck.IsChecked == true)
                {
                    newEntry.Temperatura = result.Temperature;
                }
                if (PressureCheck.IsChecked == true)
                {
                    newEntry.Ciśnienie = result.Pressure.ToString();
                }
                db.Pogoda.Add(newEntry);
                db.SaveChanges();
                // try
                //{
                //    db.SaveChanges();
                //}
                //catch (Exception ex)
                //{
                //   db.Pogoda.Local.Remove(newEntry);
                // Debug.WriteLine("Error, id is not unique!");
                //}

            }

        }





        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            this.db.Dispose();
        }

        private void AddUser_Click(object sender, RoutedEventArgs e)
        {

            
                var newEntry = new Pogoda();
                newEntry.Miasto = CityUser.Text;
                newEntry.Wiatr = WindUser.Text;
                newEntry.Ciśnienie = PressureUser.Text;
                newEntry.Temperatura = float.Parse(TemperatureUser.Text);
                db.Pogoda.Add(newEntry);
                db.SaveChanges();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            int id = int.Parse(IdDelete.Text);
                // Get movie to delete
                var movieToDelete = db.Pogoda.First(m => m.Id == id);

            // Delete 
                db.Pogoda.Remove(movieToDelete);
                db.SaveChanges();

            
        }
    }
}
    

    