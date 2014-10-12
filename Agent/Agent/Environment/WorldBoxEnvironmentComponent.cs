﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent
{
  public class WorldBoxEnvironmentComponent : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the WorldBoxEnvironmentComponent class.
    /// </summary>
    public WorldBoxEnvironmentComponent()
      : base("WorldBoxEnvironmentComponent", "WBoxEnv",
          "A World Box Environment",
          "Agent", "Environments")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddBoxParameter("Box", "B", "A Box aligned to World Axes.", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Wrap", "W", "If true, agents that hit the edge"
                               + "of the environment will be wrapped aound to"
                               + "the other side. If false, agents will bounce"
                               + " off the edges.", GH_ParamAccess.item);
      pManager[1].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("World Box Environment", "En", "World Box Environment", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess DA)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      Interval interval = new Interval(-100.0, 100.0);
      Box box = new Box(Plane.WorldXY, interval, interval, interval);
      bool wrap = false;

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!DA.GetData(0, ref box)) return;
      DA.GetData(1, ref wrap);

      // We should now validate the data and warn the user if invalid data is supplied.
      if (!box.Plane.Equals(Plane.WorldXY))
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Box must be aligned to WorldXY.");
        return;
      }

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      WorldBoxEnvironmentType wBoxEnv = new WorldBoxEnvironmentType(box, wrap);

      // Finally assign the spiral to the output parameter.
      DA.SetData(0, wBoxEnv);
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
      get { return new Guid("{132d89e3-3614-4c54-b15b-9087ff50c412}"); }
    }
  }
}