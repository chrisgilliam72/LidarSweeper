using LidarLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LidarSweeper;

public interface ILidarService : IDisposable
{
    LidarPoint? LastPoint { get; set; }

    Task StartAsync(CancellationToken cancellationToken = default);
}
