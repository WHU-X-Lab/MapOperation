using System;
using System.Windows.Forms;

namespace MapOperation
{
    public partial class ScreenShot : Form
    {
        private FormMain mform;
        public ScreenShot(FormMain formMain)
        {
            InitializeComponent();
            mform = formMain;//直接把主窗口传递进来，以调用主窗口的方法
        }

        private void ScreenShot_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //选择存储位置
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                //if (System.IO.Directory.GetFiles(folderBrowserDialog.SelectedPath).Length > 0)
                //{
                //    MessageBox.Show("请选择一个空文件夹！", "提示");
                //}
                //else
                //{
                    savePath.Text = folderBrowserDialog.SelectedPath;
                //}
                
            }
            folderBrowserDialog.Dispose();
        }
        public string GetPath()
        {
            string path = this.savePath.Text;
            if (path == null)
            {
                MessageBox.Show("请选择截图存储位置！","ERROR");
                return null;
            }
            return path;
        }
        public  int GetRate()
        {
            return (int)this.clipperRate.Value;
        }
        public  int GetMethod()
        {
            if (this.shotMethod.Text == "按外接矩形截图")
            {
                return 1;
            }
            else if (this.shotMethod.Text == "按地理范围截图")
            {
                return 2;
            }
            return 1;
        }
        public  int GetWidth()
        {
            try
            { 
                int width = int.Parse(this.picWidth.Text);
                return width;
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
                return 0;
            }
        }
        public  int GetHeight()
        {
            try
            {
                int height = int.Parse(this.picHeight.Text);
                return height;
            }
            catch (Exception ec)
            {
                MessageBox.Show(ec.Message);
                return 0;
            }
        }
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.Text = "修改clipper rate的值";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //生成截图
            //获取截图参数
            string savepath = GetPath();
            int cliprate = GetRate();
            int picW = GetWidth();
            int picH = GetHeight();
            int shotMethod = GetMethod();
            Form _frm = Application.OpenForms["ScreenShotForm"];  //查找是否打开过窗体  
            if ((_frm == null) || (_frm.IsDisposed))       //如果没有打开过
            {
               int nSelection =  mform.mainMapControl.Map.SelectionCount;
                if (nSelection != 0) {
                    mform.Para_Screenshot(savepath, cliprate, picW, picH, shotMethod);
                    ScreenShotForm shot = new ScreenShotForm(savepath);
                    shot.Show();//以无模式窗体方式调用
                }
                else
                    MessageBox.Show("请先进行要素选择！", "提示");
            }
            else
            {
                _frm.Activate();
                _frm.WindowState = FormWindowState.Normal;
            }
           

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void savePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void shotMethod_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void picType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.picType.Text = "png";
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
