using SmartHome.Models;
using SmartHome.Views;
using System;
using System.Collections.Generic;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

using Xamarin.Forms;

namespace SmartHome.ViewModels
{
    public class DetailViewModel : INotifyPropertyChanged 
    {
        private DeviceModel device;


        public DeviceModel Device { get => device; set { device = value; NotifyPropertyChanged("Device"); } }

        public event PropertyChangedEventHandler PropertyChanged;
       
        public DetailViewModel()
        {
           
        }


        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
