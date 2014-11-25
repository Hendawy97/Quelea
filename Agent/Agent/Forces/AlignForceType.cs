﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rhino.Geometry;

namespace Agent
{
  class AlignForceType : ForceType
  {

    public AlignForceType()
      : base()
    {

    }

    // Constructor with initial values
    public AlignForceType(double weight, double visionRadiusMultiplier)
      : base(weight, visionRadiusMultiplier)
    {

    }

    // Copy Constructor
    public AlignForceType(AlignForceType force)
      : base(force)
    {

    }

    public override Vector3d calcForceWithOctree(AgentType agent, IList<AgentType> agents, OctTree agentsOctree)
    {
      throw new NotImplementedException();
    }


    public override Vector3d calcForce(AgentType agent, IList<AgentType> agents)
    {
      Vector3d sum = new Vector3d();
      int count = 0;
      Vector3d steer = new Vector3d();

      foreach (AgentType other in agents)
      {
        //Add up all the velocities and divide by the total to calculate
        //the average velocity.
        double d = agent.RefPosition.DistanceTo(other.RefPosition);
        if ((d > 0) && (d < agent.VisionRadius * this.visionRadiusMultiplier))
        {
          //Adding up all the others' location
          sum = Vector3d.Add(sum, new Vector3d(other.Velocity));
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
        steer = limit(steer, agent.MaxForce);
        //Multiply the resultant vector by weight.
        steer = Vector3d.Multiply(this.weight, steer);
      }
      //Seek the average location of our neighbors.
      return steer;
    }

    public override Grasshopper.Kernel.Types.IGH_Goo Duplicate()
    {
      return new AlignForceType(this);
    }

    public override Vector3d calcForceWithKdTree(AgentType a, IList<AgentType> list, KdTree.IKdTree<float, AgentType> kdTree)
    {
      throw new NotImplementedException();
    }
  }
}
