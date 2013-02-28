using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;

namespace BillboardPipeline
{
    public class TreePositionContent
    {
        IList<Vector3> trees;

        public IList<Vector3> Trees
        {
            get { return trees; }
        }

        public TreePositionContent(IList<Vector3> treePos)
        {
            trees = treePos;
        }
    }

    [ContentTypeWriter]
    public class TreePositionWriter : ContentTypeWriter<TreePositionContent>
    {
        protected override void Write(ContentWriter output, TreePositionContent value)
        {
            IList<Vector3> positions = value.Trees;

            output.Write(positions.Count);

            foreach (Vector3 pos in positions)
                output.Write(pos);
        }

        /// <summary>
        /// Tells the content pipeline what CLR type the
        /// data will be loaded into at runtime.
        /// </summary>
        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return "Billboard.TreePosition, " +
                "Billboard, Version=1.0.0.0, Culture=neutral";
        }


        /// <summary>
        /// Tells the content pipeline what worker type
        /// will be used to load the data.
        /// </summary>
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "Billboard.TreePositionReader, " +
                "Billboard, Version=1.0.0.0, Culture=neutral";
        }
    }
}
