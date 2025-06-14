using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public abstract class SoundPlayerUtil
{
    public static void Play(string path)
    {
        Task.Run(() =>
        {
            try
            {
                using var audioFile = new AudioFileReader(path);
                using var outputDevice = new WaveOutEvent();

                outputDevice.Init(audioFile);
                outputDevice.Play();
                
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                Console.WriteLine(Resources.Sound_Player_Error, ex.Message);
            }
        });
    }
}