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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CrazyGuessing
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region Fields

        Accelerometer _ac;

        List<string> runningPageList = new List<string>();

        Random random = new Random();

        private bool isCheckingDirectionRight;
        private bool isRunning;
        private bool isWaitingForFlapToSkip;
        private bool isWaitingForFlapToNext;
        private DateTime lastReachVerticalDirectionTime;

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
            runningPageList = GetPageList("Page" + button.Tag.ToString() + ".txt");

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
                M_TimesTextBlock.Text = (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();
            }
            // if times up, end the round
            if (timerCount == 0)
            {
                isRunning = false;
                isCheckingDirectionRight = false;
                isWaitingForFlapToSkip = false;
                isWaitingForFlapToNext = false;

                Dispatcher.BeginInvoke(() =>
                {
                    M_GamingPanel.Visibility = Visibility.Collapsed;
                    MCountTextBlock.Text = totalCount.ToString();
                    M_CountPanel.Visibility = Visibility.Visible;
                });
                timer.Stop();
                _ac.Stop();
                timerCount = 72;
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

        void _ac_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (!isRunning) return;
            if (isCheckingDirectionRight)
            {
                CheckIsDirectionRightToStart(e);
                return;
            }
            if (!isWaitingForFlapToNext && !isWaitingForFlapToSkip)
            {
                if (e.SensorReading.Acceleration.Z > 0.8)
                {
                    isWaitingForFlapToNext = true;
                    lastReachVerticalDirectionTime = DateTime.Now;
                    return;
                }

                if (e.SensorReading.Acceleration.Z < -1)
                {
                    isWaitingForFlapToSkip = true;
                    lastReachVerticalDirectionTime = DateTime.Now;
                    return;
                }
                return;
            }

            if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.2 && (isWaitingForFlapToNext || isWaitingForFlapToSkip))
            {
                if (DateTime.Now.Subtract(lastReachVerticalDirectionTime).TotalSeconds < 0.3)
                {
                    return;
                }

                if (isWaitingForFlapToNext)
                {
                    totalCount++;
                }
                if (DateTime.Now - lastDateTime <= new TimeSpan(0, 0, 0, 1)) return;
                lastDateTime = DateTime.Now;
                var isRight = isWaitingForFlapToNext;
                isWaitingForFlapToNext = false;
                isWaitingForFlapToSkip = false;
                PlaySound(isRight ? "Correct" : "Pass");

                new Thread(() => Dispatcher.BeginInvoke(() =>
                {
                    if (runningPageList.Count == 1)
                    {
                        M_StringTextBlock.Text = "猜的太快了!";
                    };
                    runningPageList.Remove(M_StringTextBlock.Text);
                    M_StringTextBlock.Text = runningPageList[random.Next(0, runningPageList.Count - 1)];
                })).Start();
            }
        }

        private void CheckIsDirectionRightToStart(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.3)
            {
                isCheckingDirectionRight = false;
                new Thread(
                    () =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                            M_NotificationTextBlock.Text = "3";
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
                        Dispatcher.BeginInvoke(() =>
                        {
                            M_NotificationTextBlock.Visibility = Visibility.Collapsed;
                            M_StringTextBlock.Text =
                                runningPageList[random.Next(0, runningPageList.Count - 1)];
                            M_TimesTextBlock.Text =
                                    (timerCount / 60).ToString() + " : " + (timerCount % 60).ToString();

                            M_GamingPanel.Visibility = Visibility.Visible;
                            timer.Start();
                        });
                    }).Start();

            }
            else
            {
                Dispatcher.BeginInvoke(() =>
                {
                    M_NotificationTextBlock.Text = "请垂直放于额头";
                });
            }
        }

        private void ReturnButton_OnClick(object sender, RoutedEventArgs e)
        {
            totalCount = 0;

            M_CountPanel.Visibility = Visibility.Collapsed;
            M_FrontPagePanel.Visibility = Visibility.Visible;
        }

        private void ViewPlayRule_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/CrazyGuessing;component/PlayRulePage.xaml", UriKind.Relative));
        }
    }
}