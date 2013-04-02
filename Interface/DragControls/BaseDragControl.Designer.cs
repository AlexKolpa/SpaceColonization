namespace Interface.DragControls
{
    partial class BaseDragControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ProductionRule = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ProductionRule
            // 
            this.ProductionRule.AutoSize = true;
            this.ProductionRule.BackColor = System.Drawing.Color.Transparent;
            this.ProductionRule.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F);
            this.ProductionRule.Location = new System.Drawing.Point(3, 11);
            this.ProductionRule.Name = "ProductionRule";
            this.ProductionRule.Size = new System.Drawing.Size(57, 26);
            this.ProductionRule.TabIndex = 0;
            this.ProductionRule.Text = "Rule";
            // 
            // BaseDragControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::Interface.Properties.Resources.SimpleDrag;
            this.Controls.Add(this.ProductionRule);
            this.Name = "BaseDragControl";
            this.Size = new System.Drawing.Size(250, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ProductionRule;
    }
}
