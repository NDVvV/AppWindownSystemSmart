using SmartHome.Models;
using SmartHome.Views;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace SmartHome.ViewModels
{
    public class HomeViewModel
    {
        public INavigation NavigationPage { get; set; }
        public ObservableCollection<DeviceModel> DeviceData { get; set; } = new ObservableCollection<DeviceModel>
        { new DeviceModel
            {
                Image ="livingroom2.png",
                NameRoom ="Living Room",
                NumberDevice="3",
                ColorBg ="#B2A4FF"
            },
            new DeviceModel
            {
                Image ="kitchen_panda.png",
                NameRoom ="Kitchen",
                NumberDevice="3",
                ColorBg ="#7D9D9C"
            },
            new DeviceModel
            {
                Image ="office_elephant.png",
                NameRoom ="Office",
                NumberDevice="3",
                ColorBg ="#D8CCA3"
            },
            new DeviceModel
            {
                Image ="bedroom_cat.png",
                NameRoom ="Bedroom",
                NumberDevice="3",
                ColorBg ="#FFFFDE"
            },

        };
        public ICommand SelectItemCommand { get; private set; }
        public HomeViewModel()
        {
            SelectItemCommand = new Command((obj) => SelectItem(obj));
        }

        private void SelectItem(object obj)
        {
            if (obj != null)
            {
                var data = (DeviceModel)obj;
                NavigationPage.PushAsync(new DetailPage(data));
            }
        }
    }
}