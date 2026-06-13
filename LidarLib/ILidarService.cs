using LidarLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace LidarSweeper;

public interface ILidarService : IDisposable
{
    LidarPoint? LastPoint { get; set; }

    void Init();
    Task StartAsync(CancellationToken cancellationToken = default);
}
