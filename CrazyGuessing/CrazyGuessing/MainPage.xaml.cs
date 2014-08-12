using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using CrazyGuessing.Resources;

namespace CrazyGuessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        Accelerometer _ac;
        List<string> pageList1 = new List<string>();
        Random random = new Random();

        public MainPage()
        {
            InitializeComponent();

            var i = string.Empty;
            using (StreamReader streamReader = new StreamReader(File.OpenRead("Page1.txt"), Encoding.UTF8))
            {
                i = streamReader.ReadToEnd();
            }

            var temp = string.Empty;
            foreach (var item in i)
            {
                if (item == '\n' || item == '\r')
                {
                    if (temp != string.Empty)
                    {
                        pageList1.Add(temp);
                        temp = string.Empty;
                    }
                }
                else
                {
                    temp = temp + item;
                }
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (timerCount > 0)
            {
                timerCount--;

                MTimesTextBlock.Text = (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();
            }
            else
            {
                MCountTextBlock.Text = totalCount.ToString();
                Reset();
            }
        }

        private void Reset()
        {
            _ac.ReadingChanged -= _ac_ReadingChanged;
            _ac.Stop();
            _ac = null;
            timer.Stop();
            timer.Tick -= timer_Tick;
            timer = null;
            count = 0;
            timerCount = 72;
            isStart = true;
            isSkip = false;
            isStatus = false;
            MNumbersTextBlock.Text = "";
            MNotificationTextBlock.Text = "";
            MStringTextBlock.Text = "";
            MTimesTextBlock.Text = "";
            MCountPanel.Visibility = Visibility.Visible;
        }

        private void _ac_ReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            //通过Dispatcher.BeginInvoke方法来更新UI，传入事件变量AccelerometerReadingEventArgs
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessAccelerometerReading(e));
        }

        private bool isStart = true;
        private bool isSkip = false;
        private int count = 0;
        private bool isStatus = false;
        private int totalCount = 0;

        private DispatcherTimer timer = null;
        private int timerCount = 20;
        private DateTime dataTime = new DateTime();

        private void ProcessAccelerometerReading(AccelerometerReadingEventArgs e)
        {
            if (Math.Abs(e.Z) < 0.2 && isStart)
            {
                isStart = false;
                MStringTextBlock.Text = pageList1[random.Next(0, pageList1.Count - 1)];
            }

            if (isStart)
            {
                MNotificationTextBlock.Visibility = Visibility.Visible;
                return;
            }

            MNotificationTextBlock.Visibility = Visibility.Collapsed;

            if (timer==null)
            {
                timer = new DispatcherTimer();
                timer.Tick += timer_Tick;
                timer.Interval = new TimeSpan(0, 0, 0, 1);
                timer.Start();
                MTimesTextBlock.Text = (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();
            }
            if (e.Z > 0.5) isStatus = true;
            if (e.Z < -0.5) isSkip = true;
            if (Math.Abs(e.Z) < 0.2 && isStatus)
            {
                isStatus = false;
                new Thread(
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            if (DateTime.Now - dataTime <= new TimeSpan(0, 0, 0, 1)) return;
                            dataTime = DateTime.Now;
                            MStringTextBlock.Text = pageList1[random.Next(0, pageList1.Count - 1)];
                            totalCount++;
                        });
                    }).Start();
            }
            if (Math.Abs(e.Z) < 0.2 && isSkip)
            {
                isSkip = false;
                new Thread(
                    () =>
                    {
                        if (DateTime.Now - dataTime <= new TimeSpan(0, 0, 0, 1)) return;
                        dataTime = DateTime.Now;
                        Dispatcher.BeginInvoke(() =>
                        {
                            MStringTextBlock.Text = pageList1[random.Next(0, pageList1.Count - 1)];
                        });
                    }).Start();
            }
        }

        private void M_1Button_OnClick(object sender, RoutedEventArgs e)
        {
            new Thread(
                () =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        MPage1Panel.Visibility = Visibility.Collapsed;
                        MNumbersTextBlock.Text = "3";
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MNumbersTextBlock.Text = "2";
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MNumbersTextBlock.Text = "1";
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MNumbersTextBlock.Text = "Go";
                    });
                    Thread.Sleep(500);
                    Dispatcher.BeginInvoke(() =>
                    {
                        MNumbersTextBlock.Text = "";
                    });

                    _ac = new Accelerometer();
                    _ac.ReadingChanged += new EventHandler<AccelerometerReadingEventArgs>(_ac_ReadingChanged);
                    _ac.Start();
                }).Start();
        }

        private void ReturnButton_OnClick(object sender, RoutedEventArgs e)
        {
            MPage1Panel.Visibility = Visibility.Visible;
            MCountPanel.Visibility = Visibility.Collapsed;
        }
    }
}