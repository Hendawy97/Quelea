﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Geometry;

namespace Agent
{

  public class SpatialCollectionAsBinLattice<T> : ISpatialCollection<T> where T : class
  {
    private IList<T> spatialObjects; //List of all the spatial objects
    private LinkedList<T>[][][] lattice; // Lattice of DoublyLinkedLists for intersection test
    private int cols, rows, layers;
    private double binSize;
    private Point3d min, max;

    public SpatialCollectionAsBinLattice()
    {
      this.spatialObjects = new List<T>();
      this.binSize = 5;
      double binRadius = binSize / 2.0;
      this.min = new Point3d(-binRadius, -binRadius, -binRadius);
      this.max = new Point3d(binRadius, binRadius, binRadius);
      PopulateLattice();
    }

    public SpatialCollectionAsBinLattice(Point3d min, Point3d max, int binSize)
    {
      this.spatialObjects = new List<T>();
      this.binSize = binSize;
      InitBounds(min, max);
      PopulateLattice();
    }

    private void InitBounds(Point3d min, Point3d max)
    {
      double binRadius = binSize/2.0;
      if (min.X.Equals(max.X))
      {
        this.min.X = min.X - binRadius;
        this.max.X = max.X + binRadius;
      }
      else
      {
        this.min.X = min.X;
      }
      if (min.Y.Equals(max.Y))
      {
        this.min.Y = min.Y - binRadius;
        this.max.Y = max.Y + binRadius;
      }
      else
      {
        this.min.Y = min.Y;
      }
      if (min.Z.Equals(max.Z))
      {
        this.min.Z = min.Z - binRadius;
        this.max.Z = max.Z + binRadius;
      }
      else
      {
        this.min.Z = min.Z;
      }
    }

    public SpatialCollectionAsBinLattice(Point3d min, Point3d max, int binSize, IList<T> items)
    {
      this.spatialObjects = items;
      this.binSize = binSize;
      InitBounds(min, max);
      PopulateLattice();
    }

    private void PopulateLattice()
    {
      this.cols = (int)Math.Ceiling((this.max.X - this.min.X) / this.binSize);
      this.rows = (int)Math.Ceiling((this.max.Y - this.min.Y) / this.binSize);
      this.layers = (int)Math.Ceiling((this.max.Z - this.min.Z) / this.binSize);

      //Initialize lattice as 3D array of empty LinkedLists
      this.lattice = new LinkedList<T>[cols][][];
      for (int i = 0; i < cols; i++)
      {
        this.lattice[i] = new LinkedList<T>[rows][];
        for (int j = 0; j < rows; j++)
        {
          this.lattice[i][j] = new LinkedList<T>[layers];
          for (int k = 0; k < layers; k++)
          {
            lattice[i][j][k] = new LinkedList<T>();
          }
        }
      }

      foreach (T item in this.spatialObjects)
      {
        this.AddToLattice(item);
      }
    }

    public SpatialCollectionAsBinLattice(SpatialCollectionAsBinLattice<T> collection)
    {
      this.spatialObjects = collection.spatialObjects;
      this.min = collection.min;
      this.max = collection.max;
      PopulateLattice();
    }

    public SpatialCollectionAsBinLattice(ISpatialCollection<T> spatialCollection)
    {
      // TODO: Complete member initialization
      this.spatialObjects = (IList<T>)spatialCollection.SpatialObjects;
      if(spatialCollection is SpatialCollectionAsBinLattice<T>) {
        SpatialCollectionAsBinLattice<T> sC = ((SpatialCollectionAsBinLattice<T>)spatialCollection);
        this.binSize = sC.binSize;
        this.min = sC.min;
        this.max = sC.max;
        this.lattice = sC.lattice;
      }
      else
      {
        updateBounds();
        this.binSize = (this.max.X - this.min.X) / 10;
      }
      
      PopulateLattice();
    }

    private void updateBounds()
    {
      this.min.X = min.Y = min.Z = Double.MaxValue;
      this.max.X = max.Y = max.Z = Double.MinValue;
      IPosition position;
      Point3d p;
      foreach (T item in this.spatialObjects)
      {
         position= (IPosition)item;
         p = position.GetPoint3D();
         updateBounds(p);
      }
    }

