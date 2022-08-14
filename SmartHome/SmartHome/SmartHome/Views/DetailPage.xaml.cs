using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using SmartHome.Models;
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
    public partial class DetailPage : ContentPage
    {
        public IMqttClient client;
        public IMqttClientOptions options;
        string topicSubLight = "/v1.6/devices/esp8266/lightsensor/lv";
        string topicPubChung = "/v1.6/devices/esp8266/temperature";
        string topicSubChung = "/v1.6/devices/esp8266/temperature/lv";
        int  valueLight = 0, count = 1, status = 0;

        public DetailPage(DeviceModel device)
        {
            InitializeComponent();
            var vm = (DetailViewModel)BindingContext;
            if (vm != null)
            {
                vm.Device = device;
            }
            InitMqtt();
            connect();

        }

        public void InitMqtt()
        {
            var mqttFactory = new MqttFactory();
            client = mqttFactory.CreateMqttClient();
            options = new MqttClientOptionsBuilder()
                .WithClientId("viet")
                .WithCredentials("BBFF-FQE4IHzsudYIO5RMbBhjvn7Fz7Y2LR", "BBFF-FQE4IHzsudYIO5RMbBhjvn7Fz7Y2LR")
                .WithTcpServer("industrial.api.ubidots.com", 1883)
                .WithCleanSession()
                .Build();

            client.UseConnectedHandler(async e =>
            {
                Console.WriteLine("Connected");
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(topicSubChung).Build());
                await client.SubscribeAsync(new TopicFilterBuilder().WithTopic(topicSubLight).Build());
            });

            client.UseDisconnectedHandler(e =>
            {
                Console.WriteLine("Disconnected");
            });

            client.UseApplicationMessageReceivedHandler(e =>
            {
                string ReceivedMessage = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                string topicReceived = e.ApplicationMessage.Topic;
                int data = (int)Double.Parse(ReceivedMessage);
                Console.WriteLine($"Received message -{topicReceived} data is {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");


                Device.BeginInvokeOnMainThread(() =>
                {
                    
                    if (topicReceived == topicSubLight)
                    {
                        valueLight = data;
                        LightSensorResult.Text = data.ToString();
                    }
                    else if (topicReceived == topicSubChung)
                    {
                        status = data;
                        if ((status & (1 << window.TabIndex)) == 4)
                        {
                            //bit2.Text = "1";
                            window.Background = Brush.LightGreen;
                        } else
                        {
                            //bit2.Text = "0";
                            window.Background = Brush.LightGray;
                        }

                        if ((status & (1 << curtain.TabIndex)) == 2)
                        {
                            //bit1.Text = "1";
                            curtain.Background = Brush.LightGreen;
                        }
                        else
                        {
                            //bit1.Text = "0";
                            curtain.Background = Brush.LightGray;
                        }

                        if ((status & (1 << lockwindow.TabIndex)) == 1)
                        {
                            //bit0.Text = "1";
                            lockwindow.Background = Brush.LightGreen;
                        }
                        else
                        {
                            //bit0.Text = "0";
                            lockwindow.Background = Brush.LightGray;
                        }

                        if ((status & (1 << RainSensorResult.TabIndex)) == 8)
                        {
                            RainSensorResult.Text = "1";
                        }
                        else
                        {
                            RainSensorResult.Text = "0";
                        }

                        if ((status & (1 << MoveSensorResult.TabIndex)) == 16)
                        {
                            MoveSensorResult.Text = "1";
                        }
                        else
                        {
                            MoveSensorResult.Text = "0";
                        }
                    }

                    if (count == 0)
                    {
                        
                        //controlDevices();
                    }
                    else
                    {
                        count--;
                    }
                    Console.WriteLine("Gia tri count: " + count);
                });

            });
        }


        void controlDevices()
        {
            System.Threading.Thread.Sleep(3000);
            Console.WriteLine("Chay vao trong controlDevices.");
            if (((status & (1 << MoveSensorResult.TabIndex)) == 0) && (((status & (1 << window.TabIndex)) != 0) || ((status & (1 << curtain.TabIndex)) != 0) || ((status & (1 << lockwindow.TabIndex)) != 0)))
            {
                status &= 24;
                Console.WriteLine("&24 status la : " + status);
                PublishMessages(topicPubChung, status.ToString());
            }
            else if ((status & (1 << MoveSensorResult.TabIndex)) == 16)
            {
                if ((status & (1 << RainSensorResult.TabIndex)) == 8)
                {
                    if ((valueLight > 450) && (((status & (1 << window.TabIndex)) != 0) || ((status & (1 << curtain.TabIndex)) != 0) || ((status & (1 << lockwindow.TabIndex)) != 0)))
                    {
                        status &= 24;
                        Console.WriteLine(">450 khi co mua status la : " + status);
                        PublishMessages(topicPubChung, status.ToString());
                    }
                    else if (((status & (1 << window.TabIndex)) != 0) || ((status & (1 << curtain.TabIndex)) != 2) || ((status & (1 << lockwindow.TabIndex)) != 0))
                    {
                        status |= 1 << curtain.TabIndex;
                        status &= ~(1 << window.TabIndex);
                        status &= ~(1 << lockwindow.TabIndex);
                        Console.WriteLine("<450 khi co mua status la : " + status);
                        PublishMessages(topicPubChung, status.ToString());
                    }
                }
                else
                {
                    if ((valueLight > 450) && ((status & (1 << window.TabIndex)) != 4) || ((status & (1 << curtain.TabIndex)) != 0) || ((status & (1 << lockwindow.TabIndex)) != 1))
                    {
                        status &= ~(1 << curtain.TabIndex);
                        status |= 1 << window.TabIndex;
                        status |= 1 << lockwindow.TabIndex;
                        Console.WriteLine(">450 khi khong mua status la : " + status);
                        PublishMessages(topicPubChung, status.ToString());
                    }
                    else if (((status & (1 << window.TabIndex)) != 4) || ((status & (1 << curtain.TabIndex)) != 2) || ((status & (1 << lockwindow.TabIndex)) != 1))
                    {
                        status |= 1 << curtain.TabIndex;
                        status |= 1 << window.TabIndex;
                        status |= 1 << lockwindow.TabIndex;
                        Console.WriteLine("<450 khi khong mua status la : " + status);
                        PublishMessages(topicPubChung, status.ToString());
                    }
                }
            }

            
            
            
        }

        public async void connect()
        {
            await client.ConnectAsync(options);
        }

        public async void disconnectMqtt()
        {
            await client.DisconnectAsync();
        }

        public async void PublishMessages(string topicMsg, string payloadMsg)
        {
            
            var message = new MqttApplicationMessageBuilder()
             .WithTopic(topicMsg)
             .WithPayload(payloadMsg)
             .WithExactlyOnceQoS()
             .WithRetainFlag()
             .Build();
            Console.WriteLine("Publish topic " + topicMsg + " value: " + payloadMsg);
            await client.PublishAsync(message);
            
        }

        private void window_Clicked(object sender, EventArgs e)
        {
            if ((status & (1 << window.TabIndex)) == 0)
            {
                status ^= 1 << window.TabIndex;
                status ^= 1 << lockwindow.TabIndex;
            }
            else
            {
                status &= ~(1 << window.TabIndex);
                status &= ~(1 << lockwindow.TabIndex);
            }
            PublishMessages(topicPubChung, status.ToString());
        }

        private void curtain_Clicked(object sender, EventArgs e)
        {
            if ((status & (1 << curtain.TabIndex)) == 0)
            {
                status ^= 1 << curtain.TabIndex;
            }
            else
            {
                status &= ~(1 << curtain.TabIndex);
            }
            PublishMessages(topicPubChung, status.ToString());
        }

        private void lockwindow_Clicked(object sender, EventArgs e)
        {
            if ((status & (1 << lockwindow.TabIndex)) == 0)
            {
                status ^= 1 << lockwindow.TabIndex;
                status ^= 1 << window.TabIndex;
            }
            else
            {
                status &= ~(1 << lockwindow.TabIndex);
                status &= ~(1 << window.TabIndex);
            }
            PublishMessages(topicPubChung, status.ToString());
        }
    }
}