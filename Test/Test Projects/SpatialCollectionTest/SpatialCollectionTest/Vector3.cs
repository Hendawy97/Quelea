﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
  public class Vector3
  {
    public float x, y, z;
    public float magnitude;

    public Vector3(float x, float y, float z)
    {
      this.x = x;
      this.y = y;
      this.z = z;
      this.magnitude = (float)Math.Sqrt(x * x + y * y + z * z);
    }

    public static Vector3 operator +(Vector3 v1, Vector3 v2)
    {
      return new Vector3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
    }

    public static Vector3 operator -(Vector3 v1, Vector3 v2)
    {
      return new Vector3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
    }

    public static Vector3 Cross(Vector3 v1, Vector3 v2)
    {
      return new Vector3(v1.y * v2.z - v1.z * v2.y,
                         v1.z * v2.x - v1.x * v2.z,
                         v1.x * v2.y - v1.y * v2.x);
    }

    public double DistanceTo(Vector3 v)
    {
      return Math.Sqrt(Math.Pow(this.x + v.x, 2) + Math.Pow(this.y + v.y, 2) + Math.Pow(this.z + v.z, 2));
    }

    public float X
    {
      get
      {
        return this.x;
      }
      set
      {
        this.x = value;
      }
    }

    public float Y
    {
      get
      {
        return this.y;
      }
      set
      {
        this.y = value;
      }
    }

    public float Z
    {
      get
      {
        return this.z;
      }
      set
      {
        this.z = value;
      }
    }
  }

  public class Bounds
  {
    public Vector3 center, r;  // center, radius
    public Vector3 lb, rt; // left-bottom (minimal corner), right-top (maximal corner)

    public Bounds(Vector3 center, Vector3 sideLength)
    {
      this.center = center;
      this.r = new Vector3(sideLength.x / 2, sideLength.y / 2, sideLength.z / 2);
      this.lb = this.center - this.r;
      this.rt = this.center + this.r;
    }

    public Bounds(Point3d min, Point3d max)
    {
      Vector3 lb = new Vector3((float)min.X, (float)min.Y, (float)min.Z);
      Vector3 rt = new Vector3((float)max.X, (float)max.Y, (float)max.Z);
      this.center = new Vector3((lb.X + rt.X) / 2, (lb.Y + rt.Y) / 2, (lb.Z + rt.Z) / 2);
      this.r = new Vector3(rt.X - center.X, rt.Y - center.Y, rt.Z - center.Z);
      this.lb = lb;
      this.rt = rt;
    }

    public bool Contains(Vector3 p)
    {
      Vector3 c = center;
      return ((c.x - r.x < p.x) && (c.x + r.x >= p.x) &&
              (c.y - r.y < p.y) && (c.y + r.y >= p.y) &&
              (c.z - r.z < p.z) && (c.z + r.z >= p.z));
    }

    public static float min(float f1, float f2)
    {
      return (f1 < f2) ? f1 : f2;
    }

    public static float max(float f1, float f2)
    {
      return (f1 < f2) ? f2 : f1;
    }

    public bool IntersectRay(Ray ray)
    {
      // See http://gamedev.stackexchange.com/questions/18436/most-efficient-aabb-vs-ray-collision-algorithms
      // r.dir is unit direction vector of ray
      Vector3 dirfrac = new Vector3(0, 0, 0);
      dirfrac.x = 1.0f / ray.direction.x;
      dirfrac.y = 1.0f / ray.direction.y;
      dirfrac.z = 1.0f / ray.direction.z;
      // lb is the corner of AABB with minimal coordinates - left bottom, rt is maximal corner
      // r.org is origin of ray
      float t1 = (lb.x - ray.origin.x) * dirfrac.x;
      float t2 = (rt.x - ray.origin.x) * dirfrac.x;
      float t3 = (lb.y - ray.origin.y) * dirfrac.y;
      float t4 = (rt.y - ray.origin.y) * dirfrac.y;
      float t5 = (lb.z - ray.origin.z) * dirfrac.z;
      float t6 = (rt.z - ray.origin.z) * dirfrac.z;

      float tmin = max(max(min(t1, t2), min(t3, t4)), min(t5, t6));
      float tmax = min(min(max(t1, t2), max(t3, t4)), max(t5, t6));

      // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
      float t;
      if (tmax < 0)
      {
        t = tmax;
        return false;
      }

      // if tmin > tmax, ray doesn't intersect AABB
      if (tmin > tmax)
      {
        t = tmax;
        return false;
      }

      t = tmin;
      return true;
    }
  }

  public class Ray
  {
    public Vector3 direction, origin;
    public Ray(Vector3 direction, Vector3 origin) {
      this.direction = direction;
      this.origin = origin;
    }
  }
}
