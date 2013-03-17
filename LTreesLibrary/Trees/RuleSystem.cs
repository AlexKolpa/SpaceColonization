using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LTreesLibrary.Trees
{
    public class RuleSystem
    {
        private static HashSet<char> excludedChars = new HashSet<char>(new[] { ' ', '\t', '\n', '\r' });
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

        public class SystemVariables
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

            public float backwardLength;
            public float backwardVariation;
        }

        public static RuleSystem ParseRuleSystemFromString(String rules, SystemVariables inVars, String inRoot)
        {
            String[] splitRules = rules.Trim().Split('\n');
            
            MultiMap<string, string> ruleSet = new MultiMap<string, string>();


            foreach (string rule in splitRules)
            {                
                String[] keyValuePair = rule.Split('=');
                if (keyValuePair.Length != 2)
                {
                    throw new ArgumentException("Rule was not constructed properly");
                }
                string key = keyValuePair[0].Trim();
                if (!isLegitimateKey(key))
                {
                    throw new ArgumentException("Keys can only be unicode letters");
                }

                string value = keyValuePair[1].Trim();
                if (!isLegitimateValue(value))
                {
                    throw new ArgumentException("Values cant contain spaces");
                }

                //TODO: handle unknown call chars

                ruleSet.Add(key, value);
            }

            return new RuleSystem(ruleSet, inVars, inRoot);
        }

        private static bool isLegitimateKey(string inKey)
        {
            if (inKey.Length > 1)
                return false;

            return Char.IsLetter(inKey, 0);
        }

        private static bool isLegitimateValue(string inValue)
        {
            for (int i = 0; i < inValue.Length; i++)
            {
                if (excludedChars.Contains(inValue[i]))
                    return false;
            }

            return true;
        }
    }
}
