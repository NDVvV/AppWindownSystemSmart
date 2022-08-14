using MQTTnet.Client;
using MQTTnet.Client.Options;
using SmartHome.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome
{
    public partial class App : Application
    {

        

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage())
            {
                BarTextColor = Color.Black,
                BarBackgroundColor = Color.White
            };
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
