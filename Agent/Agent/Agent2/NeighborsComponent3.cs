﻿using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

namespace Agent.Agent2
{
  public class NeighborsComponent3 : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the NeighborsComponent3 class.
    /// </summary>
    public NeighborsComponent3()
      : base("Neighbors in Radius", "Neighbors3",
          "Gets Neighbors in Radius",
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
      pManager.AddGenericParameter("Agent", "A", "The Agent to get neighbors for.", GH_ParamAccess.item);
      pManager.AddGenericParameter("AgentCollection", "AC", "The AgentCollection to search through.", GH_ParamAccess.item);
      pManager.AddNumberParameter("Vision Radius", "VR", "The radius around which the Agent will see other Agents.", GH_ParamAccess.item, Constants.VisionRadius);
      pManager.AddNumberParameter("Vision Angle", "VA", "The angle around which the Agent will see other Agents.", GH_ParamAccess.item, Constants.VisionAngle);
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
      pManager.AddGenericParameter("Neighbors", "AC", "The neighbors of the Agent.", GH_ParamAccess.item);

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
      AgentType agent = new AgentType();
      SpatialCollectionType agentCollection = new SpatialCollectionType();
      double visionRadius = Constants.VisionRadius;
      double visionAngle = Constants.VisionAngle;

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!DA.GetData(0, ref agent)) return;
      if (!DA.GetData(1, ref agentCollection)) return;
      DA.GetData(2, ref visionRadius);
      DA.GetData(3, ref visionAngle);


      // We should now validate the data and warn the user if invalid data is supplied.
      if (!(0.0 <= visionRadius))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Vision Radius must be greater than 0.");
        return;
      }
      if (!(0.0 <= visionAngle && visionAngle <= 360.0))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Vision Angle must be between 0 and 360.");
        return;
      }

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:

      SpatialCollectionType neighbors = run(agent, agentCollection, visionRadius, visionAngle);

      // Finally assign the output parameter.
      DA.SetData(0, neighbors);
    }

    private SpatialCollectionType run(AgentType agent, SpatialCollectionType agentCollection,
                               double visionRadius, double visionAngle)
    {
      ISpatialCollection<AgentType> neighbors = agentCollection.Agents.getNeighborsInSphere(agent, visionRadius);
      return new SpatialCollectionType(neighbors);
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
      get { return new Guid("{2477a136-9479-4e3e-b60a-1570ee1ab896}"); }
    }
  }
}