    private void updateBounds(Point3d p)
    {
      if (p.X > max.X) max.X = p.X;
      if (p.Y > max.Y) max.Y = p.Y;
      if (p.Z > max.Z) max.Z = p.Z;
      if (p.X < min.X) min.X = p.X;
      if (p.Y < min.Y) min.Y = p.Y;
      if (p.Z < min.Z) min.Z = p.Z;
    }

    public ISpatialCollection<T> GetNeighborsInSphere(T item, double r)
    {
      double rSquared = r * r;
      // ISpatialCollection<T> neighbors = new SpatialCollectionAsBinLattice<T>();
      IPosition position = (IPosition)item;
      //LinkedList<T> possibleNeighbors = getBin(item);
      List<T> possibleNeighbors = GetBins(item, r);
      ISpatialCollection<T> neighbors = new SpatialCollectionAsList<T>();

      foreach (T other in possibleNeighbors)
      {
        if (!Object.ReferenceEquals(item, other))
        {
          Point3d p1 = position.GetPoint3D();
          Point3d p2 = ((IPosition)other).GetPoint3D();
          if (Util.Point.DistanceSquared(p1, p2) < rSquared)
          {
            neighbors.Add(other);
          }
        }
      }

      return neighbors;
    }

    private LinkedList<T> GetBin(T item)
    {
      Point3d p = ((IPosition)item).GetPoint3D();
      int col = (int)((p.X - min.X) / this.binSize);
      int row = (int)((p.Y - min.Y) / this.binSize);
      int layer = (int)((p.Z - min.Z) / this.binSize);
      return this.lattice[col][row][layer];
    }

    private List<T> GetBins(T item, double radius)
    {
      List<T> possibleNeighbors = new List<T>();
      Point3d p = ((IPosition)item).GetPoint3D();
      //int col = (int)(p.X - min.X) / this.binSize;
      //int row = (int)(p.Y - min.Y) / this.binSize;
      //int layer = (int)(p.Z - min.Z) / this.binSize;

      //int firstCol = (int)(col * this.binSize - radius) / this.binSize;
      //int firstRow = (int)(row * this.binSize - radius) / this.binSize;
      //int firstLayer = (int)(layer * this.binSize - radius) / this.binSize;
      //int lastCol = (int)(col * this.binSize + radius) / this.binSize;
      //int lastRow = (int)(row * this.binSize + radius) / this.binSize;
      //int lastLayer = (int)(layer * this.binSize + radius) / this.binSize;
      double offsetX = p.X - min.X;
      double offsetY = p.Y - min.Y;
      double offsetZ = p.Z - min.Z;
      int firstCol = (int)((offsetX - radius) / this.binSize);
      int firstRow = (int)((offsetY - radius) / this.binSize);
      int firstLayer = (int)((offsetZ - radius) / this.binSize);
      int lastCol = (int)((offsetX + radius) / this.binSize);
      int lastRow = (int)((offsetY + radius) / this.binSize);
      int lastLayer = (int)((offsetZ + radius) / this.binSize);

      if (firstCol < 0) firstCol = 0;
      if (firstRow < 0) firstRow = 0;
      if (firstLayer < 0) firstLayer = 0;
      if (lastCol >= this.cols) lastCol = this.cols - 1;
      if (lastRow >= this.rows) lastRow = this.rows - 1;
      if (lastLayer >= this.layers) lastLayer = this.layers - 1;


      for (int c = firstCol; c <= lastCol; c++)
      {
        for (int r = firstRow; r <= lastRow; r++)
        {
          for (int l = firstLayer; l <= lastLayer; l++)
          {
            possibleNeighbors.AddRange(this.lattice[c][r][l]);
          }
        }
      }

      return possibleNeighbors;
    }

