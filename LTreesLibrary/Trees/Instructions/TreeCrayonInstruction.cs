using System;
using System.Collections.Generic;
using System.Text;

namespace LTreesLibrary.Trees
{
    unsafe public interface TreeCrayonInstruction
    {
        void Execute(TreeCrayon crayon, Random rnd);
    }
}
