﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;
using System.Collections;

namespace Agent
{
  class SeparateForceType : ForceType
  {

    public SeparateForceType()
      : base()
    {

    }

    // Constructor with initial values
    public SeparateForceType(double weight, double visionRadiusMultiplier)
      : base(weight, visionRadiusMultiplier)
    {

    }

    // Copy Constructor
    public SeparateForceType(SeparateForceType force)
      : base(force)
    {

    }

    public override Vector3d CalcForce(AgentType agent, ISpatialCollection<AgentType> neighbors)
    {
      Vector3d steer = new Vector3d();
      Vector3d sum = new Vector3d();
      int count = 0;

      if (this.visionRadiusMultiplier != 0)
      {
        neighbors = neighbors.GetNeighborsInSphere(agent, agent.VisionRadius * this.visionRadiusMultiplier);
      }

      foreach (AgentType other in neighbors)
      {
        double d = agent.RefPosition.DistanceTo(other.RefPosition);
        if (d > 0)
        {
          //double d = Vector3d.Subtract(agent.RefPosition, other.RefPosition).Length;
          //if we are not comparing the seeker to iteself and it is at least
          //desired separation away:
          Vector3d diff = Point3d.Subtract(agent.RefPosition, other.RefPosition);
          diff.Unitize();

          //Weight the magnitude by distance to other
          diff = Vector3d.Divide(diff, d);

          sum = Vector3d.Add(sum, diff);

          //For an average, we need to keep track of how many boids
          //are in our vision.
          count++;
        }
      }

      if (count > 0)
      {
        sum = Vector3d.Divide(sum, count);
        sum.Unitize();
        sum = Vector3d.Multiply(sum, agent.MaxSpeed);
        steer = Vector3d.Subtract(sum, agent.Velocity);
        steer = Limit(steer, agent.MaxForce);
        //Multiply the resultant vector by weight.
        steer = Vector3d.Multiply(this.weight, steer);
      }
      //Seek the average position of our neighbors.
      return steer;
    }

    public override Grasshopper.Kernel.Types.IGH_Goo Duplicate()
    {
      return new SeparateForceType(this);
    }
  }
}
