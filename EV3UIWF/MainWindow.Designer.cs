
namespace EV3UIWF
{
    partial class frmMain
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
            this.lblDistance = new System.Windows.Forms.Label();
            this.lblDegree = new System.Windows.Forms.Label();
            this.txtDegree = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSet = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.imgCameraView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgCameraView
            // 
            this.imgCameraView.Location = new System.Drawing.Point(12, 51);
            this.imgCameraView.Name = "imgCameraView";
            this.imgCameraView.Size = new System.Drawing.Size(663, 365);
            this.imgCameraView.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.imgCameraView.TabIndex = 0;
            this.imgCameraView.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
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
            // 
            // btnLeft
            // 
            this.btnLeft.Location = new System.Drawing.Point(27, 39);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(75, 23);
            this.btnLeft.TabIndex = 3;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            // 
            // btnRight
            // 
            this.btnRight.Location = new System.Drawing.Point(108, 39);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(75, 23);
            this.btnRight.TabIndex = 4;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            // 
            // btnBackward
            // 
            this.btnBackward.Location = new System.Drawing.Point(65, 68);
            this.btnBackward.Name = "btnBackward";
            this.btnBackward.Size = new System.Drawing.Size(75, 23);
            this.btnBackward.TabIndex = 5;
            this.btnBackward.Text = "Backward";
            this.btnBackward.UseVisualStyleBackColor = true;
            // 
            // textToSay
            // 
            this.textToSay.Location = new System.Drawing.Point(68, 12);
            this.textToSay.Name = "textToSay";
            this.textToSay.Size = new System.Drawing.Size(534, 23);
            this.textToSay.TabIndex = 6;
            // 
            // btnSayIt
            // 
            this.btnSayIt.Location = new System.Drawing.Point(600, 12);
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
            this.panel1.Location = new System.Drawing.Point(681, 316);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(198, 100);
            this.panel1.TabIndex = 8;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 422);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(866, 180);
            this.txtLog.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(681, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "Distance:";
            // 
            // txtDistance
            // 
            this.txtDistance.Location = new System.Drawing.Point(746, 51);
            this.txtDistance.Name = "txtDistance";
            this.txtDistance.Size = new System.Drawing.Size(64, 23);
            this.txtDistance.TabIndex = 11;
            // 
            // lblDistance
            // 
            this.lblDistance.Location = new System.Drawing.Point(816, 51);
            this.lblDistance.Name = "lblDistance";
            this.lblDistance.Size = new System.Drawing.Size(63, 23);
            this.lblDistance.TabIndex = 12;
            this.lblDistance.Text = "100";
            this.lblDistance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblDegree
            // 
            this.lblDegree.Location = new System.Drawing.Point(816, 80);
            this.lblDegree.Name = "lblDegree";
            this.lblDegree.Size = new System.Drawing.Size(63, 23);
            this.lblDegree.TabIndex = 15;
            this.lblDegree.Text = "100";
            this.lblDegree.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtDegree
            // 
            this.txtDegree.Location = new System.Drawing.Point(746, 80);
            this.txtDegree.Name = "txtDegree";
            this.txtDegree.Size = new System.Drawing.Size(64, 23);
            this.txtDegree.TabIndex = 14;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(681, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 15);
            this.label4.TabIndex = 13;
            this.label4.Text = "Degree:";
            // 
            // btnSet
            // 
            this.btnSet.Location = new System.Drawing.Point(681, 109);
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(197, 24);
            this.btnSet.TabIndex = 16;
            this.btnSet.Text = "Set";
            this.btnSet.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 614);
            this.Controls.Add(this.btnSet);
            this.Controls.Add(this.lblDegree);
            this.Controls.Add(this.txtDegree);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblDistance);
            this.Controls.Add(this.txtDistance);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSayIt);
            this.Controls.Add(this.textToSay);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.imgCameraView);
            this.Name = "frmMain";
            this.Text = "EV3 UI";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.imgCameraView)).EndInit();
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Label lblDistance;
        private System.Windows.Forms.Label lblDegree;
        private System.Windows.Forms.TextBox txtDegree;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSet;
    }
}

