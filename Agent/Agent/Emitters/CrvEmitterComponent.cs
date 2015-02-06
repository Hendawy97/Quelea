﻿using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace Agent
{
  public class CrvEmitterComponent : GH_Component
  {
    /// <summary>
    /// Each implementation of GH_Component must provide a public 
    /// constructor without any arguments.
    /// Category represents the Tab in which the component will appear, 
    /// Subcategory the panel. If you use non-existing tab or panel names, 
    /// new tabs/panels will automatically be created.
    /// </summary>
    public CrvEmitterComponent()
      : base("Curve Emitter", "crvEmit",
          "Emit agents from a curve.",
          "Agent", "Emitters")
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
      pManager.AddCurveParameter("Curve", "C", "Base curve for emitter", GH_ParamAccess.item);
      pManager.AddBooleanParameter("Continuous Flow", "C", "If true, Agents will be emitted every Rth timestep.\n If false, N Agents will be emitted once.", GH_ParamAccess.item, true);
      pManager.AddIntegerParameter("Creation Rate", "R", "Rate at which new Agents are created. Every Rth timestep.", GH_ParamAccess.item, 1);
      pManager.AddIntegerParameter("Number of Agents", "N", "The number of Agents", GH_ParamAccess.item, 0);

      pManager[2].Optional = true;
      pManager[3].Optional = true;
      // If you want to change properties of certain parameters, 
      // you can use the pManager instance to access them by index:
      //pManager[0].Optional = true;
    }

    /// <summary>
    /// Registers all the output parameters for this component.
    /// </summary>
    protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
    {
      // Use the pManager object to register your output parameters.
      // Output parameters do not have default values, but they too must have the correct access type.
      pManager.AddGenericParameter("Curve Emitter", "E", "Curve Emitter", GH_ParamAccess.item);
      //pManager.AddPointParameter("Point Emitter Location", "P", "Point Emitter Location", GH_ParamAccess.item);

      // Sometimes you want to hide a specific parameter from the Rhino preview.
      // You can use the HideParameter() method as a quick way:
      //pManager.HideParameter(1);
    }

    /// <summary>
    /// This is the method that actually does the work.
    /// </summary>
    /// <param name="da">The DA object can be used to retrieve data from input parameters and 
    /// to store data in output parameters.</param>
    protected override void SolveInstance(IGH_DataAccess da)
    {
      // First, we need to retrieve all data from the input parameters.
      // We'll start by declaring variables and assigning them starting values.
      Curve crv = null;
      bool continuousFlow = true;
      int creationRate = 1;
      int numAgents = 0;

      // Then we need to access the input parameters individually. 
      // When data cannot be extracted from a parameter, we should abort this method.
      if (!da.GetData(0, ref crv)) return;
      if (!da.GetData(1, ref continuousFlow)) return;
      if (!da.GetData(2, ref creationRate)) return;
      if (!da.GetData(3, ref numAgents)) return;

      // We should now validate the data and warn the user if invalid data is supplied.
      if (creationRate <= 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Creation rate must be greater than 0.");
        return;
      }
      if (numAgents < 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Number of Agents must be greater than or equal to 0.");
        return;
      }

      // We're set to create the output now. To keep the size of the SolveInstance() method small, 
      // The actual functionality will be in a different method:
      CrvEmitterType emitter = new CrvEmitterType(crv, continuousFlow, creationRate, numAgents);

      // Finally assign the spiral to the output parameter.
      da.SetData(0, emitter);
    }

    /// <summary>
    /// The Exposure property controls where in the panel a component icon 
    /// will appear. There are seven possible locations (primary to septenary), 
    /// each of which can be combined with the GH_Exposure.obscure flag, which 
    /// ensures the component will only be visible on panel dropdowns.
    /// </summary>
    public override GH_Exposure Exposure
    {
      get { return GH_Exposure.primary; }
    }

    /// <summary>
    /// Provides an Icon for every component that will be visible in the User Interface.
    /// Icons need to be 24x24 pixels.
    /// </summary>
    protected override System.Drawing.Bitmap Icon
    {
      get
      {
        // You can add image files to your project resources and access them like this:
        //return Resources.IconForThisComponent;
        //return null;
        return Properties.Resources.icon_crvEmitter;
      }
    }

    /// <summary>
    /// Each component must have a unique Guid to identify it. 
    /// It is vital this Guid doesn't change otherwise old ghx files 
    /// that use the old ID will partially fail during loading.
    /// </summary>
    public override Guid ComponentGuid
    {
      get { return new Guid("{fdfef03a-611a-4c07-8b59-317ab92ee69a}"); }
    }
  }
}
