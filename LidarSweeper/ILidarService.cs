using System;
using System.Collections.Generic;
using System.Text;

namespace LidarSweeper;

public interface ILidarService : IDisposable
{
    int Distance { get; }
    int Strength { get; }
    double Temperature { get; }
    DateTime LastUpdate { get; }

    Task StartAsync(CancellationToken cancellationToken = default);
}
