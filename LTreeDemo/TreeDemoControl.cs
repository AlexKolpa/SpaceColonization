using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using LTreesLibrary.Trees;
using System.Drawing.Drawing2D;
using System.IO;
using LTreeDemo;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using LTreesLibrary.Trees.Wind;

namespace LTreeDemo
{
    public class TreeDemoControl : GraphicsDeviceControl
    {
        public const float MaxCamDistance = 1000f;        

        private ContentManager content;
        private int seed;
        private int profileIndex;
        private SimpleTree tree;
        private Matrix treeWorld = Matrix.Identity;
        private Stopwatch timer;
        private int lastDraw = 0;
        private TreeWindAnimator animator;
        private WindStrengthSin wind;
        private FpsUtil fps = new FpsUtil();
        private Quad groundPlane;
        private BasicEffect groundEffect;
        private Matrix groundWorld = Matrix.CreateRotationX(-MathHelper.PiOver2);
        private RuleSystem rules;
        private Sky skybox;

        public RuleSystem Rules 
        { 
            get {
                return rules;
            }  
            set { 
                rules = value; 
                if(Initialized)
                    RebuildSystem(); 
            } 
        }        
        public RuleSystem.SystemVariables TreeVariables { get; private set; }

        public float CameraOrbitAngle { get; set; }
        public float CameraPitchAngle { get; set; }
        public float CameraDistance { get; set; }
        public float CameraHeight { get; set; }

        public bool Initialized { get; private set; }

        /// <summary>
        /// Names of each profile.
        /// </summary>
        public List<String> ProfileNames { get; private set; }

        /// <summary>
        /// List of available tree profiles.
        /// </summary>
        public List<TreeProfile> Profiles { get; private set; }
        
        /// <summary>
        /// Index of the currently displayed tree profile. Setting this will regenerate the tree.
        /// </summary>
        public int ProfileIndex
        {
            get { return profileIndex; }
            set { profileIndex = value; UpdateTree(); }
        }

        /// <summary>
        /// The currently displayed tree profile.
        /// </summary>
        public TreeProfile CurrentProfile
        {
            get { return Profiles[profileIndex]; }
        }

        public Camera Camera { get; private set; }

        /// <summary>
        /// Seed used to generate trees. Setting this will regenerate the tree.
        /// </summary>
        public int Seed 
        {
            get { return seed; }
            set { seed = value; UpdateTree(); }
        }

        public SimpleTree Tree
        {
            get { return tree; }
        }

        public float CurrentFramesPerSecond
        {
            get { return fps.Fps; }
        }

        public bool EnableTrunk { get; set; }
        public bool EnableLeaves { get; set; }
        public bool EnableBones { get; set; }
        public bool EnableLight1 { get; set; }
        public bool EnableLight2 { get; set; }
        public bool EnableWind { get; set; }
        public bool EnableGround { get; set; }

        [DisplayName("TreeUpdated")]
        public event EventHandler TreeUpdated;

        public Color BackgroundColor { get; set; }
        
        public void UpdateTree()
        {
            if (!Initialized)
                return;

            tree = CurrentProfile.GenerateSimpleTree(new Random(seed));

            if (TreeUpdated != null)
                TreeUpdated(this, EventArgs.Empty);
        }

        public TreeDemoControl()
        {
            CameraHeight = 2000;
            BackgroundColor = Color.CornflowerBlue;
            Profiles = new List<TreeProfile>();
            ProfileNames = new List<string>();
            timer = new Stopwatch();
            timer.Start();
        }

        public const int FramesPerSecond = 60;

        void Application_Idle(object sender, EventArgs e)
        {
            Invalidate();
        }

		private void ApplyStateBlock()
		{
			GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
			GraphicsDevice.BlendState = BlendState.Opaque;
			GraphicsDevice.DepthStencilState = DepthStencilState.Default;
			GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
		}
        
