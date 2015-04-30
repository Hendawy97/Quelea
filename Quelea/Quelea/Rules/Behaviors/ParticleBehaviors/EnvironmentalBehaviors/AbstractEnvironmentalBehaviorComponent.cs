﻿using System.Drawing;
using Grasshopper.Kernel;
using RS = Quelea.Properties.Resources;

namespace Quelea
{
  public abstract class AbstractEnvironmentalBehaviorComponent : AbstractParticleBehaviorComponent
  {
    protected AbstractEnvironmentType environment;
    /// <summary>
    /// Initializes a new instance of the EatBehaviorComponent class.
    /// </summary>
    protected AbstractEnvironmentalBehaviorComponent(string name, string nickname, string description,
                                                     Bitmap icon, string componentGuid)
      : base(name, nickname, description, icon, componentGuid)
    {
      environment = new AxisAlignedBoxEnvironmentType();
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      base.RegisterInputParams(pManager);
      pManager.AddGenericParameter(RS.environmentName, RS.environmentNickname, RS.bounceContainBehaviorEnvironmentDescription, GH_ParamAccess.item);
    }

    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!base.GetInputs(da)) return false;
      if (!da.GetData(nextInputIndex++, ref environment)) return false;
      return true;
    }
  }
}