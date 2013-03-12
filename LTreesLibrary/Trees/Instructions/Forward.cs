using System;
using System.Collections.Generic;
using System.Text;

namespace LTreesLibrary.Trees.Instructions
{
    public class Forward : TreeCrayonInstruction
    {
        private RuleSystem.SystemVariables vars;

        public float RadiusScale
        {
            get { return vars.branchScale; }
        }

        public float Variation
        {
            get { return vars.lengthVariation; }
        }


        public float Distance
        {
            get { return vars.branchLength; }
        }

        unsafe public Forward(RuleSystem.SystemVariables inVars)
        {
            this.vars = inVars;
        }

        #region TreeCrayonInstruction Members

        unsafe public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.Forward(Distance + Variation * ((float)rnd.NextDouble() * 2 - 1), RadiusScale);
        }

        #endregion
    }
}
