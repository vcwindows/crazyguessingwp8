using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrazyGuessing.Resources;

namespace CrazyGuessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        Accelerometer _ac;

        public MainPage()
        {
            InitializeComponent();

            _ac = new Accelerometer();
            _ac.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(_ac_ReadingChanged);
        }

        private void _ac_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            //通过Dispatcher.BeginInvoke方法来更新UI，传入事件变量AccelerometerReadingEventArgs
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessAccelerometerReading(e));
        }

        private bool isStart = true;
        private int count = 0;
        private bool isStatus = false;

        private void ProcessAccelerometerReading(AccelerometerReadingEventArgs e)
        {
            if (Math.Abs(e.Z) < 0.2 && isStart)
            {
                isStart = false;
                MessageBox.Show("Start!");
            }

            if (isStart) return;

            if (e.Z > 0.6) isStatus = true;
            if (Math.Abs(e.Z) < 0.2 && isStatus)
            {
                isStatus = false;
                count++;
            }

        }

        private void M_1Button_OnClick(object sender, RoutedEventArgs e)
        {
            

            MPage1Panel.Visibility= Visibility.Collapsed;
        }
    }
}