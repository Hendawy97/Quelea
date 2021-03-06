﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Grasshopper;
using Grasshopper.Kernel.Data;
using T = Agent.Types;

namespace Agent
{
  public class Engine : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the Engine class.
    /// </summary>
    public Engine()
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
      pManager.AddBooleanParameter("Live Update", "L", "Update the parameters each timestep? (Slower)", GH_ParamAccess.item, true);
      pManager.AddGenericParameter("Systems", "S", "Systems in scene.", GH_ParamAccess.list);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Agents", "A", "Agents", GH_ParamAccess.tree);
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
      bool liveUpdate = true;
      List<T.AgentSystem> systems = new List<T.AgentSystem>();

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!DA.GetData(0, ref reset)) return;
      if (!DA.GetData(1, ref liveUpdate)) return;
      if (!DA.GetDataList(2, systems)) return;

      // We should now validate the data and warn the user if invalid data is supplied.

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      DataTree<AgentType> agents = run(reset, liveUpdate, systems);
      //List<Point3d> agents = new List<Point3d>();

      // Finally assign the spiral to the output parameter.
      DA.SetDataTree(0, agents);
    }
    List<T.AgentSystem> agentSystems = new List<T.AgentSystem>();
    List<Point3d> pts = new List<Point3d>();
    private DataTree<AgentType> run(Boolean reset, bool liveUpdate, List<T.AgentSystem> systems)
    {
      int index = 0;
      pts.Clear();
      if (reset)
      {
        agentSystems.Clear();
        foreach (T.AgentSystem system in systems)
        {
          agentSystems.Add(new T.AgentSystem(system));
          foreach (EmitterType emitter in system.Emitters)
          {
            if (!emitter.ContinuousFlow)
            {
              for (int i = 0; i < emitter.NumAgents; i++)
              {
                agentSystems[index].addAgent(emitter);
              }
            }
          }
          index++;
        }


      }
      else
      {
        if (liveUpdate)
        {
          if (systems.Count > agentSystems.Count)
          {
            //Find the system that is not in agentSystems and add it.
            foreach (T.AgentSystem system in systems)
            {
              if (!agentSystems.Contains(system))
              {
                agentSystems.Add(new T.AgentSystem(system));
              }
            }
          }
          else if (systems.Count < agentSystems.Count)
          {
            foreach (T.AgentSystem agentSystem in agentSystems)
            {
              if (!systems.Contains(agentSystem))
              {
                agentSystems.Remove(agentSystem);
              }
            }
          }
          foreach (T.AgentSystem system in systems)
          {
            agentSystems[index].Emitters = systems[index].Emitters;
            agentSystems[index].AgentsSettings = systems[index].AgentsSettings;
            agentSystems[index].Forces = systems[index].Forces;
            agentSystems[index].Environment = systems[index].Environment;
            agentSystems[index].Behaviors = systems[index].Behaviors;
            index++;
          }
        }
        foreach (T.AgentSystem system in agentSystems)
        {
          system.run();
        }
      }

      DataTree<AgentType> tree = new DataTree<AgentType>();
      int counter = 0;
      foreach (T.AgentSystem system in agentSystems)
      {
        foreach (AgentType agent in system.Agents)
        {
          tree.Add(agent, new GH_Path(counter));
        }
        counter++;
      }

        return tree;
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
        return Properties.Resources.icon_engine;
      }
    }


    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{6367e8ac-793b-42c8-888b-b6adaa8c577b}"); }
    }
  }
}