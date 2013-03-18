/*
 * Copyright (c) 2007-2009 Asger Feldthaus
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using LTreesLibrary.Trees;
using LTreesLibrary;

namespace LTreeDemo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            tabPage2.Controls.Remove(label15);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (base.DesignMode)
                return;

            xnaControl.TreeUpdated += new EventHandler(xnaControl_OnTreeUpdated);

            for (int i = 0; i < xnaControl.ProfileNames.Count; i++)
            {
                profileBox.Items.Add(xnaControl.ProfileNames[i]);
            }
            profileBox.SelectedIndex = 0;
            UpdateOptions(null, null);
        }

        private void profileBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            xnaControl.ProfileIndex = profileBox.SelectedIndex;
        }

        private void UpdateOptions(object sender, EventArgs e)
        {
            xnaControl.EnableLeaves = leavesBox.Checked;
            xnaControl.EnableLight1 = light1Box.Checked;
            xnaControl.EnableLight2 = light2Box.Checked;
            xnaControl.EnableTrunk = branchesBox.Checked;
            xnaControl.EnableWind = windBox.Checked;
            xnaControl.EnableBones = bonesBox.Checked;
            xnaControl.EnableGround = groundBox.Checked;

            bonelevels.Value = xnaControl.TreeVariables.boneLevels;
            iterations.Value = xnaControl.TreeVariables.iterations;
            branchlength.Value = (decimal)xnaControl.TreeVariables.branchLength;
            branchscale.Value = (decimal)xnaControl.TreeVariables.branchScale;
            twistangle.Value = (decimal)xnaControl.TreeVariables.twistAngle;
            pitchangle.Value = (decimal)xnaControl.TreeVariables.pitchAngle;

            fill_rulesystem();
        }

        private void seedBox_ValueChanged(object sender, EventArgs e)
        {
            xnaControl.Seed = (int)seedBox.Value;
        }

        Random random = new Random();
        private void randomButton_Click(object sender, EventArgs e)
        {
            if (tabPage2.Controls.Contains(label15))
            {
                fill_rulesystem();
                tabPage2.Controls.Remove(label15);
            }
            seedBox.Value = random.Next((int)seedBox.Maximum);
        }

        private void xnaControl_OnTreeUpdated(object sender, EventArgs e)
        {
            verticesLabel.Text = "" + xnaControl.Tree.TrunkMesh.NumberOfVertices;
            polygonsLabel.Text = "" + xnaControl.Tree.TrunkMesh.NumberOfTriangles;
            leavesLabel.Text = "" + xnaControl.Tree.Skeleton.Leaves.Count;
            bonesLabel.Text = "" + xnaControl.Tree.Skeleton.Bones.Count;
        }

        private bool IsImageFileFormatFromFilenamePng(string filename)
        {
            String ext = filename.Substring(filename.LastIndexOf(".")).ToLowerInvariant();
            switch (ext)
            {
                case ".png":
                    return true;
                case ".jpg":
                    return false;
                default:
                    return true; // Just use this as default
            }
        }

        private void bonelevels_valueChanged(object sender, EventArgs e)
        {
            xnaControl.bonelevels_valueChanged((NumericUpDown)sender);
        }

        private void iterations_valueChanged(object sender, EventArgs e)
        {
            xnaControl.iterations_valueChanged((NumericUpDown)sender);
        }

        private void branchlength_valueChanged(object sender, EventArgs e)
        {
            xnaControl.branchlength_valueChanged((NumericUpDown)sender);
        }

        private void branchscale_valueChanged(object sender, EventArgs e)
        {
            xnaControl.branchscale_valueChanged((NumericUpDown)sender);
        }

        private void pitchangle_valueChanged(object sender, EventArgs e)
        {
            xnaControl.pitchangle_valueChanged((NumericUpDown)sender);
        }

        private void twistangle_valueChanged(object sender, EventArgs e)
        {
            xnaControl.twistangle_valueChanged((NumericUpDown)sender);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabPage2.Controls.Remove(label15);
            RuleSystem rules;
            try
            {
                rules = RuleSystem.ParseRuleSystemFromString("R=" + textBox1.Text + "\n" + textBox2.Text, xnaControl.TreeVariables, "R");
            }
            catch (ArgumentException ex)
            {
                tabPage2.Controls.Add(label15);
                return;
            }

            xnaControl.Rules = rules;
        }

        void fill_rulesystem()
        {
            MultiMap<string, string> rules = xnaControl.Rules.Rules;

            //set root rule
            textBox1.Text = rules[xnaControl.Rules.Root][0];

            //build string for rule system
            StringBuilder sb = new StringBuilder();
            foreach (string key in rules.Keys)
            {
                if (key.Equals(xnaControl.Rules.Root))
                    continue;

                foreach (string value in rules[key])
                {
                    sb.Append(key);
                    sb.Append("=");
                    sb.AppendLine(value);
                }
            }

            textBox2.Text = sb.ToString();
        }
    }
}
