﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;

namespace Agent
{
    public abstract class EmitterType : GH_Goo<Object>
    {
        protected bool continuousFlow;
        protected int creationRate;
        protected int numAgents;

        public EmitterType()
        {
            // TODO: Complete member initialization
        }

        abstract public Vector3d emit();

        abstract public override IGH_Goo Duplicate();

        public override bool IsValid
        {
            get
            {
                return (this.creationRate > 0 && this.numAgents >= 0);
            }
        }

        public override string ToString()
        {
            string continuousFlow = "ContinuousFlow: " + this.continuousFlow.ToString() + "\n";
            string creationRate = "Creation Rate: " + this.creationRate.ToString() + "\n";
            string numAgents = "Number of Agents: " + this.numAgents.ToString() + "\n";
            return continuousFlow + creationRate + numAgents;
        }

        public override string TypeDescription
        {
            get { return "An Emitter"; }
        }

        public override string TypeName
        {
            get { return "EmitterType"; }
        }
    }
}
