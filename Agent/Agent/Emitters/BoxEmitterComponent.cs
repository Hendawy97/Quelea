﻿using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Agent.Properties.Resources;

namespace Agent
{
  public class BoxEmitterComponent : AbstractEmitterComponent
  {
    private Box box;
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public BoxEmitterComponent()
      : base(RS.boxEmitName, RS.boxEmitNickName,
          RS.boxEmitDescription, RS.icon_boxEmitter, RS.boxEmitGUID)
    {
      box = new Box();
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_InputParamManager pManager, int particlesName)
    {
      base.RegisterInputParams(pManager, pManager.AddGenericParameter("Particles","P", RS.agentDescription, 
        GH_ParamAccess.list));
      pManager.AddBoxParameter(RS.boxName, RS.boxNickName, RS.boxForEmitterDescription, GH_ParamAccess.item);
    }

    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!base.GetInputs(da)) return false;
      if (!da.GetData(nextInputIndex++, ref box)) return false;
      return true;
    }

    protected override void SetOutputs(IGH_DataAccess da)
    {
      AbstractEmitterType emitter = new BoxEmitterType(box, continuousFlow, creationRate, numAgents);
      da.SetData(nextOutputIndex++, emitter);
    }
  }
}
