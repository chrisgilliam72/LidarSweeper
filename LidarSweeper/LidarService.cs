using System;
using System.Collections.Generic;
using System.Text;

namespace LidarSweeper;

using Microsoft.Extensions.Logging;
using System.IO.Ports;

public class LidarService : ILidarService
{
    private readonly ILogger<LidarService> _logger;
    private readonly SerialPort _port;

    private Task? _readerTask;
    private CancellationTokenSource? _cts;

    public int Distance { get; private set; }
    public int Strength { get; private set; }
    public double Temperature { get; private set; }
    public DateTime LastUpdate { get; private set; }

    public LidarService(ILogger<LidarService> logger)
    {
        _logger = logger;

        _port = new SerialPort("/dev/serial0", 115200)
        {
            ReadTimeout = 1000
        };
    }

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_readerTask != null)
            return Task.CompletedTask;

        _port.Open();

        _logger.LogInformation("Opened lidar on {Port}", _port.PortName);

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        _readerTask = Task.Run(() => ReadLoop(_cts.Token));

        return Task.CompletedTask;
    }

    private void ReadLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                int b;

                do
                {
                    b = _port.ReadByte();
                }
                while (b != 0x59);

                if (_port.ReadByte() != 0x59)
                    continue;

                byte[] frame = new byte[7];

                int offset = 0;

                while (offset < 7)
                {
                    offset += _port.Read(frame, offset, 7 - offset);
                }

                int checksum =
                    0x59 +
                    0x59 +
                    frame[0] +
                    frame[1] +
                    frame[2] +
                    frame[3] +
                    frame[4] +
                    frame[5];

                checksum &= 0xFF;

                if (checksum != frame[6])
                {
                    _logger.LogWarning("Bad checksum");
                    continue;
                }

                Distance = frame[0] | (frame[1] << 8);
                Strength = frame[2] | (frame[3] << 8);

                int temperatureRaw = frame[4] | (frame[5] << 8);
                Temperature = temperatureRaw / 8.0 - 256.0;

                LastUpdate = DateTime.UtcNow;
            }
            catch (TimeoutException)
            {
                // Ignore and keep reading
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading lidar");
            }
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();

        try
        {
            _readerTask?.Wait(1000);
        }
        catch
        {
        }

        if (_port.IsOpen)
            _port.Close();

        _port.Dispose();
    }
}