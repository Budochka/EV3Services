
namespace EV3UIWF
{
    partial class FrmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.imgCameraView = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnForward = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnBackward = new System.Windows.Forms.Button();
            this.textToSay = new System.Windows.Forms.TextBox();
            this.btnSayIt = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDistance = new System.Windows.Forms.TextBox();
            this.txtTorqueMove = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtDegree = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTorqueRotate = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHeadRight = new System.Windows.Forms.Button();
            this.btnHeadLeft = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtDegreeHead = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTorqueHead = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.imgCameraView)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgCameraView
            // 
            this.imgCameraView.Location = new System.Drawing.Point(14, 76);
            this.imgCameraView.Name = "imgCameraView";
            this.imgCameraView.Size = new System.Drawing.Size(663, 365);
            this.imgCameraView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgCameraView.TabIndex = 0;
            this.imgCameraView.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Text";
            // 
            // btnForward
            // 
            this.btnForward.Location = new System.Drawing.Point(65, 10);
            this.btnForward.Name = "btnForward";
            this.btnForward.Size = new System.Drawing.Size(75, 23);
            this.btnForward.TabIndex = 2;
            this.btnForward.Text = "Forward";
            this.btnForward.UseVisualStyleBackColor = true;
            this.btnForward.Click += new System.EventHandler(this.btnForward_Click);
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(27, 39);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(108, 39);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 4;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnBackward
            // 
            this.btnBackward.Location = new System.Drawing.Point(65, 68);
            this.btnBackward.Name = "btnBackward";
            this.btnBackward.Size = new System.Drawing.Size(75, 23);
            this.btnBackward.TabIndex = 5;
            this.btnBackward.Text = "Backward";
            this.btnBackward.UseVisualStyleBackColor = true;
            this.btnBackward.Click += new System.EventHandler(this.btnBackward_Click);
            // 
            // textToSay
            // 
            this.textToSay.Location = new System.Drawing.Point(70, 37);
            this.textToSay.Name = "textToSay";
            this.textToSay.Size = new System.Drawing.Size(534, 23);
            this.textToSay.TabIndex = 6;
            // 
            // btnSayIt
            // 
            this.btnSayIt.Location = new System.Drawing.Point(602, 37);
            this.btnSayIt.Name = "btnSayIt";
            this.btnSayIt.Size = new System.Drawing.Size(75, 23);
            this.btnSayIt.TabIndex = 7;
            this.btnSayIt.Text = "Say It!";
            this.btnSayIt.UseVisualStyleBackColor = true;
            this.btnSayIt.Click += new System.EventHandler(this.btnSayIt_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRight);
            this.panel1.Controls.Add(this.btnForward);
            this.panel1.Controls.Add(this.btnLeft);
            this.panel1.Controls.Add(this.btnBackward);
            this.panel1.Location = new System.Drawing.Point(709, 290);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 100);
            this.panel1.TabIndex = 8;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(14, 447);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(1129, 180);
            this.txtLog.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Distance:";
            // 
            // txtDistance
            // 
            this.txtDistance.Location = new System.Drawing.Point(76, 23);
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.Size = new System.Drawing.Size(64, 23);
            this.txtDistance.TabIndex = 11;
            this.txtDistance.Text = "10";
            // 
            // txtTorqueMove
            // 
            this.txtTorqueMove.Location = new System.Drawing.Point(76, 52);
            this.txtTorqueMove.Name = "txtTorqueMove";
            this.txtTorqueMove.Size = new System.Drawing.Size(64, 23);
            this.txtTorqueMove.TabIndex = 18;
            this.txtTorqueMove.Text = "100";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 15);
            this.label5.TabIndex = 17;
            this.label5.Text = "Torque:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDistance);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtTorqueMove);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(709, 98);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(197, 90);
            this.groupBox1.TabIndex = 19;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Move";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtDegree);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtTorqueRotate);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(709, 194);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(197, 90);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rotate";
            // 
            // txtDegree
            // 
            this.txtDegree.Location = new System.Drawing.Point(76, 23);
            this.txtDegree.Name = "txtDegree";
            this.txtDegree.Size = new System.Drawing.Size(64, 23);
            this.txtDegree.TabIndex = 11;
            this.txtDegree.Text = "10";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 10;
            this.label3.Text = "Degree:";
            // 
            // txtTorqueRotate
            // 
            this.txtTorqueRotate.Location = new System.Drawing.Point(76, 52);
            this.txtTorqueRotate.Name = "txtTorqueRotate";
            this.txtTorqueRotate.Size = new System.Drawing.Size(64, 23);
            this.txtTorqueRotate.TabIndex = 18;
            this.txtTorqueRotate.Text = "100";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "Torque:";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pluginsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1155, 24);
            this.menuStrip.TabIndex = 21;
            // 
            // pluginsToolStripMenuItem
            // 
            this.pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            this.pluginsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.pluginsToolStripMenuItem.Text = "Plugins";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnHeadRight);
            this.panel2.Controls.Add(this.btnHeadLeft);
            this.panel2.Location = new System.Drawing.Point(935, 290);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(179, 44);
            this.panel2.TabIndex = 9;
            // 
            // btnHeadRight
            // 
            this.btnHeadRight.Location = new System.Drawing.Point(93, 10);
            this.btnHeadRight.Name = "btnHeadRight";
            this.btnHeadRight.Size = new System.Drawing.Size(75, 23);
            this.btnHeadRight.TabIndex = 4;
            this.btnHeadRight.Text = "Right";
            this.btnHeadRight.UseVisualStyleBackColor = true;
            this.btnHeadRight.Click += new System.EventHandler(this.btnHeadRight_Click);
            // 
            // btnHeadLeft
            // 
            this.btnHeadLeft.Location = new System.Drawing.Point(12, 10);
            this.btnHeadLeft.Name = "btnHeadLeft";
            this.btnHeadLeft.Size = new System.Drawing.Size(75, 23);
            this.btnHeadLeft.TabIndex = 3;
            this.btnHeadLeft.Text = "Left";
            this.btnHeadLeft.UseVisualStyleBackColor = true;
            this.btnHeadLeft.Click += new System.EventHandler(this.btnHeadLeft_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDegreeHead);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtTorqueHead);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Location = new System.Drawing.Point(935, 98);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(179, 90);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Rotate";
            // 
            // txtDegreeHead
            // 
            this.txtDegreeHead.Location = new System.Drawing.Point(76, 23);
            this.txtDegreeHead.Name = "txtDegreeHead";
            this.txtDegreeHead.Size = new System.Drawing.Size(64, 23);
            this.txtDegreeHead.TabIndex = 11;
            this.txtDegreeHead.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 15);
            this.label6.TabIndex = 10;
            this.label6.Text = "Degree:";
            // 
            // txtTorqueHead
            // 
            this.txtTorqueHead.Location = new System.Drawing.Point(76, 52);
            this.txtTorqueHead.Name = "txtTorqueHead";
            this.txtTorqueHead.Size = new System.Drawing.Size(64, 23);
            this.txtTorqueHead.TabIndex = 18;
            this.txtTorqueHead.Text = "100";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 60);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 15);
            this.label7.TabIndex = 17;
            this.label7.Text = "Torque:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(709, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "Body";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(935, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 15);
            this.label9.TabIndex = 23;
            this.label9.Text = "Head";
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1155, 641);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSayIt);
            this.Controls.Add(this.textToSay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imgCameraView);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FrmMain";
            this.Text = "EV3 UI";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgCameraView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox imgCameraView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnBackward;
        private System.Windows.Forms.TextBox textToSay;
        private System.Windows.Forms.Button btnSayIt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDistance;
        private System.Windows.Forms.TextBox txtTorqueMove;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDegree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTorqueRotate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem pluginsToolStripMenuItem;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnHeadRight;
        private System.Windows.Forms.Button btnHeadLeft;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtDegreeHead;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtTorqueHead;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}

