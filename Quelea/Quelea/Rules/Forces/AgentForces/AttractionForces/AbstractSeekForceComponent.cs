﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Quelea.Properties.Resources;

namespace Quelea
{
  public abstract class AbstractSeekForceComponent : AbstractAgentForceComponent
  {
    protected Point3d targetPt;
    /// <summary>
    /// Initializes a new instance of the AbstractSeekForceComponent class.
    /// </summary>
    protected AbstractSeekForceComponent(string name, string nickname, string description,
                                         Bitmap icon, String componentGuid)
      : base(name, nickname, description, icon, componentGuid)
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      base.RegisterInputParams(pManager);
      pManager.AddPointParameter("Target Point", "P", "Point to be attracted to.", GH_ParamAccess.item, Point3d.Origin);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!base.GetInputs(da)) return false;
      if (!da.GetData(nextInputIndex++, ref targetPt)) return false;
      targetPt = agent.Environment.MapTo2D(targetPt);
      return true;
    }
  }
}