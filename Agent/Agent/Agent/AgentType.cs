﻿using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Random = Agent.Util.Random;
using RS = Agent.Properties.Resources;

namespace Agent
{
  public class AgentType : GH_Goo<Object>, IPosition
  {
    private int lifespan;
    private readonly double mass;
    private readonly double bodySize;
    private readonly double maxSpeed;
    private readonly double maxForce;
    private readonly int historyLength;
    private readonly CircularArray<Point3d> positionHistory;

    private Point3d position;
    private Vector3d velocity;
    private Vector3d acceleration;

    private Point3d refPosition;

    private bool initialVelocitySet;
    public AgentType()
    {
      lifespan = RS.lifespanDefault;
      mass = RS.massDefault;
      bodySize = RS.bodySizeDefault;
      maxSpeed = RS.maxSpeedDefault;
      maxForce = RS.maxForceDefault;
      historyLength = RS.historyLenDefault;
      positionHistory = new CircularArray<Point3d>(historyLength);
      position = Point3d.Origin;
      refPosition = Point3d.Origin;
      positionHistory.Add(position);
      velocity = Random.RandomVector(-RS.velocityDefault, RS.velocityDefault);
      acceleration = Vector3d.Zero;
      initialVelocitySet = false;
    }

    public AgentType(int lifespan, double mass, double bodySize,
                     double maxSpeed, double maxForce, int historyLength)
    {
      this.lifespan = lifespan;
      this.mass = mass;
      this.bodySize = bodySize;
      this.maxSpeed = maxSpeed;
      this.maxForce = maxForce;
      this.historyLength = historyLength;
      positionHistory = new CircularArray<Point3d>(historyLength);
      position = Point3d.Origin;
      refPosition = Point3d.Origin;
      positionHistory.Add(position);
      velocity = Random.RandomVector(-RS.velocityDefault, RS.velocityDefault);
      acceleration = Vector3d.Zero;
      initialVelocitySet = false;
    }

    public AgentType(int lifespan, double mass, double bodySize,
                     double maxSpeed, double maxForce, int historyLength, Point3d position)
    {
      this.lifespan = lifespan;
      this.mass = mass;
      this.bodySize = bodySize;
      this.maxSpeed = maxSpeed;
      this.maxForce = maxForce;
      this.historyLength = historyLength;
      positionHistory = new CircularArray<Point3d>(historyLength);
      this.position = position;
      refPosition = position;
      positionHistory.Add(position);
      velocity = Random.RandomVector(-RS.velocityDefault, RS.velocityDefault);
      acceleration = Vector3d.Zero;
      initialVelocitySet = false;
    }

    public AgentType(AgentType agent)
    {
      lifespan = agent.lifespan;
      mass = agent.mass;
      bodySize = agent.bodySize;
      maxSpeed = agent.maxSpeed;
      maxForce = agent.maxForce;
      historyLength = agent.historyLength;
      positionHistory = new CircularArray<Point3d>(historyLength);
      position = agent.position;
      refPosition = agent.refPosition;
      positionHistory.Add(position);
      velocity = agent.velocity;
      acceleration = agent.acceleration;
      initialVelocitySet = false;
    }

    public AgentType(AgentType agent, Point3d position)
    {
      lifespan = agent.lifespan;
      mass = agent.mass;
      bodySize = agent.bodySize;
      maxSpeed = agent.maxSpeed;
      maxForce = agent.maxForce;
      historyLength = agent.historyLength;
      positionHistory = new CircularArray<Point3d>(historyLength);
      refPosition = position;
      this.position = position;
      positionHistory.Add(position);
      velocity = velocity = Random.RandomVector(-RS.velocityDefault, RS.velocityDefault);
      acceleration = agent.acceleration;
      initialVelocitySet = false;
    }

    public AgentType(AgentType agent, Point3d position, Point3d refPosition)
    {
      lifespan = agent.lifespan;
      mass = agent.mass;
      bodySize = agent.bodySize;
      maxSpeed = agent.maxSpeed;
      maxForce = agent.maxForce;
      historyLength = agent.historyLength;
      positionHistory = new CircularArray<Point3d>(historyLength);
      this.position = position;
      this.refPosition = refPosition;
      positionHistory.Add(position);
      velocity = velocity = Random.RandomVector(-RS.velocityDefault, RS.velocityDefault);
      acceleration = agent.acceleration;
      initialVelocitySet = false;
    }

    public int Lifespan
    {
      get
      {
        return lifespan;
      }
    }

    public double Mass
    {
      get
      {
        return mass;
      }
    }

    public double BodySize
    {
      get
      {
        return bodySize;
      }
    }

