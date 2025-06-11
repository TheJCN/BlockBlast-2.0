using NAudio.Wave;

namespace BlockBlast_2._0.utils;

public class LoopStream(WaveStream sourceStream) : WaveStream
{
    public override WaveFormat WaveFormat => sourceStream.WaveFormat;
    public override long Length => long.MaxValue; 
    public override long Position
    {
        get => sourceStream.Position;
        set => sourceStream.Position = value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = sourceStream.Read(buffer, offset, count);
        if (read != 0) return read;
        sourceStream.Position = 0;
        read = sourceStream.Read(buffer, offset, count);
        return read;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            sourceStream.Dispose();
        base.Dispose(disposing);
    }
}