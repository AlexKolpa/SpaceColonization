using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTreesLibrary.Trees
{
    public class RuleSystem
    {
        private MultiMap<string, string> rules;

        public MultiMap<String, String> Rules
        {
            get { return rules; }
            set { rules = value; }
        }

        private String root;

        public String Root
        {
            get { return root; }
            set { root = value; }
        }

        private SystemVariables variables;

        public SystemVariables Variables
        {
            get { return variables; }
            set { variables = value; }
        }

        /// <summary>
        /// Contains a rule system which can be used to construct a L-System.
        /// </summary>
        /// <param name="inRules">The rules defining the L-System</param>
        /// <param name="inVariabels">Variables that determine the appearance of the system</param>
        /// <param name="inRoot">The rule that is ran first</param>
        public RuleSystem(MultiMap<string, string> inRules, SystemVariables inVariabels, String inRoot)
        {
            if (inRules.Map.Count == 0)
                throw new ArgumentException("The Rule System should contain at least 1 rule!");

            rules = inRules;
            variables = inVariabels;
            root = inRoot;
        }

        public struct SystemVariables
        {
            public int iterations;
            public int boneLevels;

            public float pitchAngle;
            public float pitchVariation;

            public float branchLength;
            public float lengthVariation;
            public float branchScale;

            public float twistAngle;
            public float twistVariation;
        }
    }
}
