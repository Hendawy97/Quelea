﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;

namespace Agent
{
  class CoheseForceType : ForceType
  {
    
    public CoheseForceType()
      : base() 
    { 

    }

    // Constructor with initial values
    public CoheseForceType(double weight, double visionRadiusMultiplier)
      : base(weight, visionRadiusMultiplier)
    {

    }

    // Copy Constructor
    public CoheseForceType(CoheseForceType force)
      : base(force)
    {

    }


    public override Vector3d calcForce(AgentType agent, List<AgentType> agents)
    {
      Vector3d sum = new Vector3d();
      int count = 0;
      Vector3d steer = new Vector3d();

      foreach (AgentType other in agents)
      {
        //Add up all the velocities and divide by the total to calculate
        //the average velocity.
        Vector3d vec = Vector3d.Subtract(agent.Location, other.Location);
        double d = vec.Length;
        if ((d > 0) && (d < agent.VisionRadius*this.visionRadiusMultiplier))
        {
          //Adding up all the others' location
          sum = Vector3d.Add(sum, other.Location);
          //For an average, we need to keep track of how many boids
          //are in our vision.
          count++;
        }
      }

      if (count > 0)
      {
        //We desire to go in that direction at maximum speed.
        sum = Vector3d.Divide(sum, count);
        steer = this.seek(agent, sum);
      }

      //Multiply the resultant vector by weight.
      steer = Vector3d.Multiply(this.weight, steer);

      //Seek the average location of our neighbors.
      return steer;
    }

    public override Grasshopper.Kernel.Types.IGH_Goo Duplicate()
    {
      return new CoheseForceType(this);
    }
  }
}
