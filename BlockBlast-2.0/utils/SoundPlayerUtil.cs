using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public class SoundPlayerUtil
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
                Console.WriteLine($"Ошибка при воспроизведении звука: {ex.Message}");
            }
        });
    }
}