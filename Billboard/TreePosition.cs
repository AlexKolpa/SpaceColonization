using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Billboard
{
    public class TreePosition
    {
        IList<Vector3> trees;
        public IList<Vector3> Trees
        {
            get { return trees; }
        }

        public TreePosition(IList<Vector3> treePos)
        {
            trees = treePos;
        }
    }

    public class TreePositionReader : ContentTypeReader<TreePosition>
    {
        protected override TreePosition Read(ContentReader input, TreePosition existingInstance)
        {
            int size = input.ReadInt32();

            IList<Vector3> trees = new List<Vector3>();

            for (int i = 0; i < size; i++)
            {
                trees.Add(input.ReadVector3());
            }

            return new TreePosition(trees);
        } 
    }
}
