﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent
{
  public class EngineComponent : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the Engine class.
    /// </summary>
    public EngineComponent()
      : base("Engine", "Engine",
          "Engine that runs the simulation.",
          "Agent", "Agent")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddBooleanParameter("Reset", "R", "Reset the scene?", GH_ParamAccess.item, true);
      pManager.AddBooleanParameter("Live Update", "L", "Update the parameters each timestep? (Slower)", GH_ParamAccess.item, false);
      pManager.AddGenericParameter("Agent Systems", "S", "Agent Systems in scene.", GH_ParamAccess.list);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddPointParameter("Agents", "A", "Agents", GH_ParamAccess.list);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      Boolean reset = true;
      bool liveUpdate = false;
      List<EmitterType> emitters = new List<EmitterType>();
      List<AgentSystemType> systems = new List<AgentSystemType>();

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!DA.GetData(0, ref reset)) return;
      if (!DA.GetData(1, ref liveUpdate)) return;
      if (!DA.GetDataList(2, systems)) return;

      // We should now validate the data and warn the user if invalid data is supplied.

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      List<Point3d> agents = run(reset, liveUpdate, systems);
      //List<Point3d> agents = new List<Point3d>();

      // Finally assign the spiral to the output parameter.
      DA.SetDataList(0, agents);
    }

    AgentSystemType agentSystem;
    private List<Point3d> run(Boolean reset, bool liveUpdate, List<AgentSystemType> systems)
    {
      List<Point3d> pts = new List<Point3d>();

      if (reset)
      {
        foreach (AgentSystemType system in systems)
        {
          agentSystem = new AgentSystemType(system);
          agentSystem.Agents.Clear();
          setup(system.Emitters, system.AgentsSettings);
          foreach (EmitterType emitter in system.Emitters)
          {
            if (!emitter.ContinuousFlow)
            {
              for (int i = 0; i < emitter.NumAgents; i++)
              {
                agentSystem.addAgent(emitter);
              }
            }
          }
        }

      }
      else
      {
        foreach (AgentSystemType system in systems)
        {
          if (liveUpdate)
          {
            setup(system.Emitters, system.AgentsSettings);
          }
          agentSystem.run();
          foreach (AgentType p in agentSystem.Agents)
          {
            Point3d pt = new Point3d(p.Location);
            pts.Add(pt);
          }
        }
      }

      return pts;
    }

    private void setup(EmitterType[] emitters, AgentType[] agentSettings)
    {
      agentSystem.Emitters = emitters;
      agentSystem.AgentsSettings = agentSettings;

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
        return null;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{3c316736-b260-4c09-8f5a-6f6b40709cbe}"); }
    }
  }
}