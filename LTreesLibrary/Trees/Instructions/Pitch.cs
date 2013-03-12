using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace LTreesLibrary.Trees.Instructions
{
    public class Pitch : TreeCrayonInstruction
    {
        private RuleSystem.SystemVariables vars;

        private int direction;

        public float Variation
        {
            get { return MathHelper.ToRadians(vars.pitchVariation); }
        }


        public float Angle
        {
            get { return MathHelper.ToRadians(vars.pitchAngle) * direction; }
        }

        public Pitch(RuleSystem.SystemVariables inVars, int direction)
        {
            this.vars = inVars;
            this.direction = direction;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.Pitch(Util.Random(rnd, Angle, Variation));
        }

        #endregion
    }
}