        protected override void Initialize()
        {
            content = new ContentManager(Services, "Content");

            InitRuleSystem();

            // Load a single profile
            Profiles.Add(new TreeProfile(GraphicsDevice,
                TreeGenerator.ParseFromRuleSystem(Rules),
                content.Load<Texture2D>("Textures\\BirchBark"),
                content.Load<Texture2D>("Textures\\BirchLeaf"),
                content.Load<Effect>("LTreeShaders\\Trunk"),
                content.Load<Effect>("LTreeShaders\\Leaves")));
            ProfileNames.Add("Birch");

            profileIndex = 0;

            // Create the wind animator
            wind = new WindStrengthSin();
            animator = new TreeWindAnimator(wind);

            // Create the ground plane and an effect for it
            groundPlane = new Quad(GraphicsDevice, 10000, 10000);
            groundEffect = new BasicEffect(GraphicsDevice);
            groundEffect.Texture = content.Load<Texture2D>("Textures/Grass");
            groundEffect.TextureEnabled = true;

            // Create a camera
            Camera = new Camera();
            Camera.Position = new Vector3(4000, 4000, 4000);
            Camera.Target = new Vector3(0, 2000, 0);
            Camera.AspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;

            skybox = content.Load<Sky>("Skybox\\duskcloudy");

            CameraOrbitAngle = 0.0f;
            CameraPitchAngle = -10.0f;
            CameraDistance = 5000.0f;

            // Enable mipmaps
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            // Store the initial renderstate
            //block = new StateBlock(GraphicsDevice);
            //block.Capture();

            Initialized = true;

            UpdateTree();

            Application.Idle += new EventHandler(Application_Idle);
        }

        void InitRuleSystem()
        {
            MultiMap<string, string> ruleMap = new MultiMap<string, string>();

            ruleMap.Add("R", "fffffbA");
            ruleMap.Add("A", "ff[>++Al][--Al]>>>A");

            TreeVariables = new RuleSystem.SystemVariables();
            TreeVariables.boneLevels = 3;
            TreeVariables.iterations = 5;
            TreeVariables.twistAngle = 0;
            TreeVariables.twistVariation = 360;
            TreeVariables.branchLength = 260;
            TreeVariables.lengthVariation = 0;
            TreeVariables.branchScale = 0.8f;
            TreeVariables.pitchAngle = 20;
            TreeVariables.pitchVariation = 0;

            rules = new RuleSystem(ruleMap, TreeVariables, "R");
        }

        protected override bool Draw()
        {
            int deltaMs = (int)(timer.Elapsed.TotalMilliseconds - lastDraw);
            if (deltaMs * FramesPerSecond < 1000)
            {
                return false;
            }

            GameTime gameTime = new GameTime(TimeSpan.FromMilliseconds(deltaMs), TimeSpan.FromMilliseconds(deltaMs));

            fps.NewFrame(gameTime);

            lastDraw = (int)timer.Elapsed.TotalMilliseconds;

            Camera.Target = new Vector3(0, CameraHeight, 0);
            Camera.SetThirdPersonView(CameraOrbitAngle, CameraPitchAngle, CameraDistance);

            GraphicsDevice.Clear(BackgroundColor);

            // Draw the skybox
            skybox.Draw(Camera.View, Camera.Projection);

            // Draw the ground
            if (EnableGround)
            {
				ApplyStateBlock();
                groundEffect.World = groundWorld;
                groundEffect.View = Camera.View;
                groundEffect.Projection = Camera.Projection;
                groundEffect.DirectionalLight0.Enabled = EnableLight1;
                groundEffect.DirectionalLight1.Enabled = EnableLight2;
                groundPlane.Draw(groundEffect);
            }

            tree.TrunkEffect.Parameters["DirLight0Enabled"].SetValue(EnableLight1);
            tree.LeafEffect.Parameters["DirLight0Enabled"].SetValue(EnableLight1);
            tree.TrunkEffect.Parameters["DirLight1Enabled"].SetValue(EnableLight2);
            tree.LeafEffect.Parameters["DirLight1Enabled"].SetValue(EnableLight2);
            
            if (EnableWind)
            {
                wind.Update(gameTime);
                animator.Animate(tree.Skeleton, tree.AnimationState, gameTime);
            }

            if (EnableTrunk)
            {
				ApplyStateBlock(); // quick way to clean up the mess left in the render states
                tree.DrawTrunk(treeWorld, Camera.View, Camera.Projection);
            }

            if (EnableLeaves)
            {
				ApplyStateBlock();
                tree.DrawLeaves(treeWorld, Camera.View, Camera.Projection);
            }

            if (EnableBones)
            {
				ApplyStateBlock();
                GraphicsDevice.DepthStencilState = DepthStencilState.None;
                tree.DrawBonesAsLines(treeWorld, Camera.View, Camera.Projection);
            }

			return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (content != null)
                {
                    content.Unload();
                }
            }

            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // This works when expanding the size, but when contracting it does not work too 
            // well because the backbuffer remains the same size while the viewport shrinks.
            // To work for contracting, we would need to build a better projection matrix (I think).
            // Resizing of the MainForm is disabled because of this.
            if (Camera != null)
                Camera.AspectRatio = GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height;
        }

