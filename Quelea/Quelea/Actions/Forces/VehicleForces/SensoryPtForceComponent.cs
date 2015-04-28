﻿using Quelea.Util;
using Grasshopper.Kernel;
using Rhino.Geometry;
using RS = Quelea.Properties.Resources;

namespace Quelea
{
  public class SensePointForceComponent : AbstractVehicleForceComponent
  {
    private Point3d sourcePt;
    private double radius;
    private double sensorLeftValue, sensorRightValue;
    private bool crossed;
    public SensePointForceComponent()
      : base("Sensory Point Force", "SensePt",
          "Sensory Point Force",
          RS.icon_sensePointForce, "2e0a5081-6a73-440e-91b5-c5ed96d512f0")
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
      sourcePt = vehicle.Environment.MapTo2D(sourcePt);
      Point3d sensorLeftPos = vehicle.GetPartPosition(vehicle.BodySize, RS.HALF_PI);
      Point3d sensorRightPos = vehicle.GetPartPosition(vehicle.BodySize, -RS.HALF_PI);
      sensorLeftValue = sensorLeftPos.DistanceTo(sourcePt);
      sensorRightValue = sensorRightPos.DistanceTo(sourcePt);
      sensorLeftValue = Number.Map(sensorLeftValue, 0, radius, 0, 1, false);
      sensorRightValue = Number.Map(sensorRightValue, 0, radius, 0, 1, false);
      double wheelDiff;
      if (crossed)
      {
        vehicle.SetSpeedChanges(sensorRightValue, sensorLeftValue);
        wheelDiff = sensorRightValue * vehicle.WheelRadius - sensorLeftValue * vehicle.WheelRadius;
      }
      else
      {
        vehicle.SetSpeedChanges(sensorLeftValue, sensorRightValue);
        wheelDiff = sensorLeftValue * vehicle.WheelRadius - sensorRightValue * vehicle.WheelRadius;
      }
      double angle = wheelDiff / vehicle.BodySize;
      Vector3d desired = vehicle.Velocity;
      desired.Rotate(angle, vehicle.Orientation.ZAxis);

      return desired;
    }
  }
}
