﻿using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Agent
{
  public class WorldBoxEnvironmentType : EnvironmentType
  {

    private Box box;
    private bool wrap;

    // Default Constructor. Defaults to continuous flow, creating a new Agent every timestep.
    public WorldBoxEnvironmentType()
      : base()
    {
      Interval interval = new Interval(-100.0, 100.0);
      this.box = new Box(Plane.WorldXY, interval, interval, interval);
    }

    // Constructor with initial values.
    public WorldBoxEnvironmentType(Box box, bool wrap)
    {
      this.box = box;
      this.wrap = wrap;
    }

    // Copy Constructor
    public WorldBoxEnvironmentType(WorldBoxEnvironmentType environment)
    {
      this.box = environment.box;
      this.wrap = environment.wrap;
    }

    public override bool Equals(object obj)
    {
      // If parameter cannot be cast to ThreeDPoint return false:
      WorldBoxEnvironmentType p = obj as WorldBoxEnvironmentType;
      if ((object)p == null)
      {
        return false;
      }

      return base.Equals(obj) && this.box.Equals(p.box) && this.wrap.Equals(p.wrap);
    }

    public bool Equals(WorldBoxEnvironmentType p)
    {
      return base.Equals((WorldBoxEnvironmentType)p) && this.box.Equals(p.box) && this.wrap.Equals(p.wrap);
    }

    public override int GetHashCode()
    {
      return this.box.GetHashCode() ^ this.wrap.GetHashCode();
    }

    public override IGH_Goo Duplicate()
    {
      return new WorldBoxEnvironmentType(this);
    }

    public override bool IsValid
    {
      get
      {
        return (this.box.IsValid) && box.Plane.Equals(Plane.WorldXY);
      }

    }

    public override string ToString()
    {

      string box = "Box: " + this.box.ToString() + "\n";
      string wrap = "Wrap: " + this.wrap.ToString() + "\n";
      return box + wrap;
    }

    public override string TypeDescription
    {
      get { return "A World Box Environment"; }
    }

    public override string TypeName
    {
      get { return "World Box Environment"; }
    }


    public override Point3d closestPoint(Point3d emittionPt)
    {
      return this.box.ClosestPoint(emittionPt);
    }
  }
}
