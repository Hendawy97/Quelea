﻿using System;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Agent.Properties.Resources;

namespace Agent
{
  public abstract class AbstractForceComponent : AbstractActionComponent
  {
    private double weightMultiplier;

    /// <summary>
    /// Initializes a new instance of the ViewForceComponent class.
    /// </summary>
    protected AbstractForceComponent(string name, string nickname, string description,
                              string subcategory, Bitmap icon, String componentGuid)
      : base(name, nickname, description, subcategory, icon, componentGuid)
    {
      weightMultiplier = RS.weightMultiplierDefault;
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      base.RegisterInputParams(pManager);
      // Use the pManager object to register your input parameters.
      // You can often supply default values when creating parameters.
      // All parameters must have the correct access type. If you want 
      // to import lists or trees of values, modify the ParamAccess flag.
      pManager.AddNumberParameter(RS.weightMultiplierName, RS.weightMultiplierNickName, RS.weightMultiplierDescription,
        GH_ParamAccess.item, RS.weightMultiplierDefault);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      // Use the pManager object to register your output parameters.
      // Output parameters do not have default values, but they too must have the correct access type.
      pManager.AddGenericParameter("Force", RS.forceNickName,
                                   "The resulting force vector for debugging purposes.", GH_ParamAccess.item);

      // Sometimes you want to hide a specific parameter from the Rhino preview.
      // You can use the HideParameter() method as a quick way:
      //pManager.HideParameter(1);
    }

    protected override bool GetInputs(IGH_DataAccess da)
    {
      if(!base.GetInputs(da)) return false;
      // First, we need to retrieve all data from the input parameters.

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!da.GetData(nextInputIndex++, ref weightMultiplier)) return false;

      return true;
    }

    protected override void SetOutputs(IGH_DataAccess da)
    {
      if (!apply)
      {
        da.SetData(nextOutputIndex++, Vector3d.Zero);
        return;
      }

      Vector3d force = Run();
      da.SetData(nextOutputIndex++, force);
    }

    protected Vector3d Run()
    {
      Vector3d force = CalcForce();
      return ApplyForce(force);
    }

    private Vector3d ApplyForce(Vector3d force)
    {
      Vector3d weightedForce = Vector3d.Multiply(force, weightMultiplier);
      agent.ApplyForce(weightedForce);
      return weightedForce;
    }

    protected abstract Vector3d CalcForce();
  }
}