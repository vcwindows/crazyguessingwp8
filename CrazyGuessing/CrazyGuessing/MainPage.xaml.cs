using System.Windows.Controls;
using Microsoft.Devices.Sensors;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace CrazyGuessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        Accelerometer _ac;

        List<string> runningPageList = new List<string>();

        Random random = new Random();

        private bool isStart = false;
        private bool isRunning = false;
        private bool isSkip = false;
        private bool isOK = false;

        private int totalCount = 0;

        private DispatcherTimer timer = null;
        private int timerCount = 72;

        private DateTime lastDateTime = new DateTime();

        #endregion

        #region Constructor
        public MainPage()
        {
            InitializeComponent();

            _ac = new Accelerometer();
            _ac.CurrentValueChanged += _ac_CurrentValueChanged;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 1);
            timer.Tick += timer_Tick;
        }

        #endregion

        #region Events

        private void Page_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            var button = sender as Button;
            runningPageList = GetPageList("Page" + button.Tag.ToString() + ".txt");

            Prepare();
        }

        #endregion

        #region Private Methods

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!isRunning) return;
            if (isStart) return;

            if (timerCount == 0)
            {
                isRunning = false;
                isStart = false;
                isSkip = false;
                isOK = false;

                Dispatcher.BeginInvoke(() =>
                {
                    M_GamingPanel.Visibility = Visibility.Collapsed;
                    MCountTextBlock.Text = totalCount.ToString();
                    M_CountPanel.Visibility = Visibility.Visible;
                });

                timer.Stop();
                _ac.Stop();

                timerCount = 72;
            }

            if (timerCount == 10)
            {
                new Thread(() => Dispatcher.BeginInvoke(() => PlaySound("cardAppear")));
            }

            if (timerCount == 2)
            {
                new Thread(() => Dispatcher.BeginInvoke(() => PlaySound("GameEnd")));
            }

            if (timerCount > 0)
            {
                timerCount--;

                M_TimesTextBlock.Text = (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();
            }
        }

        private void Prepare()
        {
            M_FrontPagePanel.Visibility = Visibility.Collapsed;
            M_NotificationTextBlock.Visibility = Visibility.Visible;

            new Thread(
                () =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "猜对请前翻";
                        PlaySound("BeginCountDown");
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "放弃请后翻";
                        PlaySound("BeginCountDown");
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "切记勿手抖";
                        PlaySound("BeginCountDown");
                    });
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "Go!";
                        PlaySound("Begin");
                    });
                    Thread.Sleep(1000);

                    isStart = true;
                    isRunning = true;
                    _ac.Start();
                }).Start();
        }

        private List<string> GetPageList(string path)
        {
            var i = string.Empty;
            var pageList = new List<string>();
            using (var streamReader = new StreamReader(File.OpenRead(path), Encoding.UTF8))
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
                        pageList.Add(temp);
                        temp = string.Empty;
                    }
                }
                else
                {
                    temp = temp + item;
                }
            }

            return pageList;
        }

        private void PlaySound(string soundName)
        {
            SoundMediaElement.Source = new Uri("Resources/" + soundName + ".wav", UriKind.Relative);
            SoundMediaElement.Play();
        }

        #endregion

        void _ac_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => ProcessAccelerometerReading(e));
        }

        private void ProcessAccelerometerReading(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (!isRunning) return;
            if (isStart)
            {
                if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.3)
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        isStart = false;
                        M_NotificationTextBlock.Visibility = Visibility.Collapsed;
                        M_StringTextBlock.Text =
                            runningPageList[random.Next(0, runningPageList.Count - 1)];
                        M_TimesTextBlock.Text =
                                (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();

                        M_GamingPanel.Visibility = Visibility.Visible;
                        PlaySound("GameStart");
                    });

                    timer.Start();
                }
                else
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "请垂直放于额头";
                    });
                    return;
                }
            }

            if (e.SensorReading.Acceleration.Z > 0.8)
            {
                isOK = true;
            }

            if (e.SensorReading.Acceleration.Z < -1)
            {
                isSkip = true;
            }

            if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.2 && (isOK || isSkip))
            {
                new Thread(() => Dispatcher.BeginInvoke(() =>
                {
                    if (DateTime.Now - lastDateTime <= new TimeSpan(0, 0, 0, 1)) return;
                    lastDateTime = DateTime.Now;

                    if (isOK)
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            totalCount++;
                        });
                    }

                    if (runningPageList.Count == 1)
                    {
                        M_StringTextBlock.Text = "猜的太快了!";
                    };

                    runningPageList.Remove(M_StringTextBlock.Text);
                    M_StringTextBlock.Text = runningPageList[random.Next(0, runningPageList.Count - 1)];

                    if (isOK) PlaySound("Correct");
                    if (isSkip) PlaySound("Pass");

                    isOK = false;
                    isSkip = false;
                })).Start();
            }
        }

        private void ReturnButton_OnClick(object sender, RoutedEventArgs e)
        {
            totalCount = 0;

            M_CountPanel.Visibility = Visibility.Collapsed;
            M_FrontPagePanel.Visibility = Visibility.Visible;
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            SystemTray.IsVisible = false;
        }
    }
}