        System.Drawing.Point dragPoint;

        bool leftMouseButton = false;
        bool rightMouseButton = false;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            int dx = e.X - dragPoint.X;
            int dy = e.Y - dragPoint.Y;

            dragPoint = e.Location;

            if (leftMouseButton && !rightMouseButton)
            {
                CameraOrbitAngle += dx * MathHelper.Pi / 300.0f;
                CameraPitchAngle -= dy * MathHelper.Pi / 300.0f;
            }
            if (rightMouseButton && !leftMouseButton)
            {
                CameraHeight += dy * 5.0f;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // Focus the control so we may receive mouse wheel events
            this.Focus();

            // Start rotating the camera if the left mouse button was pressed.
            // If the mouse leaves the control's area we will still get the corresponding
            // mouse move and mouse up events until the button is lifted.
            if (e.Button == MouseButtons.Left)
            {
                leftMouseButton = true;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseButton = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            // Stop rotating the camera when the left mouse button is lifted
            if (e.Button == MouseButtons.Left)
            {
                leftMouseButton = false;
            }
            if (e.Button == MouseButtons.Right)
            {
                rightMouseButton = false;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // Zooming exponentially is much nicer than linearly
            // One 'notch' on the mouse wheel seems to be 120.0f in e.Delta
            CameraDistance *= (float)Math.Exp(-e.Delta / 120.0f / 10.0f);
        }

        public void bonelevels_valueChanged(NumericUpDown send)
        {
            TreeVariables.boneLevels = (int)send.Value;
            RebuildSystem();
        }

        public void iterations_valueChanged(NumericUpDown send)
        {
            TreeVariables.iterations = (int)send.Value;
            RebuildSystem();
        }

        public void twistangle_valueChanged(NumericUpDown send)
        {
            TreeVariables.twistAngle = (float)send.Value;
            RecalculateTree();
        }

        public void pitchangle_valueChanged(NumericUpDown send)
        {
            TreeVariables.pitchAngle = (float)send.Value;
            RecalculateTree();
        }

        public void branchlength_valueChanged(NumericUpDown send)
        {
            TreeVariables.branchLength = (float)send.Value;
            RecalculateTree();
        }

        public void branchscale_valueChanged(NumericUpDown send)
        {
            TreeVariables.branchScale = (float)send.Value;
            RecalculateTree();
        }

        void RecalculateTree()
        {
            Profiles[0].RecalculateSimpleTree(Tree);
            if (TreeUpdated != null)
                TreeUpdated(this, EventArgs.Empty);
        }

        void RebuildSystem()
        {
            Profiles[0].Generator = TreeGenerator.ParseFromRuleSystem(Rules);
            Profiles[0].RecalculateSimpleTree(tree);
            if (TreeUpdated != null)
                TreeUpdated(this, EventArgs.Empty);
        }
    }
}
