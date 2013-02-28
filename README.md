Bachelorseminarium
==================

This project contains an XNA solution to generate procedurally modelled trees in real-time.

To use this project, you will need Visual Studio 2010 and XNA GameStudio 4.0.

You should open the solution by opening the Billboard(Windows).sln file. This will load the solution into Visual studio.
In Visual Studio, there will be five projects. First is the BillboardPipeline project. This is used to build everything in the world besides the trees.
Next is the Billboard project. From this project, the game is controlled. The DebugPipeline has been added to make it easier to debug the XNA pipeline.
Finally, there is the Ltrees library. This consists of two projects, one which manages the run-time code of the trees, and the Pipeline, which constructs the compiled bytecode for the tree profiles.

To run the project, make sure the LTrees library has been built first, before you run the Billboard project. Then you can run it, and navigate through the demo.