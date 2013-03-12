using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace LTreesLibrary.Trees.Instructions
{
    unsafe public class Twist : TreeCrayonInstruction
    {
        private RuleSystem.SystemVariables vars;
        private int direction;

        public float Variation
        {
            get { return MathHelper.ToRadians(vars.twistVariation); }
        }


        public float Angle
        {
            get { return MathHelper.ToRadians(vars.twistAngle)*direction; }
        }

        public Twist(RuleSystem.SystemVariables inVars, int direction)
        {
            this.vars = inVars;
            this.direction = direction;
        }

        #region TreeCrayonInstruction Members

        public void Execute(TreeCrayon crayon, Random rnd)
        {
            crayon.Twist(Util.Random(rnd, Angle, Variation));
        }

        #endregion
    }
}
