using System.Windows.Controls;
using System.Windows.Navigation;
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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CrazyGuessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        private const int ARoundSeconds = 72;
        Accelerometer _ac;

        List<string> runningPageList = new List<string>();

        Random random = new Random();

        private bool isCheckingDirectionRight;
        private bool isRunning;

        private DispatcherTimer timer;
        private int timerCount = ARoundSeconds;

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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }
        }

        private void PlaySound(string soundName)
        {
            // Get the sound from the XAP file
            var info = App.GetResourceStream(
              new Uri(String.Format("Resources/{0}.wav", soundName), UriKind.Relative));
            // Load the SoundEffect
            var effect = SoundEffect.FromStream(info.Stream);
            // Tell the XNA Libraries to continue to run
            FrameworkDispatcher.Update();

            // Play the Sound
            effect.Play();
        }

        #region Start A Round

        private void Page_OnClick(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            var button = sender as Button;

            if (button.Tag.ToString() == "R")
            {
                runningPageList = GetPageList(@"Words/Page" + random.Next(1, 4).ToString() + ".txt");
            }
            else
            {
                runningPageList = GetPageList(@"Words/Page" + button.Tag.ToString() + ".txt");
            }

            Prepare();
        }

        private void Prepare()
        {
            M_FrontPagePanel.Visibility = Visibility.Collapsed;
            M_NotificationTextBlock.Visibility = Visibility.Visible;

            isCheckingDirectionRight = true;
            isRunning = true;
            _ac.Start();
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

        #endregion

        #region Private Methods

        private void timer_Tick(object sender, EventArgs e)
        {
            if (!isRunning) return;
            if (isCheckingDirectionRight) return;
            // minus time count
            if (timerCount > 0)
            {
                timerCount--;

                M_TimesTextBlock.Text = String.Format("{0} : {1:00}", timerCount / 60, timerCount % 60);
            }
            // if times up, end the round
            if (timerCount == 0)
            {
                isRunning = false;
                isCheckingDirectionRight = false;
                timer.Stop();
                _ac.Stop();
                timerCount = ARoundSeconds;
                Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/CrazyGuessing;component/ViewResultPage.xaml", UriKind.Relative)));
                return;
            }
            // last 10 seconds, play tip sound
            if (timerCount <= 10 && timerCount > 2)
            {
                PlaySound("cardAppear");
                return;
            }

            if (timerCount == 2)
            {
                PlaySound("GameEnd");
                return;
            }

        }

        #endregion

        private int trackRecordPointsCount;
        private bool acceptBigTurn = true;
        private bool waitToCalm = false;
        private double lastZValue;

        void _ac_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (!isRunning) return;
            if (isCheckingDirectionRight)
            {
                CheckIsDirectionRightToStart(e);
                return;
            }
            if (acceptBigTurn)
            {
                HandleBigTurn(e);
                return;
            }
            if (waitToCalm)
                HandleToCalm(e);
        }

        private void HandleToCalm(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) >= 0.4)
            {
                trackRecordPointsCount = 0;
                return;
            }
            if (trackRecordPointsCount == 0)
            {
                lastZValue = e.SensorReading.Acceleration.Z;
                trackRecordPointsCount++;
            }
            else
            {
                if (Math.Abs(e.SensorReading.Acceleration.Z - lastZValue) > 0.2)
                {
                    trackRecordPointsCount = 0;
                    return;
                }
                trackRecordPointsCount++;
                if (trackRecordPointsCount >= 5)
                {
                    acceptBigTurn = true;
                    waitToCalm = false;
                }
            }
        }

        private void HandleBigTurn(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.9) return;
            acceptBigTurn = false;
            waitToCalm = true;
            trackRecordPointsCount = 0;
            var isRight = e.SensorReading.Acceleration.Z > 0;
            PlaySound(isRight ? "Correct" : "Pass");
            new Thread(() => Dispatcher.BeginInvoke(() =>
            {
                if (runningPageList.Count == 1)
                {
                    M_StringTextBlock.Text = "猜的太快了!";
                };
                if (runningPageList.Contains(M_StringTextBlock.Text))
                {
                    ViewResultPage.ResultItems.Add(new ViewResultItem
                    {
                        Content = M_StringTextBlock.Text,
                        IsRight = isRight
                    });
                    runningPageList.Remove(M_StringTextBlock.Text);
                }
                SetWordContent(runningPageList[random.Next(0, runningPageList.Count - 1)]);

            })).Start();
        }

        private void SetWordContent(string p)
        {
            if (p.Length <= 4)
                M_StringTextBlock.FontSize = 200;
            else if (p.Length == 5)
                M_StringTextBlock.FontSize = 160;
            else
                M_StringTextBlock.FontSize = 130;
            M_StringTextBlock.Text = p;
        }

        private void CheckIsDirectionRightToStart(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) >= 0.3)
            {

                Dispatcher.BeginInvoke(() =>
                {
                    M_NotificationTextBlock.Text = "请垂直放于额头";
                    M_NotificationTextBlock.FontSize = 90;
                });
                return;
            }
            isCheckingDirectionRight = false;
            isRunning = false;
            new Thread(
                () =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "3";
                        M_NotificationTextBlock.FontSize = 200;
                    });
                    PlaySound("BeginCountDown");
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "2";
                    });
                    PlaySound("BeginCountDown");
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "1";
                    });
                    PlaySound("BeginCountDown");
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Text = "Go!";
                    });
                    PlaySound("Begin");
                    Thread.Sleep(1000);
                    PlaySound("GameStart");
                    isRunning = true;
                    trackRecordPointsCount = 0;
                    acceptBigTurn = true;
                    waitToCalm = false;
                    ViewResultPage.ResultItems.Clear();
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_NotificationTextBlock.Visibility = Visibility.Collapsed;
                        SetWordContent(runningPageList[random.Next(0, runningPageList.Count - 1)]);
                        M_TimesTextBlock.Text = String.Format("{0} : {1:00}", timerCount / 60, timerCount % 60);

                        M_GamingPanel.Visibility = Visibility.Visible;
                        timer.Start();
                    });
                }).Start();
        }

        private void M_PlayRuleButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CrazyGuessing;component/PlayRulePage.xaml", UriKind.Relative));
        }

        private void M_InfoButton_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CrazyGuessing;component/ViewInfoPage.xaml", UriKind.Relative));
        }
    }
}