﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace Quelea
{
  public interface IPosition
  {
    Point3d GetPoint3D();
  }
}
