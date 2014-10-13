using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace CrazyGuessing.Functions
{
    public static class SoundController
    {
        public static void PlaySound(SoundEnum soundEnum)
        {
            var sound = soundEnum.ToString();
            // Get the sound from the XAP file
            var info = App.GetResourceStream(
              new Uri(String.Format("Resources/Music/{0}.wav", sound), UriKind.Relative));
            // Load the SoundEffect
            var effect = SoundEffect.FromStream(info.Stream);
            // Tell the XNA Libraries to continue to run
            FrameworkDispatcher.Update();

            // Play the Sound
            effect.Play();
        }
    }

    public enum SoundEnum
    {
        Begin = 1,
        BeginCountDown = 2,
        ButtonClick = 3,
        cardAppear = 4,
        Correct = 5,
        CountDown = 6,
        GameEnd = 7,
        GameStart = 8,
        GameStart0 = 9,
        Next = 10,
        Pass = 11,
        woosh = 12,
    }
}
