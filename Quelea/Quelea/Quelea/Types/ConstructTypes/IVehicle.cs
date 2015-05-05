﻿using Rhino.Geometry;

namespace Quelea
{
  public interface IVehicle : IAgent
  {
    IWheel[] Wheels { get; set; }
    double WheelRadius { get; set; }
    Point3d GetWheelPosition(double gapSize, double rotation);
    void SetSpeedChanges(double leftValue, double rightValue);

    Vector3d CalculateSensorForce(double leftWheelValue, double rightWheelValue);
    Point3d GetSensorPosition(double visionRadiusMultiplier, double visionAngleMultiplier);
  }

  public interface ISensor
  {
    Point3d Position { get; set; }
    double MaxReading { get; set; }
    double Reading { get; set; }
  }

  public interface IWheel
  {
    Point3d Position { get; set; }
    double AngularVelocity { get; set; }
    double Angle { get; set; }
    double Radius { get; set; }
    double TangentialVelocity { get; set; }
    void SetSpeed(double angularVelocity);
    void SetSpeedChange(double increment);
    void Run();
  }
}
