using CrazyGuessing.Functions;
using CrazyGuessing.StatusMachine;
using System.Threading;
using System.Windows.Controls;

namespace CrazyGuessing.UI
{
    public partial class CountBackwards
    {
        public CountBackwards()
        {
            InitializeComponent();

            new Thread(
                () =>
                {
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_CountTestBlock.Text = "3";
                    });
                    SoundController.PlaySound(SoundEnum.BeginCountDown);
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_CountTestBlock.Text = "2";
                    });
                    SoundController.PlaySound(SoundEnum.BeginCountDown);
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_CountTestBlock.Text = "1";
                    });
                    SoundController.PlaySound(SoundEnum.BeginCountDown);
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() =>
                    {
                        M_CountTestBlock.Text = "Go!";
                    });
                    SoundController.PlaySound(SoundEnum.Begin);
                    Thread.Sleep(1000);
                    Dispatcher.BeginInvoke(() => StatusController.SetStatus(StatusEnum.GuessWords));
                }).Start();
        }
    }
}
