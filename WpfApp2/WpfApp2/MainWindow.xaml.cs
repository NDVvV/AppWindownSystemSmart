using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WpfAppWindownSystemSmart
{

    public partial class MainWindow : Window
    {
        MqttClient client;
        string BrokerAddress = "industrial.api.ubidots.com";
        string id = "20182886";
        string user = "";//token ubidot
        string pass = "";//token ubidot
        string topicSubLight = "/v1.6/devices/esp8266/lightsensor/lv";
        string topicPubLight = "/v1.6/devices/esp8266/lightSensor";
        string topicPubStatus = "/v1.6/devices/esp8266/mrwcl";
        string topicSubStatus = "/v1.6/devices/esp8266/mrwcl/lv";
        int status = 0, lightLevel = 0;
        string urlBase = "C:/Users/DucViet/source/repos/WpfApp2/WpfApp2/Image/";

        public MainWindow()
        {

            InitializeComponent();

            client = new MqttClient(BrokerAddress);

            // register a callback-function (we have to implement, see below) which is called by the library when a message was received
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;

            client.Connect(id, user, pass);
            client.Subscribe(new string[] { topicSubLight }, new byte[] { 0 });
            client.Subscribe(new string[] { topicSubStatus }, new byte[] { 0 });
        }

        void showImg()
        {
            string tempUrl = "";
            tempUrl = (status & (1 << 2)) == 4 ? "windownsOpen.png" : "windownsClose.png";
            imgWindowns.Source = new BitmapImage(new Uri(urlBase + tempUrl));

            tempUrl = (status & (1 << 1)) == 2 ? "curtainsOpen.png" : "curtainsClose.png";
            imgCurtains.Source = new BitmapImage(new Uri(urlBase + tempUrl));

            tempUrl = (status & (1 << 0)) == 1 ? "lockOpen.png" : "lockClose.png";
            imgMyLock.Source = new BitmapImage(new Uri(urlBase + tempUrl));
        }

        void showTB(int data, string topicReceived)
        {
            if (topicReceived == topicSubLight)
            {
                txtBlockLight.Text = data.ToString();
            }
            else if (topicReceived == topicSubStatus)
            {
                status = data;
                if ((status & (1 << ButtonMoveSensor.TabIndex)) == 16)
                {
                    txtBlockMove.Text = "1";
                    ButtonMoveSensor.Background = Brushes.LightGreen;
                }
                else
                {
                    txtBlockMove.Text = "0";
                    ButtonMoveSensor.Background = Brushes.LightGray;
                }

                if ((status & (1 << ButtonRainSensor.TabIndex)) == 8)
                {
                    txtBlockRain.Text = "1";
                    ButtonRainSensor.Background = Brushes.LightGreen;
                }
                else
                {
                    txtBlockRain.Text = "0";
                    ButtonRainSensor.Background = Brushes.LightGray;
                }

                if ((status & (1 << 2)) == 4)
                {
                    if ((status & (1 << 0)) == 0)
                    {
                        status |= 1;
                        Thread.Sleep(3000);
                        client.Publish(topicPubStatus, Encoding.ASCII.GetBytes(status.ToString()));
                    }
                        
                }

                if ((status & (1 << 0)) == 0)
                {
                    if ((status & (1 << 2)) == 1)
                    {
                        status &= ~(1 << 2);
                        Thread.Sleep(3000);
                        client.Publish(topicPubStatus, Encoding.ASCII.GetBytes(status.ToString()));
                    }
                        
                }
            }
            
        }

        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            string ReceivedMessage = Encoding.UTF8.GetString(e.Message);
            string topicMsgPublishReceived = e.Topic;
            int integerData = (int)Double.Parse(ReceivedMessage);


            Dispatcher.Invoke(delegate
            {// we need this construction because the receiving code in the library and the UI with textbox run on different threads

                showTB(integerData, topicMsgPublishReceived);
                showImg();
            });
        }

        private void ButtonLightSensorClick(object sender, RoutedEventArgs e)
        {
            
            if (int.TryParse(txtBox.Text, out lightLevel) && (lightLevel >= 0) && (lightLevel <= 100000))
            {
                client.Publish(topicPubLight, Encoding.ASCII.GetBytes(txtBox.Text.ToString()));
            }
            else
            {
                txtBlockLight.Text = "Vui long nhap lai";
            }
        }

        private void ButtonMoveSensorClick(object sender, RoutedEventArgs e)
        {
            if ((status & (1 << 4)) == 0)
            {
                status |= 1 << 4;
            }
            else
                status &= ~(1 << 4);
            client.Publish(topicPubStatus, Encoding.ASCII.GetBytes(status.ToString()));
        }

        private void ButtonRainSensorClick(object sender, RoutedEventArgs e)
        {
            if ((status & (1 << 3)) == 0)
            {
                status |= 1 << 3;
            }
            else
                status &= ~(1 << 3);
            client.Publish(topicPubStatus, Encoding.ASCII.GetBytes(status.ToString()));
        }

        protected override void OnClosed(EventArgs e)
        {
            client.Disconnect();

            base.OnClosed(e);
            App.Current.Shutdown();
        }
    }
}
