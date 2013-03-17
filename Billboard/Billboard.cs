#region File Description
//-----------------------------------------------------------------------------
// Billboard.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LTreesLibrary.Trees;
using LTreesLibrary.Trees.Wind;
using System.Collections.Generic;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Point = System.Drawing.Point;
#endregion

namespace Billboard
{
    /// <summary>
    /// Sample showing how to efficiently render billboard sprites.
    /// </summary>
    public class BillboardGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        GraphicsDeviceManager graphics;
        
        KeyboardState currentKeyboardState = new KeyboardState();
        GamePadState currentGamePadState = new GamePadState();

        bool mousePressed = false;
        Vector2 deltaMousePoint = new Vector2();
        Vector2 prevMousePos = new Vector2();

        Vector3 cameraPosition = new Vector3(0, 50, 50);
        Vector3 cameraFront = new Vector3(0, 0, -1);

        Model landscape;

        TreePosition treePositions;

        TreeProfile[] profiles;
        int currentTree = 0;

        SimpleTree tree;

        TreeWindAnimator animator;
        WindStrengthSin wind;

        TextBox inputRules;
        RuleSystem rules;
        RuleSystem.SystemVariables treeVariables;

        public bool EnableTrunk { get; set; }
        public bool EnableLeaves { get; set; }
        public bool EnableBones { get; set; }
        public bool EnableLight1 { get; set; }
        public bool EnableLight2 { get; set; }
        public bool EnableWind { get; set; }
        public bool EnableGround { get; set; }

        private struct EventValuePair
        {
            public EventHandler eventValue;
            public float value;
            public float maximum;

            public EventValuePair(EventHandler inEventValue, float inValue, float inMaximum)
            {
                this.eventValue = inEventValue;
                this.value = inValue;
                this.maximum = inMaximum;
            }
        }

        #endregion

        #region Initialization

