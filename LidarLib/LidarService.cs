using System;
using System.Collections.Generic;
using System.Text;

namespace LidarSweeper;

using LidarLib;
using Microsoft.Extensions.Logging;
using System.IO.Ports;
using Ultraborg;
using Ultraborg.Library.Servo;
using UnitsNet;

public class LidarService : ILidarService
{
    private readonly ILogger _logger;
    private readonly IUltraborgAPI _ultraborgAPI;
    private readonly SerialPort _port;

    private Task? _readerTask;
    private CancellationTokenSource? _cts;

    private int _currentPosition = 0;
    private UltraborgServo? _ultraborgServo;
    public LidarPoint? LastPoint { get; set; }

    public LidarService(ILogger<LidarService> logger, IUltraborgAPI ultraborgAPI)
    {
        _logger = logger;
        _ultraborgAPI= ultraborgAPI;
        _port = new SerialPort("/dev/serial0", 115200)
        {
            ReadTimeout = 1000
        };
    }

    public void Init()
    {
        _ultraborgAPI.Setup();
        var ultraborg = _ultraborgAPI?.Ultraborg;
        _ultraborgServo = new UltraborgServo(4, 0, _logger);
        _ultraborgServo.Init(ultraborg!);
        _currentPosition = _ultraborgServo.GetCurrentPosition();
        _logger.LogInformation($"Current Position: {_currentPosition}");
        int servoMax = _ultraborgServo.ServoMax;
        int servoMin = _ultraborgServo.ServoMin;
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

        double servoPos = -1;
        int angle= 0;
        bool rightLeft = true;
        _ultraborgServo!.SetServoPosition(servoPos);
        while (!token.IsCancellationRequested)
        {
            try
            {
                var lastPoint=  GetLidarPoint();
                if (rightLeft)
                {
                    angle += 10;
                    servoPos += 0.1;
                    if (servoPos >= 1)
                        rightLeft = false;
                }
                else
                {
                    angle -= 10;
                    servoPos -= 0.1;
                    if (servoPos<=-1)
                        rightLeft = true;
                }           


                if (lastPoint is not null)
                {
                    lastPoint = lastPoint with { Angle = angle };
                    _logger.LogInformation($"Angle= {lastPoint.Angle} Distance={lastPoint.Distance} cm  Strength={lastPoint.Strengh}" );
                }

                Thread.Sleep(100);
                _ultraborgServo!.SetServoPosition(servoPos);
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

    private LidarPoint? GetLidarPoint()
    {
        _port.DiscardInBuffer();
        int b;

        do
        {
            b = _port.ReadByte();
        }
        while (b != 0x59);

        if (_port.ReadByte() != 0x59)
            return null;

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

        if (checksum == frame[6])
        {
            var Distance = frame[0] | (frame[1] << 8);
            var Strength = frame[2] | (frame[3] << 8);

            int temperatureRaw = frame[4] | (frame[5] << 8);
            var Temperature = temperatureRaw / 8.0 - 256.0;

            return new LidarPoint(0, Distance, Strength, DateTime.Now);

        }
        else
        {
            _logger.LogWarning("Bad checksum");
            return null;
        }
          

    }
}