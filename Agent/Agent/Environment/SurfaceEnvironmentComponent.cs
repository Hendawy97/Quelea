﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent
{
  public class SurfaceEnvironmentComponent : GH_Component
  {
    /// <summary>
    /// Initializes a new instance of the WorldBoxEnvironmentComponent class.
    /// </summary>
    public SurfaceEnvironmentComponent()
      : base("SurfaceEnvironment", "SrfEnv",
          "A Surface Environment",
          "Agent", "Environments")
    {
    }

    /// <summary>
    /// Registers all the input parameters for this component.
    /// </summary>
    protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
    {
      pManager.AddSurfaceParameter("Surface", "S", "An untrimmed Surface", GH_ParamAccess.item);
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      pManager.AddGenericParameter("Surface Environment", "En", "Surface Environment", GH_ParamAccess.item);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object is used to retrieve from inputs and store in outputs.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      Point3d pt1 = new Point3d(0, 0, 0);
      Point3d pt2 = new Point3d(100, 0, 0);
      Point3d pt3 = new Point3d(0, 100, 0);
      Surface srf = NurbsSurface.CreateFromCorners(pt1, pt2, pt3);

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!da.GetData(0, ref srf)) return;

      // We should now validate the data and warn the user if invalid data is supplied.

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      EnvironmentType environment = new SurfaceEnvironmentType(srf);

      // Finally assign the spiral to the output parameter.
      da.SetData(0, environment);
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
      get { return new Guid("{6366359a-7e08-440a-8cab-78403729c8e2}"); }
    }
  }
}