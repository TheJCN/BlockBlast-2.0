using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public class MusicPlayerUtil
{
    private IWavePlayer? _outputDevice;
    private WaveStream? _audioFile;

    public void Play(Stream stream, bool loop = false)
    {
        Stop();

        try
        {
            var reader = new WaveFileReader(stream); // WAV only
            _audioFile = loop ? new LoopStream(reader) : reader;

            _outputDevice = new WaveOutEvent();
            _outputDevice.Init(_audioFile);
            _outputDevice.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Music player error: " + ex.Message);
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