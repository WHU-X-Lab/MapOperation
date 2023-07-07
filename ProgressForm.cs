using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace MapOperation
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        public void Addprogess(int count, int sp)
        {
            pBar1.Visible = true;// 显示进度条控件.
            pBar1.Minimum = 1;// 设置进度条最小值.
            pBar1.Maximum = count+1;// 设置进度条最大值.
            pBar1.Step = 1;// 设置每次增加的步长
            //创建Graphics对象
            Graphics g = this.pBar1.CreateGraphics();
            pBar1.Value++;
            Convert.ToDouble(sp);
            //执行PerformStep()函数
            string str = Math.Round(Convert.ToDouble(100.0 * sp / count), 2).ToString("#0.00 ") + "%";
            Font font = new Font("Times New Roman", (float)10, FontStyle.Regular);
            PointF pt = new PointF(this.pBar1.Width / 2 - 17, this.pBar1.Height / 2 - 7);
            g.DrawString(str, font, Brushes.Blue, pt);
            Thread.Sleep(100);  // 毫秒ms
            //MessageBox.Show("success!");
        }
        private void ProgressForm_Load(object sender, EventArgs e)
        {

        }
    }
}
