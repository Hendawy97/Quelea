﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent
{
  public class AgentSystemComponentList : GH_Component
  {
    AgentSystemTypeList system = null;
    /// <summary>
    /// Initializes a new instance of the AgentSystemComponentList class.
    /// </summary>
    public AgentSystemComponentList()
      : base("AgentSystemList", "SystemAsList",
          "Represents a self-contained Agent System of Agents and Emitters.",
          "Agent", "Agent")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams
      (GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddGenericParameter("Agents", "A", "Agents", 
                                    GH_ParamAccess.list);
      pManager.AddGenericParameter("Emitters", "E", "Emitters",
                                    GH_ParamAccess.list);
      pManager.AddGenericParameter("Environment", "En", "Environment",
                                   GH_ParamAccess.item);
      pManager.AddGenericParameter("Forces", "F", "Forces",
                                   GH_ParamAccess.list);
      pManager.AddGenericParameter("Behaviors", "B", "Behaviors",
                                   GH_ParamAccess.list);
      pManager[2].Optional = true;
      pManager[3].Optional = true;
      pManager[4].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams
      (GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Agent System", "S", "Agent System", 
                                   GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and 
    /// store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      List<AgentType> agents = new List<AgentType>();
      List<EmitterType> emitters = new List<EmitterType>();
      EnvironmentType environment = null;
      List<ForceType> forces = new List<ForceType>();
      List<BehaviorType> behaviors = new List<BehaviorType>();

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this
      // method.
      if (!DA.GetDataList(0, agents)) return;
      if (!DA.GetDataList(1, emitters)) return;
      DA.GetData(2, ref environment);
      DA.GetDataList(3, forces);
      DA.GetDataList(4, behaviors);
      
      //if (!DA.GetDataList(2, forces)) return;

      // We should now validate the data and warn the user if invalid data is 
      // supplied.
      if (agents.Count <= 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be at " + 
          "least 1 Agent.");
        return;
      }
      if (emitters.Count <= 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be at " +
          "least 1 Emitter.");
        return;
      }

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      if (system == null)
      {
        system = new AgentSystemTypeList(agents.ToArray(), emitters.ToArray(), environment, forces.ToArray(), behaviors.ToArray());
      }
      else
      {
        system = new AgentSystemTypeList(agents.ToArray(), emitters.ToArray(), environment, forces.ToArray(), behaviors.ToArray(), system);
      }
      //AgentSystemType system = new AgentSystemType(agents.ToArray(), emitters.ToArray(), environment, forces.ToArray(), behaviors.ToArray());

      // Finally assign the spiral to the output parameter.
      DA.SetData(0, system);
    }

    /// <summary>
    /// Provides an Icon for the component.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        //You can add image files to your project resources and access them like this:
        // return Resources.IconForThisComponent;
        return Properties.Resources.icon_system;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{01a4e121-b1ed-4882-ab11-a9376b202e79}"); }
    }
  }
}