using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public static class SoundPlayerUtil
{
    public static void Play(Stream stream)
    {
        Task.Run(() =>
        {
            try
            {
                using var reader = new WaveFileReader(stream); // WAV only
                using var outputDevice = new WaveOutEvent();
                outputDevice.Init(reader);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                    Thread.Sleep(50);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Sound player error: " + ex.Message);
            }
        });
    }
}