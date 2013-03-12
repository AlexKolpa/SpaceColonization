using System;
using System.Collections.Generic;
using System.Text;

namespace LTreesLibrary.Trees.Instructions
{
    public class Backward : TreeCrayonInstruction
    {
        private RuleSystem.SystemVariables vars;

        public float DistanceVariation
        {
            get { return vars.backwardVariation; }
        }


        public float Distance
        {
            get { return vars.backwardLength; }
        }

        public Backward(RuleSystem.SystemVariables inVars)
        {
            this.vars = inVars;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.Backward(Util.Random(rnd.NextDouble(), Distance, DistanceVariation));
        }

        #endregion
    }
}
