using System;
using System.Collections.Generic;
using System.Text;

namespace LTreesLibrary.Trees.Instructions
{
    unsafe public class Scale : TreeCrayonInstruction
    {
        RuleSystem.SystemVariables vars;

        //TODO: add to rulesystem when necessary
        public float Variation
        {
            get { return 0; }
        }


        public float Value
        {
            get { return 0; }
        }

        public Scale(RuleSystem.SystemVariables inVars)
        {
            this.vars = inVars;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.Scale(Util.Random(rnd, Value, Variation));
        }


        #endregion
    }
}
