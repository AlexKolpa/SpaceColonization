using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LTreesLibrary.Trees;
using Microsoft.Xna.Framework.Graphics;

namespace LTreesLibrary
{
    class RuleSystemProfiles
    {
        private TreeProfile pine;
        private TreeProfile birch;
        private TreeProfile palm;
        private TreeProfile willow;

        public TreeProfile Pine
        {
            get { return pine; }
        }

        public TreeProfile Birch
        {
            get { return birch; }
        }

        public TreeProfile Palm
        {
            get { return palm; }
        }

        public TreeProfile Willow
        {
            get { return willow; }
        }

        public RuleSystemProfiles(GraphicsDevice device, Texture2D[] barkTextures, Texture2D[] leafTextures, Effect trunkEffect, Effect leafEffect)
        {
            buildPine(device, barkTextures[0], leafTextures[0], trunkEffect, leafEffect);
            buildBirch(device, barkTextures[1], leafTextures[1], trunkEffect, leafEffect);
            buildPalm(device, barkTextures[2], leafTextures[2], trunkEffect, leafEffect);
            buildWillow(device, barkTextures[2], leafTextures[3], trunkEffect, leafEffect);
        }

        private void buildPine(GraphicsDevice device, Texture2D barkTexture, Texture2D leafTexture, Effect trunkEffect, Effect leafEffect)
        {            
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", ">+f-ff-fA");
            ruleMap.Add("A", "fb++B>+++fB>---fB2{E}");
            ruleMap.Add("B", "[>[!+++C]<[!--C]]");
            ruleMap.Add("C", "f+Dl");
            ruleMap.Add("D", "f+[>--C][<++C]Dl");
            ruleMap.Add("E", "fl");

            RuleSystem.SystemVariables TreeVariables = new RuleSystem.SystemVariables();
            TreeVariables.boneLevels = 1;
            TreeVariables.iterations = 4;
            TreeVariables.twistAngle = 10;
            TreeVariables.twistVariation = 5;
            TreeVariables.branchLength = 300f;
            TreeVariables.lengthVariation = 50f;
            TreeVariables.branchScale = 0.8f;
            TreeVariables.pitchAngle = 0f;
            TreeVariables.pitchVariation = 5f;
            TreeVariables.branchWidth = 128f;
            TreeVariables.backwardLength = 128f;

            RuleSystem rules = new RuleSystem(ruleMap, TreeVariables, "R");

            pine = new TreeProfile(device, TreeGenerator.ParseFromRuleSystem(rules), barkTexture, leafTexture, trunkEffect, leafEffect, rules);
        }

        private void buildBirch(GraphicsDevice device, Texture2D barkTexture, Texture2D leafTexture, Effect trunkEffect, Effect leafEffect)
        {
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", "ffffA");
            ruleMap.Add("A", ">+fbB<+fB>-fB1{D}");
            ruleMap.Add("B", "[<[!-C]>[!+C]]");
            ruleMap.Add("B", "[>[!+C]<[!-C]>[!-C]]");
            ruleMap.Add("C", "#f+f+D");
            ruleMap.Add("D", "<[+C]>[+C]l");


            RuleSystem.SystemVariables TreeVariables = new RuleSystem.SystemVariables();
            TreeVariables.boneLevels = 1;
            TreeVariables.iterations = 4;
            TreeVariables.twistAngle = 30;
            TreeVariables.twistVariation = 5f;
            TreeVariables.branchLength = 260f;
            TreeVariables.lengthVariation = 30f;
            TreeVariables.branchScale = 0.8f;
            TreeVariables.pitchAngle = 20f;
            TreeVariables.pitchVariation = 5f;
            TreeVariables.branchWidth = 128f;
            TreeVariables.backwardLength = 128f;

            RuleSystem rules = new RuleSystem(ruleMap, TreeVariables, "R");

            birch = new TreeProfile(device, TreeGenerator.ParseFromRuleSystem(rules), barkTexture, leafTexture, trunkEffect, leafEffect,rules);
        }

        private void buildPalm(GraphicsDevice device, Texture2D barkTexture, Texture2D leafTexture, Effect trunkEffect, Effect leafEffect)
        {
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", ">+f+f+f+f+A");
            ruleMap.Add("A", ">+fbB>+fB>+fB1{C}");
            ruleMap.Add("C", "fl");
            ruleMap.Add("B", "[>[!+D]>[!+D]]");
            ruleMap.Add("B", "[>[!+D]>[!+D]>[!+D]]");
            ruleMap.Add("D", "f+El");
            ruleMap.Add("E", "f+[>+D][>+D]El");

            RuleSystem.SystemVariables TreeVariables = new RuleSystem.SystemVariables();
            TreeVariables.boneLevels = 1;
            TreeVariables.iterations = 4;
            TreeVariables.twistAngle = 0;
            TreeVariables.twistVariation = 360f;
            TreeVariables.branchLength = 260f;
            TreeVariables.lengthVariation = 50f;
            TreeVariables.branchScale = 0.8f;
            TreeVariables.pitchAngle = 20f;
            TreeVariables.pitchVariation = 50f;
            TreeVariables.branchWidth = 128f;

            RuleSystem rules = new RuleSystem(ruleMap, TreeVariables, "R");

            palm = new TreeProfile(device, TreeGenerator.ParseFromRuleSystem(rules), barkTexture, leafTexture, trunkEffect, leafEffect,rules);
        }

        private void buildWillow(GraphicsDevice device, Texture2D barkTexture, Texture2D leafTexture, Effect trunkEffect, Effect leafEffect)
        {
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", ">+A");
            ruleMap.Add("A", "f+f+bf+f+f+f+f+bB");
            ruleMap.Add("B", "f+f+f+f+bCBl");
            ruleMap.Add("C", "[>[+E]>[+E]]");
            ruleMap.Add("C", "[>[+E]>[+E]>[+E]]");
            ruleMap.Add("E", "#f+f+f+f+bCl");        

            RuleSystem.SystemVariables TreeVariables = new RuleSystem.SystemVariables();
            TreeVariables.boneLevels = 1;
            TreeVariables.iterations = 4;
            TreeVariables.twistAngle = 0;
            TreeVariables.twistVariation = 360f;
            TreeVariables.branchLength = 260f;
            TreeVariables.lengthVariation = 50f;
            TreeVariables.branchScale = 0.8f;
            TreeVariables.pitchAngle = 20f;
            TreeVariables.pitchVariation = 50f;
            TreeVariables.branchWidth = 128f;

            RuleSystem rules = new RuleSystem(ruleMap, TreeVariables, "R");

            willow = new TreeProfile(device, TreeGenerator.ParseFromRuleSystem(rules), barkTexture, leafTexture, trunkEffect, leafEffect,rules);
        }
    }
}
