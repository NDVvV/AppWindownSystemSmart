using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SmartHome.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {

        /*public IMqttClient client;
        public IMqttClientOptions options;*/

        public HomePage()
        {
            InitializeComponent();
            var vm = (HomeViewModel)BindingContext;
            if (vm != null)
               vm.NavigationPage = Navigation;
            
        }

        











    }
}