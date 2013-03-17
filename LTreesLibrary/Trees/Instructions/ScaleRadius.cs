using System;
using System.Collections.Generic;
using System.Text;

namespace LTreesLibrary.Trees.Instructions
{
    public class ScaleRadius : TreeCrayonInstruction
    {
        RuleSystem.SystemVariables vars;

        //TODO: add to rulesystem when necessary
        public float Variation
        {
            get { return 0; }
        }


        public float Scale
        {
            get { return 0; }
        }

        public ScaleRadius(RuleSystem.SystemVariables inVars)
        {
            this.vars = inVars;
        }


        #region TreeCrayonInstruction Members

        public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.ScaleRadius(Util.Random(rnd, Scale, Variation));
        }

        #endregion
    }
}
