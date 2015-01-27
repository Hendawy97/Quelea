﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent.Agent2
{
  public class CoheseForceComponent3 : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the CoheseForceComponent class.
    /// </summary>
    public CoheseForceComponent3()
      : base("Cohese Force", "Cohese3",
          "Cohesion",
          "Agent", "Agent2")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      // Use the pManager object to register your input parameters.
      // You can often supply default values when creating parameters.
      // All parameters must have the correct access type. If you want 
      // to import lists or trees of values, modify the ParamAccess flag.
      pManager.AddGenericParameter("System 1", "S1", "The System to affect.", GH_ParamAccess.item);
      pManager.AddGenericParameter("System 2", "S2", "The System to react to.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Vision Angle", "A", "The angle around which the Agent will see other Agents.", GH_ParamAccess.item, Constants.VisionAngle);
      pManager.AddNumberParameter("Vision Radius Mutliplier", "R", "The radius around which the Agent will see other Agents.", GH_ParamAccess.item, Constants.VisionRadiusMultiplier);

      // If you want to change properties of certain parameters, 
      // you can use the pManager instance to access them by index:
      pManager[2].Optional = true;
      pManager[3].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      // Use the pManager object to register your output parameters.
      // Output parameters do not have default values, but they too must have the correct access type.
      pManager.AddGenericParameter("Cohesion Force", "F", "Cohesion Force", GH_ParamAccess.item);

      // Sometimes you want to hide a specific parameter from the Rhino preview.
      // You can use the HideParameter() method as a quick way:
      //pManager.HideParameter(1);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      AgentSystemType system1 = new AgentSystemType();
      AgentSystemType system2 = new AgentSystemType();
      double visionAngle = Constants.VisionAngle;
      double visionRadiusMultiplier = Constants.VisionRadiusMultiplier;

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!DA.GetData(0, ref system1)) return;
      if (!DA.GetData(1, ref system2)) return;
      DA.GetData(2, ref visionAngle);
      DA.GetData(3, ref visionRadiusMultiplier);

      // We should now validate the data and warn the user if invalid data is supplied.
      if (!(0.0 <= visionAngle && visionAngle <= 360.0))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Vision Angle must be between 0 and 360.");
        return;
      }
      if (!(0.0 <= visionRadiusMultiplier && visionRadiusMultiplier <= 1.0))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Vision Radius must be between 0 and 1.");
        return;
      }


      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:

      List<Vector3d> forces = run(system1, system2, visionAngle, visionRadiusMultiplier);

      // Finally assign the output parameter.
      DA.SetDataList(0, forces);
    }

    private List<Vector3d> run(AgentSystemType system1, AgentSystemType system2,
                               double visionAngle, double visionRadiusMultiplier)
    {
      List<Vector3d> forces = new List<Vector3d>();
      foreach (AgentType agent in system1.Agents)
      {
        ISpatialCollection<AgentType> neighbors = system2.Agents.getNeighborsInSphere(agent, agent.VisionRadius * visionRadiusMultiplier);
        //forces.Add(calcForce(agent, neighbors));
        agent.applyForce(calcForce(agent, neighbors));
      }

      return forces;
    }

    private Vector3d calcForce(AgentType agent, ISpatialCollection<AgentType> neighbors)
    {
      Vector3d sum = new Vector3d();
      int count = 0;

      foreach (AgentType neighbor in neighbors)
      {
        //Adding up all the others' location
        sum = Vector3d.Add(sum, new Vector3d(neighbor.RefPosition));
        //For an average, we need to keep track of how many boids
        //are in our vision.
        count++;
      }

      if (count > 0)
      {
        //We desire to go in that direction at maximum speed.
        sum = Vector3d.Divide(sum, count);
        sum = Util.Agent.seek(agent, sum);
      }
      //Seek the average location of our neighbors.
      return sum;
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
        return Properties.Resources.icon_coheseForce;
      }
    }

    /// <summary>
    /// Gets the unique ID for this component. Do not change this ID after release.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{a39185b1-35bb-4805-9684-8d0097017414}"); }
    }
  }
}