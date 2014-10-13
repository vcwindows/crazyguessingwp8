using CrazyGuessing.StatusMachine;
using Microsoft.Devices.Sensors;
using System;
using System.Threading;

namespace CrazyGuessing.UI
{
    public partial class PreparePage
    {
        Accelerometer _ac;

        public PreparePage()
        {
            InitializeComponent();

            new Thread(() =>
            {
                Thread.Sleep(1000);
                _ac = new Accelerometer();
                _ac.CurrentValueChanged += _ac_CurrentValueChanged;
                _ac.Start();
            }).Start();
        }

        private void _ac_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            if (Math.Abs(e.SensorReading.Acceleration.Z) <= 0.3)
            {
                _ac.Stop();
                _ac.Dispose();

                Dispatcher.BeginInvoke(() => StatusController.SetStatus(StatusEnum.CountBackwards));
            }
        }
    }
}
