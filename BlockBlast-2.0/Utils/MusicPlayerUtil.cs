using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public class MusicPlayerUtil
{
    private IWavePlayer? _outputDevice;
    private WaveStream? _audioFile;
    public void Play(string path, bool loop = false)
    {
        Stop(); 
        try
        {
            _outputDevice = new WaveOutEvent();
            var reader = new AudioFileReader(path);

            _audioFile = loop ? new LoopStream(reader) : reader;
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при воспроизведении музыки: {ex.Message}");
        }
    }

    public void Stop()
    {
        _outputDevice?.Stop();
        _audioFile?.Dispose();
        _outputDevice?.Dispose();
        _audioFile = null;
        _outputDevice = null;
    }
}