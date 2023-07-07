
namespace MapOperation
{
    partial class ScreenShot
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
            this.savePath = new System.Windows.Forms.TextBox();
            this.choosePath = new System.Windows.Forms.Button();
            this.shotMethod = new System.Windows.Forms.ComboBox();
            this.CreatePic = new System.Windows.Forms.Button();
            this.picHeight = new System.Windows.Forms.TextBox();
            this.picWidth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.picType = new System.Windows.Forms.ComboBox();
            this.clipperRate = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.clipperRate)).BeginInit();
            this.SuspendLayout();
            // 
            // savePath
            // 
            this.savePath.Location = new System.Drawing.Point(57, 148);
            this.savePath.Name = "savePath";
            this.savePath.Size = new System.Drawing.Size(165, 21);
            this.savePath.TabIndex = 0;
            this.savePath.Text = "C:\\Users\\11853\\Desktop\\Screenshot";
            this.savePath.TextChanged += new System.EventHandler(this.savePath_TextChanged);
            // 
            // choosePath
            // 
            this.choosePath.Location = new System.Drawing.Point(254, 148);
            this.choosePath.Name = "choosePath";
            this.choosePath.Size = new System.Drawing.Size(100, 23);
            this.choosePath.TabIndex = 1;
            this.choosePath.Text = "选择存储位置";
            this.choosePath.UseVisualStyleBackColor = true;
            this.choosePath.Click += new System.EventHandler(this.button1_Click);
            // 
            // shotMethod
            // 
            this.shotMethod.FormattingEnabled = true;
            this.shotMethod.Items.AddRange(new object[] {
            "按外接矩形截图",
            "按地理范围截图"});
            this.shotMethod.Location = new System.Drawing.Point(57, 96);
            this.shotMethod.Name = "shotMethod";
            this.shotMethod.Size = new System.Drawing.Size(165, 20);
            this.shotMethod.TabIndex = 2;
            this.shotMethod.Text = "按外接矩形截图";
            this.shotMethod.SelectedIndexChanged += new System.EventHandler(this.shotMethod_SelectedIndexChanged);
            // 
            // CreatePic
            // 
            this.CreatePic.Location = new System.Drawing.Point(254, 203);
            this.CreatePic.Name = "CreatePic";
            this.CreatePic.Size = new System.Drawing.Size(100, 23);
            this.CreatePic.TabIndex = 4;
            this.CreatePic.Text = "生成截图";
            this.CreatePic.UseVisualStyleBackColor = true;
            this.CreatePic.Click += new System.EventHandler(this.button2_Click);
            // 
            // picHeight
            // 
            this.picHeight.Location = new System.Drawing.Point(299, 47);
            this.picHeight.Name = "picHeight";
            this.picHeight.Size = new System.Drawing.Size(55, 21);
            this.picHeight.TabIndex = 5;
            this.picHeight.Text = "500";
            // 
            // picWidth
            // 
            this.picWidth.Location = new System.Drawing.Point(299, 97);
            this.picWidth.Name = "picWidth";
            this.picWidth.Size = new System.Drawing.Size(55, 21);
            this.picWidth.TabIndex = 6;
            this.picWidth.Text = "500";
            this.picWidth.TextChanged += new System.EventHandler(this.textBox3_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(252, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "图片高";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(252, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "图片宽";
            // 
            // picType
            // 
            this.picType.FormattingEnabled = true;
            this.picType.Items.AddRange(new object[] {
            "png",
            "jpeg",
            "gif"});
            this.picType.Location = new System.Drawing.Point(152, 203);
            this.picType.Name = "picType";
            this.picType.Size = new System.Drawing.Size(70, 20);
            this.picType.TabIndex = 9;
            this.picType.Text = "png";
            this.picType.SelectedIndexChanged += new System.EventHandler(this.picType_SelectedIndexChanged);
            // 
            // clipperRate
            // 
            this.clipperRate.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.clipperRate.Location = new System.Drawing.Point(140, 48);
            this.clipperRate.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.clipperRate.Name = "clipperRate";
            this.clipperRate.Size = new System.Drawing.Size(82, 21);
            this.clipperRate.TabIndex = 10;
            this.clipperRate.Tag = "";
            this.clipperRate.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.clipperRate.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(57, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "Clipper rate";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(57, 208);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "请选择截图格式";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // ScreenShot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(392, 259);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.clipperRate);
            this.Controls.Add(this.picType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picWidth);
            this.Controls.Add(this.picHeight);
            this.Controls.Add(this.CreatePic);
            this.Controls.Add(this.shotMethod);
            this.Controls.Add(this.choosePath);
            this.Controls.Add(this.savePath);
            this.Name = "ScreenShot";
            this.Text = "ScreenShot";
            this.Load += new System.EventHandler(this.ScreenShot_Load);
            ((System.ComponentModel.ISupportInitialize)(this.clipperRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox savePath;
        private System.Windows.Forms.Button choosePath;
        private System.Windows.Forms.ComboBox shotMethod;
        private System.Windows.Forms.Button CreatePic;
        private System.Windows.Forms.TextBox picHeight;
        private System.Windows.Forms.TextBox picWidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox picType;
        private System.Windows.Forms.NumericUpDown clipperRate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}