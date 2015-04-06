﻿using System;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Agent.Properties.Resources;

namespace Agent
{
  public class SensoryPointForceComponent : AbstractVehicleForceComponent
  {
    private Point3d sourcePt;
    private double radius;
    private double sensorLeftValue, sensorRightValue;
    private bool crossed;
    public SensoryPointForceComponent()
      : base("Sensory Point Force", "SensePt",
          "Sensory Point Force",
          null, "2e0a5081-6a73-440e-91b5-c5ed96d512f0")
    {
    }

    protected override void RegisterInputParams(GH_InputParamManager pManager)
    {
      base.RegisterInputParams(pManager);
      pManager.AddPointParameter(RS.pointName, RS.pointNickname, "The source point for the sensory field.", GH_ParamAccess.item, Point3d.Origin);
      pManager.AddNumberParameter("Radius", "R", "The radius of the range of the sensory field falloff.", GH_ParamAccess.item, 10);
      pManager.AddBooleanParameter("Crossed?", "C", "If true, the sensors will affect the wheels on the opposite side.",
        GH_ParamAccess.item, false);
    }

    protected override void RegisterOutputParams(GH_OutputParamManager pManager)
    {
      base.RegisterOutputParams(pManager);
      pManager.AddNumberParameter("SL", "SL", "SL", GH_ParamAccess.item);
      pManager.AddNumberParameter("SR", "SR", "SR", GH_ParamAccess.item);
    }

    protected override bool GetInputs(IGH_DataAccess da)
    {
      if (!base.GetInputs(da)) return false;
      if (!da.GetData(nextInputIndex++, ref sourcePt)) return false;
      if (!da.GetData(nextInputIndex++, ref radius)) return false;
      if (!da.GetData(nextInputIndex++, ref crossed)) return false;
      if (radius < 0)
      {
        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Radius must be positive.");
        return false;
      }
      return true;
    }

    protected override void SetOutputs(IGH_DataAccess da)
    {
      base.SetOutputs(da);
      da.SetData(nextOutputIndex++, sensorLeftValue);
      da.SetData(nextOutputIndex++, sensorRightValue);
    }

    protected override Vector3d CalcForce()
    {
      const double halfPi = Math.PI/2;
      double halfBodySize = vehicle.BodySize/2;
      Vector3d sensorLeftVec = vehicle.Velocity;
      Vector3d sensorRightVec = vehicle.Velocity;
      sensorLeftVec.Rotate(halfPi, vehicle.Orientation.ZAxis);
      sensorRightVec.Rotate(-halfPi, vehicle.Orientation.ZAxis);
      sensorLeftVec.Unitize();
      sensorRightVec.Unitize();
      sensorLeftVec = Vector3d.Multiply(sensorLeftVec, halfBodySize);
      sensorRightVec = Vector3d.Multiply(sensorRightVec, halfBodySize);
      Point3d sensorLeftPos = vehicle.Position;
      Point3d sensorRightPos = vehicle.Position;
      sensorLeftPos.Transform(Transform.Translation(sensorLeftVec));
      sensorRightPos.Transform(Transform.Translation(sensorRightVec));
      sensorLeftValue = sensorLeftPos.DistanceTo(sourcePt);
      sensorRightValue = sensorRightPos.DistanceTo(sourcePt);
      sensorLeftValue = Util.Number.Map(sensorLeftValue, 0, radius, 0, 1);
      sensorRightValue = Util.Number.Map(sensorRightValue, 0, radius, 0, 1);
      if (crossed)
      {
        vehicle.WheelLeft.SetSpeed(sensorRightValue);
        vehicle.WheelRight.SetSpeed(sensorLeftValue);
      }
      else
      {
        vehicle.WheelLeft.SetSpeed(sensorLeftValue);
        vehicle.WheelRight.SetSpeed(sensorRightValue);
      }
      
      return Vector3d.Zero;
    }
  }
}
