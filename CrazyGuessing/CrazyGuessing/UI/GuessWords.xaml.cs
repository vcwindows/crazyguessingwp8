using System.Collections.Generic;
using System.Threading;
using CrazyGuessing.Functions;
using CrazyGuessing.Models;
using CrazyGuessing.StatusMachine;
using System;
using System.Windows.Threading;
using Microsoft.Devices.Sensors;

namespace CrazyGuessing.UI
{
    public partial class GuessWords
    {
        private const int ARoundSeconds = 10;
        private int timerCount = ARoundSeconds;

        private int trackRecordPointsCount;
        private bool acceptBigTurn = true;
        private bool waitToCalm = false;
        private double lastZValue;

        readonly Random random = new Random();

        private List<string> GuessingList = new List<string>();
        private List<ResultData> ResultList = new List<ResultData>();

        private readonly DispatcherTimer timer;
        private readonly Accelerometer _ac;

        public GuessWords(List<string> guessingList)
        {
            InitializeComponent();
            GuessingList = guessingList;

            SetWordContent(GuessingList[random.Next(0, GuessingList.Count - 1)]);
            M_TimesTextBlock.Text = String.Format("{0} : {1:00}", timerCount / 60, timerCount % 60);

            timer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            timer.Tick += timer_Tick;
            timer.Start();

            _ac = new Accelerometer();
            _ac.CurrentValueChanged += _ac_CurrentValueChanged;
            _ac.Start();
        }

        private void _ac_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (acceptBigTurn)
            {
                HandleBigTurn(e);
                return;
            }
            if (waitToCalm)
                HandleToCalm(e);
        }

        private void HandleBigTurn(SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) < 0.9) return;
            acceptBigTurn = false;
            waitToCalm = true;
            trackRecordPointsCount = 0;
            var isRight = e.SensorReading.Acceleration.Z > 0;
            SoundController.PlaySound(isRight ? SoundEnum.Correct : SoundEnum.Pass);
            new Thread(() => Dispatcher.BeginInvoke(() => Dispatcher.BeginInvoke(() =>
            {
                if (GuessingList.Count == 1)
                {
                    M_StringTextBlock.Text = "猜的太快了!";
                };
                if (GuessingList.Contains(M_StringTextBlock.Text))
                {
                    ResultList.Add(new ResultData() { Content = M_StringTextBlock.Text, IsRight = isRight });
                    GuessingList.Remove(M_StringTextBlock.Text);
                }

                SetWordContent(GuessingList[random.Next(0, GuessingList.Count - 1)]);
            }))).Start();
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

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timerCount > 0)
            {
                timerCount--;

                M_TimesTextBlock.Text = String.Format("{0} : {1:00}", timerCount / 60, timerCount % 60);
            }
            else
            {
                _ac.Stop();
                timer.Stop();
                StatusController.SetStatus(StatusEnum.ResultPage, ResultList);
            }

            // last 10 seconds, play tip sound
            if (timerCount <= 10 && timerCount > 2)
            {
                SoundController.PlaySound(SoundEnum.cardAppear);
                return;
            }

            if (timerCount == 2)
            {
                SoundController.PlaySound(SoundEnum.GameEnd);
            }
        }
    }
}
