﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Quelea.Properties.Resources;

namespace Quelea
{
  public class AxisAlignedBoxEnvironmentComponent : AbstractEnvironmentComponent
  {
    private Box box;
    private bool wrap;
    /// <summary>
    /// Initializes a new instance of the AbstractEnvironmentComponent class.
    /// </summary>
    public AxisAlignedBoxEnvironmentComponent()
      : base(RS.AABoxEnvName, RS.AABoxEnvNickName,
          RS.AABoxEnvDescription, RS.icon_AABoxEnvironment, RS.AABoxEnvGUID)
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      pManager.AddBoxParameter(RS.boxName, RS.boxNickname, RS.AABoxDescription, GH_ParamAccess.item);
      pManager.AddBooleanParameter(RS.wrapName, RS.wrapNickname, RS.wrapDescription, GH_ParamAccess.item, RS.wrapDefault);
    }

    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!da.GetData(nextInputIndex++, ref box)) return false;
      if (!da.GetData(nextInputIndex++, ref wrap)) return false;

      // We should now validate the data and warn the user if invalid data is supplied.
      if (!(box.Plane.XAxis.Equals(Plane.WorldXY.XAxis) && box.Plane.YAxis.Equals(Plane.WorldXY.YAxis)))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, RS.AABoxError);
        return false;
      }
      return true;
    }

    protected override void SetOutputs(IGH_DataAccess da)
    {
      AbstractEnvironmentType environment = new AxisAlignedBoxEnvironmentType(box, wrap);
      da.SetData(nextOutputIndex++, environment);
    }
  }
}