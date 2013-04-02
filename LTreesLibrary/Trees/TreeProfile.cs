using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace LTreesLibrary.Trees
{
    /// <summary>
    /// Contains a tree generator and textures and effects to render the associated trees.
    /// </summary>
    public class TreeProfile
    {
        public GraphicsDevice GraphicsDevice { get; private set; }
        public TreeGenerator Generator { get; set; }
        public Texture2D TrunkTexture { get; set; }
        public Texture2D LeafTexture { get; set; }
        public Effect TrunkEffect { get; set; }
        public Effect LeafEffect { get; set; }
        public RuleSystem Rules { get; set; }

        private Random defaultRandom = new Random(123);

        public TreeProfile(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }
        public TreeProfile(GraphicsDevice device, TreeGenerator generator, Texture2D trunkTexture, Texture2D leafTexture, Effect trunkEffect, Effect leafEffect, RuleSystem rs)
        {
            GraphicsDevice = device;
            Generator = generator;
            TrunkTexture = trunkTexture;
            LeafTexture = leafTexture;
            TrunkEffect = trunkEffect;
            LeafEffect = leafEffect;
            Rules = rs;
        }
        
        public SimpleTree GenerateSimpleTree(Random random)
        {
            int seed = random.Next();
            SimpleTree tree = new SimpleTree(GraphicsDevice, Generator.GenerateTree(new Random(seed)));
            tree.Skeleton.RandomSeed = seed;
            tree.TrunkTexture = TrunkTexture;
            tree.LeafTexture = LeafTexture;
            tree.TrunkEffect = TrunkEffect;
            tree.LeafEffect = LeafEffect;
            return tree;
        }
        public SimpleTree GenerateSimpleTree()
        {
            return GenerateSimpleTree(defaultRandom);
        }

        public void RebuildSimpleTree(SimpleTree tree)
        {
            Generator = TreeGenerator.ParseFromRuleSystem(Rules);

            RecalculateSimpleTree(tree);
        }

        public void RecalculateSimpleTree(SimpleTree tree)
        {
            tree.Skeleton = Generator.GenerateTree(new Random(tree.Skeleton.RandomSeed));
            tree.UpdateSkeleton();
        }

    }
}
