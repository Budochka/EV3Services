
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
            imgCameraView = new System.Windows.Forms.PictureBox();
            label1 = new System.Windows.Forms.Label();
            btnForward = new System.Windows.Forms.Button();
            btnLeft = new System.Windows.Forms.Button();
            btnRight = new System.Windows.Forms.Button();
            btnBackward = new System.Windows.Forms.Button();
            textToSay = new System.Windows.Forms.TextBox();
            btnSayIt = new System.Windows.Forms.Button();
            panel1 = new System.Windows.Forms.Panel();
            txtLog = new System.Windows.Forms.TextBox();
            label2 = new System.Windows.Forms.Label();
            txtDistance = new System.Windows.Forms.TextBox();
            txtTorqueMove = new System.Windows.Forms.TextBox();
            label5 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            txtDegree = new System.Windows.Forms.TextBox();
            label3 = new System.Windows.Forms.Label();
            txtTorqueRotate = new System.Windows.Forms.TextBox();
            label4 = new System.Windows.Forms.Label();
            menuStrip = new System.Windows.Forms.MenuStrip();
            pluginsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            panel2 = new System.Windows.Forms.Panel();
            btnHeadRight = new System.Windows.Forms.Button();
            btnHeadLeft = new System.Windows.Forms.Button();
            groupBox3 = new System.Windows.Forms.GroupBox();
            txtDegreeHead = new System.Windows.Forms.TextBox();
            label6 = new System.Windows.Forms.Label();
            txtTorqueHead = new System.Windows.Forms.TextBox();
            label7 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            cmbMode = new System.Windows.Forms.ComboBox();
            btnSet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)imgCameraView).BeginInit();
            panel1.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            menuStrip.SuspendLayout();
            panel2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // imgCameraView
            // 
            imgCameraView.Location = new System.Drawing.Point(14, 76);
            imgCameraView.Name = "imgCameraView";
            imgCameraView.Size = new System.Drawing.Size(663, 365);
            imgCameraView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            imgCameraView.TabIndex = 0;
            imgCameraView.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(14, 41);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(28, 15);
            label1.TabIndex = 1;
            label1.Text = "Text";
            // 
            // btnForward
            // 
            btnForward.Location = new System.Drawing.Point(65, 10);
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(75, 23);
            btnForward.TabIndex = 2;
            btnForward.Text = "Forward";
            btnForward.UseVisualStyleBackColor = true;
            btnForward.Click += btnForward_Click;
            // 
            // btnLeft
            // 
            btnLeft.Location = new System.Drawing.Point(27, 39);
            btnLeft.Name = "btnLeft";
            btnLeft.Size = new System.Drawing.Size(75, 23);
            btnLeft.TabIndex = 3;
            btnLeft.Text = "Left";
            btnLeft.UseVisualStyleBackColor = true;
            btnLeft.Click += btnLeft_Click;
            // 
            // btnRight
            // 
            btnRight.Location = new System.Drawing.Point(108, 39);
            btnRight.Name = "btnRight";
            btnRight.Size = new System.Drawing.Size(75, 23);
            btnRight.TabIndex = 4;
            btnRight.Text = "Right";
            btnRight.UseVisualStyleBackColor = true;
            btnRight.Click += btnRight_Click;
            // 
            // btnBackward
            // 
            btnBackward.Location = new System.Drawing.Point(65, 68);
            btnBackward.Name = "btnBackward";
            btnBackward.Size = new System.Drawing.Size(75, 23);
            btnBackward.TabIndex = 5;
            btnBackward.Text = "Backward";
            btnBackward.UseVisualStyleBackColor = true;
            btnBackward.Click += btnBackward_Click;
            // 
            // textToSay
            // 
            textToSay.Location = new System.Drawing.Point(70, 37);
            textToSay.Name = "textToSay";
            textToSay.Size = new System.Drawing.Size(534, 23);
            textToSay.TabIndex = 6;
            // 
            // btnSayIt
            // 
            btnSayIt.Location = new System.Drawing.Point(602, 37);
            btnSayIt.Name = "btnSayIt";
            btnSayIt.Size = new System.Drawing.Size(75, 23);
            btnSayIt.TabIndex = 7;
            btnSayIt.Text = "Say It!";
            btnSayIt.UseVisualStyleBackColor = true;
            btnSayIt.Click += btnSayIt_Click;
            // 
            // panel1
            // 
            panel1.Controls.Add(btnRight);
            panel1.Controls.Add(btnForward);
            panel1.Controls.Add(btnLeft);
            panel1.Controls.Add(btnBackward);
            panel1.Location = new System.Drawing.Point(709, 290);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(198, 100);
            panel1.TabIndex = 8;
            // 
            // txtLog
            // 
            txtLog.Location = new System.Drawing.Point(14, 447);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new System.Drawing.Size(1129, 180);
            txtLog.TabIndex = 9;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(12, 26);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(55, 15);
            label2.TabIndex = 10;
            label2.Text = "Distance:";
            // 
            // txtDistance
            // 
            txtDistance.Location = new System.Drawing.Point(76, 23);
            txtDistance.Name = "txtDistance";
            txtDistance.Size = new System.Drawing.Size(64, 23);
            txtDistance.TabIndex = 11;
            txtDistance.Text = "10";
            // 
            // txtTorqueMove
            // 
            txtTorqueMove.Location = new System.Drawing.Point(76, 52);
            txtTorqueMove.Name = "txtTorqueMove";
            txtTorqueMove.Size = new System.Drawing.Size(64, 23);
            txtTorqueMove.TabIndex = 18;
            txtTorqueMove.Text = "100";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(12, 60);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(46, 15);
            label5.TabIndex = 17;
            label5.Text = "Torque:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(txtDistance);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txtTorqueMove);
            groupBox1.Controls.Add(label5);
            groupBox1.Location = new System.Drawing.Point(709, 98);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(197, 90);
            groupBox1.TabIndex = 19;
            groupBox1.TabStop = false;
            groupBox1.Text = "Move";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(txtDegree);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(txtTorqueRotate);
            groupBox2.Controls.Add(label4);
            groupBox2.Location = new System.Drawing.Point(709, 194);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(197, 90);
            groupBox2.TabIndex = 20;
            groupBox2.TabStop = false;
            groupBox2.Text = "Rotate";
            // 
            // txtDegree
            // 
            txtDegree.Location = new System.Drawing.Point(76, 23);
            txtDegree.Name = "txtDegree";
            txtDegree.Size = new System.Drawing.Size(64, 23);
            txtDegree.TabIndex = 11;
            txtDegree.Text = "10";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 26);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(47, 15);
            label3.TabIndex = 10;
            label3.Text = "Degree:";
            // 
            // txtTorqueRotate
            // 
            txtTorqueRotate.Location = new System.Drawing.Point(76, 52);
            txtTorqueRotate.Name = "txtTorqueRotate";
            txtTorqueRotate.Size = new System.Drawing.Size(64, 23);
            txtTorqueRotate.TabIndex = 18;
            txtTorqueRotate.Text = "100";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(12, 60);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(46, 15);
            label4.TabIndex = 17;
            label4.Text = "Torque:";
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { pluginsToolStripMenuItem });
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new System.Drawing.Size(1155, 24);
            menuStrip.TabIndex = 21;
            // 
            // pluginsToolStripMenuItem
            // 
            pluginsToolStripMenuItem.Name = "pluginsToolStripMenuItem";
            pluginsToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            pluginsToolStripMenuItem.Text = "Plugins";
            // 
            // panel2
            // 
            panel2.Controls.Add(btnHeadRight);
            panel2.Controls.Add(btnHeadLeft);
            panel2.Location = new System.Drawing.Point(935, 290);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(179, 44);
            panel2.TabIndex = 9;
            // 
            // btnHeadRight
            // 
            btnHeadRight.Location = new System.Drawing.Point(93, 10);
            btnHeadRight.Name = "btnHeadRight";
            btnHeadRight.Size = new System.Drawing.Size(75, 23);
            btnHeadRight.TabIndex = 4;
            btnHeadRight.Text = "Right";
            btnHeadRight.UseVisualStyleBackColor = true;
            btnHeadRight.Click += btnHeadRight_Click;
            // 
            // btnHeadLeft
            // 
            btnHeadLeft.Location = new System.Drawing.Point(12, 10);
            btnHeadLeft.Name = "btnHeadLeft";
            btnHeadLeft.Size = new System.Drawing.Size(75, 23);
            btnHeadLeft.TabIndex = 3;
            btnHeadLeft.Text = "Left";
            btnHeadLeft.UseVisualStyleBackColor = true;
            btnHeadLeft.Click += btnHeadLeft_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(txtDegreeHead);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(txtTorqueHead);
            groupBox3.Controls.Add(label7);
            groupBox3.Location = new System.Drawing.Point(935, 98);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(179, 90);
            groupBox3.TabIndex = 21;
            groupBox3.TabStop = false;
            groupBox3.Text = "Rotate";
            // 
            // txtDegreeHead
            // 
            txtDegreeHead.Location = new System.Drawing.Point(76, 23);
            txtDegreeHead.Name = "txtDegreeHead";
            txtDegreeHead.Size = new System.Drawing.Size(64, 23);
            txtDegreeHead.TabIndex = 11;
            txtDegreeHead.Text = "10";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(12, 26);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(47, 15);
            label6.TabIndex = 10;
            label6.Text = "Degree:";
            // 
            // txtTorqueHead
            // 
            txtTorqueHead.Location = new System.Drawing.Point(76, 52);
            txtTorqueHead.Name = "txtTorqueHead";
            txtTorqueHead.Size = new System.Drawing.Size(64, 23);
            txtTorqueHead.TabIndex = 18;
            txtTorqueHead.Text = "100";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(12, 60);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(46, 15);
            label7.TabIndex = 17;
            label7.Text = "Torque:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(709, 67);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(34, 15);
            label8.TabIndex = 22;
            label8.Text = "Body";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(935, 67);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(35, 15);
            label9.TabIndex = 23;
            label9.Text = "Head";
            // 
            // cmbMode
            // 
            cmbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbMode.FormattingEnabled = true;
            cmbMode.Items.AddRange(new object[] { "Direct Control", "Explore", "Greeting" });
            cmbMode.Location = new System.Drawing.Point(709, 408);
            cmbMode.Name = "cmbMode";
            cmbMode.Size = new System.Drawing.Size(198, 23);
            cmbMode.TabIndex = 24;
            // 
            // btnSet
            // 
            btnSet.Location = new System.Drawing.Point(947, 408);
            btnSet.Name = "btnSet";
            btnSet.Size = new System.Drawing.Size(156, 23);
            btnSet.TabIndex = 25;
            btnSet.Text = "Set";
            btnSet.UseVisualStyleBackColor = true;
            btnSet.Click += btnSet_Click;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1155, 641);
            Controls.Add(btnSet);
            Controls.Add(cmbMode);
            Controls.Add(label9);
            Controls.Add(label8);
            Controls.Add(groupBox3);
            Controls.Add(panel2);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(txtLog);
            Controls.Add(panel1);
            Controls.Add(btnSayIt);
            Controls.Add(textToSay);
            Controls.Add(label1);
            Controls.Add(imgCameraView);
            Controls.Add(menuStrip);
            MainMenuStrip = menuStrip;
            Name = "FrmMain";
            Text = "EV3 UI";
            FormClosed += frmMain_FormClosed;
            Load += FrmMain_Load;
            ((System.ComponentModel.ISupportInitialize)imgCameraView).EndInit();
            panel1.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            panel2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.ComboBox cmbMode;
        private System.Windows.Forms.Button btnSet;
    }
}

