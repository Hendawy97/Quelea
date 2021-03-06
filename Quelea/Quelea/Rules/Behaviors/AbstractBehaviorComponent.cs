﻿using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Quelea.Properties.Resources;

namespace Quelea
{
  public abstract class AbstractBehaviorComponent : AbstractRuleComponent
  {
    /// <summary>
    /// Initializes a new instance of the AbstractAgentRuleComponent class.
    /// </summary>
    protected AbstractBehaviorComponent(string name, string nickname, string description, 
                                     string subcategory, Bitmap icon, string componentGuid)
      : base(name, nickname, description, subcategory, icon, componentGuid)
    {
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      // Use the pManager object to register your output parameters.
      // Output parameters do not have default values, but they too must have the correct access type.
      pManager.AddBooleanParameter(RS.behaviorAppliedName, RS.behaviorNickName, RS.behaviorAppliedDescription, GH_ParamAccess.item);

      // Sometimes you want to hide a specific parameter from the Rhino preview.
      // You can use the HideParameter() method as a quick way:
      //pManager.HideParameter(1);
    }
    protected abstract bool Run();

    protected override void SetOutputs(IGH_DataAccess da)
    {
      if (!apply)
      {
        da.SetData(nextOutputIndex++, false);
        return;
      }

      bool behaviorApplied = Run();
      da.SetData(nextOutputIndex++, behaviorApplied);
    }

    public override GH_Exposure Exposure
    {
      get { return GH_Exposure.secondary; }
    }
  }
}