    private bool CheckBounds(Point3d p)
    {
      bool beyondMax = false;
      double sizeX, sizeY, sizeZ;
      while (p.X > max.X)
      {
        sizeX = max.X - min.X;
        max.X += sizeX;
        beyondMax = true;
      }
      while (p.X < min.X)
      {
        sizeX = max.X - min.X;
        min.X -= sizeX;
        beyondMax = true;
      }
      while (p.Y > max.Y)
      {
        sizeY = max.Y - min.Y;
        max.Y += sizeY;
        beyondMax = true;
      }
      while (p.Y < min.Y)
      {
        sizeY = max.Y - min.Y;
        min.Y -= sizeY;
        beyondMax = true;
      }
      while (p.Z > max.Z)
      {
        sizeZ = max.Z - min.Z;
        max.Z += sizeZ;
        beyondMax = true;
      }
      while (p.Z < min.Z)
      {
        sizeZ = max.Z - min.Z;
        min.Z -= sizeZ;
        beyondMax = true;
      }

      if (beyondMax)
      {
        this.PopulateLattice();
      }

      return beyondMax;
    }

    private bool CheckBounds2(Point3d p)
    {
      bool beyondMax = false;
      if (p.X > max.X)
      {
        max.X =p.X;
        beyondMax = true;
      }
      if (p.X < min.X)
      {
        min.X = p.X;
        beyondMax = true;
      }
      if (p.Y > max.Y)
      {
        max.Y = p.Y;
        beyondMax = true;
      }
      if (p.Y < min.Y)
      {
        min.Y = p.Y;
        beyondMax = true;
      }
      if (p.Z > max.Z)
      {
        max.Z = p.Z;
        beyondMax = true;
      }
      if (p.Z < min.Z)
      {
        min.Z = p.Z;
        beyondMax = true;
      }

      if (beyondMax)
      {
        this.PopulateLattice();
      }

      return beyondMax;
    }

    public void Add(T item)
    {
      this.spatialObjects.Add(item);
      AddToLattice(item);
    }

    private void AddToLattice(T item)
    {
      Point3d p = ((IPosition)item).GetPoint3D();

      CheckBounds(p);

      int col = (int)((p.X - min.X) / this.binSize);
      int row = (int)((p.Y - min.Y) / this.binSize);
      int layer = (int)((p.Z - min.Z) / this.binSize);
      // It goes in 27 cells, i.e. every Thing is tested against other Things in its cell
      // as well as its 26 neighbors 
      //for (int dCol = -1; dCol <= 1; dCol++)
      //{
      //  for (int dRow = -1; dRow <= 1; dRow++)
      //  {
      //    for (int dLayer = -1; dLayer <= 1; dLayer++)
      //    {
      //      if (col + dCol >= 0 && col + dCol < this.cols &&
      //          row + dRow >= 0 && row + dRow < this.rows &&
      //          layer + dLayer >= 0 && layer + dLayer < this.layers)
      //      {
      //        lattice[col + dCol][row + dRow][layer + dLayer].AddLast(item);
      //      }
      //    }
      //  }
      //}
      lattice[col][row][layer].AddLast(item);
    }

    public void Clear()
    {
      this.spatialObjects.Clear();
      for (int i = 0; i < cols; i++)
      {
        for (int j = 0; j < rows; j++)
        {
          for (int k = 0; k < layers; k++)
          {
            lattice[i][j][k].Clear();
          }
        }
      }
    }

    public bool Contains(T item)
    {
      return this.spatialObjects.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
      this.spatialObjects.CopyTo(array, arrayIndex);
    }

    public int Count
    {
      get { return this.spatialObjects.Count; }
    }

    public bool IsReadOnly
    {
      get { return this.spatialObjects.IsReadOnly; }
    }

    public bool Remove(T item)
    {
      LinkedList<T> bin = GetBin(item);
      return this.spatialObjects.Remove(item) && bin.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
      return this.spatialObjects.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.spatialObjects.GetEnumerator();
    }

    public IEnumerable<T> SpatialObjects
    {
      get { return this.spatialObjects; }
    }

    public void UpdateDatastructure(Point3d min, Point3d max, int minNodeSize, IList<T> spatialObjects)
    {
      this.spatialObjects = spatialObjects;
      this.binSize = minNodeSize;
      InitBounds(min, max);
      
      PopulateLattice();
    }
  }
}
