namespace Pong
{
    partial class PongForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Canvas = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // Canvas
            // 
            this.Canvas.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.Canvas.Location = new System.Drawing.Point(1, 0);
            this.Canvas.Name = "Canvas";
            this.Canvas.Size = new System.Drawing.Size(600, 400);
            this.Canvas.TabIndex = 0;
            this.Canvas.Paint += new System.Windows.Forms.PaintEventHandler(this.Canvas_Paint);
            // 
            // PongForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(599, 400);
            this.Controls.Add(this.Canvas);
            this.Name = "PongForm";
            this.Text = "Pong";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PongForm_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PongForm_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PongForm_KeyUp);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel Canvas;

    }
}

