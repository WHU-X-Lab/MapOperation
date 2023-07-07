using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MapOperation
{
    public partial class ScreenShotForm : Form
    {

        //string strPath = @"E:\Naraku\maskwork\data_code";
        public ScreenShotForm(string strPath)
        {
            InitializeComponent();
            // FileStream fsall = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            DirectPath = strPath;
            foreach (string Path in System.IO.Directory.GetFiles(strPath))
            {
                string picName = Path.Substring(Path.LastIndexOf("\\") + 1, Path.LastIndexOf(".") - Path.LastIndexOf("\\") - 1);
                string PathExt = Path.Substring(Path.Length - 3, 3);
                if (PathExt == "jpg" || PathExt == "bmp" || PathExt == "png") //筛选图片格式
                {
                    ImagePaths.Add(Path);
                    this.imageList1.Images.Add(Image.FromFile(Path));
                }
                if (picName != "")
                {
                    listBox1.Items.Add(picName);
                }
            }
            if (ImagePaths.Count != 0)
            {
                //初始显示图片
                ImageCount = ImagePaths.Count; //获取图片总数
                FileStream fs = new FileStream(ImagePaths[0], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
                label2.Text = "截图1";
                fs.Close();
                fs.Dispose();
            }
            else
            {
                MessageBox.Show("未成功生成截图！");
               
            }
            //------------更新listview----------------
          
            //显示文件列表
            this.listView1.Items.Clear();
            this.listView1.LargeImageList = this.imageList1;
            this.listView1.View = View.LargeIcon;        //大图标显示
            //imageList1.ImageSize = new Size(40, 40);   //不能设置ImageList的图像大小 属性处更改
            //开始绑定
            this.listView1.BeginUpdate();
            //增加图片至ListView控件中
            for (int i = 0; i < imageList1.Images.Count; i++)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ImageIndex = i;
                lvi.Text = "pic" + i;
                this.listView1.Items.Add(lvi);
            }
            this.listView1.EndUpdate();

        }
        FileStream fs;
        private string DirectPath=null;
        private int ImageCount; //图片总数
        private List<string> ImagePaths = new List<string>(); //图片路径列表
        //private int nowCount = 0; //已显示图片个数
        //轮换图片存在bug需要fix
        private void button1_Click(object sender, EventArgs e)
        {
            //下一张
            if (pictureBox1.Image != null)
            {
                if (index == ImageCount - 1) //最后一张图片
                {
                    index = 0;
                    //显示图片
                    fs = new FileStream(ImagePaths[index], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                  //  this.pictureBox1.Image = Image.FromFile(ImagePaths[index]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
                    //label1.Text = ImagePaths[index].Substring(ImagePaths[index].LastIndexOf("\\") + 1, ImagePaths[index].LastIndexOf(".") - ImagePaths[index].LastIndexOf("\\") - 1);
                    label2.Text = "截图" + index;
                    fs.Close();
                    fs.Dispose();
                }
                else
                {
                    index++;
                    //显示图片
                    fs = new FileStream(ImagePaths[index], FileMode.Open, FileAccess.Read,FileShare.ReadWrite);
                    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                     //  this.pictureBox1.Image = Image.FromFile(ImagePaths[index]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
                    //label1.Text = ImagePaths[index].Substring(ImagePaths[index].LastIndexOf("\\") + 1, ImagePaths[index].LastIndexOf(".") - ImagePaths[index].LastIndexOf("\\") - 1);
                    label2.Text = "截图" + index;
                    fs.Close();
                    fs.Dispose();
                }
            }

            //if (ImageCount == 1)
            //{
            //    FileStream fs = new FileStream(ImagePaths[0], FileMode.Open, FileAccess.Read);
            //    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
            //    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
            //    label2.Text = "截图1";
            //    fs.Close();
            //    fs.Dispose();
            //}
            //else
            //{
            //    if (nowCount == ImageCount)
            //    {
            //        nowCount = 0; //计数清零
            //    }
            //    if (nowCount > -1 & nowCount < ImageCount)
            //    {
            //        FileStream fs = new FileStream(ImagePaths[nowCount], FileMode.Open, FileAccess.Read);
            //        this.pictureBox1.Image = Image.FromStream(fs); //加载图片
            //        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //        label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
            //        label2.Text = "截图" + (nowCount + 1);
            //        if (nowCount < ImageCount )
            //        {
            //            nowCount++;
            //        }
            //        fs.Close();
            //        fs.Dispose();
            //    }
            //}
            pictureBox1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //上一张
            if (pictureBox1.Image != null)
            {
                if (index > 0)
                {
                    index--;
                    //显示图片
                    fs = new FileStream(ImagePaths[index], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                     //  this.pictureBox1.Image = Image.FromFile(ImagePaths[index]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
                    //label1.Text = ImagePaths[index].Substring(ImagePaths[index].LastIndexOf("\\") + 1, ImagePaths[index].LastIndexOf(".") - ImagePaths[index].LastIndexOf("\\") - 1);
                    label2.Text = "截图" + index;
                    fs.Close();
                    fs.Dispose();
                }
                else if (index == 0)
                {
                    index = ImageCount;
                    index--;
                    //显示图片
                    fs = new FileStream(ImagePaths[index], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                    //  this.pictureBox1.Image = Image.FromFile(ImagePaths[index]);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
                    //label1.Text = ImagePaths[index].Substring(ImagePaths[index].LastIndexOf("\\") + 1, ImagePaths[index].LastIndexOf(".") - ImagePaths[index].LastIndexOf("\\") - 1);
                    label2.Text = "截图" + index;
                    fs.Close();
                    fs.Dispose();
                }
            }


            //if (ImageCount == 1)
            //{
            //    FileStream fs = new FileStream(ImagePaths[0], FileMode.Open, FileAccess.Read);
            //    this.pictureBox1.Image = Image.FromStream(fs); //加载图片
            //    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //    label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
            //    label2.Text = "截图1";
            //    fs.Close();
            //    fs.Dispose();
            //}
            //else
            //{
            //    if (nowCount == 0)
            //    {
            //        nowCount = ImageCount;
            //    }
            //    if (nowCount >-1 & nowCount< ImageCount)
            //    {
            //        //this.pictureBox1.Image = Bitmap.FromFile(ImagePaths[nowCount]); //加载图片
            //        //label1.Text = System.IO.Path.GetFileName(ImagePaths[nowCount]);
            //        //label2.Text = "截图" + (nowCount + 1);
            //        //if (nowCount > 0)
            //        //{
            //        //    nowCount--;
            //        //}
            //        FileStream fs = new FileStream(ImagePaths[nowCount], FileMode.Open, FileAccess.Read);
            //        this.pictureBox1.Image = Image.FromStream(fs); //加载图片
            //        pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            //        label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") -fs.Name.LastIndexOf("\\") - 1);
            //        label2.Text = "截图" + (nowCount + 1);
            //        if (nowCount < ImageCount )
            //        {
            //            nowCount++;
            //        }
            //        fs.Close();
            //        fs.Dispose();
            //    }
            //}
            pictureBox1.Refresh();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            //文件名


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void ScreenShotForm_Load(object sender, EventArgs e)
        {
           

        }

        private void button3_Click(object sender, EventArgs e)
        {

            //继续截图??如何实现，需要找出未释放的空间！
            if (MessageBox.Show("要保存当前截图吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                MessageBox.Show("保存成功!");
                ImagePaths.Clear();
                ImagePaths = null;
                this.pictureBox1.Image.Dispose();
                this.Dispose();
            }
            else
            {
                int sp = 1;                                                 //处理步骤计数
                ProgressForm progress = new ProgressForm();
                progress.Show();
                
                if (ImageCount != 0)
                {
                    try
                    {
                        for (int i = 0; i < ImageCount; i++)
                        {
                            System.IO.File.Delete(ImagePaths[i]);
                            //添加进度条
                            progress.Addprogess(ImageCount, sp);
                            sp++;
                        }
                       
                    }
                 catch(Exception ex)
                    {
                        MessageBox.Show("暂不支持删除此截图文件!"+ex.Message, "", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
                progress.Close();
                ImagePaths.Clear();
                ImagePaths = null;

                this.pictureBox1.Image.Dispose();
                this.Dispose();

              

            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //退出该窗口
            int sp = 1;                                                 //处理步骤计数
            ProgressForm progress = new ProgressForm();
            progress.Show();
            try
            {
                if (ImageCount != 0)
                {
                    for (int i = 0; i < ImageCount; i++)
                    {
                        //string filename = ImagePaths[i];
                        //解决文件占用问题？？
                        System.IO.File.Delete(ImagePaths[i]);
                        //添加进度条
                        progress.Addprogess(ImageCount, sp);
                        sp++;
                    }
                   
                }   
            }
             catch(Exception ex)
            {
                MessageBox.Show("暂不支持删除此截图文件!" + ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            progress.Close();
            ImagePaths.Clear();
            ImagePaths = null;

            this.pictureBox1.Image.Dispose();
            this.Dispose();
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           //已弃用
            string filepath = DirectPath +@"\"+ this.listBox1.SelectedItem.ToString();
            if (filepath != null)
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                this.pictureBox1.Image = Image.FromStream(fs); //加载图片
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                label1.Text = this.listBox1.SelectedItem.ToString();
                label2.Text = null;

                fs.Close();
                fs.Dispose();
            }
            pictureBox1.Refresh();
        }

        private string picDirPath = null;                        //图片路径
        private int index;//listview中图片序号
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (this.listView1.SelectedItems.Count == 0)
            //    return;
            //采用索引方式 imagePathList记录图片真实路径
            index = this.listView1.Items.IndexOf(listView1.FocusedItem);
            //显示图片
            FileStream fs = new FileStream(ImagePaths[index], FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            this.pictureBox1.Image = Image.FromStream(fs); //加载图片
            //图片被拉伸或收缩适合pictureBox大小
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            label1.Text = fs.Name.Substring(fs.Name.LastIndexOf("\\") + 1, fs.Name.LastIndexOf(".") - fs.Name.LastIndexOf("\\") - 1);
            //label1.Text = ImagePaths[index].Substring(ImagePaths[index].LastIndexOf("\\") + 1, ImagePaths[index].LastIndexOf(".") - ImagePaths[index].LastIndexOf("\\") - 1);
            label2.Text = "截图" + index;
            fs.Close();
            fs.Dispose();

        }
    }
}
