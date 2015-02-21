﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Agent.Properties.Resources;

namespace Agent
{
  public abstract class AbstractAttractionForceComponent : AbstractForceComponent
  {
    protected Point3d targetPt;
    protected double radius;
    /// <summary>
    /// Initializes a new instance of the ViewForceComponent class.
    /// </summary>
    protected AbstractAttractionForceComponent(string name, string nickname, string description,
                              string category, string subCategory, Bitmap icon, String componentGuid)
      : base(name, nickname, description, category, subCategory, icon, componentGuid)
    {
      targetPt = new Point3d();
      radius = RS.attractionRadiusDefault;
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      base.RegisterInputParams(pManager);
      pManager.AddGenericParameter("Target Point", "P", "Point to be attracted to.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Attraction Radius", "R", "The radius within which Agents will be affected by the attractor. If negative, the radius will be assumed to be infinite.",
        GH_ParamAccess.item, RS.attractionRadiusDefault);
    }
    
    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!base.GetInputs(da)) return false;

      if (!da.GetData(3, ref targetPt)) return false;
      if (!da.GetData(3, ref radius)) return false;

      
      return true;
    }
  }
}