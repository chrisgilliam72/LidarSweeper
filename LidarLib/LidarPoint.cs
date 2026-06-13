using System;
using System.Collections.Generic;
using System.Text;

namespace LidarLib;

public record LidarPoint(int Angle, int Distance, int Strengh, DateTime Time);