    public double MaxSpeed
    {
      get
      {
        return maxSpeed;
      }
    }

    public double MaxForce
    {
      get
      {
        return maxForce;
      }
    }

    public int HistoryLength
    {
      get
      {
        return historyLength;
      }
    }

    public CircularArray<Point3d> PositionHistory
    {
      get
      {
        return positionHistory;
      }
    }

    public List<Point3d> GetPositionHistoryList()
    {
      return positionHistory.ToList();
    }

    public Point3d Position
    {
      get
      {
        return position;
      }
      set
      {
        position = value;
      }
    }

    public Vector3d Velocity
    {
      get
      {
        return velocity;
      }
      set
      {
        velocity = value;
      }
    }

    public Vector3d Acceleration
    {
      get
      {
        return acceleration;
      }
    }

    public Point3d RefPosition
    {
      get
      {
        return refPosition;
      }
      set
      {
        refPosition = value;
      }
    }

    public void Update()
    {
      velocity = Vector3d.Add(velocity, acceleration);
      refPosition.Transform(Transform.Translation(velocity));
      position.Transform(Transform.Translation(velocity)); //So disconnecting the environment allows the agent to continue from its current position.
      acceleration = Vector3d.Multiply(acceleration, 0);
      lifespan -= 1;
    }

    public void ApplyForce(Vector3d force)
    {
      force = Vector3d.Divide(force, mass);
      acceleration = Vector3d.Add(acceleration, force);
    }

    public Boolean IsDead()
    {
      return (lifespan == 0);
    }

    public void Run()
    {
      Update();
    }

    public override bool Equals(Object obj)
    {
      // If parameter is null return false.

      // If parameter cannot be cast to Point return false.
      AgentType p = obj as AgentType;
      if (p == null)
      {
        return false;
      }

      // Return true if the fields match:
      return acceleration.Equals(p.acceleration) &&
             bodySize.Equals(p.bodySize) &&
             historyLength.Equals(p.historyLength) &&
             lifespan.Equals(p.lifespan) &&
             position.Equals(p.position) &&
             refPosition.Equals(p.refPosition) &&
             mass.Equals(p.mass) &&
             maxForce.Equals(p.maxForce) &&
             maxSpeed.Equals(p.maxSpeed) &&
             velocity.Equals(p.velocity);
    }

    public bool Equals(AgentType p)
    {
      // If parameter is null return false:
      if (p == null)
      {
        return false;
      }

      // Return true if the fields match:
      return acceleration.Equals(p.acceleration) &&
             bodySize.Equals(p.bodySize) &&
             historyLength.Equals(p.historyLength) &&
             lifespan.Equals(p.lifespan) &&
             position.Equals(p.position) &&
             refPosition.Equals(p.refPosition) &&
             mass.Equals(p.mass) &&
             maxForce.Equals(p.maxForce) &&
             maxSpeed.Equals(p.maxSpeed) &&
             velocity.Equals(p.velocity);
    }

    public override int GetHashCode()
    {
      // Return true if the fields match:
      return bodySize.GetHashCode() ^
             historyLength.GetHashCode() ^
             mass.GetHashCode() ^
             maxForce.GetHashCode() ^
             maxSpeed.GetHashCode();
    }

    public override IGH_Goo Duplicate()
    {
      return new AgentType(this);
    }

    public override bool IsValid
    {
      get
      {
        return (lifespan > 0 && mass > 0 && bodySize >= 0 && maxForce >= 0 &&
                maxSpeed >= 0 && historyLength >= 1);
      }
    }

    public override string ToString()
    {
      string lifespanStr = RS.lifespanName + ": " + lifespan + "\n";
      string massStr = RS.massName + ": " + mass + "\n";
      string bodySizeStr = RS.bodySizeName + ": " + bodySize + "\n";
      string maxForceStr = RS.maxForceName + ": " + maxForce + "\n";
      string maxSpeedStr = RS.maxSpeedName + ": " + maxSpeed + "\n";
      string historyLengthStr = RS.historyLenName + ": " + historyLength + "\n";
      return lifespanStr + massStr + bodySizeStr + maxForceStr + maxSpeedStr
             + historyLengthStr;
    }

    public override string TypeDescription
    {
      get { return RS.agentDescription; }
    }

    public override string TypeName
    {
      get { return RS.agentName; }
    }

    public Point3d GetPoint3D()
    {
      return refPosition;
    }

    public bool InitialVelocitySet
    {
      get { return initialVelocitySet; }
      set { initialVelocitySet = value; }
    }

    internal void Die()
    {
      lifespan = 0;
    }
  }
}