        public BillboardGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //initialize form
            this.InitRuleSystem();
            this.CreateControls();
        }


        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            landscape = Content.Load<Model>("landscape");
            treePositions = landscape.Tag as TreePosition;

            if (treePositions == null)
                treePositions = new TreePosition(new List<Vector3>());

            LoadTreeGenerators();
            NewTree();

            wind = new WindStrengthSin();
            animator = new TreeWindAnimator(wind);
        }

        void LoadTreeGenerators()
        {
            // Here we load all the tree profiles using the content manager
            // The tree profiles contain information about how to generate a tree
            profiles = new TreeProfile[1];

            profiles[0] = new TreeProfile(GraphicsDevice, TreeGenerator.ParseFromRuleSystem(rules),
                Content.Load<Texture2D>("Textures/BirchBark"),
                Content.Load<Texture2D>("Textures/BirchLeaf"),
                Content.Load<Effect>("LTreeShaders/Trunk"),
                Content.Load<Effect>("LTreeShaders/Leaves")
                );
        }

        void InitRuleSystem()
        {
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", "fffffbA");
            ruleMap.Add("A", "ff[>++Al][--Al]>>>A");

            treeVariables = new RuleSystem.SystemVariables();
            treeVariables.boneLevels = 3;
            treeVariables.iterations = 5;
            treeVariables.twistAngle = 0;
            treeVariables.twistVariation = 360;
            treeVariables.branchLength = 260;
            treeVariables.lengthVariation = 0;
            treeVariables.branchScale = 0.8f;
            treeVariables.pitchAngle = 20;
            treeVariables.pitchVariation = 0;
            
            rules = new RuleSystem(ruleMap, treeVariables, "R");
        }

        void NewTree()
        {
            // Generates a new tree using the currently selected tree profile
            // We call TreeProfile.GenerateSimpleTree() which does three things for us:
            // 1. Generates a tree skeleton
            // 2. Creates a mesh for the branches
            // 3. Creates a particle cloud (TreeLeafCloud) for the leaves
            tree = profiles[currentTree].GenerateSimpleTree();
        }

        #endregion

        #region Form Controls

        private void CreateControls()
        {
            Form winForm = Control.FromHandle(this.Window.Handle) as Form;            

            Panel pnl = new Panel();
            pnl.Location = new Point(0, 0);
            pnl.Dock = DockStyle.Left;
            pnl.Size = new System.Drawing.Size(200, 500);

            CheckBox drawLeaves = new CheckBox();
            drawLeaves.Location = new Point(10, 10);
            drawLeaves.Text = "Display Leaves";
            drawLeaves.Checked = EnableLeaves;
            drawLeaves.CheckedChanged += new EventHandler(drawLeaves_CheckedChanged);

            pnl.Controls.Add(drawLeaves);

            pnl.Controls.AddRange(new Control[]{
                createInputForm(new Point(10, 40), 0, 0, 7, treeVariables.boneLevels, 1, new EventHandler(boneLevels_ValueChanged)), //amount of bones
                createInputForm(new Point(10, 70), 0, 0, 10, treeVariables.iterations, 1, new EventHandler(iterations_ValueChanged)), //number of iterations
                createInputForm(new Point(10, 100), 0, 0, 360, treeVariables.twistAngle, 1, new EventHandler(twistAngle_ValueChanged)), //angle of twisting per instruction
                createInputForm(new Point(10, 130), 0, 0, 400, treeVariables.branchLength, 5, new EventHandler(branchLength_ValueChanged)), //branch length per instruction
                createInputForm(new Point(10, 160), 2, 0, 2, treeVariables.branchScale, 0.05f, new EventHandler(branchScale_ValueChanged)), //scale of branch radius
                createInputForm(new Point(10, 190), 0, 0, 360, treeVariables.pitchAngle, 1, new EventHandler(pitchAngle_ValueChanged)), //angle of pitching per instruction
            });

            pnl.Controls.AddRange(new Control[]{
                CreateLabel(new Point(70, 40), "Bone Levels"),
                CreateLabel(new Point(70, 70), "Iteration"),
                CreateLabel(new Point(70, 100), "Twist Angle"),
                CreateLabel(new Point(70, 130), "Branch Length"),
                CreateLabel(new Point(70, 160), "Branch Scale"),
                CreateLabel(new Point(70, 190), "Pitch Angle"),                
            });

            inputRules = new TextBox();
            inputRules.Multiline = true;
            inputRules.Location = new Point(10, 300);
            inputRules.Size = new System.Drawing.Size(180, 130);
            inputRules.ScrollBars = ScrollBars.Both;
            inputRules.AcceptsReturn = true;
            inputRules.WordWrap = false;
            inputRules.AcceptsTab = false;

            UpdateRulesInputBox();

            Button runRulesButton = new Button();
            runRulesButton.Location = new Point(10, 450);
            runRulesButton.Text = "Build Rules";
            runRulesButton.Click += new EventHandler(build_rules_clicked);
            pnl.Controls.Add(inputRules);

            winForm.Controls.Add(pnl);
        }

        void UpdateRulesInputBox()
        {
            String rulesString = "";

            MultiMap<String, String> map = rules.Rules;

            foreach (String key in map.Keys)
            {
                foreach (String value in map[key])
                {
                    rulesString += key;

                    rulesString += " -> ";

                    rulesString += value;

                    rulesString += "\n";
                }
            }

            inputRules.Text = rulesString;
        }

        NumericUpDown createInputForm(Point pos, int decimalPlaces, int minimum, int maximum, float currentValue, float increment, EventHandler e)
        {
            NumericUpDown inUpDown = new NumericUpDown();
            inUpDown.Location = pos;

            inUpDown.Maximum = (decimal)maximum;
            inUpDown.Minimum = (decimal)minimum;
            inUpDown.Value = (decimal)currentValue;
            inUpDown.DecimalPlaces = decimalPlaces;
            inUpDown.Increment = (decimal)increment;
            inUpDown.ReadOnly = true;
            inUpDown.Width = 60;
            inUpDown.ValueChanged += e;

            return inUpDown;
        }

        Label CreateLabel(Point pos, String name)
        {
            Label inputLabel = new Label();
            inputLabel.Text = name;
            inputLabel.Location = pos;

            return inputLabel;
        }

        void build_rules_clicked(Object sender, EventArgs e)
        {

        }

        void boneLevels_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.boneLevels = (int)numSend.Value;
            RebuildSystem();
        }

        void iterations_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.iterations = (int)numSend.Value;
            RebuildSystem();
        }

        void twistAngle_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.twistAngle = (float)numSend.Value;
            RecalculateTree();
        }

        void branchLength_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.branchLength = (float)numSend.Value;
            RecalculateTree();
        }

        void branchScale_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.branchScale = (float)numSend.Value;
            RecalculateTree();
        }

        void pitchAngle_ValueChanged(Object sender, EventArgs e)
        {
            NumericUpDown numSend = (NumericUpDown)sender;
            treeVariables.pitchAngle = (float)numSend.Value;
            RecalculateTree();
        }

        void drawLeaves_CheckedChanged(Object sender, EventArgs e)
        {
            EnableLeaves = !EnableLeaves;
        }

        void ParseNewRules(String root, TextBox box)
        {
        }

        void RebuildSystem()
        {
            profiles[0].Generator = TreeGenerator.ParseFromRuleSystem(rules);
            profiles[0].RecalculateSimpleTree(tree);
        }

        void RecalculateTree()
        {
            profiles[0].RecalculateSimpleTree(tree);
        }

        #endregion

        #region Update and Draw


        /// <summary>
        /// Allows the game to run logic.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            UpdateCamera(gameTime);

            wind.Update(gameTime);
            animator.Animate(tree.Skeleton, tree.AnimationState, gameTime);

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = graphics.GraphicsDevice;

            device.Clear(Color.CornflowerBlue);

            // Compute camera matrices.
            Matrix view = Matrix.CreateLookAt(cameraPosition,
                                              cameraPosition + cameraFront,
                                              Vector3.Up);

            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                                                                    device.Viewport.AspectRatio,
                                                                    1, 10000);

            Vector3 lightDirection = Vector3.Normalize(new Vector3(3, -1, 1));
            Vector3 lightColor = new Vector3(0.3f, 0.4f, 0.2f);

            // Time is scaled down to make things wave in the wind more slowly.
            float time = (float)gameTime.TotalGameTime.TotalSeconds * 0.333f;

            // First we draw the ground geometry using BasicEffect.
            foreach (ModelMesh mesh in landscape.Meshes)
            {
                if (mesh.Name != "Billboards")
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.View = view;
                        effect.Projection = projection;

                        effect.LightingEnabled = true;

                        effect.DirectionalLight0.Enabled = true;
                        effect.DirectionalLight0.Direction = lightDirection;
                        effect.DirectionalLight0.DiffuseColor = lightColor;

                        effect.AmbientLightColor = new Vector3(0.1f, 0.2f, 0.1f);
                    }

                    device.BlendState = BlendState.Opaque;
                    device.DepthStencilState = DepthStencilState.Default;
                    device.RasterizerState = RasterizerState.CullCounterClockwise;

                    mesh.Draw();
                }
            }

            // Then we use a two-pass technique to render alpha blended billboards with
            // almost-correct depth sorting. The only way to make blending truly proper for
            // alpha objects is to draw everything in sorted order, but manually sorting all
            // our billboards would be very expensive. Instead, we draw in two passes.
            //
            // The first pass has alpha blending turned off, alpha testing set to only accept
            // ~95% or more opaque pixels, and the depth buffer turned on. Because this is only
            // rendering the solid parts of each billboard, the depth buffer works as
            // normal to give correct sorting, but obviously only part of each billboard will
            // be rendered.
            //
            // Then in the second pass we enable alpha blending, set alpha test to only accept
            // pixels with fractional alpha values, and set the depth buffer to test against
            // the existing data but not to write new depth values. This means the translucent
            // areas of each billboard will be sorted correctly against the depth buffer
            // information that was previously written while drawing the opaque parts, although
            // there can still be sorting errors between the translucent areas of different
            // billboards.
            //
            // In practice, sorting errors between translucent pixels tend not to be too
            // noticable as long as the opaque pixels are sorted correctly, so this technique
            // often looks ok, and is much faster than trying to sort everything 100%
            // correctly. It is particularly effective for organic textures like grass and
            // trees.
            foreach (ModelMesh mesh in landscape.Meshes)
            {
                if (mesh.Name == "Billboards")
                {
                    // First pass renders opaque pixels.
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["LightDirection"].SetValue(lightDirection);
                        effect.Parameters["WindTime"].SetValue(time);
                        effect.Parameters["AlphaTestDirection"].SetValue(1f);
                    }

                    device.BlendState = BlendState.Opaque;
                    device.DepthStencilState = DepthStencilState.Default;
                    device.RasterizerState = RasterizerState.CullNone;
                    device.SamplerStates[0] = SamplerState.LinearClamp;

                    mesh.Draw();

                    // Second pass renders the alpha blended fringe pixels.
                    foreach (Effect effect in mesh.Effects)
                    {
                        effect.Parameters["AlphaTestDirection"].SetValue(-1f);
                    }

                    device.BlendState = BlendState.NonPremultiplied;
                    device.DepthStencilState = DepthStencilState.DepthRead;

                    mesh.Draw();
                }
            }

            device.DepthStencilState = DepthStencilState.Default;
            device.BlendState = BlendState.AlphaBlend;

            IList<Vector3> treePos = treePositions.Trees;
            //Console.WriteLine(treePos.Count);
            foreach (Vector3 position in treePos)
            {
                tree.DrawTrunk(Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(position), view, projection);
            }

            if (EnableLeaves)
            {
                foreach (Vector3 position in treePos)
                {
                    tree.DrawLeaves(Matrix.CreateScale(0.02f) * Matrix.CreateTranslation(position), view, projection);
                }
            }

            base.Draw(gameTime);
        }

        #endregion

        #region Handle Input


        /// <summary>
        /// Handles input for quitting the game.
        /// </summary>
        private void HandleInput()
        {            
            currentKeyboardState = Keyboard.GetState();
            currentGamePadState = GamePad.GetState(PlayerIndex.One);

            // Check for exit.
            if (currentKeyboardState.IsKeyDown(Keys.Escape) ||
                currentGamePadState.Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }
        }


        /// <summary>
        /// Handles camera input.
        /// </summary>
        private void UpdateCamera(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                if (!mousePressed)
                {
                    prevMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    mousePressed = true;
                }
                else
                {
                    Vector2 newMousePos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
                    deltaMousePoint = newMousePos - prevMousePos;
                }
            }
            else if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                deltaMousePoint = Vector2.Zero;
                mousePressed = false;
            }

            // Check for input to rotate the camera.
            float pitch = -currentGamePadState.ThumbSticks.Right.Y * time * 0.001f;
            float turn = -currentGamePadState.ThumbSticks.Right.X * time * 0.001f;

            pitch += deltaMousePoint.Y * 0.001f;

            turn -= deltaMousePoint.X * 0.001f;

            Vector3 cameraRight = Vector3.Cross(Vector3.Up, cameraFront);
            Vector3 flatFront = Vector3.Cross(cameraRight, Vector3.Up);

            Matrix pitchMatrix = Matrix.CreateFromAxisAngle(cameraRight, pitch);
            Matrix turnMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, turn);

            Vector3 tiltedFront = Vector3.TransformNormal(cameraFront, pitchMatrix *
                                                          turnMatrix);

            // Check angle so we cant flip over
            if (Vector3.Dot(tiltedFront, flatFront) > 0.001f)
            {
                cameraFront = Vector3.Normalize(tiltedFront);
            }

            // Check for input to move the camera around.
            if (currentKeyboardState.IsKeyDown(Keys.W))
                cameraPosition += cameraFront * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.S))
                cameraPosition -= cameraFront * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.A))
                cameraPosition += cameraRight * time * 0.1f;

            if (currentKeyboardState.IsKeyDown(Keys.D))
                cameraPosition -= cameraRight * time * 0.1f;

            cameraPosition += cameraFront *
                              currentGamePadState.ThumbSticks.Left.Y * time * 0.1f;

            cameraPosition -= cameraRight *
                              currentGamePadState.ThumbSticks.Left.X * time * 0.1f;

            if (currentGamePadState.Buttons.RightStick == ButtonState.Pressed ||
                currentKeyboardState.IsKeyDown(Keys.R))
            {
                cameraPosition = new Vector3(0, 50, 50);
                cameraFront = new Vector3(0, 0, -1);
            }
        }


        #endregion
    }


    #region Entry Point

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        static void Main()
        {
            using (BillboardGame game = new BillboardGame())
            {
                game.Run();
            }
        }
    }

    #endregion
}
