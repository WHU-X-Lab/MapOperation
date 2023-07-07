using ClipperLib;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;

namespace MapOperation
{
    using Path = List<IntPoint>;
    using Paths = List<List<IntPoint>>;
    //toolstripitem defined by myself,to extend with IItemDef
    //public class record 
    //{
    //   public string osmid = null;
    //   public string topid = null;
    //   public string expert = null;
    //}
    public partial class FormMain : Form
    {
        #region 变量定义




        private ArrayList plugins = new ArrayList();

        private List<ToolStripItem> controlList;     //Tool Controls List on the mainform
        private IToolbarControl2 ESRIToolbarControl = null; //ToolbarControl
        private ICommandPool2 ESRICommandPool = null; //CommandPool
        private Dictionary<string, ICommand> commandDictionary; //a Dictionary store the background ICommand object
        private ToolStripItem btn;
        private int tabb;




        private Dictionary<string, ILayer> dclayers;
        IMap map = null;
        public static ILayer selectedLayer = null;
        IFeatureClass selectedFeatureClass = null;
        private IHookHelper m_hookHelper = null;
        //IActiveView activeView = null;
        public IApplication m_application = null;
        private static int cid = 0; //颜色id

        //局部评价
        INewPolygonFeedback polygonFeedback = null; // 正在绘制中的临时选框
        INewLineFeedback polylineFeedback = null;
        IActiveView activeView = null;
        int numMouseDown = 0; // 记录鼠标左键按下去的次数，在每一次框选擦除之后将其归零
        int t = 0;


        int nameFieldIndex = -1;
        string[] fieldNames = new string[7] { "name", "fclass", "ref", "oneway", "maxspeed", "bridge", "tunnel" };
        int[] fieldIds = new int[7];  //按循序存储"name", "highway", "fclass", "ref", "oneway", "maxspeed", "bridge"和"tunnel"的索引



        private INewLineFeedback pNewLineFeedback;           //追踪线对象
        private INewPolygonFeedback pNewPolygonFeedback;     //追踪面对象
        private IPoint pPointPt = null;                      //鼠标点击点
        private IPoint pMovePt = null;                       //鼠标移动时的当前点
        private double dToltalLength = 0;                    //量测总长度
        private double dSegmentLength = 0;                   //片段距离
        private IPointCollection pAreaPointCol = new MultipointClass();  //面积量算时画的点进行存储；  

        private string sMapUnits = "未知单位";             //地图单位变量
        private object missing = Type.Missing;

        //TOC菜单
        IFeatureLayer pTocFeatureLayer = null;            //点击的要素图层
        private FormAtrribute frmAttribute = null;        //图层属性窗体
        private ILayer pMoveLayer;                        //需要调整显示顺序的图层
        private int toIndex;                              //存放拖动图层移动到的索引号     

        //鹰眼同步
        private bool bCanDrag;              //鹰眼地图上的矩形框可移动的标志
        private IPoint pMoveRectPoint;      //记录在移动鹰眼地图上的矩形框时鼠标的位置
        private IEnvelope pEnv;             //记录数据视图的Extent
        #endregion

        #region 初始化
        public FormMain()
        {
            GeoProcessor gp = new GeoProcessorClass();

            InitializeComponent();
            axTOCControl.SetBuddyControl(mainMapControl);

            EagleEyeMapControl.Extent = mainMapControl.FullExtent;
            pEnv = EagleEyeMapControl.Extent;
            DrawRectangle(pEnv);

        }
        #endregion

        #region 整体框架
        #region 数据加载

        #region LoadMxFile方法加载地图文档文件
        private void btnLoadMxFile_Click(object sender, EventArgs e)
        {
            //加载数据前如果有数据则清空
            try
            {
                OpenFileDialog pOpenFileDialog = new OpenFileDialog();
                pOpenFileDialog.CheckFileExists = true;
                pOpenFileDialog.Title = "打开地图文档";
                pOpenFileDialog.Filter = "ArcMap文档(*.mxd)|*.mxd;|ArcMap模板(*.mxt)|*.mxt|发布地图文件(*.pmf)|*.pmf|所有地图格式(*.mxd;*.mxt;*.pmf)|*.mxd;*.mxt;*.pmf";
                pOpenFileDialog.Multiselect = false;   //不允许多个文件同时选择
                pOpenFileDialog.RestoreDirectory = true;   //存储打开的文件路径
                if (pOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pFileName = pOpenFileDialog.FileName;
                    if (pFileName == "")
                    {
                        return;
                    }
                    if (mainMapControl.CheckMxFile(pFileName)) //检查地图文档有效性
                    {
                        ClearAllData();
                        mainMapControl.LoadMxFile(pFileName);
                    }
                    else
                    {
                        MessageBox.Show(pFileName + "是无效的地图文档!", "信息提示");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开地图文档失败" + ex.Message);
            }
        }
        #endregion

        #region IMapDocument方法加载Mxd文档文件
        private void btnIMapDocument_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog pOpenFileDialog = new OpenFileDialog();
                pOpenFileDialog.CheckFileExists = true;
                pOpenFileDialog.Title = "打开地图文档";
                pOpenFileDialog.Filter = "ArcMap文档(*.mxd)|*.mxd;|ArcMap模板(*.mxt)|*.mxt|发布地图文件(*.pmf)|*.pmf|所有地图格式(*.mxd;*.mxt;*.pmf)|*.mxd;*.mxt;*.pmf";
                pOpenFileDialog.Multiselect = false;
                pOpenFileDialog.RestoreDirectory = true;
                if (pOpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string pFileName = pOpenFileDialog.FileName;
                    if (pFileName == "")
                    {
                        return;
                    }

                    if (mainMapControl.CheckMxFile(pFileName)) //检查地图文档有效性
                    {
                        //将数据载入pMapDocument并与Map控件关联
                        IMapDocument pMapDocument = new MapDocument();//using ESRI.ArcGIS.Carto;
                        pMapDocument.Open(pFileName, "");
                        //获取Map中激活的地图文档
                        mainMapControl.Map = pMapDocument.ActiveView.FocusMap;
                        mainMapControl.ActiveView.Refresh();
                    }
                    else
                    {
                        MessageBox.Show(pFileName + "是无效的地图文档!", "信息提示");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开地图文档失败" + ex.Message);
            }
        }
        #endregion

        #region ControlsOpenDocCommandClass加载地图
        private void btncontrolsOpenDocCommandClass_Click(object sender, EventArgs e)
        {


            ICommand command = new ControlsOpenDocCommandClass();
            command.OnCreate(mainMapControl.Object);
            command.OnClick();
        }
        #endregion

        #region 加载Shape文件
        private void btnAddShapefile_Click(object sender, EventArgs e)
        {
            //ClearAllData();
            try
            {
                OpenFileDialog pOpenFileDialog = new OpenFileDialog();
                pOpenFileDialog.CheckFileExists = true;
                pOpenFileDialog.Title = "打开Shape文件";
                pOpenFileDialog.Filter = "Shape文件（*.shp）|*.shp";
                pOpenFileDialog.ShowDialog();

                ////获取文件路径
                //FileInfo pFileInfo = new FileInfo(pOpenFileDialog.FileName);
                //string pPath = pOpenFileDialog.FileName.Substring(0, pOpenFileDialog.FileName.Length - pFileInfo.Name.Length);
                //mainMapControl.AddShapeFile(pPath, pFileInfo.Name);

                IWorkspaceFactory pWorkspaceFactory;
                IFeatureWorkspace pFeatureWorkspace;
                IFeatureLayer pFeatureLayer;

                string pFullPath = pOpenFileDialog.FileName;
                if (pFullPath == "") return;

                int pIndex = pFullPath.LastIndexOf("\\");
                string pFilePath = pFullPath.Substring(0, pIndex); //文件路径
                string pFileName = pFullPath.Substring(pIndex + 1); //文件名

                //实例化ShapefileWorkspaceFactory工作空间，打开Shape文件
                pWorkspaceFactory = new ShapefileWorkspaceFactory();
                pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(pFilePath, 0);
                //创建并实例化要素集
                IFeatureClass pFeatureClass = pFeatureWorkspace.OpenFeatureClass(pFileName);
                pFeatureLayer = new FeatureLayer();
                pFeatureLayer.FeatureClass = pFeatureClass;
                pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;

                //ClearAllData();    //新增删除数据

                mainMapControl.Map.AddLayer(pFeatureLayer);
                mainMapControl.ActiveView.Refresh();
                //同步鹰眼
                SynchronizeEagleEye();
            }
            catch (Exception ex)
            {
                MessageBox.Show("图层加载失败！" + ex.Message);
            }
        }
        #endregion

        #region 加载栅格文件
        private void btnAddRaster_Click(object sender, EventArgs e)
        {
            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.CheckFileExists = true;
            pOpenFileDialog.Title = "打开Raster文件";
            pOpenFileDialog.Filter = "栅格文件 (*.*)|*.bmp;*.tif;*.jpg;*.img|(*.bmp)|*.bmp|(*.tif)|*.tif|(*.jpg)|*.jpg|(*.img)|*.img";
            pOpenFileDialog.ShowDialog();

            string pRasterFileName = pOpenFileDialog.FileName;
            if (pRasterFileName == "")
            {
                return;
            }

            string pPath = System.IO.Path.GetDirectoryName(pRasterFileName);
            string pFileName = System.IO.Path.GetFileName(pRasterFileName);

            IWorkspaceFactory pWorkspaceFactory = new RasterWorkspaceFactory();
            IWorkspace pWorkspace = pWorkspaceFactory.OpenFromFile(pPath, 0);
            IRasterWorkspace pRasterWorkspace = pWorkspace as IRasterWorkspace;
            IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pFileName);
            //影像金字塔判断与创建
            IRasterPyramid3 pRasPyrmid;
            pRasPyrmid = pRasterDataset as IRasterPyramid3;
            if (pRasPyrmid != null)
            {
                if (!(pRasPyrmid.Present))
                {
                    pRasPyrmid.Create(); //创建金字塔
                }
            }
            IRaster pRaster;
            pRaster = pRasterDataset.CreateDefaultRaster();
            IRasterLayer pRasterLayer;
            pRasterLayer = new RasterLayerClass();
            pRasterLayer.CreateFromRaster(pRaster);
            ILayer pLayer = pRasterLayer as ILayer;
            mainMapControl.AddLayer(pLayer, 0);
        }
        #endregion

        #region 分图层加载CAD数据
        private void btnAddCADByLayer_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pWorkspaceFactory;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass;

            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "CAD(*.dwg)|*.dwg";
            pOpenFileDialog.Title = "打开CAD数据文件";
            pOpenFileDialog.ShowDialog();

            string pFullPath = pOpenFileDialog.FileName;
            if (pFullPath == "")
            {
                return;
            }
            //获取文件名和文件路径
            int pIndex = pFullPath.LastIndexOf("\\");
            string pFilePath = pFullPath.Substring(0, pIndex);
            string pFileName = pFullPath.Substring(pIndex + 1);

            pWorkspaceFactory = new CadWorkspaceFactory();
            pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(pFilePath, 0);
            //加载CAD文件中的线文件
            pFeatureClass = pFeatureWorkspace.OpenFeatureClass(pFileName + ":polyline");
            pFeatureLayer = new FeatureLayerClass();
            pFeatureLayer.Name = pFileName;
            pFeatureLayer.FeatureClass = pFeatureClass;

            ClearAllData();    //新增删除数据

            mainMapControl.Map.AddLayer(pFeatureLayer);
            mainMapControl.ActiveView.Refresh();
            //同步鹰眼
            SynchronizeEagleEye();
        }
        #endregion

        #region 加载整幅CAD图数据
        private void btnAddWholeCAD_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pWorkspaceFactory;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            IFeatureDataset pFeatureDataset;

            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "CAD(*.dwg)|*.dwg";
            pOpenFileDialog.Title = "打开CAD数据文件";
            pOpenFileDialog.ShowDialog();

            string pFullPath = pOpenFileDialog.FileName;
            if (pFullPath == "")
            {
                return;
            }
            //获取文件名和文件路径
            int pIndex = pFullPath.LastIndexOf("\\");
            string pFilePath = pFullPath.Substring(0, pIndex);
            string pFileName = pFullPath.Substring(pIndex + 1);
            //打开CAD数据集
            pWorkspaceFactory = new CadWorkspaceFactoryClass(); //using ESRI.ArcGIS.DataSourcesFile;
            pFeatureWorkspace = (IFeatureWorkspace)pWorkspaceFactory.OpenFromFile(pFilePath, 0);
            //打开一个要素集
            pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(pFileName);
            //IFeatureClassContainer可以管理IFeatureDataset中的每个要素类
            IFeatureClassContainer pFeatClassContainer = (IFeatureClassContainer)pFeatureDataset;

            ClearAllData();    //新增删除数据

            //对CAD文件中的要素进行遍历处理
            for (int i = 0; i < pFeatClassContainer.ClassCount; i++)
            {
                IFeatureClass pFeatClass = pFeatClassContainer.get_Class(i);
                //如果是注记，则添加注记层
                if (pFeatClass.FeatureType == esriFeatureType.esriFTCoverageAnnotation)
                {
                    pFeatureLayer = new CadAnnotationLayerClass();
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    pFeatureLayer.FeatureClass = pFeatClass;
                    mainMapControl.Map.AddLayer(pFeatureLayer);
                }
                else //如果是点、线、面则添加要素层
                {
                    pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.Name = pFeatClass.AliasName;
                    pFeatureLayer.FeatureClass = pFeatClass;
                    mainMapControl.Map.AddLayer(pFeatureLayer);
                }
                mainMapControl.ActiveView.Refresh();
            }
            //同步鹰眼
            SynchronizeEagleEye();
        }
        #endregion

        #region 把CAD作为栅格地图进行加载
        private void btnAddRasterByCAD_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pCadWorkspaceFactory;
            IWorkspace pWorkspace;
            ICadDrawingWorkspace pCadDrawingWorkspace;
            ICadDrawingDataset pCadDrawingDataset;
            ICadLayer pCadLayer;

            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "CAD(*.dwg)|*.dwg";
            pOpenFileDialog.Title = "打开CAD数据文件";
            pOpenFileDialog.ShowDialog();

            string pFullPath = pOpenFileDialog.FileName;
            if (pFullPath == "")
            {
                return;
            }
            //获取文件名和文件路径
            int pIndex = pFullPath.LastIndexOf("\\");
            string pFilePath = pFullPath.Substring(0, pIndex);
            string pFileName = pFullPath.Substring(pIndex + 1);
            pCadWorkspaceFactory = new CadWorkspaceFactoryClass();

            pWorkspace = pCadWorkspaceFactory.OpenFromFile(pFilePath, 0);
            pCadDrawingWorkspace = (ICadDrawingWorkspace)pWorkspace;
            //获得CAD文件的数据集
            pCadDrawingDataset = pCadDrawingWorkspace.OpenCadDrawingDataset(pFileName);
            pCadLayer = new CadLayerClass();
            pCadLayer.CadDrawingDataset = pCadDrawingDataset;

            mainMapControl.Map.AddLayer(pCadLayer);
            mainMapControl.ActiveView.Refresh();
        }
        #endregion

        #region 加载personGeodatabase
        private void btnAddPersonGeodatabase_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pAccessWorkspaceFactory;

            OpenFileDialog pOpenFileDialog = new OpenFileDialog();
            pOpenFileDialog.Filter = "Personal Geodatabase(*.mdb)|*.mdb";
            pOpenFileDialog.Title = "打开PersonGeodatabase文件";
            pOpenFileDialog.ShowDialog();

            string pFullPath = pOpenFileDialog.FileName;
            if (pFullPath == "")
            {
                return;
            }
            pAccessWorkspaceFactory = new AccessWorkspaceFactory(); //using ESRI.ArcGIS.DataSourcesGDB;
            //获取工作空间
            IWorkspace pWorkspace = pAccessWorkspaceFactory.OpenFromFile(pFullPath, 0);

            ClearAllData();    //新增删除数据

            //加载工作空间里的数据
            AddAllDataset(pWorkspace, mainMapControl);
        }
        #endregion

        #region 加载文件地理库
        private void btnAddFileDatabase_Click(object sender, EventArgs e)
        {
            IWorkspaceFactory pFileGDBWorkspaceFactory;

            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            string pFullPath = dlg.SelectedPath;

            if (pFullPath == "")
            {
                return;
            }
            pFileGDBWorkspaceFactory = new FileGDBWorkspaceFactoryClass(); //using ESRI.ArcGIS.DataSourcesGDB;

            ClearAllData();    //新增删除数据

            //获取工作空间
            IWorkspace pWorkspace = pFileGDBWorkspaceFactory.OpenFromFile(pFullPath, 0);
            AddAllDataset(pWorkspace, mainMapControl);
        }
        #endregion

        #region 加载SDE数据库
        /// <summary>
        /// 服务器连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSDEByService_Click(object sender, EventArgs e)
        {
            IWorkspace pWorkspace;
            pWorkspace = arcSDEWorkspaceOpen("192.168.70.110", "esri_sde", "sde", "sde", "", "SDE.DEFAULT");

            //如果工作空间不为空则进行加载
            if (pWorkspace != null)
            {
                AddAllDataset(pWorkspace, mainMapControl);
            }
        }

        /// <summary>
        /// 直连
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddSDEByDirect_Click(object sender, EventArgs e)
        {
            IWorkspace pWorkspace;
            pWorkspace = arcSDEWorkspaceOpen("", "sde:oracle11g:orcl", "sde", "sde", "", "SDE.DEFAULT");

            //如果工作空间不为空则进行加载
            if (pWorkspace != null)
            {
                AddAllDataset(pWorkspace, mainMapControl);
            }
        }

        /// <summary>
        /// 通过SDE连接打开SDE数据库
        /// </summary>
        /// <param name="server">服务器IP</param>
        /// <param name="instance">数据库实例，应用服务器连接为：5151或esri_sde，直连为sde:oracle11g:orcl(orcl为服务名)</param>
        /// <param name="user">SDE用户名</param>
        /// <param name="password">用户密码</param>
        /// <param name="database">数据库</param>
        /// <param name="version">SDE版本，缺省为"SDE.DEFAULT"</param>
        /// <returns></returns>
        private IWorkspace arcSDEWorkspaceOpen(string server, string instance, string user, string password, string database, string version)
        {
            IWorkspace pWorkSpace = null;
            //创建和实例化数据集
            IPropertySet pPropertySet = new PropertySetClass();
            pPropertySet.SetProperty("SERVER", server);
            pPropertySet.SetProperty("INSTANCE", instance);
            pPropertySet.SetProperty("USER", user);
            pPropertySet.SetProperty("PASSWORD", password);
            pPropertySet.SetProperty("DATABASE", database);
            pPropertySet.SetProperty("VERSION", version);
            IWorkspaceFactory2 pWorkspaceFactory = new SdeWorkspaceFactoryClass();

            try
            {
                pWorkSpace = pWorkspaceFactory.Open(pPropertySet, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return pWorkSpace;
        }
        #endregion


        #endregion

        #region 保存
        //保存地图
        private void btnSaveMap_Click(object sender, EventArgs e)
        {
            try
            {
                string sMxdFileName = mainMapControl.DocumentFilename;
                IMapDocument pMapDocument = new MapDocumentClass();
                if (sMxdFileName != null && mainMapControl.CheckMxFile(sMxdFileName))
                {
                    if (pMapDocument.get_IsReadOnly(sMxdFileName))
                    {
                        MessageBox.Show("本地图文档是只读的，不能保存!");
                        pMapDocument.Close();
                        return;
                    }
                }
                else
                {
                    SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                    pSaveFileDialog.Title = "请选择保存路径";
                    pSaveFileDialog.OverwritePrompt = true;
                    pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                    pSaveFileDialog.RestoreDirectory = true;
                    if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sMxdFileName = pSaveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                pMapDocument.New(sMxdFileName);
                pMapDocument.ReplaceContents(mainMapControl.Map as IMxdContents);
                pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                pMapDocument.Close();
                MessageBox.Show("保存地图文档成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //ICommand command = new ControlsSaveAsDocCommandClass();
            //command.OnCreate(mainMapControl.Object);
            //command.OnClick();
        }

        //地图另存为
        private void btnSaveAsMap_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                pSaveFileDialog.Title = "另存为";
                pSaveFileDialog.OverwritePrompt = true;
                pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                pSaveFileDialog.RestoreDirectory = true;
                if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sFilePath = pSaveFileDialog.FileName;

                    IMapDocument pMapDocument = new MapDocumentClass();
                    pMapDocument.New(sFilePath);
                    pMapDocument.ReplaceContents(mainMapControl.Map as IMxdContents);
                    pMapDocument.Save(true, true);
                    pMapDocument.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 地图浏览
        //拉框放大
        string pMouseOperate = null;
        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            mainMapControl.CurrentTool = null;
            pMouseOperate = "ZoomIn";
            mainMapControl.MousePointer = esriControlsMousePointer.esriPointerZoomIn;
        }

        //拉框缩小
        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            //mainMapControl.CurrentTool = null;
            //pMouseOperate = "ZoomOut";
            //mainMapControl.MousePointer = esriControlsMousePointer.esriPointerZoomOut;

            //IE.PutCoords();
            #region 自定义拉框缩小
            IEnvelope trackExtent, currentExtent, NewIEN = null;
            currentExtent = mainMapControl.Extent;
            trackExtent = mainMapControl.TrackRectangle();
            double dXmin = 0, dYmin = 0, dXmax = 0, dYmax = 0, dHeight = 0, dWidth = 0;
            dWidth = currentExtent.Width * (currentExtent.Width / trackExtent.Width);
            dHeight = currentExtent.Height * (currentExtent.Height / trackExtent.Height);
            dXmin = currentExtent.XMin - ((trackExtent.XMin - currentExtent.XMin) * (currentExtent.Width / trackExtent.Width));
            dYmin = currentExtent.YMin - ((trackExtent.YMin - currentExtent.YMin) * (currentExtent.Height / trackExtent.Height));
            dXmax = dXmin + dWidth;
            dYmax = dYmin + dHeight;

            NewIEN = new EnvelopeClass();
            NewIEN.PutCoords(dXmin, dYmin, dXmax, dYmax);
            mainMapControl.Extent = NewIEN;
            #endregion
        }


        //逐级放大
        private void btnZoomInStep_Click(object sender, EventArgs e)
        {
            mainMapControl.CurrentTool = null;
            //pMouseOperate = "ZoomIn";
            mainMapControl.MousePointer = esriControlsMousePointer.esriPointerZoomIn;
            IEnvelope pEnvelope;
            pEnvelope = mainMapControl.Extent;
            pEnvelope.Expand(0.5, 0.5, true);     //这里设置放大为2倍，可以根据需要具体设置
            mainMapControl.Extent = pEnvelope;
            mainMapControl.ActiveView.Refresh();
        }

        //逐级缩小
        private void btnZoomOutStep_Click(object sender, EventArgs e)
        {
            //IEnvelope pEnvelope;
            //pEnvelope = mainMapControl.Extent;
            //pEnvelope.Expand(1.5, 1.5, true);
            //mainMapControl.Extent = pEnvelope;
            //mainMapControl.ActiveView.Refresh();

            IActiveView pActiveView = mainMapControl.ActiveView;
            IPoint centerPoint = new PointClass();
            centerPoint.PutCoords((pActiveView.Extent.XMin + pActiveView.Extent.XMax) / 2, (pActiveView.Extent.YMax + pActiveView.Extent.YMin) / 2);
            IEnvelope envlope = pActiveView.Extent;
            envlope.Expand(1.5, 1.5, true);       //和放大的区别在于Expand函数的参数不同
            pActiveView.Extent.CenterAt(centerPoint);
            pActiveView.Extent = envlope;
            pActiveView.Refresh();
        }

        //漫游
        private void btnPan_Click(object sender, EventArgs e)
        {
            mainMapControl.CurrentTool = null;
            pMouseOperate = "Pan";
            mainMapControl.MousePointer = esriControlsMousePointer.esriPointerPan;
        }

        IExtentStack pExtentStack;
        //前一视图       


        //后一视图


        //全图显示
        private void btnFullView_Click(object sender, EventArgs e)
        {
            mainMapControl.Extent = mainMapControl.FullExtent;
        }
        #endregion

        #region 要素选择
        //要素选择
        private void btnSelFeature_Click(object sender, EventArgs e)
        {
            //bSelectFeature = true;
            #region 调用类库资源
            mainMapControl.CurrentTool = null;
            ControlsSelectFeaturesTool pTool = new ControlsSelectFeaturesToolClass();
            pTool.OnCreate(mainMapControl.Object);
            mainMapControl.CurrentTool = pTool as ITool;
            #endregion
            //pMouseOperate = "SelFeature";
        }

        //缩放至选择
        private void btnZoomToSel_Click(object sender, EventArgs e)
        {
            #region 调用类库资源
            //ICommand pCommand = new ESRI.ArcGIS.Controls.ControlsZoomToSelectedCommandClass();
            //pCommand.OnCreate(mainMapControl.Object);
            //pCommand.OnClick();
            #endregion

            int nSlection = mainMapControl.Map.SelectionCount;
            if (nSlection == 0)
            {
                MessageBox.Show("请先选择要素！", "提示");
            }
            else
            {
                ISelection selection = mainMapControl.Map.FeatureSelection;
                IEnumFeature enumFeature = (IEnumFeature)selection;
                enumFeature.Reset();
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeature pFeature = enumFeature.Next();

                while (pFeature != null)
                {

                    pEnvelope.Union(pFeature.Extent);
                    pFeature = enumFeature.Next();
                }
                pEnvelope.Expand(1.1, 1.1, true);
                mainMapControl.ActiveView.Extent = pEnvelope;
                mainMapControl.ActiveView.Refresh();
            }
        }
        //要素截图
        private void btnScreenshot_Click(object sender, EventArgs e)
        {
            //mainMapControl.CurrentTool = null;
            //pMouseOperate = "Screenshot";
            //mainMapControl.MousePointer = esriControlsMousePointer.esriPointerZoomIn;

        }
        //清除选择
        private void btnClearSel_Click(object sender, EventArgs e)
        {
            #region 调用类库资源
            //ICommand pCommand = new ESRI.ArcGIS.Controls.ControlsClearSelectionCommandClass();
            //pCommand.OnCreate(mainMapControl.Object);
            //pCommand.OnClick();
            #endregion

            IActiveView pActiveView = mainMapControl.ActiveView;
            pActiveView.FocusMap.ClearSelection();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);
        }
        #endregion

        #region mainMapControl事件
        private void mainMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 4)//中键
            {
                mainMapControl.Pan();
            }
            if (t == 1)
            {
                if (e.button == 1)//左键
                {
                    numMouseDown++;  // 记录一次左键点击操作 
                    // 绘制第一个点
                    if (activeView == null)
                    {
                        activeView = mainMapControl.ActiveView;
                    }
                    IPoint point = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(Control.MousePosition.X - 500, Control.MousePosition.Y - 55);
                    // 如果多边形是空的
                    if (polygonFeedback == null)
                    {
                        // 提取多边形的边
                        polygonFeedback = new NewPolygonFeedbackClass();
                        polylineFeedback = new NewLineFeedbackClass();//创建线段
                        polygonFeedback.Display = activeView.ScreenDisplay;
                        // 将光标点作为选框的第一个点
                        polygonFeedback.Start(point);
                        polylineFeedback.Start(point);
                    }
                    else
                    {
                        // 选框已经存在则加入光标点
                        polygonFeedback.AddPoint(point);
                        polylineFeedback.AddPoint(point);
                    }
                }

            }

            if (t == 0)
            {
                pPointPt = (mainMapControl.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);

                if (e.button == 1)
                {


                    IActiveView pActiveView = mainMapControl.ActiveView;
                    IEnvelope pEnvelope = new EnvelopeClass();

                    switch (pMouseOperate)
                    {
                        #region 拉框放大

                        case "ZoomIn":
                            pEnvelope = mainMapControl.TrackRectangle();
                            //如果拉框范围为空则返回
                            if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                            {
                                return;
                            }
                            //如果有拉框范围，则放大到拉框范围
                            pActiveView.Extent = pEnvelope;
                            pActiveView.Refresh();
                            break;

                        #endregion

                        #region 拉框缩小

                        case "ZoomOut":
                            pEnvelope = mainMapControl.TrackRectangle();

                            //如果拉框范围为空则退出
                            if (pEnvelope == null || pEnvelope.IsEmpty || pEnvelope.Height == 0 || pEnvelope.Width == 0)
                            {
                                return;
                            }
                            //如果有拉框范围，则以拉框范围为中心，缩小倍数为：当前视图范围/拉框范围
                            else
                            {
                                double dWidth = pActiveView.Extent.Width * pActiveView.Extent.Width / pEnvelope.Width;
                                double dHeight = pActiveView.Extent.Height * pActiveView.Extent.Height / pEnvelope.Height;
                                double dXmin = pActiveView.Extent.XMin -
                                               ((pEnvelope.XMin - pActiveView.Extent.XMin) * pActiveView.Extent.Width /
                                                pEnvelope.Width);
                                double dYmin = pActiveView.Extent.YMin -
                                               ((pEnvelope.YMin - pActiveView.Extent.YMin) * pActiveView.Extent.Height /
                                                pEnvelope.Height);
                                double dXmax = dXmin + dWidth;
                                double dYmax = dYmin + dHeight;
                                pEnvelope.PutCoords(dXmin, dYmin, dXmax, dYmax);
                            }
                            pActiveView.Extent = pEnvelope;
                            pActiveView.Refresh();
                            break;

                        #endregion
                        #region 要素截图

                        case "Screenshot":

                            break;

                        #endregion
                        #region 漫游

                        case "Pan":
                            mainMapControl.Pan();
                            break;

                        #endregion

                        #region 选择要素

                        case "SelFeature":
                            IEnvelope pEnv = mainMapControl.TrackRectangle();
                            IGeometry pGeo = pEnv as IGeometry;
                            //矩形框若为空，即为点选时，对点范围进行扩展
                            if (pEnv.IsEmpty == true)
                            {
                                tagRECT r;
                                r.left = e.x - 5;
                                r.top = e.y - 5;
                                r.right = e.x + 5;
                                r.bottom = e.y + 5;
                                pActiveView.ScreenDisplay.DisplayTransformation.TransformRect(pEnv, ref r, 4);
                                pEnv.SpatialReference = pActiveView.FocusMap.SpatialReference;
                            }
                            pGeo = pEnv as IGeometry;
                            mainMapControl.Map.SelectByShape(pGeo, null, false);
                            mainMapControl.Refresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                            break;

                        #endregion



                        #region 距离量算
                        case "MeasureLength":
                            //判断追踪线对象是否为空，若是则实例化并设置当前鼠标点为起始点
                            if (pNewLineFeedback == null)
                            {
                                //实例化追踪线对象
                                pNewLineFeedback = new NewLineFeedbackClass();
                                pNewLineFeedback.Display = (mainMapControl.Map as IActiveView).ScreenDisplay;
                                //设置起点，开始动态线绘制
                                pNewLineFeedback.Start(pPointPt);
                                dToltalLength = 0;
                            }
                            else //如果追踪线对象不为空，则添加当前鼠标点
                            {
                                pNewLineFeedback.AddPoint(pPointPt);
                            }
                            //pGeometry = m_PointPt;
                            if (dSegmentLength != 0)
                            {
                                dToltalLength = dToltalLength + dSegmentLength;
                            }
                            break;
                        #endregion

                        #region 面积量算
                        case "MeasureArea":
                            if (pNewPolygonFeedback == null)
                            {
                                //实例化追踪面对象
                                pNewPolygonFeedback = new NewPolygonFeedback();
                                pNewPolygonFeedback.Display = (mainMapControl.Map as IActiveView).ScreenDisplay;
                                ;
                                pAreaPointCol.RemovePoints(0, pAreaPointCol.PointCount);
                                //开始绘制多边形
                                pNewPolygonFeedback.Start(pPointPt);
                                pAreaPointCol.AddPoint(pPointPt, ref missing, ref missing);
                            }
                            else
                            {
                                pNewPolygonFeedback.AddPoint(pPointPt);
                                pAreaPointCol.AddPoint(pPointPt, ref missing, ref missing);
                            }
                            break;
                        #endregion

                        #region 要素选择
                        case "SelectFeature":
                            IPoint point = new PointClass();
                            IGeometry pGeometry = point as IGeometry;
                            mainMapControl.Map.SelectByShape(pGeometry, null, false);
                            mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
                            break;
                        #endregion

                        default:
                            break;
                    }
                }
                else if (e.button == 2)//右键
                {
                    pMouseOperate = "";
                    mainMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }

            }
        }

        private void mainMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {

            if (t == 1)
            {

                //if (e.button == 1)
                {
                    if (numMouseDown == 0) return;
                    // 如果多边形不为空
                    if (polygonFeedback != null)
                    {
                        if (activeView == null)
                        {
                            activeView = (IActiveView)m_hookHelper.FocusMap;
                        }
                        // 将选框的最后一个点与光标点相连，但是该点不加入选框
                        IPoint point = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(Control.MousePosition.X - 500, Control.MousePosition.Y - 55);
                        polygonFeedback.MoveTo(point);
                    }
                }
            }
            if (t == 0)
            {

                sMapUnits = GetMapUnit(mainMapControl.Map.MapUnits);
                barCoorTxt.Text = String.Format("当前坐标：X = {0:#.###} Y = {1:#.###} {2}", e.mapX, e.mapY, sMapUnits);
                pMovePt = (mainMapControl.Map as IActiveView).ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);



            }
        }

        private void mainMapControl_OnDoubleClick(object sender, IMapControlEvents2_OnDoubleClickEvent e)
        {








        }
        #endregion

        #region 封装的方法
        /// <summary>
        /// 加载工作空间里面的要素和栅格数据
        /// </summary>
        /// <param name="pWorkspace"></param>
        private void AddAllDataset(IWorkspace pWorkspace, AxMapControl mapControl)
        {
            IEnumDataset pEnumDataset = pWorkspace.get_Datasets(ESRI.ArcGIS.Geodatabase.esriDatasetType.esriDTAny);
            pEnumDataset.Reset();
            //将Enum数据集中的数据一个个读到DataSet中
            IDataset pDataset = pEnumDataset.Next();
            //判断数据集是否有数据
            while (pDataset != null)
            {
                if (pDataset is IFeatureDataset)  //要素数据集
                {
                    IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                    IFeatureDataset pFeatureDataset = pFeatureWorkspace.OpenFeatureDataset(pDataset.Name);
                    IEnumDataset pEnumDataset1 = pFeatureDataset.Subsets;
                    pEnumDataset1.Reset();
                    IGroupLayer pGroupLayer = new GroupLayerClass();
                    pGroupLayer.Name = pFeatureDataset.Name;
                    IDataset pDataset1 = pEnumDataset1.Next();
                    while (pDataset1 != null)
                    {
                        if (pDataset1 is IFeatureClass)  //要素类
                        {
                            IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                            pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(pDataset1.Name);
                            if (pFeatureLayer.FeatureClass != null)
                            {
                                pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                                pGroupLayer.Add(pFeatureLayer);
                                mapControl.Map.AddLayer(pFeatureLayer);
                            }
                        }
                        pDataset1 = pEnumDataset1.Next();
                    }
                }
                else if (pDataset is IFeatureClass) //要素类
                {
                    IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
                    IFeatureLayer pFeatureLayer = new FeatureLayerClass();
                    pFeatureLayer.FeatureClass = pFeatureWorkspace.OpenFeatureClass(pDataset.Name);

                    pFeatureLayer.Name = pFeatureLayer.FeatureClass.AliasName;
                    mapControl.Map.AddLayer(pFeatureLayer);
                }
                else if (pDataset is IRasterDataset) //栅格数据集
                {
                    IRasterWorkspaceEx pRasterWorkspace = (IRasterWorkspaceEx)pWorkspace;
                    IRasterDataset pRasterDataset = pRasterWorkspace.OpenRasterDataset(pDataset.Name);
                    //影像金字塔判断与创建
                    IRasterPyramid3 pRasPyrmid;
                    pRasPyrmid = pRasterDataset as IRasterPyramid3;
                    if (pRasPyrmid != null)
                    {
                        if (!(pRasPyrmid.Present))
                        {
                            pRasPyrmid.Create(); //创建金字塔
                        }
                    }
                    IRasterLayer pRasterLayer = new RasterLayerClass();
                    pRasterLayer.CreateFromDataset(pRasterDataset);
                    ILayer pLayer = pRasterLayer as ILayer;
                    mapControl.AddLayer(pLayer, 0);
                }
                pDataset = pEnumDataset.Next();
            }

            mapControl.ActiveView.Refresh();
            //同步鹰眼
            SynchronizeEagleEye();
        }

        private void ClearAllData()
        {
            if (mainMapControl.Map != null && mainMapControl.Map.LayerCount > 0)
            {
                //新建mainMapControl中Map
                IMap dataMap = new MapClass();
                dataMap.Name = "Map";
                mainMapControl.DocumentFilename = string.Empty;
                mainMapControl.Map = dataMap;

                //新建EagleEyeMapControl中Map
                IMap eagleEyeMap = new MapClass();
                eagleEyeMap.Name = "eagleEyeMap";
                EagleEyeMapControl.DocumentFilename = string.Empty;
                EagleEyeMapControl.Map = eagleEyeMap;
            }
        }

        /// <summary>
        /// 获取RGB颜色
        /// </summary>
        /// <param name="intR">红</param>
        /// <param name="intG">绿</param>
        /// <param name="intB">蓝</param>
        /// <returns></returns>
        private IRgbColor GetRgbColor(int intR, int intG, int intB)
        {
            IRgbColor pRgbColor = null;
            if (intR < 0 || intR > 255 || intG < 0 || intG > 255 || intB < 0 || intB > 255)
            {
                return pRgbColor;
            }
            pRgbColor = new RgbColorClass();
            pRgbColor.Red = intR;
            pRgbColor.Green = intG;
            pRgbColor.Blue = intB;
            return pRgbColor;
        }

        /// <summary>
        /// 获取地图单位
        /// </summary>
        /// <param name="_esriMapUnit"></param>
        /// <returns></returns>
        private string GetMapUnit(esriUnits _esriMapUnit)
        {
            string sMapUnits = string.Empty;
            switch (_esriMapUnit)
            {
                case esriUnits.esriCentimeters:
                    sMapUnits = "厘米";
                    break;
                case esriUnits.esriDecimalDegrees:
                    sMapUnits = "十进制";
                    break;
                case esriUnits.esriDecimeters:
                    sMapUnits = "分米";
                    break;
                case esriUnits.esriFeet:
                    sMapUnits = "尺";
                    break;
                case esriUnits.esriInches:
                    sMapUnits = "英寸";
                    break;
                case esriUnits.esriKilometers:
                    sMapUnits = "千米";
                    break;
                case esriUnits.esriMeters:
                    sMapUnits = "米";
                    break;
                case esriUnits.esriMiles:
                    sMapUnits = "英里";
                    break;
                case esriUnits.esriMillimeters:
                    sMapUnits = "毫米";
                    break;
                case esriUnits.esriNauticalMiles:
                    sMapUnits = "海里";
                    break;
                case esriUnits.esriPoints:
                    sMapUnits = "点";
                    break;
                case esriUnits.esriUnitsLast:
                    sMapUnits = "UnitsLast";
                    break;
                case esriUnits.esriUnknownUnits:
                    sMapUnits = "未知单位";
                    break;
                case esriUnits.esriYards:
                    sMapUnits = "码";
                    break;
                default:
                    break;
            }
            return sMapUnits;
        }

        /// <summary>
        /// 绘制多边形
        /// </summary>
        /// <param name="mapCtrl"></param>
        /// <returns></returns>
        public IPolygon DrawPolygon(AxMapControl mapCtrl)
        {
            IGeometry pGeometry = null;
            if (mapCtrl == null) return null;
            IRubberBand rb = new RubberPolygonClass();
            pGeometry = rb.TrackNew(mapCtrl.ActiveView.ScreenDisplay, null);
            return pGeometry as IPolygon;
        }
        #endregion

        #region 鹰眼的实现及同步
        private void mainMapControl_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            SynchronizeEagleEye();
        }

        private void SynchronizeEagleEye()
        {
            if (EagleEyeMapControl.LayerCount > 0)
            {
                EagleEyeMapControl.ClearLayers();
            }
            //设置鹰眼和主地图的坐标系统一致
            EagleEyeMapControl.SpatialReference = mainMapControl.SpatialReference;
            for (int i = mainMapControl.LayerCount - 1; i >= 0; i--)
            {
                //使鹰眼视图与数据视图的图层上下顺序保持一致
                ILayer pLayer = mainMapControl.get_Layer(i);
                if (pLayer is IGroupLayer || pLayer is ICompositeLayer)
                {
                    ICompositeLayer pCompositeLayer = (ICompositeLayer)pLayer;
                    for (int j = pCompositeLayer.Count - 1; j >= 0; j--)
                    {
                        ILayer pSubLayer = pCompositeLayer.get_Layer(j);
                        IFeatureLayer pFeatureLayer = pSubLayer as IFeatureLayer;
                        if (pFeatureLayer != null)
                        {
                            //由于鹰眼地图较小，所以过滤点图层不添加
                            if (pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint
                                && pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryMultipoint)
                            {
                                EagleEyeMapControl.AddLayer(pLayer);
                            }
                        }
                    }
                }
                else
                {
                    IFeatureLayer pFeatureLayer = pLayer as IFeatureLayer;
                    if (pFeatureLayer != null)
                    {
                        if (pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryPoint
                            && pFeatureLayer.FeatureClass.ShapeType != esriGeometryType.esriGeometryMultipoint)
                        {
                            EagleEyeMapControl.AddLayer(pLayer);
                        }
                    }
                }
                //设置鹰眼地图全图显示  

                EagleEyeMapControl.Extent = mainMapControl.FullExtent;
                pEnv = mainMapControl.Extent as IEnvelope;
                DrawRectangle(pEnv);
                EagleEyeMapControl.ActiveView.Refresh();
            }
        }

        private void EagleEyeMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (EagleEyeMapControl.Map.LayerCount > 0)
            {
                //按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    //如果指针落在鹰眼的矩形框中，标记可移动
                    if (e.mapX > pEnv.XMin && e.mapY > pEnv.YMin && e.mapX < pEnv.XMax && e.mapY < pEnv.YMax)
                    {
                        bCanDrag = true;
                    }
                    pMoveRectPoint = new PointClass();
                    pMoveRectPoint.PutCoords(e.mapX, e.mapY);  //记录点击的第一个点的坐标
                }
                //按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelope = EagleEyeMapControl.TrackRectangle();

                    IPoint pTempPoint = new PointClass();
                    pTempPoint.PutCoords(pEnvelope.XMin + pEnvelope.Width / 2, pEnvelope.YMin + pEnvelope.Height / 2);
                    mainMapControl.Extent = pEnvelope;
                    //矩形框的高宽和数据试图的高宽不一定成正比，这里做一个中心调整
                    mainMapControl.CenterAt(pTempPoint);
                }
            }
        }

        //移动矩形框
        private void EagleEyeMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.mapX > pEnv.XMin && e.mapY > pEnv.YMin && e.mapX < pEnv.XMax && e.mapY < pEnv.YMax)
            {
                //如果鼠标移动到矩形框中，鼠标换成小手，表示可以拖动
                EagleEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerHand;
                if (e.button == 2)  //如果在内部按下鼠标右键，将鼠标演示设置为默认样式
                {
                    EagleEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
                }
            }
            else
            {
                //在其他位置将鼠标设为默认的样式
                EagleEyeMapControl.MousePointer = esriControlsMousePointer.esriPointerDefault;
            }

            if (bCanDrag)
            {
                double Dx, Dy;  //记录鼠标移动的距离
                Dx = e.mapX - pMoveRectPoint.X;
                Dy = e.mapY - pMoveRectPoint.Y;
                pEnv.Offset(Dx, Dy); //根据偏移量更改 pEnv 位置
                pMoveRectPoint.PutCoords(e.mapX, e.mapY);
                DrawRectangle(pEnv);
                mainMapControl.Extent = pEnv;
            }
        }

        private void EagleEyeMapControl_OnMouseUp(object sender, IMapControlEvents2_OnMouseUpEvent e)
        {
            if (e.button == 1 && pMoveRectPoint != null)
            {
                if (e.mapX == pMoveRectPoint.X && e.mapY == pMoveRectPoint.Y)
                {
                    mainMapControl.CenterAt(pMoveRectPoint);
                }
                bCanDrag = false;
            }
        }

        //绘制矩形框
        private void mainMapControl_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            //得到当前视图范围
            pEnv = (IEnvelope)e.newEnvelope;
            DrawRectangle(pEnv);
        }

        //在鹰眼地图上面画矩形框
        private void DrawRectangle(IEnvelope pEnvelope)
        {
            //在绘制前，清除鹰眼中之前绘制的矩形框
            IGraphicsContainer pGraphicsContainer = EagleEyeMapControl.Map as IGraphicsContainer;
            IActiveView pActiveView = pGraphicsContainer as IActiveView;
            pGraphicsContainer.DeleteAllElements();
            //得到当前视图范围
            IRectangleElement pRectangleElement = new RectangleElementClass();
            IElement pElement = pRectangleElement as IElement;
            pElement.Geometry = pEnvelope;
            //设置矩形框（实质为中间透明度面）
            IRgbColor pColor = new RgbColorClass();
            pColor = GetRgbColor(255, 0, 0);
            pColor.Transparency = 255;
            ILineSymbol pOutLine = new SimpleLineSymbolClass();
            pOutLine.Width = 2;
            pOutLine.Color = pColor;

            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pColor = new RgbColorClass();
            pColor.Transparency = 0;
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutLine;
            //向鹰眼中添加矩形框
            IFillShapeElement pFillShapeElement = pElement as IFillShapeElement;
            pFillShapeElement.Symbol = pFillSymbol;
            pGraphicsContainer.AddElement((IElement)pFillShapeElement, 0);
            //刷新
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        #endregion

        #region 布局视图与数据视图的同步
        private void CopyToPageLayout()
        {
            IObjectCopy pObjectCopy = new ObjectCopyClass();
            object copyFromMap = mainMapControl.Map;
            object copiedMap = pObjectCopy.Copy(copyFromMap);//复制地图到copiedMap中
            object copyToMap = axPageLayoutControl.ActiveView.FocusMap;
            pObjectCopy.Overwrite(copiedMap, ref copyToMap); //复制地图
            axPageLayoutControl.ActiveView.Refresh();
        }

        private void mainMapControl_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            IActiveView pActiveView = (IActiveView)axPageLayoutControl.ActiveView.FocusMap;
            IDisplayTransformation displayTransformation = pActiveView.ScreenDisplay.DisplayTransformation;
            displayTransformation.VisibleBounds = mainMapControl.Extent;
            axPageLayoutControl.ActiveView.Refresh();
            CopyToPageLayout();
        }
        #endregion

        #region TOC右键菜单的添加及功能实现
        private ESRI.ArcGIS.Geometry.Point pMoveLayerPoint = new ESRI.ArcGIS.Geometry.Point();  //鼠标在TOC中左键按下时点的位置
        //TOC右键菜单的添加
        private void axTOCControl_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            try
            {
                if (e.button == 2)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pMap = null;
                    ILayer pLayer = null;
                    object unk = null;
                    object data = null;
                    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                    pTocFeatureLayer = pLayer as IFeatureLayer;
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer && pTocFeatureLayer != null)
                    {

                        contextMenuStrip.Show(Control.MousePosition);
                    }
                }
                if (e.button == 1)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pMap = null; object unk = null;
                    object data = null; ILayer pLayer = null;
                    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pMap, ref pLayer, ref unk, ref data);
                    if (pLayer == null) return;

                    pMoveLayerPoint.PutCoords(e.x, e.y);
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer)
                    {
                        if (pLayer is IAnnotationSublayer)
                        {
                            return;
                        }
                        else
                        {
                            pMoveLayer = pLayer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void axTOCControl_OnMouseUp(object sender, ITOCControlEvents_OnMouseUpEvent e)
        {
            try
            {
                if (e.button == 1 && pMoveLayer != null && pMoveLayerPoint.Y != e.y)
                {
                    esriTOCControlItem pItem = esriTOCControlItem.esriTOCControlItemNone;
                    IBasicMap pBasicMap = null; object unk = null;
                    object data = null; ILayer pLayer = null;
                    axTOCControl.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pLayer, ref unk, ref data);
                    IMap pMap = mainMapControl.ActiveView.FocusMap;
                    if (pItem == esriTOCControlItem.esriTOCControlItemLayer || pLayer != null)
                    {
                        if (pMoveLayer != pLayer)
                        {
                            ILayer pTempLayer;
                            //获得鼠标弹起时所在图层的索引号
                            for (int i = 0; i < pMap.LayerCount; i++)
                            {
                                pTempLayer = pMap.get_Layer(i);
                                if (pTempLayer == pLayer)
                                {
                                    toIndex = i;
                                }
                            }
                        }
                    }
                    //移动到最前面
                    else if (pItem == esriTOCControlItem.esriTOCControlItemMap)
                    {
                        toIndex = 0;
                    }
                    //移动到最后面
                    else if (pItem == esriTOCControlItem.esriTOCControlItemNone)
                    {
                        toIndex = pMap.LayerCount - 1;
                    }
                    pMap.MoveLayer(pMoveLayer, toIndex);
                    mainMapControl.ActiveView.Refresh();
                    axTOCControl.Update();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // 属性表窗口
        private void btnAttribute_Click(object sender, EventArgs e)
        {
            if (frmAttribute == null || frmAttribute.IsDisposed)
            {
                frmAttribute = new FormAtrribute();
            }
            frmAttribute.CurFeatureLayer = pTocFeatureLayer;
            frmAttribute.InitUI();
            frmAttribute.ShowDialog();
        }

        //缩放到图层
        private void btnZoomToLayer_Click(object sender, EventArgs e)
        {
            if (pTocFeatureLayer == null) return;
            (mainMapControl.Map as IActiveView).Extent = pTocFeatureLayer.AreaOfInterest;
            (mainMapControl.Map as IActiveView).PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        // 移除图层
        private void btnRemoveLayer_Click(object sender, EventArgs e)
        {
            try
            {
                if (pTocFeatureLayer == null) return;
                DialogResult result = MessageBox.Show("是否删除[" + pTocFeatureLayer.Name + "]图层", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (result == DialogResult.OK)
                {
                    mainMapControl.Map.DeleteLayer(pTocFeatureLayer);
                    EagleEyeMapControl.Map.DeleteLayer(pTocFeatureLayer);
                }
                mainMapControl.ActiveView.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region 已封装函数
        private void axPageLayoutControl_OnMouseDown(object sender, IPageLayoutControlEvents_OnMouseDownEvent e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void 另存为ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                pSaveFileDialog.Title = "另存为";
                pSaveFileDialog.OverwritePrompt = true;
                pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                pSaveFileDialog.RestoreDirectory = true;
                if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string sFilePath = pSaveFileDialog.FileName;

                    IMapDocument pMapDocument = new MapDocumentClass();
                    pMapDocument.New(sFilePath);
                    pMapDocument.ReplaceContents(mainMapControl.Map as IMxdContents);
                    pMapDocument.Save(true, true);
                    pMapDocument.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string sMxdFileName = mainMapControl.DocumentFilename;
                IMapDocument pMapDocument = new MapDocumentClass();
                if (sMxdFileName != null && mainMapControl.CheckMxFile(sMxdFileName))
                {
                    if (pMapDocument.get_IsReadOnly(sMxdFileName))
                    {
                        MessageBox.Show("本地图文档是只读的，不能保存!");
                        pMapDocument.Close();
                        return;
                    }
                }
                else
                {
                    SaveFileDialog pSaveFileDialog = new SaveFileDialog();
                    pSaveFileDialog.Title = "请选择保存路径";
                    pSaveFileDialog.OverwritePrompt = true;
                    pSaveFileDialog.Filter = "ArcMap文档（*.mxd）|*.mxd|ArcMap模板（*.mxt）|*.mxt";
                    pSaveFileDialog.RestoreDirectory = true;
                    if (pSaveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sMxdFileName = pSaveFileDialog.FileName;
                    }
                    else
                    {
                        return;
                    }
                }

                pMapDocument.New(sMxdFileName);
                pMapDocument.ReplaceContents(mainMapControl.Map as IMxdContents);
                pMapDocument.Save(pMapDocument.UsesRelativePaths, true);
                pMapDocument.Close();
                MessageBox.Show("保存地图文档成功!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //ICommand command = new ControlsSaveAsDocCommandClass();
            //command.OnCreate(mainMapControl.Object);
            //command.OnClick();
        }

        private void testToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            ToolStripButton tsb = new ToolStripButton(this.listBox1.SelectedIndex.ToString());
            if (this.listBox1.SelectedIndex == -1) return;
            object selObj = this.plugins[this.listBox1.SelectedIndex];
            Type t = selObj.GetType();
            //tsb.Click += new EventHandler(method);
            tsb.Click += delegate (object o, EventArgs ee)
            { method(Convert.ToInt32(tsb.Text)); };
            //tsb.Image = Image.FromFile(@"E:\code\MapOperation\Properties\blank.bmp");
            toolStrip1.Items.Add(tsb);
            toolStrip1.Items[toolStrip1.Items.Count - 1].Visible = true;
            listBox1.Visible = false;

            MethodInfo OnCreate = t.GetMethod("OnCreate");
            object[] para = new object[] { (IMapControl2)mainMapControl.Object };
            OnCreate.Invoke(selObj, para);
        }


        private int tab;

        public void method(int i)
        {
            if (this.listBox1.SelectedIndex == -1) return;
            object selObj = this.plugins[i];
            Type t = selObj.GetType();
            MethodInfo OnClick = t.GetMethod("OnClick");
            // MethodInfo OnShowInfo = t.GetMethod("OnShowInfo");
            OnClick.Invoke(selObj, null);
            //OnClick1.Invoke(selObj, para);
            // object returnValue = OnShowInfo.Invoke(selObj, null);
        }

        private bool TraversalAssembly(Assembly assem)
        {

            Module[] modules = assem.GetModules(false);
            foreach (Module module in modules)
            {
                Type[] types = module.GetTypes();
                foreach (Type type in types)
                {
                    try
                    {
                        Type interface_command = type.GetInterface("ICommand");
                        if (interface_command != null && type.IsClass)
                        {
                            CreateCommandButton(type);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return false;
        }

        private void CreateCommandButton(Type type)
        {
            try
            {
                //创建该命令的***********唯一实例************
                object obj = Activator.CreateInstance(type);
                //QI 
                ICommand m_command = obj as ICommand;
                int subTypeCount;
                if (type.GetInterface("ICommandSubType") != null)
                {
                    //QI for CommandSubType
                    ICommandSubType m_commandSubType = m_command as ICommandSubType;
                    subTypeCount = m_commandSubType.GetCount();
                }
                else
                {
                    subTypeCount = 1;
                }

                for (int i = 1; i <= subTypeCount; i++)
                {
                    CreateSubTypeCommand(obj, i);
                }
            }
            catch (Exception e)
            {
                throw new Exception("未能成功创建命令");
            }
        }

        private void CreateSubTypeCommand(object object_, int iSubType)
        {

            Type type = object_.GetType();
            ICommand m_command = object_ as ICommand;
            m_command.OnCreate(mainMapControl.Object);



            //Create ToolStripButton in C#
            ToolStripButton commandtool = new ToolStripButton();
            //commandtool.DisplayStyle = ToolStripItemDisplayStyle.Image;
            commandtool.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            commandtool.ToolTipText = m_command.Tooltip;
            if (m_command.Bitmap > 0)
            {
                try
                {
                    commandtool.Image = System.Drawing.Image.FromHbitmap(new IntPtr(m_command.Bitmap));
                }
                catch
                {
                    commandtool.DisplayStyle = ToolStripItemDisplayStyle.Text;
                }
            }
            commandtool.Name = m_command.Name;
            commandtool.Text = m_command.Caption;
            //associate the ToolStripButton with tools and buttons defined by ESRI
            commandtool.Enabled = m_command.Enabled;
            commandtool.Tag = m_command.Category;

            //Create ToolStripMenuItem in C#


            commandtool.Click += delegate (object o, EventArgs e)
            {

            };
            commandtool.MouseDown += delegate (object o, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    btn = o as ToolStripButton;
                }

                else if (e.Button == MouseButtons.Left)
                {

                }

            };

            commandtool.MouseUp += delegate (object o, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    try
                    {
                        if (type.GetInterface("ITool") != null)
                        {
                            mainMapControl.CurrentTool = m_command as ITool;
                        }
                        else
                        {
                            m_command.OnClick();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "抛出异常");
                    }
                }

                else if (e.Button == MouseButtons.Right)
                {

                }

            };

            commandtool.MouseMove += delegate (object o, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {

                }
            };

            this.controlList.Add((ToolStripItem)commandtool);

        }

        public List<ToolStripItem> ToolStripItemList
        {
            get { return this.controlList; }
        }

        private void 删除控件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip1.Items.Remove(btn);
        }

        public static void ExportView(IActiveView view, IGeometry pGeo, int OutputResolution, int Width, int Height, string ExpPath)
        {
            //OutputResolution = (short)view.ScreenDisplay.DisplayTransformation.Resolution;
            IExport pExport = null;
            tagRECT exportRect = new tagRECT(); //定义输出的范围
            IEnvelope pEnvelope = pGeo.Envelope;
            string sType = System.IO.Path.GetExtension(ExpPath);
            switch (sType)
            {
                case ".jpg":
                    pExport = new ExportJPEGClass();
                    break;
                case ".bmp":
                    pExport = new ExportBMPClass();
                    break;
                case ".gif":
                    pExport = new ExportGIFClass();
                    break;
                case ".tif":
                    pExport = new ExportTIFFClass();
                    break;
                case ".png":
                    pExport = new ExportPNGClass();
                    break;
                case ".pdf":
                    pExport = new ExportPDFClass();
                    break;
                default:
                    MessageBox.Show("没有输出格式，默认到JPEG格式");
                    pExport = new ExportJPEGClass();
                    break;
            }
            pExport.ExportFileName = ExpPath;
            //pEnvelope控制输出范围,//export控制pixelbounds//outputresolution为dpi dpi = (int)(outRect.Width / pEnvelope.Width);
            exportRect.left = 0; exportRect.top = 0;
            exportRect.right = Width;
            exportRect.bottom = Height;
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords((double)exportRect.left, (double)exportRect.top, (double)exportRect.right, (double)exportRect.bottom);
            pExport.PixelBounds = envelope; //输出像素范围（0,0，width，height）
            view.Output(pExport.StartExporting(), OutputResolution, ref exportRect, pEnvelope, null);//倒数第二个改为pEnvelope可以固定为外接矩形方式输出
            pExport.FinishExporting();
            pExport.Cleanup();
        }
        public static void ExportView_test(IActiveView view, IGeometry pGeo, int OutputResolution, int Width, int Height, string ExpPath)
        {
            OutputResolution = (short)view.ScreenDisplay.DisplayTransformation.Resolution;
            IExport pExport = null;
            tagRECT exportRect = new tagRECT(); //定义输出的范围
            IEnvelope pEnvelope = view.Extent;
            string sType = System.IO.Path.GetExtension(ExpPath);
            switch (sType)
            {
                case ".jpg":
                    pExport = new ExportJPEGClass();
                    break;
                case ".bmp":
                    pExport = new ExportBMPClass();
                    break;
                case ".gif":
                    pExport = new ExportGIFClass();
                    break;
                case ".tif":
                    pExport = new ExportTIFFClass();
                    break;
                case ".png":
                    pExport = new ExportPNGClass();
                    break;
                case ".pdf":
                    pExport = new ExportPDFClass();
                    break;
                default:
                    MessageBox.Show("没有输出格式，默认到JPEG格式");
                    pExport = new ExportJPEGClass();
                    break;
            }
            pExport.ExportFileName = ExpPath;
            //pEnvelope控制输出范围,//export控制pixelbounds//outputresolution为dpi dpi = (int)(outRect.Width / pEnvelope.Width);
            exportRect = view.ScreenDisplay.DisplayTransformation.get_DeviceFrame();
            exportRect.left = 0; exportRect.top = 0;
            exportRect.right = Width;
            exportRect.bottom = Height;
            IEnvelope envelope = new EnvelopeClass();
            envelope.PutCoords((double)exportRect.left, (double)exportRect.top, (double)exportRect.right, (double)exportRect.bottom);
            pExport.PixelBounds = envelope; //输出像素范围（0,0，width，height）
            view.Output(pExport.StartExporting(), OutputResolution, ref exportRect, pEnvelope, null);//倒数第二个改为pEnvelope可以固定为外接矩形方式输出
            pExport.FinishExporting();
            pExport.Cleanup();
        }
        public static bool ExportView_test2(IActiveView activeView, string fileName, IEnvelope env, int dpi)
        {
            //try
            //{
            IExport export = null;
            string filter = fileName.Substring(fileName.LastIndexOf('.'));
            switch (filter)
            {
                case ".jpg":
                    export = new ExportJPEGClass();
                    break;
                case ".bmp":
                    export = new ExportBMPClass();
                    break;
                case ".emf":
                    export = new ExportEMFClass();
                    break;
                case ".gif":
                    export = new ExportGIFClass();
                    break;
                case ".ai":
                    export = new ExportAIClass();
                    break;
                case ".pdf":
                    export = new ExportPDFClass();
                    break;
                case ".png":
                    export = new ExportPNGClass();
                    break;
                case ".eps":
                    export = new ExportPSClass();
                    break;
                case ".svg":
                    export = new ExportSVGClass();
                    break;
                case ".tif":
                    export = new ExportTIFFClass();
                    break;
                default:
                    //MessageBox.Show("输出格式错误");
                    return false;
            }
            double screenResolution = activeView.ScreenDisplay.DisplayTransformation.Resolution;
            tagRECT exportRECT;
            exportRECT.left = 0;
            exportRECT.top = 0;
            int pixw = 0;
            int pixh = 0;
            pixw = (int)(env.Width * (dpi / screenResolution));
            pixh = (int)(env.Height * (dpi / screenResolution));
            exportRECT.right = pixw;
            exportRECT.bottom = pixh;
            IEnvelope pEnv = new EnvelopeClass();
            pEnv.PutCoords(exportRECT.left, exportRECT.bottom, exportRECT.right, exportRECT.top);
            export.PixelBounds = pEnv;
            export.ExportFileName = fileName;
            export.Resolution = dpi;
            IOutputRasterSettings pOutputRasterSettings = activeView.ScreenDisplay.DisplayTransformation as IOutputRasterSettings;
            pOutputRasterSettings.ResampleRatio = 1;
            activeView.Output(export.StartExporting(), dpi, ref exportRECT, env, null);
            export.FinishExporting();
            export.Cleanup();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(export);
            return true;
            // }
            //catch 
            //{
            //    return false;
            //}
        }
        private void 截图ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                IFeature pFeature = null;
                ILayer pLayer = null;
                pLayer = mainMapControl.get_Layer(0);
                pTocFeatureLayer = pLayer as IFeatureLayer;
                IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
                pFeature = pFeatureCursor.NextFeature();
                IFeatureLayerDefinition featureLayerDefinition = pTocFeatureLayer as IFeatureLayerDefinition;
                while (pFeature != null)
                {

                    string ss = "FID =" + pFeature.get_Value(0).ToString();
                    featureLayerDefinition.DefinitionExpression = ss;
                    //MessageBox.Show(pFeature.get_Value(0).ToString());
                    IEnvelope pEnvelope = new EnvelopeClass();
                    pEnvelope.Union(pFeature.Extent);
                    pEnvelope.Expand(1.1, 1.1, true);
                    mainMapControl.ActiveView.Extent = pEnvelope;
                    mainMapControl.ActiveView.Refresh();

                    IActiveView pActiveView = mainMapControl.ActiveView;
                    double x; double y;
                    x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                    y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                    IEnvelope IE = new EnvelopeClass();
                    IE.PutCoords(x - 20, y - 20, x + 20, y + 20);
                    ExportView(mainMapControl.ActiveView, IE, 500, 2500, 1500, @"D:\数据集\" + pFeature.get_Value(0).ToString() + ".jpg");
                    mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                    mainMapControl.ActiveView.Refresh();
                    pFeature = pFeatureCursor.NextFeature();
                }
                featureLayerDefinition.DefinitionExpression = null;
                //释放指针
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);

                MessageBox.Show("OK!");
                //int resolution = int.Parse(cboResolution.Text);  //输出图片的分辨率
                //int width = int.Parse(txtWidth.Text);            //输出图片的宽度，以像素为单位
                //int height = int.Parse(txtHeight.Text);          //输出图片的高度，以像素为单位
                //ExportView(mainMapControl.ActiveView, mainMapControl.ActiveView.Extent, 96, 464, 378, @"E:\数据集\1.jpg");
                //mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                //mainMapControl.ActiveView.Refresh();
            }
            catch (Exception)
            {
                MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void menuStrip1_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e)
        {

        }
        # endregion
        #endregion

        /// <summary>提供偏置方法
        /// 
        /// </summary>
        /// <param name="offset1"></param>
        /// <param name="rate_dis"></param>
        /// <param name="org"></param>
        private void offser_func(ClipperOffset offset1, double rate_dis, ref Paths org)
        {
            offset1 = new ClipperOffset();
            offset1.Clear();
            offset1.AddPaths(org, JoinType.jtMiter, EndType.etClosedPolygon);
            Paths result = new Paths();
            offset1.Execute(ref result, rate_dis);
            org = result;
        }
        /// <summary>获取一个新图层的类
        /// 保存路径默认为D:\test_data\work
        /// </summary>
        /// <param name="FeatureType"></param>
        /// <returns></returns>
        public IFeatureClass GetIFeatureClass(string FeatureType)
        {
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
            IWorkspace pWorkSpace = pWorkspaceFactory.OpenFromFile(@"D:\实验\experiment\work", 0);
            //初始化要素工作空间
            IFeatureWorkspace pFeatWorkSpace = pWorkSpace as IFeatureWorkspace;
            IFeatureClass pFeatureClass = pFeatWorkSpace.OpenFeatureClass(FeatureType);
            return pFeatureClass;
        }
        /// <summary>清空要素类中的所有要素（保存索引）
        /// 
        /// </summary>
        /// <param name="PFeatureclass"></param>
        private void clear_feature(IFeatureClass PFeatureclass)
        {
            IQueryFilter pQueryFilter;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "";
            //string DeleteNum = PFeatureclass.FeatureCount(pQueryFilter).ToString();
            //IDataset pDataset = PFeatureclass as IDataset;
            //pDataset.Workspace.ExecuteSQL("delete from " + PFeatureclass.AliasName + " where objectid<=" + DeleteNum);
            ITable pTable = PFeatureclass as ITable;
            pTable.DeleteSearchedRows(pQueryFilter);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pQueryFilter);
        }
        /// <summary>清空路径下的文件夹
        /// 
        /// </summary>
        /// <param name="dir"></param>
        public static void DeleteFolder(string dir)
        {
            foreach (string d in Directory.GetFileSystemEntries(dir))
            {
                if (File.Exists(d))
                {
                    FileInfo fi = new FileInfo(d);
                    if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
                        fi.Attributes = FileAttributes.Normal;
                    File.Delete(d);//直接删除其中的文件  
                }
                else
                {
                    DirectoryInfo d1 = new DirectoryInfo(d);
                    if (d1.GetFiles().Length != 0)
                    {
                        DeleteFolder(d1.FullName);
                    }
                    Directory.Delete(d);
                }
            }
        }
        /// <summary>将新建的shapefile另存为
        /// 
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="ExportFilePath"></param>
        /// <param name="ExportFileShortName"></param>
        /// <param name="IsSave"></param>
        public void SaveShpToFile(IFeatureClass pFeatureClass, string ExportFilePath, string ExportFileShortName, bool IsSave)
        {   //删除先前的文件
            string fileFullPath = ExportFilePath + "\\" + ExportFileShortName;
            DeleteFolder(fileFullPath);
            //设置导出要素类的参数
            IFeatureClassName pOutFeatureClassName = new FeatureClassNameClass();
            IDataset pOutDataset = (IDataset)pFeatureClass;
            pOutFeatureClassName = (IFeatureClassName)pOutDataset.FullName;
            //创建一个输出shp文件的工作空间
            IWorkspaceFactory pShpWorkspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspaceName pInWorkspaceName = new WorkspaceNameClass();
            pInWorkspaceName = pShpWorkspaceFactory.Create(ExportFilePath, ExportFileShortName, null, 0);

            //创建一个要素类
            IFeatureClassName pInFeatureClassName = new FeatureClassNameClass();
            IDatasetName pInDatasetClassName;
            pInDatasetClassName = (IDatasetName)pInFeatureClassName;
            pInDatasetClassName.Name = ExportFileShortName;//作为输出参数
            pInDatasetClassName.WorkspaceName = pInWorkspaceName;
            IFeatureDataConverter pShpToClsConverter = new FeatureDataConverterClass();
            pShpToClsConverter.ConvertFeatureClass(pOutFeatureClassName, null, null, pInFeatureClassName, null, null, "", 1000, 0);
        }
        /// <summary>进行差异部分单独查询
        /// 必须先获取当前的要素，当前图层，以及一个空的要素集，
        /// </summary>
        /// <param name="diffFeature">差异部分要素</param>
        /// <param name="pFeatSet">空的要素集，用来返回</param>
        /// <param name="pTocFeatureLayer">当前要素图层</param>
        /// <returns>查询到返回查询语句，反之返回null</returns>
        public string DiffFeatureQuery(IFeature diffFeature, ref ISelectionSet pFeatSet, IFeatureLayer pTocFeatureLayer)
        {
            string sql = null;
            string osmid = null;
            string topid = null;
            string expert = null;
            string osmcount = null;
            string top10count = null;
            string isboth = null;
            string isShift = null;
            string is_union = diffFeature.get_Value(4).ToString();
            isboth = diffFeature.get_Value(8).ToString();
            //isShift = unionFeature.get_Value(11).ToString();
            osmid = diffFeature.get_Value(3).ToString();
            topid = diffFeature.get_Value(2).ToString();
                expert =diffFeature.get_Value(5).ToString();
                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                //开始查询
                sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                pQueryFilter.WhereClause = sql;
                pFeatSel = pTocFeatureLayer as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            return sql;
        }
            /// <summary>进行连锁查询
            /// 必须先获取当前的要素，当前图层，以及一个空的要素集，
            /// </summary>
            /// <param name="unionFeature">当前的要素</param>
            /// <param name="pFeatSet">空的要素集，用来返回</param>
            /// <param name="pTocFeatureLayer">当前要素图层</param>
            /// <returns>查询到返回查询语句，反之返回null</returns>
            public string MultiFeatureQuery(IFeature unionFeature,ref  ISelectionSet pFeatSet, IFeatureLayer pTocFeatureLayer)
        {
            string sql = null;
            string osmid = null;
            string topid = null;
            string expert = null;
            string osmcount = null;
            string top10count = null;
            string isboth = null;
            string isShift = null;
            string is_union = unionFeature.get_Value(4).ToString();
            isboth = unionFeature.get_Value(8).ToString();
            //isShift = unionFeature.get_Value(11).ToString();
            if (is_union == "1" && isboth == "0")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                //开始查询
                sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                pQueryFilter.WhereClause = sql;
                pFeatSel = pTocFeatureLayer as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "2")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                top10count = unionFeature.get_Value(7).ToString();
                int count = Convert.ToInt32(top10count);
                //定义查询模块
                IQueryFilter pQueryFilter_count;
                pQueryFilter_count = new QueryFilterClass();
                IFeatureSelection pFeatSel_count;
                //定义查询指针
                IFeatureCursor pFeatCursor_count;
                ICursor pCursor_count;
                ISelectionSet pFeatSet_count;
                string sql_count = "TOP10_ID = " + topid + "AND osm_id <> 0";
                pQueryFilter_count.WhereClause = sql_count;
                pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet_count = pFeatSel_count.SelectionSet;
                pFeatSet_count.Search(null, true, out pCursor_count);
                pFeatCursor_count = pCursor_count as IFeatureCursor;

                //获取osmid数组
                string[] CO = new string[count];
                IFeature pfeat_count;
                pfeat_count = pFeatCursor_count.NextFeature();
                int i = 0;
                int xlink = 0;  //标记是否是连锁数值，1为连锁
                while (pfeat_count != null)
                {
                    if (pfeat_count != null)
                    {
                        int k = Convert.ToInt32(pfeat_count.get_Value(8));
                        if (k != 2)  //判断是否连锁
                        {
                            xlink = 1;
                        }
                        CO[i] = pfeat_count.get_Value(3).ToString();
                        i++;

                    }
                    pfeat_count = pFeatCursor_count.NextFeature();
                }
                if (xlink == 1)
                {
                    sql = null;
                    return sql;
                }

                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                //开始查询
                string reco = null;
                for (int j = 0; j < count; j++)
                {
                    if (j == count - 1)
                    { reco = reco + CO[j]; }
                    else
                    { reco = reco + CO[j] + ","; }
                }
                sql = "TOP10_ID = " + topid + "OR osm_id in (" + reco + ")";
                pQueryFilter.WhereClause = sql;
                pFeatSel = pTocFeatureLayer as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "3")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                osmcount = unionFeature.get_Value(6).ToString();
                int count = Convert.ToInt32(osmcount);
                //定义查询模块
                IQueryFilter pQueryFilter_count;
                pQueryFilter_count = new QueryFilterClass();
                IFeatureSelection pFeatSel_count;
                //定义查询指针
                IFeatureCursor pFeatCursor_count;
                ICursor pCursor_count;
                ISelectionSet pFeatSet_count;
                string sql_count = "osm_id = " + osmid + "AND TOP10_ID <> 0";
                pQueryFilter_count.WhereClause = sql_count;
                pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet_count = pFeatSel_count.SelectionSet;
                pFeatSet_count.Search(null, true, out pCursor_count);
                pFeatCursor_count = pCursor_count as IFeatureCursor;

                //获取top10id数组
                string[] CO = new string[count];
                IFeature pfeat_count = pFeatCursor_count.NextFeature();
                int i = 0;
                int xlink = 0;  //标记是否是连锁数值，1为连锁
                while (pfeat_count != null)
                {
                    if (pfeat_count != null)
                    {
                        int k = Convert.ToInt32(pfeat_count.get_Value(8));
                        if (k != 3)  //判断是否连锁
                        {
                            xlink = 1;
                        }
                        CO[i] = pfeat_count.get_Value(2).ToString();
                        i++;

                    }
                    pfeat_count = pFeatCursor_count.NextFeature();
                }
                if (xlink == 1)
                {
                    sql = null;
                    return sql;
                }

                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                IFeatureLayerDefinition pFeatLyrdef;
                pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;

                //开始查询
                string reco = null;
                for (int j = 0; j < count; j++)
                {
                    if (j == count - 1)
                    { reco = reco + CO[j]; }
                    else
                    { reco = reco + CO[j] + ","; }
                }
                sql = "osm_id = " + osmid + "OR TOP10_ID in (" + reco + ")";
                pQueryFilter.WhereClause = sql;
                pFeatSel = pTocFeatureLayer as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "1")
            {
                top10count = unionFeature.get_Value(7).ToString();
                osmcount = unionFeature.get_Value(6).ToString();
                if (top10count == "2" && osmcount == "2")
                {
                    osmid = unionFeature.get_Value(3).ToString();
                    topid = unionFeature.get_Value(2).ToString();
                    expert = unionFeature.get_Value(5).ToString();
                    //定义查询模块
                    IQueryFilter pQueryFilter_count;
                    pQueryFilter_count = new QueryFilterClass();
                    IFeatureSelection pFeatSel_count;
                    //定义查询指针
                    IFeatureCursor pFeatCursor_count;
                    ICursor pCursor_count;
                    ISelectionSet pFeatSet_count;
                    string sql_count = "(TOP10_ID =" + topid + "AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id = " + osmid + ")"; //查询符合条件的三个union
                    pQueryFilter_count.WhereClause = sql_count;
                    pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                    pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet_count = pFeatSel_count.SelectionSet;
                    pFeatSet_count.Search(null, true, out pCursor_count);
                    pFeatCursor_count = pCursor_count as IFeatureCursor;
                    //获取其他两个关联的数据
                    IFeature pfeat_count;
                    pfeat_count = pFeatCursor_count.NextFeature();
                    string osmid_link = null;
                    string top10id_link = null;
                    string link_top = null;
                    string link_osm = null;
                    int k_count = 0;//计算特殊情况，比如两个union数值相等，但是被误记为2
                    while (pfeat_count != null)
                    {
                        if (pfeat_count != null)
                        {
                            link_top = pfeat_count.get_Value(2).ToString();
                            link_osm = pfeat_count.get_Value(3).ToString();
                            if (link_osm != osmid && link_top == topid)
                            {
                                osmid_link = link_osm;
                            }
                            else if (link_top != topid && link_osm == osmid)
                            {
                                top10id_link = link_top;
                            }
                            else if (link_top == topid && link_osm == osmid)
                            {
                                k_count++;
                            }
                            else
                            {
                                pfeat_count = pFeatCursor_count.NextFeature();
                                continue;
                            }

                        }
                        pfeat_count = pFeatCursor_count.NextFeature();
                    }

                    //定义查询模块
                    IQueryFilter pQueryFilter;
                    pQueryFilter = new QueryFilterClass();
                    IFeatureSelection pFeatSel;
                    IFeatureLayerDefinition pFeatLyrdef;
                    pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                    //定义查询指针
                    IFeatureCursor pFeatCursor;
                    ICursor pCursor;
                    //开始查询  
                    if (k_count >= 2)
                    {
                        sql = "TOP10_ID  = " + topid + "OR osm_id =" + osmid;
                    }
                    else
                    {
                        sql = "TOP10_ID IN(" + topid + "," + top10id_link + ") OR osm_id IN(" + osmid + "," + osmid_link + ")";
                    }
                    pQueryFilter.WhereClause = sql;
                    pFeatSel = pTocFeatureLayer as IFeatureSelection;
                    pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet = pFeatSel.SelectionSet;
                }
                if (top10count != "2" || osmcount != "2")//多连锁情况处理
                {
                    osmid = unionFeature.get_Value(3).ToString();
                    topid = unionFeature.get_Value(2).ToString();
                    expert = unionFeature.get_Value(5).ToString();
                    int i_count = 0; //控制循环次数
                    List<string> List_osm = new List<string>();
                    List<string> List_top = new List<string>();
                    List_osm.Add(osmid);
                    List_top.Add(topid);
                    string sql_count = null;
                    //开始循环查询数据
                    while (i_count < 5)
                    {
                        //定义查询模块
                        IQueryFilter pQueryFilter_count;
                        pQueryFilter_count = new QueryFilterClass();
                        IFeatureSelection pFeatSel_count;
                        //定义查询指针
                        IFeatureCursor pFeatCursor_count;
                        ICursor pCursor_count;
                        ISelectionSet pFeatSet_count;
                        string k_osm = null;
                        string k_top = null;
                        k_osm = string.Join(",", List_osm.ToArray());
                        k_top = string.Join(",", List_top.ToArray());
                        sql_count = "(TOP10_ID IN (" + k_top + ") AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id IN( " + k_osm + "))"; //查询符合条件的三个union
                        pQueryFilter_count.WhereClause = sql_count;
                        pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet_count = pFeatSel_count.SelectionSet;
                        pFeatSet_count.Search(null, true, out pCursor_count);
                        pFeatCursor_count = pCursor_count as IFeatureCursor;
                        IFeature pfeat_count;
                        pfeat_count = pFeatCursor_count.NextFeature();
                        string link_osm = null;
                        string link_top = null;

                        while (pfeat_count != null)
                        {
                            if (pfeat_count != null)
                            {
                                link_osm = pfeat_count.get_Value(3).ToString();
                                link_top = pfeat_count.get_Value(2).ToString();
                                if (!List_osm.Contains(link_osm))
                                {
                                    List_osm.Add(link_osm);
                                }
                                if (!List_top.Contains(link_top))
                                {
                                    List_top.Add(link_top);
                                }

                            }
                            pfeat_count = pFeatCursor_count.NextFeature();
                        }
                        i_count++;
                    }
                    string j_osm = null;
                    string j_top = null;
                    j_osm = string.Join(",", List_osm.ToArray());
                    j_top = string.Join(",", List_top.ToArray());
                    sql = "TOP10_ID IN (" + j_top + ")  OR  osm_id IN( " + j_osm + ")";
                    //定义查询模块
                    IQueryFilter pQueryFilter;
                    pQueryFilter = new QueryFilterClass();
                    IFeatureSelection pFeatSel;
                    IFeatureLayerDefinition pFeatLyrdef;
                    pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                    //定义查询指针
                    IFeatureCursor pFeatCursor;
                    ICursor pCursor;
                    pQueryFilter.WhereClause = sql;
                    pFeatSel = pTocFeatureLayer as IFeatureSelection;
                    pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet = pFeatSel.SelectionSet;
                }
                else
                {
                    sql = null;
                    return sql;
                };
            }
            else return sql;
            return sql;
        }
        /// <summary>进行连锁截图要素查询
        /// 必须先获取当前的要素，当前图层，以及一个空的要素集，
        /// </summary>
        /// <param name="unionFeature">当前的要素</param>
        /// <param name="pFeatSet">空的要素集，用来返回</param>
        /// <param name="selection">当前要素图层</param>
        /// <returns>查询到返回查询语句，反之返回null</returns>
        public string ScreenShotQuery(IFeature unionFeature, ref ISelectionSet pFeatSet, ISelection selection)
        {
            string sql = null;
            string osmid = null;
            string topid = null;
            string expert = null;
            string osmcount = null;
            string top10count = null;
            string isboth = null;
            string is_union = unionFeature.get_Value(4).ToString();
            isboth = unionFeature.get_Value(8).ToString();
            if (is_union == "1" && isboth == "0")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                //开始查询
                sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                pQueryFilter.WhereClause = sql;
                pFeatSel = selection as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "2")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                top10count = unionFeature.get_Value(7).ToString();
                int count = Convert.ToInt32(top10count);
                //定义查询模块
                IQueryFilter pQueryFilter_count;
                pQueryFilter_count = new QueryFilterClass();
                IFeatureSelection pFeatSel_count;
                //定义查询指针
                IFeatureCursor pFeatCursor_count;
                ICursor pCursor_count;
                ISelectionSet pFeatSet_count;
                string sql_count = "TOP10_ID = " + topid + "AND osm_id <> 0";
                pQueryFilter_count.WhereClause = sql_count;
                pFeatSel_count = selection as IFeatureSelection;
                pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet_count = pFeatSel_count.SelectionSet;
                pFeatSet_count.Search(null, true, out pCursor_count);
                pFeatCursor_count = pCursor_count as IFeatureCursor;

                //获取osmid数组
                string[] CO = new string[count];
                IFeature pfeat_count;
                pfeat_count = pFeatCursor_count.NextFeature();
                int i = 0;
                int xlink = 0;  //标记是否是连锁数值，1为连锁
                while (pfeat_count != null)
                {
                    if (pfeat_count != null)
                    {
                        int k = Convert.ToInt32(pfeat_count.get_Value(8));
                        if (k != 2)  //判断是否连锁
                        {
                            xlink = 1;
                        }
                        CO[i] = pfeat_count.get_Value(3).ToString();
                        i++;

                    }
                    pfeat_count = pFeatCursor_count.NextFeature();
                }
                if (xlink == 1)
                {
                    sql = null;
                    return sql;
                }

                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                //开始查询
                string reco = null;
                for (int j = 0; j < count; j++)
                {
                    if (j == count - 1)
                    { reco = reco + CO[j]; }
                    else
                    { reco = reco + CO[j] + ","; }
                }
                sql = "TOP10_ID = " + topid + "OR osm_id in (" + reco + ")";
                pQueryFilter.WhereClause = sql;
                pFeatSel = selection as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "3")
            {
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                osmcount = unionFeature.get_Value(6).ToString();
                int count = Convert.ToInt32(osmcount);
                //定义查询模块
                IQueryFilter pQueryFilter_count;
                pQueryFilter_count = new QueryFilterClass();
                IFeatureSelection pFeatSel_count;
                //定义查询指针
                IFeatureCursor pFeatCursor_count;
                ICursor pCursor_count;
                ISelectionSet pFeatSet_count;
                string sql_count = "osm_id = " + osmid + "AND TOP10_ID <> 0";
                pQueryFilter_count.WhereClause = sql_count;
                pFeatSel_count = selection as IFeatureSelection;
                pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet_count = pFeatSel_count.SelectionSet;
                pFeatSet_count.Search(null, true, out pCursor_count);
                pFeatCursor_count = pCursor_count as IFeatureCursor;

                //获取top10id数组
                string[] CO = new string[count];
                IFeature pfeat_count = pFeatCursor_count.NextFeature();
                int i = 0;
                int xlink = 0;  //标记是否是连锁数值，1为连锁
                while (pfeat_count != null)
                {
                    if (pfeat_count != null)
                    {
                        int k = Convert.ToInt32(pfeat_count.get_Value(8));
                        if (k != 3)  //判断是否连锁
                        {
                            xlink = 1;
                        }
                        CO[i] = pfeat_count.get_Value(2).ToString();
                        i++;

                    }
                    pfeat_count = pFeatCursor_count.NextFeature();
                }
                if (xlink == 1)
                {
                    sql = null;
                    return sql;
                }

                //定义查询模块
                IQueryFilter pQueryFilter;
                pQueryFilter = new QueryFilterClass();
                IFeatureSelection pFeatSel;
                IFeatureLayerDefinition pFeatLyrdef;
                pFeatLyrdef = selection as IFeatureLayerDefinition;

                //开始查询
                string reco = null;
                for (int j = 0; j < count; j++)
                {
                    if (j == count - 1)
                    { reco = reco + CO[j]; }
                    else
                    { reco = reco + CO[j] + ","; }
                }
                sql = "osm_id = " + osmid + "OR TOP10_ID in (" + reco + ")";
                pQueryFilter.WhereClause = sql;
                pFeatSel = selection as IFeatureSelection;
                pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                pFeatSet = pFeatSel.SelectionSet;
            }
            else if (is_union == "1" && isboth == "1")
            {
                top10count = unionFeature.get_Value(7).ToString();
                osmcount = unionFeature.get_Value(6).ToString();
                if (top10count == "2" && osmcount == "2")
                {
                    osmid = unionFeature.get_Value(3).ToString();
                    topid = unionFeature.get_Value(2).ToString();
                    expert = unionFeature.get_Value(5).ToString();
                    //定义查询模块
                    IQueryFilter pQueryFilter_count;
                    pQueryFilter_count = new QueryFilterClass();
                    IFeatureSelection pFeatSel_count;
                    //定义查询指针
                    IFeatureCursor pFeatCursor_count;
                    ICursor pCursor_count;
                    ISelectionSet pFeatSet_count;
                    string sql_count = "(TOP10_ID =" + topid + "AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id = " + osmid + ")"; //查询符合条件的三个union
                    pQueryFilter_count.WhereClause = sql_count;
                    pFeatSel_count = selection as IFeatureSelection;
                    pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet_count = pFeatSel_count.SelectionSet;
                    pFeatSet_count.Search(null, true, out pCursor_count);
                    pFeatCursor_count = pCursor_count as IFeatureCursor;
                    //获取其他两个关联的数据
                    IFeature pfeat_count;
                    pfeat_count = pFeatCursor_count.NextFeature();
                    string osmid_link = null;
                    string top10id_link = null;
                    string link_top = null;
                    string link_osm = null;
                    int k_count = 0;//计算特殊情况，比如两个union数值相等，但是被误记为2
                    while (pfeat_count != null)
                    {
                        if (pfeat_count != null)
                        {
                            link_top = pfeat_count.get_Value(2).ToString();
                            link_osm = pfeat_count.get_Value(3).ToString();
                            if (link_osm != osmid && link_top == topid)
                            {
                                osmid_link = link_osm;
                            }
                            else if (link_top != topid && link_osm == osmid)
                            {
                                top10id_link = link_top;
                            }
                            else if (link_top == topid && link_osm == osmid)
                            {
                                k_count++;
                            }
                            else
                            {
                                pfeat_count = pFeatCursor_count.NextFeature();
                                continue;
                            }

                        }
                        pfeat_count = pFeatCursor_count.NextFeature();
                    }

                    //定义查询模块
                    IQueryFilter pQueryFilter;
                    pQueryFilter = new QueryFilterClass();
                    IFeatureSelection pFeatSel;
                    IFeatureLayerDefinition pFeatLyrdef;
                    pFeatLyrdef = selection as IFeatureLayerDefinition;
                    //定义查询指针
                    IFeatureCursor pFeatCursor;
                    ICursor pCursor;
                    //开始查询  
                    if (k_count >= 2)
                    {
                        sql = "TOP10_ID  = " + topid + "OR osm_id =" + osmid;
                    }
                    else
                    {
                        sql = "TOP10_ID IN(" + topid + "," + top10id_link + ") OR osm_id IN(" + osmid + "," + osmid_link + ")";
                    }
                    pQueryFilter.WhereClause = sql;
                    pFeatSel = selection as IFeatureSelection;
                    pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet = pFeatSel.SelectionSet;
                }
                if (top10count != "2" || osmcount != "2")//多连锁情况处理
                {
                    osmid = unionFeature.get_Value(3).ToString();
                    topid = unionFeature.get_Value(2).ToString();
                    expert = unionFeature.get_Value(5).ToString();
                    int i_count = 0; //控制循环次数
                    List<string> List_osm = new List<string>();
                    List<string> List_top = new List<string>();
                    List_osm.Add(osmid);
                    List_top.Add(topid);
                    string sql_count = null;
                    //开始循环查询数据
                    while (i_count < 5)
                    {
                        //定义查询模块
                        IQueryFilter pQueryFilter_count;
                        pQueryFilter_count = new QueryFilterClass();
                        IFeatureSelection pFeatSel_count;
                        //定义查询指针
                        IFeatureCursor pFeatCursor_count;
                        ICursor pCursor_count;
                        ISelectionSet pFeatSet_count;
                        string k_osm = null;
                        string k_top = null;
                        k_osm = string.Join(",", List_osm.ToArray());
                        k_top = string.Join(",", List_top.ToArray());
                        sql_count = "(TOP10_ID IN (" + k_top + ") AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id IN( " + k_osm + "))"; //查询符合条件的三个union
                        pQueryFilter_count.WhereClause = sql_count;
                        pFeatSel_count = selection as IFeatureSelection;
                        pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet_count = pFeatSel_count.SelectionSet;
                        pFeatSet_count.Search(null, true, out pCursor_count);
                        pFeatCursor_count = pCursor_count as IFeatureCursor;
                        IFeature pfeat_count;
                        pfeat_count = pFeatCursor_count.NextFeature();
                        string link_osm = null;
                        string link_top = null;

                        while (pfeat_count != null)
                        {
                            if (pfeat_count != null)
                            {
                                link_osm = pfeat_count.get_Value(3).ToString();
                                link_top = pfeat_count.get_Value(2).ToString();
                                if (!List_osm.Contains(link_osm))
                                {
                                    List_osm.Add(link_osm);
                                }
                                if (!List_top.Contains(link_top))
                                {
                                    List_top.Add(link_top);
                                }

                            }
                            pfeat_count = pFeatCursor_count.NextFeature();
                        }
                        i_count++;
                    }
                    string j_osm = null;
                    string j_top = null;
                    j_osm = string.Join(",", List_osm.ToArray());
                    j_top = string.Join(",", List_top.ToArray());
                    sql = "TOP10_ID IN (" + j_top + ")  OR  osm_id IN( " + j_osm + ")";
                    //定义查询模块
                    IQueryFilter pQueryFilter;
                    pQueryFilter = new QueryFilterClass();
                    IFeatureSelection pFeatSel;
                    IFeatureLayerDefinition pFeatLyrdef;
                    pFeatLyrdef = selection as IFeatureLayerDefinition;
                    //定义查询指针
                    IFeatureCursor pFeatCursor;
                    ICursor pCursor;
                    pQueryFilter.WhereClause = sql;
                    pFeatSel = selection as IFeatureSelection;
                    pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    pFeatSet = pFeatSel.SelectionSet;
                }
                else
                {
                    sql = null;
                    return sql;
                };
            }
            else return sql;
            return sql;
        }
        /// <summary>生成图像
        /// 
        /// </summary>
        /// <param name="sql">剔除其他图像的查询语句</param>
        /// <param name="pFeatSet">查询得到的要素集</param>
        /// <param name="pTocFeatureLayer">当前的图层，用来剔除其他要素</param>
        /// <param name="path">图像保存路径</param>
        ///  /// <param name="picWay">截图方法</param>
        public void ImageDatesetProduce(string sql, ISelectionSet pFeatSet, IFeatureLayer pTocFeatureLayer,
           double x, double y, int pixel, string path, int picW, int picH, int picWay)
        {
            IFeatureCursor pFeatCursor;
            ICursor pCursor;
            IFeatureLayerDefinition pFeatLyrdef;
            pFeatSet.Search(null, true, out pCursor);
            pFeatCursor = pCursor as IFeatureCursor;
            pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
            IEnvelope pEnvelope = new EnvelopeClass();
            IActiveView pActiveView = mainMapControl.ActiveView;
            IEnvelope IE = new EnvelopeClass();

            //获取最大外接矩形
            IFeature pfeat;
            pfeat = pFeatCursor.NextFeature();
            while (pfeat != null)
            {
                if (pfeat != null)
                {
                    pEnvelope.Union(pfeat.Extent);
                }
                pfeat = pFeatCursor.NextFeature();
            }
            pFeatLyrdef.DefinitionExpression = sql;///图层选择性显示，一个样本一个建筑
            //pFeatLyrdef.DefinitionExpression = "";///完全显示
            pEnvelope.Expand(1.1, 1.1, true);
            mainMapControl.ActiveView.Extent = pEnvelope;
            //mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewAll, null, mainMapControl.ActiveView.Extent);
            //mainMapControl.ActiveView.Refresh();
            //Thread.Sleep(5000);
            //如果传过来的是空的
            if (x == 0 || y == 0)
            {
                x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
            }
            //if (x == 0 || y == 0)
            //{
            //    x = 158850.719;
            //    y = 436044.796;
            //}
           IE.PutCoords(x - 20, y - 20, x + 20, y + 20);
            //mainMapControl.ActiveView.Extent = pEnvelope;
            //mainMapControl.ActiveView.Refresh();
            IPoint pTempPoint = new PointClass();
            pTempPoint.PutCoords(x, y);
            mainMapControl.CenterAt(pTempPoint);
            mainMapControl.ActiveView.Refresh();
            mainMapControl.Update();
            //Thread.Sleep(5000);
            if (picWay == 1)
            {
                ExportView_test(mainMapControl.ActiveView, IE, 20, picW, picH, path);//外接矩形
            }
            else
                ExportView_test2(mainMapControl.ActiveView, path, IE, pixel);//按照真实的比例截图
                                                                             //mainMapControl.ActiveView.Extent = pEnvelope;                                                                 //


            mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
            //mainMapControl.ActiveView.Refresh();
            pFeatLyrdef.DefinitionExpression = null;
            pfeat = null;
            pFeatCursor = null;
            pCursor = null;
            pFeatSet = null;
            pEnvelope = null;
            //pTempPoint = null;

        }
        /// <summary>生成偏置图像
        /// 
        /// </summary>
        /// <param name="rate">偏置的比率，建议1000000起</param>
        /// <param name="pFeature_clip">所需要偏置的要素</param>
        /// <param name="sql">内部查询（暂时用不到）</param>
        public IFeature ClipProduce(int offset_rate, IFeature pFeature_clip, string sql)
        {
            //IQueryFilter pQueryFilter;
            //pQueryFilter = new QueryFilterClass();
            //pQueryFilter.WhereClause = sql;
            //IFeatureCursor pFeatCursor2;
            //ICursor pCursor2;
            //pFeatSet.Search(pQueryFilter, true, out pCursor2);
            //pFeatCursor2 = pCursor2 as IFeatureCursor;
            Int64 rate = 1000000;
            //pFeature_clip = pFeatCursor2.NextFeature();
            //List<IPoint> liPts = new List<IPoint>();
            string temp = pFeature_clip.get_Value(3).ToString();
            long osmid = Convert.ToInt64(pFeature_clip.get_Value(3));
            double topid = Convert.ToDouble(pFeature_clip.get_Value(2));
            Paths all_poly = new Paths();

            // 循环遍历
            //while (pFeature_clip != null)
            //{
            int cnt = pFeature_clip.Fields.FieldCount;
            esriFeatureType type = pFeature_clip.FeatureType;//获取要素的类型
            IPointCollection pPc = pFeature_clip.Shape as IPointCollection;
            int ptCnt = pPc.PointCount;//获取当前要素中的点数
            string coors = string.Empty;
            Path poly = new Path();
            for (int i = 0; i < ptCnt; i++)
            {
                IntPoint point1 = new IntPoint((long)(pPc.Point[i].X * rate),
                      (long)(pPc.Point[i].Y * rate));
                poly.Add(point1);                              //单个要素点集
            }
            //将游标移动到下一个要素
            //pFeature_clip = pFeatCursor2.NextFeature();
            all_poly.Add(poly);                                 //获取全部要素点集
            //}

            //Offset操作
            ClipperOffset offset = new ClipperOffset();
            offset.MiterLimit = 3;
            double distance = offset_rate * rate;
            offser_func(offset, -distance, ref all_poly);
            offser_func(offset, distance, ref all_poly);
            //List<List<ESRI.ArcGIS.Geometry.Point>> point_all_poly = null;   //存储全部要素点集
            //坐标转换到esri point列表中
            string FeatureType = "Polygon";
            IFeatureClass pFeatureClass_poly = GetIFeatureClass(FeatureType);
            //clear_feature(pFeatureClass_poly);
            IFeature pfeature_save = null;
            all_poly.ForEach(item =>
            {
                //将一个要素中的点读取并保存
                int i = 0;
                IPoint point_t;
                IPoint point_t_0 = null;
                Ring ring = new RingClass();
                item.ForEach(points_M =>
                {
                    point_t = new PointClass();
                    point_t.PutCoords(((double)points_M.X / rate), (double)points_M.Y / rate);
                    ring.AddPoint(point_t, Type.Missing, Type.Missing);
                    if (i == 0)
                        point_t_0 = point_t;
                    i++;
                });
                //使用ring方法生成多边形
                ring.AddPoint(point_t_0, Type.Missing, Type.Missing);   //添加闭合
                IGeometryCollection pointPolygon = new PolygonClass();
                pointPolygon.AddGeometry(ring as IGeometry, Type.Missing, Type.Missing);
                IPolygon pPolygon = pointPolygon as IPolygon;
                //存储生成的polygon
                IFeature pFeature_poly = pFeatureClass_poly.CreateFeature();
                pFeature_poly.Shape = pPolygon;
                pFeature_poly.set_Value(3, osmid);
                pFeature_poly.set_Value(2, topid);
                pFeature_poly.Store();
                pfeature_save = pFeature_poly;
            });
            return pfeature_save;
        }
        /// <summary>另存为图像
        /// 主要是将临时的要素集存储到文件里，需要先新建文件夹
        /// </summary>
        /// <param name="isSave">是否保存</param>
        /// <param name="path">保存路径</param>
        public void otherSave(bool isSave, string path)
        {
            string FeatureType = "Polygon";
            IFeatureClass pFeatureClass_poly = GetIFeatureClass(FeatureType);
            if (path != "" && isSave == true)
            {
                string ExportFileShortName = System.IO.Path.GetFileNameWithoutExtension(path);
                string ExportFilePath = System.IO.Path.GetDirectoryName(path);
                SaveShpToFile(pFeatureClass_poly, ExportFilePath, ExportFileShortName, isSave);
            }
            clear_feature(pFeatureClass_poly);
        }
        /// <summary>打开一个shp文件并读取为要素集
        /// 
        /// </summary>
        /// <param name="path">文件打开路径</param>
        /// <param name="pFeatureClass">返回的要素集</param>
        /// <returns></returns>
        public IFeatureClass openFeatureSet(string path, IFeatureClass pFeatureClass)
        {
            //由于shp文件的特殊性，要通过文件名和文件路径获取
            string pFilePath = System.IO.Path.GetDirectoryName(path);
            string pFileName = System.IO.Path.GetFileName(path);
            //创建工厂空间对象
            IWorkspaceFactory pWorkspaceFactory = new ShapefileWorkspaceFactory();
            //打开工作空间
            IFeatureWorkspace pFeaWorkspace = pWorkspaceFactory.OpenFromFile(pFilePath, 0) as IFeatureWorkspace;
            //打开文件
            pFeatureClass = pFeaWorkspace.OpenFeatureClass(pFileName);
            return pFeatureClass;
        }

        /// <summary>根据获取的要素计算中心点坐标
        /// 
        /// </summary>
        /// <param name="pFeature_clip">得到的要素</param>
        /// <param name="x">x轴坐标</param>
        /// <param name="y">y轴坐标</param>
        public void CenterPointCalculate(IFeature pFeature_clip, out double x, out double y)
        {
            IEnvelope pEnvelope = new EnvelopeClass();
            pEnvelope = pFeature_clip.Extent;
            x = pEnvelope.XMin + pEnvelope.Width / 2;
            y = pEnvelope.YMin + pEnvelope.Height / 2;
        }
        /// <summary>计算得到的要素集面积
        /// 
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        public double squareCalculate(ISelectionSet pFeatSet)
        {
            double square = 0.0;
            return square;
        }
        private void 测试ToolStripMenuItem_Click(object sender, EventArgs e)
        {


            //try{

            //IFeature p1Feature = null;
            //IFeature p2Feature = null;
            IActiveView pActiveView = mainMapControl.ActiveView;
            //IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            //p1Feature = pFeatureCursor.NextFeature();
            //p2Feature = pFeatureCursor.NextFeature();
            //List<record> relist = new List<record>(); //存储记录
            //通过获取每个图层的要素进行处理
            IQueryFilter pQueryFilter;
            pQueryFilter = new QueryFilterClass();
            pQueryFilter.WhereClause = "FID <= 22";
            IFeatureSelection pFeatSel;
            pFeatSel = pTocFeatureLayer as IFeatureSelection;
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(pTocFeatureLayer);
            //System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatSel);
            pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

            IEnvelope pEnvelope = new EnvelopeClass();
            ISelectionSet pFeatSet;
            pFeatSet = pFeatSel.SelectionSet;
            IFeatureCursor pFeatCursor;
            ICursor pCursor;
            pFeatSet.Search(null, true, out pCursor);
            pFeatCursor = pCursor as IFeatureCursor;
            IFeature pfeat;
            pfeat = pFeatCursor.NextFeature();

            //调整选择集的覆盖颜色
            //IRgbColor pColor;
            //pColor = new RgbColorClass();
            //pFeatSel.SelectionColor = pColor;


            //不显示其他要素
            while (pfeat != null)
            {
                pEnvelope.Union(pfeat.Extent);
                pfeat = pFeatCursor.NextFeature();
            }
            IFeatureLayerDefinition pFeatLyrdef;
            pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
            pFeatLyrdef.DefinitionExpression = "FID <= 22";


            mainMapControl.ActiveView.Extent = pEnvelope;
            mainMapControl.ActiveView.Refresh();


            //activeview中心点
            //IPoint centPoint = new PointClass();
            //centPoint.PutCoords((pEnvelope.XMin + pEnvelope.XMax) / 2, (pEnvelope.YMax + pEnvelope.YMin) / 2);
            //mainMapControl.Extent.CenterAt(centPoint);
            //mainMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            //mainMapControl.ActiveView.Refresh();

            ////-----------------clipper------------------------////
            //----------获取要素点集---------//

            Int64 rate = 1000000;
            pFeatSet = pFeatSel.SelectionSet;
            IFeatureCursor pFeatCursor2;
            ICursor pCursor2;
            pFeatSet.Search(null, true, out pCursor2);
            pFeatCursor2 = pCursor2 as IFeatureCursor;
            IFeature pFeature_clip;
            pFeature_clip = pFeatCursor2.NextFeature();
            //List<IPoint> liPts = new List<IPoint>();
            Paths all_poly = new Paths();

            // 循环遍历
            while (pFeature_clip != null)
            {
                int cnt = pFeature_clip.Fields.FieldCount;
                esriFeatureType type = pFeature_clip.FeatureType;//获取要素的类型
                IPointCollection pPc = pFeature_clip.Shape as IPointCollection;
                int ptCnt = pPc.PointCount;//获取当前要素中的点数
                string coors = string.Empty;
                Path poly = new Path();
                for (int i = 0; i < ptCnt; i++)
                {
                    IntPoint point1 = new IntPoint((long)(pPc.Point[i].X * rate),
                          (long)(pPc.Point[i].Y * rate));
                    poly.Add(point1);                              //单个要素点集
                }
                //将游标移动到下一个要素
                pFeature_clip = pFeatCursor2.NextFeature();
                all_poly.Add(poly);                                 //获取全部要素点集
            }

            //Offset操作
            ClipperOffset offset = new ClipperOffset();
            offset.MiterLimit = 3;
            double distance = 2 * rate;
            offser_func(offset, -distance, ref all_poly);
            //List<List<ESRI.ArcGIS.Geometry.Point>> point_all_poly = null;   //存储全部要素点集
            //坐标转换到esri point列表中
            string FeatureType = "Polygon";
            IFeatureClass pFeatureClass_poly = GetIFeatureClass(FeatureType);
            //clear_feature(pFeatureClass_poly);
            all_poly.ForEach(item =>
                {
                    //将一个要素中的点读取并保存
                    int i = 0;
                    IPoint point_t;
                    IPoint point_t_0 = null;
                    Ring ring = new RingClass();
                    item.ForEach(points_M =>
                    {
                        point_t = new PointClass();
                        point_t.PutCoords(((double)points_M.X / rate), (double)points_M.Y / rate);
                        ring.AddPoint(point_t, Type.Missing, Type.Missing);
                        if (i == 0)
                            point_t_0 = point_t;
                        i++;
                    });
                    //使用ring方法生成多边形
                    ring.AddPoint(point_t_0, Type.Missing, Type.Missing);   //添加闭合
                    IGeometryCollection pointPolygon = new PolygonClass();
                    pointPolygon.AddGeometry(ring as IGeometry, Type.Missing, Type.Missing);
                    IPolygon pPolygon = pointPolygon as IPolygon;
                    //存储生成的polygon
                    IFeature pFeature_poly = pFeatureClass_poly.CreateFeature();
                    pFeature_poly.Shape = pPolygon;
                    pFeature_poly.Store();
                });
            bool isSave = false;
            //文件另存为
            string path = @"D:\test_data\work\work";
            if (path != "")
            {
                string ExportFileShortName = System.IO.Path.GetFileNameWithoutExtension(path);
                string ExportFilePath = System.IO.Path.GetDirectoryName(path);
                SaveShpToFile(pFeatureClass_poly, ExportFilePath, ExportFileShortName, isSave);
            }








            //offset.Clear();
            //offset.AddPaths(all_poly, JoinType.jtMiter, EndType.etClosedPolygon);
            //Paths result = new Paths();
            //offset.Execute(ref result, -distance);
            //all_poly = result;


            //


            Clipper clib = new Clipper();
















            ////-------------------------------------------------////
            //寻找中心点
            double sx; double sy;
            sx = pEnvelope.XMin + pEnvelope.Width / 2;
            sy = pEnvelope.YMin + pEnvelope.Height / 2;

            //x = pEnvelope.XMin ;
            //y = pEnvelope.YMin ;
            IEnvelope sIE = new EnvelopeClass();
            //double length; double width;


            //IE.PutCoords(x - 20, y - 20, x + 20, y + 20);
            sIE.PutCoords(sx - 5, sy - 5, sx + 5, sy + 5);
            pActiveView = mainMapControl.ActiveView;
            pActiveView.Extent = sIE;
            ExportView(mainMapControl.ActiveView, sIE, 0, 500, 500, @"D:\test_data\image\" + "fid" + ".jpg");//需要调整Iactiveview的输出方式
            mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
            mainMapControl.ActiveView.Refresh();


            //开始栅格划分
            double divi = 3.0;
            double length = 5; double width = 5;
            double a = pEnvelope.Width; double b = pEnvelope.Height;
            double x; double y; //初始点坐标 
            x = pEnvelope.XMin;
            y = pEnvelope.YMin;
            double a1 = a / (2 * divi); double b1 = b / (2 * divi);
            double a2 = a / divi; double b2 = b / divi;
            x = x + a1; y = y + b1;
            for (int m = 0; m < divi; m++)
            {
                x = x + a2 * m; double y1 = y;
                for (int n = 0; n < divi; n++)
                {
                    y1 = y1 + b2 * n;
                    IEnvelope IE = new EnvelopeClass();
                    IE.PutCoords(x - length, y1 - width, x + length, y1 + width);
                    string ms = m.ToString(); string ns = n.ToString();
                    ExportView(mainMapControl.ActiveView, IE, 0, 500, 500, @"D:\test_data\image\" + "fid" + "m" + ms + "n" + ns + ".jpg");//需要调整Iactiveview的输出方式
                    //mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                    mainMapControl.ActiveView.Refresh();
                }

            }



        }

        private void 多要素截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //---------源代码如下-----------
            try
            {
                IFeature unionFeature = null;   //连接要素
                IFeature osmFeature = null;     //osm要素
                IFeature topFeature = null;     //top 10要素
                ILayer pLayer = null;
                pLayer = mainMapControl.get_Layer(0);
                pTocFeatureLayer = pLayer as IFeatureLayer;
                IActiveView pActiveView = mainMapControl.ActiveView;
                IEnvelope pEnvelope = new EnvelopeClass();
                //查询模块
                IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
                unionFeature = pFeatureCursor.NextFeature();
                ////统计feature个数
                //string sql_count = "";
                //IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
                //pQueryFilter_count1.WhereClause = sql_count;
                //IFeatureClass pFeatureClass = pTocFeatureLayer.FeatureClass;
                //int count = pFeatureClass.FeatureCount(pQueryFilter_count1);
                //定义查询模块
                string osmid = null;
                string topid = null;
                string expert = null;
                //List<record>relist = new List<record>(); //存储记录
                //一层循环
                //int sp = 1;                                                 //处理步骤计数
                //ProgressForm progress = new ProgressForm();
                //progress.Show();
                while (unionFeature != null)
                {
                    string is_union = unionFeature.get_Value(4).ToString();
                    if (is_union == "1")
                    {
                        osmid = unionFeature.get_Value(3).ToString();
                        topid = unionFeature.get_Value(2).ToString();
                        expert = unionFeature.get_Value(5).ToString();
                        //record re = new record() ;
                        //re.expert = expert;
                        //re.osmid = osmid;
                        //re.topid = topid;
                        //定义查询模块
                        IQueryFilter pQueryFilter;
                        pQueryFilter = new QueryFilterClass();
                        IFeatureSelection pFeatSel;
                        IFeatureLayerDefinition pFeatLyrdef;
                        pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                        //定义查询指针
                        IFeatureCursor pFeatCursor;
                        ICursor pCursor;
                        ISelectionSet pFeatSet;
                        //开始查询
                        string sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                        pQueryFilter.WhereClause = sql;
                        pFeatSel = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet = pFeatSel.SelectionSet;
                        pFeatSet.Search(null, true, out pCursor);
                        pFeatCursor = pCursor as IFeatureCursor;


                        //窗口设置
                        //IEnvelope pEnvelope = new EnvelopeClass();
                        double x; double y;
                        //IActiveView pActiveView = mainMapControl.ActiveView;
                        IEnvelope IE = new EnvelopeClass();

                        //获取最大外接矩形
                        IFeature pfeat;
                        pfeat = pFeatCursor.NextFeature();
                        while (pfeat != null)
                        {
                            if (pfeat != null)
                            {
                                pEnvelope.Union(pfeat.Extent);

                            }
                            pfeat = pFeatCursor.NextFeature();
                        }
                        //输出图形
                        pFeatLyrdef.DefinitionExpression = sql;
                        pEnvelope.Expand(1.1, 1.1, true);
                        mainMapControl.ActiveView.Extent = pEnvelope;
                        mainMapControl.ActiveView.Refresh();
                        x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                        y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                        IE.PutCoords(x - 20, y - 20, x + 20, y + 20);
                        ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test\" + "ex" + expert + "osmid" + osmid + ".jpg");
                        mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                        mainMapControl.ActiveView.Refresh();
                        unionFeature = pFeatureCursor.NextFeature();

                        //释放指针
                        pQueryFilter.WhereClause = null;
                        pFeatSel = null;
                        pFeatLyrdef.DefinitionExpression = null;
                        pfeat = null;
                        pFeatCursor = null;
                        pCursor = null;
                        pFeatSet = null;
                        pEnvelope = null;
                    }


                    else
                        unionFeature = pFeatureCursor.NextFeature();
                    //添加进度条
                    //progress.Addprogess(count, sp);

                    //sp++;
                }




                //释放指针
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);

                MessageBox.Show("OK!");
            }

            catch (Exception ex)
            {
                MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void 多要素处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try
            {
                IFeature unionFeature = null;   //连接要素
                IFeature osmFeature = null;     //osm要素
                IFeature topFeature = null;     //top 10要素
                IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
                unionFeature = pFeatureCursor.NextFeature();
                //定义查询模块
                string osmid = null;
                string topid = null;
                string expert = null;
                string osmcount = null;
                string top10count = null;
                string isboth = null;
                //一层循环
                while (unionFeature != null)
                {
                    string is_union = unionFeature.get_Value(4).ToString();
                    isboth = unionFeature.get_Value(8).ToString();
                    if (is_union == "1" && isboth == "0")
                    {
                        osmid = unionFeature.get_Value(3).ToString();
                        topid = unionFeature.get_Value(2).ToString();
                        expert = unionFeature.get_Value(5).ToString();
                        //定义查询模块
                        IQueryFilter pQueryFilter;
                        pQueryFilter = new QueryFilterClass();
                        IFeatureSelection pFeatSel;
                        IFeatureLayerDefinition pFeatLyrdef;
                        pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                        //定义查询指针
                        IFeatureCursor pFeatCursor;
                        ICursor pCursor;
                        ISelectionSet pFeatSet;
                        //开始查询
                        string sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                        pQueryFilter.WhereClause = sql;
                        pFeatSel = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet = pFeatSel.SelectionSet;
                        pFeatSet.Search(null, true, out pCursor);
                        pFeatCursor = pCursor as IFeatureCursor;


                        //窗口设置
                        IEnvelope pEnvelope = new EnvelopeClass();
                        double x; double y;
                        IActiveView pActiveView = mainMapControl.ActiveView;
                        IEnvelope IE = new EnvelopeClass();

                        //获取最大外接矩形
                        IFeature pfeat;
                        pfeat = pFeatCursor.NextFeature();
                        while (pfeat != null)
                        {
                            if (pfeat != null)
                            {
                                pEnvelope.Union(pfeat.Extent);

                            }
                            pfeat = pFeatCursor.NextFeature();
                        }
                        //输出图形
                        pFeatLyrdef.DefinitionExpression = sql;
                        //pEnvelope.Expand(1.1, 1.1, true);
                        mainMapControl.ActiveView.Extent = pEnvelope;
                        mainMapControl.ActiveView.Refresh();
                        x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                        y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                        //IE.PutCoords(x - 15, y - 15, x + 15, y + 15);
                        ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test_data\ceshi_count\" + "ex" + expert + "osmid" + osmid + ".jpg");
                        mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                        mainMapControl.ActiveView.Refresh();
                        unionFeature = pFeatureCursor.NextFeature();

                        //释放指针
                        pQueryFilter.WhereClause = null;
                        pFeatSel = null;
                        pFeatLyrdef.DefinitionExpression = null;
                        pfeat = null;
                        pFeatCursor = null;
                        pCursor = null;
                        pFeatSet = null;
                        pEnvelope = null;
                    }


                    else if (is_union == "1" && isboth == "2")
                    {
                        osmid = unionFeature.get_Value(3).ToString();
                        topid = unionFeature.get_Value(2).ToString();
                        expert = unionFeature.get_Value(5).ToString();
                        top10count = unionFeature.get_Value(7).ToString();
                        int count = Convert.ToInt32(top10count);
                        //定义查询模块
                        IQueryFilter pQueryFilter_count;
                        pQueryFilter_count = new QueryFilterClass();
                        IFeatureSelection pFeatSel_count;
                        //定义查询指针
                        IFeatureCursor pFeatCursor_count;
                        ICursor pCursor_count;
                        ISelectionSet pFeatSet_count;
                        string sql_count = "TOP10_ID = " + topid + "AND osm_id <> 0";
                        pQueryFilter_count.WhereClause = sql_count;
                        pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet_count = pFeatSel_count.SelectionSet;
                        pFeatSet_count.Search(null, true, out pCursor_count);
                        pFeatCursor_count = pCursor_count as IFeatureCursor;

                        //获取osmid数组
                        string[] CO = new string[count];
                        IFeature pfeat_count;
                        pfeat_count = pFeatCursor_count.NextFeature();
                        int i = 0;
                        int xlink = 0;  //标记是否是连锁数值，1为连锁
                        while (pfeat_count != null)
                        {
                            if (pfeat_count != null)
                            {
                                int k = Convert.ToInt32(pfeat_count.get_Value(8));
                                if (k != 2)  //判断是否连锁
                                {
                                    xlink = 1;
                                }
                                CO[i] = pfeat_count.get_Value(3).ToString();
                                i++;

                            }
                            pfeat_count = pFeatCursor_count.NextFeature();
                        }
                        if (xlink == 1)
                        {
                            unionFeature = pFeatureCursor.NextFeature();
                            continue;
                        }

                        //定义查询模块
                        IQueryFilter pQueryFilter;
                        pQueryFilter = new QueryFilterClass();
                        IFeatureSelection pFeatSel;
                        IFeatureLayerDefinition pFeatLyrdef;
                        pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                        //定义查询指针
                        IFeatureCursor pFeatCursor;
                        ICursor pCursor;
                        ISelectionSet pFeatSet;
                        //开始查询
                        string reco = null;
                        for (int j = 0; j < count; j++)
                        {
                            if (j == count - 1)
                            { reco = reco + CO[j]; }
                            else
                            { reco = reco + CO[j] + ","; }
                        }
                        string sql = "TOP10_ID = " + topid + "OR osm_id in (" + reco + ")";
                        pQueryFilter.WhereClause = sql;
                        pFeatSel = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet = pFeatSel.SelectionSet;
                        pFeatSet.Search(null, true, out pCursor);
                        pFeatCursor = pCursor as IFeatureCursor;


                        //窗口设置
                        IEnvelope pEnvelope = new EnvelopeClass();
                        double x; double y;
                        IActiveView pActiveView = mainMapControl.ActiveView;
                        IEnvelope IE = new EnvelopeClass();

                        //获取最大外接矩形
                        IFeature pfeat;
                        pfeat = pFeatCursor.NextFeature();
                        while (pfeat != null)
                        {
                            if (pfeat != null)
                            {
                                pEnvelope.Union(pfeat.Extent);

                            }
                            pfeat = pFeatCursor.NextFeature();
                        }
                        //输出图形
                        pFeatLyrdef.DefinitionExpression = sql;
                        //pEnvelope.Expand(1.1, 1.1, true);
                        mainMapControl.ActiveView.Extent = pEnvelope;
                        mainMapControl.ActiveView.Refresh();
                        x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                        y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                        //IE.PutCoords(x - 15, y - 15, x + 15, y + 15);
                        ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test_data\image\" + "ex" + expert + "osmid" + osmid + ".jpg");
                        mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                        mainMapControl.ActiveView.Refresh();
                        unionFeature = pFeatureCursor.NextFeature();

                        //释放指针
                        pQueryFilter.WhereClause = null;
                        pFeatSel = null;
                        pFeatLyrdef.DefinitionExpression = null;
                        pfeat = null;
                        pFeatCursor = null;
                        pCursor = null;
                        pFeatSet = null;
                        pEnvelope = null;

                    }
                    else if (is_union == "1" && isboth == "3")
                    {
                        osmid = unionFeature.get_Value(3).ToString();
                        topid = unionFeature.get_Value(2).ToString();
                        expert = unionFeature.get_Value(5).ToString();
                        osmcount = unionFeature.get_Value(6).ToString();
                        int count = Convert.ToInt32(osmcount);
                        //定义查询模块
                        IQueryFilter pQueryFilter_count;
                        pQueryFilter_count = new QueryFilterClass();
                        IFeatureSelection pFeatSel_count;
                        //定义查询指针
                        IFeatureCursor pFeatCursor_count;
                        ICursor pCursor_count;
                        ISelectionSet pFeatSet_count;
                        string sql_count = "osm_id = " + osmid + "AND TOP10_ID <> 0";
                        pQueryFilter_count.WhereClause = sql_count;
                        pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet_count = pFeatSel_count.SelectionSet;
                        pFeatSet_count.Search(null, true, out pCursor_count);
                        pFeatCursor_count = pCursor_count as IFeatureCursor;

                        //获取top10id数组
                        string[] CO = new string[count];
                        IFeature pfeat_count;
                        pfeat_count = pFeatCursor_count.NextFeature();
                        int i = 0;
                        int xlink = 0;  //标记是否是连锁数值，1为连锁
                        while (pfeat_count != null)
                        {
                            if (pfeat_count != null)
                            {
                                int k = Convert.ToInt32(pfeat_count.get_Value(8));
                                if (k != 3)  //判断是否连锁
                                {
                                    xlink = 1;
                                }
                                CO[i] = pfeat_count.get_Value(2).ToString();
                                i++;

                            }
                            pfeat_count = pFeatCursor_count.NextFeature();
                        }
                        if (xlink == 1)
                        {
                            unionFeature = pFeatureCursor.NextFeature();
                            continue;
                        }

                        //定义查询模块
                        IQueryFilter pQueryFilter;
                        pQueryFilter = new QueryFilterClass();
                        IFeatureSelection pFeatSel;
                        IFeatureLayerDefinition pFeatLyrdef;
                        pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                        //定义查询指针
                        IFeatureCursor pFeatCursor;
                        ICursor pCursor;
                        ISelectionSet pFeatSet;
                        //开始查询
                        string reco = null;
                        for (int j = 0; j < count; j++)
                        {
                            if (j == count - 1)
                            { reco = reco + CO[j]; }
                            else
                            { reco = reco + CO[j] + ","; }
                        }
                        string sql = "osm_id = " + osmid + "OR TOP10_ID in (" + reco + ")";
                        pQueryFilter.WhereClause = sql;
                        pFeatSel = pTocFeatureLayer as IFeatureSelection;
                        pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        pFeatSet = pFeatSel.SelectionSet;
                        pFeatSet.Search(null, true, out pCursor);
                        pFeatCursor = pCursor as IFeatureCursor;


                        //窗口设置
                        IEnvelope pEnvelope = new EnvelopeClass();
                        double x; double y;
                        IActiveView pActiveView = mainMapControl.ActiveView;
                        IEnvelope IE = new EnvelopeClass();

                        //获取最大外接矩形
                        IFeature pfeat;
                        pfeat = pFeatCursor.NextFeature();
                        while (pfeat != null)
                        {
                            if (pfeat != null)
                            {
                                pEnvelope.Union(pfeat.Extent);

                            }
                            pfeat = pFeatCursor.NextFeature();
                        }
                        //输出图形
                        pFeatLyrdef.DefinitionExpression = sql;
                        //pEnvelope.Expand(1.1, 1.1, true);
                        mainMapControl.ActiveView.Extent = pEnvelope;
                        mainMapControl.ActiveView.Refresh();
                        x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                        y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                        //IE.PutCoords(x - 15, y - 15, x + 15, y + 15);
                        ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test_data\image\" + "ex" + expert + "osmid" + osmid + ".jpg");
                        mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                        mainMapControl.ActiveView.Refresh();
                        unionFeature = pFeatureCursor.NextFeature();

                        //释放指针
                        pQueryFilter.WhereClause = null;
                        pFeatSel = null;
                        pFeatLyrdef.DefinitionExpression = null;
                        pfeat = null;
                        pFeatCursor = null;
                        pCursor = null;
                        pFeatSet = null;
                        pEnvelope = null;

                    }
                    else if (is_union == "1" && isboth == "1")
                    {
                        top10count = unionFeature.get_Value(7).ToString();
                        osmcount = unionFeature.get_Value(6).ToString();
                        if (top10count == "2" && osmcount == "2")
                        {
                            osmid = unionFeature.get_Value(3).ToString();
                            topid = unionFeature.get_Value(2).ToString();
                            expert = unionFeature.get_Value(5).ToString();
                            //定义查询模块
                            IQueryFilter pQueryFilter_count;
                            pQueryFilter_count = new QueryFilterClass();
                            IFeatureSelection pFeatSel_count;
                            //定义查询指针
                            IFeatureCursor pFeatCursor_count;
                            ICursor pCursor_count;
                            ISelectionSet pFeatSet_count;
                            string sql_count = "(TOP10_ID =" + topid + "AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id = " + osmid + ")"; //查询符合条件的三个union
                            pQueryFilter_count.WhereClause = sql_count;
                            pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                            pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                            pFeatSet_count = pFeatSel_count.SelectionSet;
                            pFeatSet_count.Search(null, true, out pCursor_count);
                            pFeatCursor_count = pCursor_count as IFeatureCursor;
                            //获取其他两个关联的数据
                            IFeature pfeat_count;
                            pfeat_count = pFeatCursor_count.NextFeature();
                            string osmid_link = null;
                            string top10id_link = null;
                            string link_top = null;
                            string link_osm = null;
                            int k_count = 0;//计算特殊情况，比如两个union数值相等，但是被误记为2
                            while (pfeat_count != null)
                            {
                                if (pfeat_count != null)
                                {
                                    link_top = pfeat_count.get_Value(2).ToString();
                                    link_osm = pfeat_count.get_Value(3).ToString();
                                    if (link_osm != osmid && link_top == topid)
                                    {
                                        osmid_link = link_osm;
                                    }
                                    else if (link_top != topid && link_osm == osmid)
                                    {
                                        top10id_link = link_top;
                                    }
                                    else if (link_top == topid && link_osm == osmid)
                                    {
                                        k_count++;
                                    }
                                    else
                                    {
                                        pfeat_count = pFeatCursor_count.NextFeature();
                                        continue;
                                    }

                                }
                                pfeat_count = pFeatCursor_count.NextFeature();
                            }

                            //定义查询模块
                            IQueryFilter pQueryFilter;
                            pQueryFilter = new QueryFilterClass();
                            IFeatureSelection pFeatSel;
                            IFeatureLayerDefinition pFeatLyrdef;
                            pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                            //定义查询指针
                            IFeatureCursor pFeatCursor;
                            ICursor pCursor;
                            ISelectionSet pFeatSet;
                            //开始查询  
                            string sql = null;
                            if (k_count >= 2)
                            {
                                sql = "TOP10_ID  = " + topid + "OR osm_id =" + osmid;
                            }
                            else
                            {
                                sql = "TOP10_ID IN(" + topid + "," + top10id_link + ") OR osm_id IN(" + osmid + "," + osmid_link + ")";
                            }
                            pQueryFilter.WhereClause = sql;
                            pFeatSel = pTocFeatureLayer as IFeatureSelection;
                            pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                            pFeatSet = pFeatSel.SelectionSet;
                            pFeatSet.Search(null, true, out pCursor);
                            pFeatCursor = pCursor as IFeatureCursor;
                            //窗口设置
                            IEnvelope pEnvelope = new EnvelopeClass();
                            double x; double y;
                            IActiveView pActiveView = mainMapControl.ActiveView;
                            IEnvelope IE = new EnvelopeClass();

                            //获取最大外接矩形
                            IFeature pfeat;
                            pfeat = pFeatCursor.NextFeature();
                            while (pfeat != null)
                            {
                                if (pfeat != null)
                                {
                                    pEnvelope.Union(pfeat.Extent);

                                }
                                pfeat = pFeatCursor.NextFeature();
                            }
                            //输出图形
                            pFeatLyrdef.DefinitionExpression = sql;
                            //pEnvelope.Expand(1.1, 1.1, true);
                            mainMapControl.ActiveView.Extent = pEnvelope;
                            mainMapControl.ActiveView.Refresh();
                            x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                            y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                            //IE.PutCoords(x - 15, y - 15, x + 15, y + 15);
                            ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test_data\ceshi_count_both1\" + "ex" + expert + "osmid" + osmid + ".jpg");
                            mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                            mainMapControl.ActiveView.Refresh();
                            unionFeature = pFeatureCursor.NextFeature();

                            //释放指针
                            pQueryFilter.WhereClause = null;
                            pFeatSel = null;
                            pFeatLyrdef.DefinitionExpression = null;
                            pfeat = null;
                            pFeatCursor = null;
                            pCursor = null;
                            pFeatSet = null;
                            pEnvelope = null;

                        }
                        else if (top10count != "2" || osmcount != "2")//多连锁情况处理
                        {
                            osmid = unionFeature.get_Value(3).ToString();
                            topid = unionFeature.get_Value(2).ToString();
                            expert = unionFeature.get_Value(5).ToString();
                            int i_count = 0; //控制循环次数
                            List<string> List_osm = new List<string>();
                            List<string> List_top = new List<string>();
                            List_osm.Add(osmid);
                            List_top.Add(topid);
                            string sql_count = null;
                            //开始循环查询数据
                            while (i_count < 5)
                            {
                                //定义查询模块
                                IQueryFilter pQueryFilter_count;
                                pQueryFilter_count = new QueryFilterClass();
                                IFeatureSelection pFeatSel_count;
                                //定义查询指针
                                IFeatureCursor pFeatCursor_count;
                                ICursor pCursor_count;
                                ISelectionSet pFeatSet_count;
                                string k_osm = null;
                                string k_top = null;
                                k_osm = string.Join(",", List_osm.ToArray());
                                k_top = string.Join(",", List_top.ToArray());
                                sql_count = "(TOP10_ID IN (" + k_top + ") AND osm_id <> 0) OR (TOP10_ID <>0 AND osm_id IN( " + k_osm + "))"; //查询符合条件的三个union
                                pQueryFilter_count.WhereClause = sql_count;
                                pFeatSel_count = pTocFeatureLayer as IFeatureSelection;
                                pFeatSel_count.SelectFeatures(pQueryFilter_count, esriSelectionResultEnum.esriSelectionResultNew, false);
                                pFeatSet_count = pFeatSel_count.SelectionSet;
                                pFeatSet_count.Search(null, true, out pCursor_count);
                                pFeatCursor_count = pCursor_count as IFeatureCursor;
                                IFeature pfeat_count;
                                pfeat_count = pFeatCursor_count.NextFeature();
                                string link_osm = null;
                                string link_top = null;

                                while (pfeat_count != null)
                                {
                                    if (pfeat_count != null)
                                    {
                                        link_osm = pfeat_count.get_Value(3).ToString();
                                        link_top = pfeat_count.get_Value(2).ToString();
                                        if (!List_osm.Contains(link_osm))
                                        {
                                            List_osm.Add(link_osm);
                                        }
                                        if (!List_top.Contains(link_top))
                                        {
                                            List_top.Add(link_top);
                                        }

                                    }
                                    pfeat_count = pFeatCursor_count.NextFeature();
                                }
                                i_count++;
                            }
                            string j_osm = null;
                            string j_top = null;
                            j_osm = string.Join(",", List_osm.ToArray());
                            j_top = string.Join(",", List_top.ToArray());
                            string sql = null;
                            sql = "TOP10_ID IN (" + j_top + ")  OR  osm_id IN( " + j_osm + ")";
                            //定义查询模块
                            IQueryFilter pQueryFilter;
                            pQueryFilter = new QueryFilterClass();
                            IFeatureSelection pFeatSel;
                            IFeatureLayerDefinition pFeatLyrdef;
                            pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                            //定义查询指针
                            IFeatureCursor pFeatCursor;
                            ICursor pCursor;
                            ISelectionSet pFeatSet;
                            pQueryFilter.WhereClause = sql;
                            pFeatSel = pTocFeatureLayer as IFeatureSelection;
                            pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                            pFeatSet = pFeatSel.SelectionSet;
                            pFeatSet.Search(null, true, out pCursor);
                            pFeatCursor = pCursor as IFeatureCursor;
                            //窗口设置
                            IEnvelope pEnvelope = new EnvelopeClass();
                            double x; double y;
                            IActiveView pActiveView = mainMapControl.ActiveView;
                            IEnvelope IE = new EnvelopeClass();

                            //获取最大外接矩形
                            IFeature pfeat;
                            pfeat = pFeatCursor.NextFeature();
                            while (pfeat != null)
                            {
                                if (pfeat != null)
                                {
                                    pEnvelope.Union(pfeat.Extent);

                                }
                                pfeat = pFeatCursor.NextFeature();
                            }
                            //输出图形
                            pFeatLyrdef.DefinitionExpression = sql;
                            //pEnvelope.Expand(1.1, 1.1, true);
                            mainMapControl.ActiveView.Extent = pEnvelope;
                            mainMapControl.ActiveView.Refresh();
                            x = pActiveView.Extent.XMin + pActiveView.Extent.Width / 2;
                            y = pActiveView.Extent.YMin + pActiveView.Extent.Height / 2;
                            //IE.PutCoords(x - 15, y - 15, x + 15, y + 15);
                            ExportView(mainMapControl.ActiveView, IE, 20, 500, 500, @"D:\test_data\image\" + "ex" + expert + "osmid" + osmid + ".jpg");
                            mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
                            mainMapControl.ActiveView.Refresh();
                            unionFeature = pFeatureCursor.NextFeature();

                            //释放指针
                            pQueryFilter.WhereClause = null;
                            pFeatSel = null;
                            pFeatLyrdef.DefinitionExpression = null;
                            pfeat = null;
                            pFeatCursor = null;
                            pCursor = null;
                            pFeatSet = null;
                            pEnvelope = null;

                        }
                        else
                            unionFeature = pFeatureCursor.NextFeature();

                    }
                    else
                        unionFeature = pFeatureCursor.NextFeature();
                }


                //释放指针
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);

                MessageBox.Show("OK!");
            }

            //catch (Exception ex)
            //{
            //    MessageBox.Show("导出失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}

        }

        private void 按固定面积截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //try{

            //IFeature p1Feature = null;
            //IFeature p2Feature = null;
            IActiveView pActiveView = mainMapControl.ActiveView;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            ////p1Feature = pFeatureCursor.NextFeature();
            ////p2Feature = pFeatureCursor.NextFeature();


            ////通过获取每个图层的要素进行处理
            //IQueryFilter pQueryFilter;
            //pQueryFilter = new QueryFilterClass();
            //pQueryFilter.WhereClause = "FID = 2";
            IFeatureSelection pFeatSel;
            pFeatSel = pTocFeatureLayer as IFeatureSelection;
            ////System.Runtime.InteropServices.Marshal.ReleaseComObject(pTocFeatureLayer);
            ////System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatSel);
            //pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);

            IEnvelope pEnvelope = new EnvelopeClass();
            //ISelectionSet pFeatSet;
            //pFeatSet = pFeatSel.SelectionSet;
            //IFeatureCursor pFeatCursor;
            //ICursor pCursor;
            //pFeatSet.Search(null, true, out pCursor);
            //pFeatCursor = pCursor as IFeatureCursor;
            //IFeature pfeat;
            //pfeat = pFeatCursor.NextFeature();

            //调整选择集的覆盖颜色
            IRgbColor pColor;
            pColor = new RgbColorClass();
            pFeatSel.SelectionColor = pColor;



            //while (pfeat != null)
            //{
            //    pEnvelope.Union(pfeat.Extent);
            //    pfeat = pFeatCursor.NextFeature();
            //}
            //IFeatureLayerDefinition pFeatLyrdef;
            //pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
            //pFeatLyrdef.DefinitionExpression = "FID = 2";

            pEnvelope = mainMapControl.ActiveView.Extent;
            ////mainMapControl.ActiveView.Extent = pEnvelope;
            ////mainMapControl.ActiveView.Refresh();
            ////activeview中心点
            ////IPoint centPoint = new PointClass();
            ////centPoint.PutCoords((pEnvelope.XMin + pEnvelope.XMax) / 2, (pEnvelope.YMax + pEnvelope.YMin) / 2);
            ////mainMapControl.ActiveView.Extent.CenterAt(centPoint);
            ////寻找中心点
            double x; double y;
            x = pEnvelope.XMin + pEnvelope.Width / 2;
            y = pEnvelope.YMin + pEnvelope.Height / 2;
            ////x = pEnvelope.XMin ;
            ////y = pEnvelope.YMin ;
            IEnvelope IE = new EnvelopeClass();
            ////IE.PutCoords(x - 20, y - 20, x + 20, y + 20);
            IE.PutCoords(x - 50, y - 50, x + 50, y + 50);
            pActiveView.Extent = IE;
            ExportView(pActiveView, IE, 0, 500, 500, @"D:\test\" + "fid" + ".jpg");
            mainMapControl.ActiveView.GraphicsContainer.DeleteAllElements();
            mainMapControl.ActiveView.Refresh();

        }

        private void 测试ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //try{

            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IActiveView pActiveView = mainMapControl.ActiveView;
            //查询模块
            IFeature unionFeature = null;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            //统计feature个数
            string sql_count = "";
            IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
            pQueryFilter_count1.WhereClause = sql_count;
            int count = pTocFeatureLayer.FeatureClass.FeatureCount(pQueryFilter_count1);

            unionFeature = pFeatureCursor.NextFeature();                //设置指针

            int sp = 1;                                                 //处理步骤计数
            ProgressForm progress = new ProgressForm();
            progress.Show();

            while (unionFeature != null)
            {
                //System.Windows.Forms.Application.DoEvents();
                string osmid = null;
                string topid = null;
                string expert = null;
                osmid = unionFeature.get_Value(3).ToString();
                topid = unionFeature.get_Value(2).ToString();
                expert = unionFeature.get_Value(5).ToString();
                //定义查询指针
                ISelectionSet pFeatSet = null;
                string sql = null;
                //开始查询
                sql = MultiFeatureQuery(unionFeature, ref pFeatSet, pTocFeatureLayer);    //生成查询到的要素集，返回sql查询语句
                double x = 0, y = 0;//截图中心点坐标
                IFeature pfeature_save = ClipProduce(1000000, unionFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素


                //对offset区域进行中心点坐标的的计算
                if (pfeature_save != null)
                {
                    CenterPointCalculate(pfeature_save, out x, out y);
                }
                else
                {//没查询到默认为0用中心代替
                    x = 0; y = 0;
                }


                //图像生成
                if (sql != null)
                {
                    string path = null;
                    if (Convert.ToInt32(expert) == 0)
                    {
                        path = @"D:\test_data\fixtrain20\0\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";
                    }
                    else
                        path = @"D:\test_data\fixtrain20\1\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";
                    //图像存储路径
                    ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 1);
                    //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数


                    unionFeature = pFeatureCursor.NextFeature();
                }
                else             //进行下一步
                    unionFeature = pFeatureCursor.NextFeature();

                //添加进度条
                progress.Addprogess(count, sp);

                sp++;
            }


            if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                bool isSave = true;
                otherSave(isSave, @"D:\test_data\work1\work1");
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
            progress.Close();
            MessageBox.Show("OK!");
        }

        private void 按中心区域截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //try{
            //目前用于生成位移/stn截图（外接矩形方法）

            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IActiveView pActiveView = mainMapControl.ActiveView;
            IEnvelope pEnvelope = new EnvelopeClass();
            //查询模块
            IFeature unionFeature = null;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            //统计feature个数
            string sql_count = "";
            IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
            pQueryFilter_count1.WhereClause = sql_count;
            IFeatureClass pFeatureClass = pTocFeatureLayer.FeatureClass;
            //int count = pFeatureClass.FeatureCount(pQueryFilter_count1);
            int fieldIndex = pFeatureClass.FindField("isShift");
            int groupidindex = pFeatureClass.FindField("groupid");
            unionFeature = pFeatureCursor.NextFeature();                //设置指针

            //int sp = 1;                                                 //处理步骤计数
            //ProgressForm progress = new ProgressForm();
            //progress.Show();
            //string uniqueSQL = null;
            //ISelectionSet uniqueFeatSet = null;
            //double unix=1.5, uniy=1.5;
            while (unionFeature != null)
            {
                //System.Windows.Forms.Application.DoEvents();
                string isunion = null;
                isunion = unionFeature.get_Value(4).ToString();

                pEnvelope.Union(unionFeature.Extent);
                
                if (isunion == "1")
                {
                    string osmid = null;
                    string topid = null;
                    string expert = null;
                    string isShift = null;
                    osmid = unionFeature.get_Value(3).ToString();
                    topid = unionFeature.get_Value(2).ToString();
                    expert = unionFeature.get_Value(5).ToString();
                    isShift = unionFeature.get_Value(fieldIndex).ToString();
                    string groupid = unionFeature.get_Value(groupidindex).ToString();
                    //定义查询指针
                    ISelectionSet pFeatSet = null;
                    string sql = null;
                    //开始查询
                    sql = MultiFeatureQuery(unionFeature, ref pFeatSet, pTocFeatureLayer);    //生成查询到的要素集，返回sql查询语句
                    double x = 0, y = 0;//截图中心点坐标
                    IFeature pfeature_save = ClipProduce(10, unionFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素（test为了节省时间，先注释掉）
                    //对offset区域进行中心点坐标的的计算
                    if (pfeature_save != null)
                    {
                       CenterPointCalculate(pfeature_save, out x, out y);
                       // x = 0; y = 0;
                    }
                    else
                    {//没查询到默认为0用中心代替
                        x = 0; y = 0;
                        //CenterPointCalculate(unionFeature, out x, out y);
                    }
               
                    //图像生成
                    if (sql != null&&groupid!="-1")//&&pFeatSet!= uniqueFeatSet)//&&sql!=uniqueSQL)
                    {
                        //uniqueSQL = sql;
                        //unix=x; uniy = y;
                        // string path = @"D:\test_data\function_test\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                        // string path = @"E:\Naraku\maskwork\data\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                        //if (expert == "0")
                        //{
                        //    //string path = @"D:\test\" + "groupid" + groupid + "ex" + expert + ".jpg";
                        //    string path = @"D:\实验\experiment\isShifted\train_shift\" + "groupid" + groupid + "ex" + expert + ".jpg";
                        //    ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);

                        //}
                        //else
                        //{
                        //    //string path = @"D:\实验\experiment\TwoWays\Area\1\" + "groupid" + groupid + "ex" + expert + ".jpg";
                        //    string path = @"D:\test\" + "groupid" + groupid + "ex" + expert + ".jpg";
                        //    ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);

                        //}
                        string path = @"D:\实验\experiment\isShifted\train_shift\" + "groupid" + groupid + "ex" + expert + ".jpg";
                        ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 1);
                        //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数

                        unionFeature = pFeatureCursor.NextFeature();
                    }
                    else             //进行下一步
                        unionFeature = pFeatureCursor.NextFeature();
                }
                else
                    unionFeature = pFeatureCursor.NextFeature();
                //添加进度条
                //progress.Addprogess(count, sp);

                //sp++;
            }

            //pEnvelope.Expand(1.1, 1.1, true);
            //mainMapControl.ActiveView.Extent = pEnvelope;
            //mainMapControl.ActiveView.Refresh();
            //if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //{
            //    bool isSave = true;
            //otherSave(isSave, @"D:\test_data\work1\work1");
            //}
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeatureCursor);
            //progress.Close();
            //MessageBox.Show("OK!");
        }

        private void FormMain_Load(object sender, EventArgs e)
        {

        }

        private void AddMXD_Click(object sender, EventArgs e)
        {

        }

        private void 地理范围ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nSlection = mainMapControl.Map.SelectionCount;
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IFeatureLayerDefinition featureLayerDefinition = pTocFeatureLayer as IFeatureLayerDefinition;
            IFeatureLayer SelectLayer = null;
            if (nSlection == 0)
            {
                MessageBox.Show("请先选择要素！", "提示");
            }
            else
            {
                ISelection selection = mainMapControl.Map.FeatureSelection;
                //IEnumFeature enumFeature = (IEnumFeature)selection;//此方法只能获取到选择要素的id
                //下列方法可以获取到属性值
                IEnumFeatureSetup enumFeatureSetup = selection as IEnumFeatureSetup;
                enumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = enumFeatureSetup as IEnumFeature;
                enumFeature.Reset();
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeature pFeature = enumFeature.Next();
                if (nSlection > 0)
                {
                    SelectLayer = featureLayerDefinition.CreateSelectionLayer("SelectionFeatures", true, null, null);
                }

                int sp = 1;                                                 //处理步骤计数
                ProgressForm progress = new ProgressForm();
                progress.Show();
                while (pFeature != null)
                {
                    pEnvelope.Union(pFeature.Extent);
                    string isunion = null;
                    isunion = pFeature.get_Value(4).ToString();
                    if (isunion == "1")
                    {
                        string osmid = null;
                        string topid = null;
                        string expert = null;
                        osmid = pFeature.get_Value(3).ToString();
                        topid = pFeature.get_Value(2).ToString();
                        expert = pFeature.get_Value(5).ToString();
                        //定义查询指针
                        ISelectionSet pFeatSet = null;
                        string sql = null;
                        //开始查询
                        //此查询原本基于layer ，后重写
                        sql = MultiFeatureQuery(pFeature, ref pFeatSet, SelectLayer);    //生成查询到的要素集，返回sql查询语句
                        double x = 0, y = 0;//截图中心点坐标
                        IFeature pfeature_save = ClipProduce(5, pFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素

                        //对offset区域进行中心点坐标的的计算
                        if (pfeature_save != null)
                        {
                            CenterPointCalculate(pfeature_save, out x, out y);
                        }
                        else
                        {//没查询到默认为0用中心代替
                            x = 0; y = 0;
                        }


                        //图像生成
                        if (sql != null)
                        {
                            // string path = @"D:\test_data\function_test\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            string path = @"E:\Naraku\maskwork\data_code\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);
                            //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数
                            pFeature = enumFeature.Next();
                        }
                        else             //进行下一步
                            pFeature = enumFeature.Next();

                    }
                    else
                        pFeature = enumFeature.Next();

                    //添加进度条
                    progress.Addprogess(nSlection, sp);
                    sp++;

                }

                pEnvelope.Expand(1.1, 1.1, true);
                mainMapControl.ActiveView.Extent = pEnvelope;
                mainMapControl.ActiveView.Refresh();

                if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    bool isSave = true;
                    otherSave(isSave, @"C:\Users\11853\Desktop\Screenshot\work");
                }
                progress.Close();
                //ScreenShotForm shot = new ScreenShotForm();
                //shot.Show();//以无模式窗体方式调用


            }
        }

        private void 外接矩形ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int nSlection = mainMapControl.Map.SelectionCount;
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IFeatureLayerDefinition featureLayerDefinition = pTocFeatureLayer as IFeatureLayerDefinition;
            IFeatureLayer SelectLayer = null;
            if (nSlection == 0)
            {
                MessageBox.Show("请先选择要素！", "提示");
            }
            else
            {
                ISelection selection = mainMapControl.Map.FeatureSelection;
                //IEnumFeature enumFeature = (IEnumFeature)selection;//此方法只能获取到选择要素的id
                //下列方法可以获取到属性值
                IEnumFeatureSetup enumFeatureSetup = selection as IEnumFeatureSetup;
                enumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = enumFeatureSetup as IEnumFeature;
                enumFeature.Reset();
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeature pFeature = enumFeature.Next();
                if (nSlection > 0)
                {
                    SelectLayer = featureLayerDefinition.CreateSelectionLayer("SelectionFeatures", true, null, null);
                }

                int sp = 1;                                                 //处理步骤计数
                ProgressForm progress = new ProgressForm();
                progress.Show();
                while (pFeature != null)
                {
                    pEnvelope.Union(pFeature.Extent);
                    string isunion = null;
                    isunion = pFeature.get_Value(4).ToString();
                    if (isunion == "1")
                    {
                        string osmid = null;
                        string topid = null;
                        string expert = null;
                        osmid = pFeature.get_Value(3).ToString();
                        topid = pFeature.get_Value(2).ToString();
                        expert = pFeature.get_Value(5).ToString();
                        //定义查询指针
                        ISelectionSet pFeatSet = null;
                        string sql = null;
                        //开始查询
                        //此查询原本基于layer ，后重写
                        sql = MultiFeatureQuery(pFeature, ref pFeatSet, SelectLayer);    //生成查询到的要素集，返回sql查询语句
                        double x = 0, y = 0;//截图中心点坐标
                        IFeature pfeature_save = ClipProduce(5, pFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素

                        //对offset区域进行中心点坐标的的计算
                        if (pfeature_save != null)
                        {
                            CenterPointCalculate(pfeature_save, out x, out y);
                        }
                        else
                        {//没查询到默认为0用中心代替
                            x = 0; y = 0;
                        }


                        //图像生成
                        if (sql != null)
                        {
                            // string path = @"D:\test_data\function_test\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            string path = @"E:\Naraku\maskwork\data_code\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 1);
                            //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数
                            pFeature = enumFeature.Next();
                        }
                        else             //进行下一步
                            pFeature = enumFeature.Next();

                    }
                    else
                        pFeature = enumFeature.Next();

                    //添加进度条
                    progress.Addprogess(nSlection, sp);
                    sp++;

                }

                pEnvelope.Expand(1.1, 1.1, true);
                mainMapControl.ActiveView.Extent = pEnvelope;
                mainMapControl.ActiveView.Refresh();

                progress.Close();
                //ScreenShotForm shot = new ScreenShotForm();
                //shot.Show();//以无模式窗体方式调用


            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MapSel_Click(object sender, EventArgs e)
        {

        }

        /// <summary>打开截图选参窗口自选要素截图
        /// 
        /// </summary>
        /// <param name="cliprate">膨胀收缩的比率</param>
        /// <param name="picH">截图高</param>
        /// <param name="picW">截图宽</param>
        ///<param name="savepath">截图存储路径</param>
        /// <param name="shotMethod">截图方式</param>
        public void Para_Screenshot(string savepath, int cliprate, int picW, int picH, int shotMethod)
        {
            //缩放至选择
            int nSlection = mainMapControl.Map.SelectionCount;
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IFeatureLayerDefinition featureLayerDefinition = pTocFeatureLayer as IFeatureLayerDefinition;
            IFeatureLayer SelectLayer = null;
            if (nSlection == 0)
            {
                //为防止弹出空窗口，已在ScreenShot窗口判断过
            }
            else
            {
                ISelection selection = mainMapControl.Map.FeatureSelection;
                // IEnumFeature enumFeature = (IEnumFeature)selection;
                IEnumFeatureSetup enumFeatureSetup = selection as IEnumFeatureSetup;
                enumFeatureSetup.AllFields = true;
                IEnumFeature enumFeature = enumFeatureSetup as IEnumFeature;
                enumFeature.Reset();
                IEnvelope pEnvelope = new EnvelopeClass();
                IFeature pFeature = enumFeature.Next();
                if (nSlection > 0)
                {
                    SelectLayer = featureLayerDefinition.CreateSelectionLayer("SelectionFeatures", true, null, null);
                }

                int sp = 1;                                                 //处理步骤计数
                ProgressForm progress = new ProgressForm();
                progress.Show();
                int feature_count = 0;
                while (pFeature != null)
                {
                    Console.WriteLine(feature_count++);
                    pEnvelope.Union(pFeature.Extent);
                    string isunion = null;
                    isunion = pFeature.get_Value(4).ToString();
                    if (isunion!=null)
                    {
                        string osmid = null;
                        string topid = null;
                        string expert = null;
                        string isdiff = null;
                        osmid = pFeature.get_Value(3).ToString();
                        topid = pFeature.get_Value(2).ToString();
                        expert = pFeature.get_Value(5).ToString();
                        //定义查询指针
                        ISelectionSet pFeatSet = null;
                        string sql = null;
                        //开始查询
                        //此查询原本基于layer ，后重写
                        sql = MultiFeatureQuery(pFeature, ref pFeatSet, SelectLayer);    //生成查询到的要素集，返回sql查询语句
                        //if (isunion == "1")
                        //{
                        //    sql = DiffFeatureQuery(pFeature, ref pFeatSet, SelectLayer);    //生成查询到的要素集，返回sql查询语句
                        //}                  
                        double x = 0, y = 0;//截图中心点坐标
                        if (isunion == "1" )
                        {
                                IFeature pfeature_save = ClipProduce(cliprate, pFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素
                                //对offset区域进行中心点坐标的的计算
                                if (pfeature_save != null)
                                {
                                //    Console.WriteLine("osmid"+osmid+ " topid" + topid);
                                //    Console.WriteLine("sql:"+sql);
                                CenterPointCalculate(pfeature_save, out x, out y);
                                    // isdiff = "1";
                                }
                                else
                                {//没查询到默认为0用中心代替
                                    x = 0; y = 0;
                                    //isdiff ="0";
                                }
                            }
                          

                        //图像生成
                        if (sql != null)
                        {
                            string path = savepath + @"\" +  "topid" + topid + "osmid" + osmid + "featureid" + feature_count + ".jpg";  //图像存储路径
                            //string path = savepath + @"\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, picW, picH, shotMethod);
                            //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数
                            pFeature = enumFeature.Next();
                        }
                        else             //进行下一步
                            pFeature = enumFeature.Next();

                    }
                    else
                        pFeature = enumFeature.Next();

                    //添加进度条
                    progress.Addprogess(nSlection, sp);
                    sp++;
                }
                pEnvelope.Expand(1.1, 1.1, true);
                mainMapControl.ActiveView.Extent = pEnvelope;
                mainMapControl.ActiveView.Refresh();
                if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    bool isSave = true;
                    otherSave(isSave, @"C:\Users\11853\Desktop\Screenshot\work");
                }
                progress.Close();
            }

            //清除选择
            IActiveView pActiveView = mainMapControl.ActiveView;
            pActiveView.FocusMap.ClearSelection();
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, pActiveView.Extent);

        }
        private void 要素截图ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form _frm = Application.OpenForms["ScreenShot"];  //查找是否打开过窗体  
            if ((_frm == null) || (_frm.IsDisposed))       //如果没有打开过
            {
                //唤起参数选择窗口
                //MessageBox.Show("请先进行要素选择！", "提示");
                ScreenShot shot = new ScreenShot(this);
                // shot.Owner = this;
                shot.Show();//以无模式窗体方式调用
            }
            else
            {
                _frm.Activate();
                _frm.WindowState = FormWindowState.Normal;
            }


            //将生成截图的功能与参数选择功能分离

        }

        private void 差异样本生成ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //废弃
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IActiveView pActiveView = mainMapControl.ActiveView;
            //查询模块
            IFeature pFeature = null;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            //统计feature个数
            string sql_count = "";
            IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
            pQueryFilter_count1.WhereClause = sql_count;
            int count = pTocFeatureLayer.FeatureClass.FeatureCount(pQueryFilter_count1);
            IEnvelope pEnvelope = new EnvelopeClass();
            pFeature = pFeatureCursor.NextFeature();                //设置指针

            int sp = 1;                                                 //处理步骤计数
            ProgressForm progress = new ProgressForm();
            progress.Show();
            int feature_count = 0;
                while (pFeature != null)
                {
                    Console.WriteLine(feature_count++);
                    //pEnvelope.Union(pFeature.Extent);
                    string isunion = null;
                pEnvelope.Union(pFeature.Extent);
                isunion = pFeature.get_Value(4).ToString();
                    if (isunion != null)
                    {
                        string osmid = null;
                        string topid = null;
                        string expert = null;
                        string isdiff = null;
                        osmid = pFeature.get_Value(3).ToString();
                        topid = pFeature.get_Value(2).ToString();
                        expert = pFeature.get_Value(5).ToString();
                        //定义查询指针
                        ISelectionSet pFeatSet = null;
                        string sql = null;
                        //开始查询
                        //此查询原本基于layer ，后重写
                        sql = DiffFeatureQuery(pFeature, ref pFeatSet, pTocFeatureLayer);    //生成查询到的要素集，返回sql查询语句
                        //if (isunion == "1")
                        //{
                        //    sql = DiffFeatureQuery(pFeature, ref pFeatSet, SelectLayer);    //生成查询到的要素集，返回sql查询语句
                        //}                  
                        double x = 0, y = 0;//截图中心点坐标
                        //if (shotMethod != 1)
                        //{
                    if (isunion == "2" || isunion == "3")
                    {
                        IFeature pfeature_save = ClipProduce(3, pFeature, sql);//对所有要素都采用裁剪，并生成裁剪要素

                        //对offset区域进行中心点坐标的的计算
                        if (pfeature_save != null)
                        {
                            Console.WriteLine("osmid" + osmid + " topid" + topid);
                            Console.WriteLine("sql:" + sql);
                            CenterPointCalculate(pfeature_save, out x, out y);
                            isdiff = "1";
                        }
                        else
                        {//没查询到默认为0用中心代替
                            x = 0; y = 0;
                            isdiff = "0";
                        }
                    }

                        //}

                        //图像生成
                        if (sql != null && isdiff == "1")
                        {
                            string path = @"E:\Naraku\experiment\isdiffer\" + "diff" + isdiff + "topid" + topid + "osmid" + osmid + "featureid" + feature_count + ".jpg";  //图像存储路径
                            //string path = savepath + @"\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                            ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path,500,500, 2);
                            //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数
                            pFeature = pFeatureCursor.NextFeature();
                        }
                        else             //进行下一步
                            pFeature = pFeatureCursor.NextFeature();

                    }
                    else
                        pFeature = pFeatureCursor.NextFeature();

                //添加进度条
                progress.Addprogess(count, sp);
                    sp++;
                }
                pEnvelope.Expand(1.1, 1.1, true);
                mainMapControl.ActiveView.Extent = pEnvelope;
                mainMapControl.ActiveView.Refresh();
                //if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                //{
                //    bool isSave = true;
                //    otherSave(isSave, @"C:\Users\11853\Desktop\Screenshot\work");
                //}
                progress.Close();
            
               
        }

        public class LGToolStripItem : IItemDef
        {
            private string clsid_;
            private bool group_;
            private int subtype_;

            public bool GetGroup
            {
                get { return group_; }
            }
            public string GetID
            {
                get { return clsid_; }
            }

            public int GetSubType
            {
                get { return subtype_; }
            }

            #region IItemDef 成员

            bool IItemDef.Group
            {
                set { group_ = value; }
            }


            string IItemDef.ID
            {
                set { clsid_ = value; }
            }


            int IItemDef.SubType
            {
                set { subtype_ = value; }
            }
            #endregion
        }

        private void 差异样本生成ToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IActiveView pActiveView = mainMapControl.ActiveView;
            //查询模块
            IFeature pFeature = null;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            IFeatureClass pFeatureClass = pTocFeatureLayer.FeatureClass;
            int fieldIndex = pFeatureClass.FindField("isDifferen");
            int groupidindex = pFeatureClass.FindField("groupid");
            int areaindex = pFeatureClass.FindField("Area");
            ITable pTable = pFeatureClass as ITable;
            //统计feature个数
            //string sql_count = "";
            //IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
            //pQueryFilter_count1.WhereClause = sql_count;
            //int count = pTocFeatureLayer.FeatureClass.FeatureCount(pQueryFilter_count1);
            IEnvelope pEnvelope = new EnvelopeClass();
            pFeature = pFeatureCursor.NextFeature();                //设置指针
            //int sp = 1;                                                 //处理步骤计数
            //ProgressForm progress = new ProgressForm();
            //progress.Show();
            int feature_count = 0;
            while (pFeature != null)
            {
                //Console.WriteLine(feature_count++);
                //pEnvelope.Union(pFeature.Extent);
                feature_count++;
                string isunion = null;
                pEnvelope.Union(pFeature.Extent);
                isunion = pFeature.get_Value(4).ToString();
                //定义查询指针
                ISelectionSet pFeatSet = null;
                string sql = null;
                //if (expert == "0" || expert == "3" || expert == "4" || expert == "5" || expert == "6")
                if (isunion == "1")
                {
                    //开始查询
                    //此查询原本基于layer ，后重写
                    //sql = DiffFeatureQuery(pFeature, ref pFeatSet, pTocFeatureLayer);
                    sql = MultiFeatureQuery(pFeature, ref  pFeatSet, pTocFeatureLayer);    //生成查询到的要素集，返回sql查询语句                
                    IFeatureCursor pFeatCursor;
                    ICursor pCursor;
                    IFeatureLayerDefinition pFeatLyrdef;
                    if (pFeatSet != null)
                    {
                        pFeatSet.Search(null, false, out pCursor);
                        pFeatCursor = pCursor as IFeatureCursor;
                        pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                        IFeature pfeat;
                        pfeat = pFeatCursor.NextFeature();

                        //定义查询模块

                        while (pfeat != null)
                        {
                            string posmid = null;
                            string ptopid = null;
                            string pexpert = null;
                            string isdiff = null;
                            posmid = pfeat.get_Value(3).ToString();
                            ptopid = pfeat.get_Value(2).ToString();
                            pexpert = pfeat.get_Value(5).ToString();
                            string groupid = pfeat.get_Value(groupidindex).ToString();
                            double area =Double.Parse(pfeat.get_Value(areaindex).ToString());
                            //string pisunion = null;                     
                            //pisunion = pfeat.get_Value(4).ToString();
                            string pisdiff = pfeat.get_Value(fieldIndex).ToString();
                            double x = 0, y = 0;//截图中心点坐标
                                                //if (pisunion == "2" || pisunion == "3")
                            if (pisdiff == "1" || pisdiff == "0")
                            {
                                //Console.WriteLine("osmid" + osmid + " topid" + topid);
                                //Console.WriteLine("sql:" + sql
                                IFeature pfeature_save = pfeat;//ClipProduce(3, pfeat, sql);//对所有要素都采用裁剪，并生成裁剪要素
                                if (pisdiff == "1" && pfeature_save != null)
                                {
                                    //显著差异部分
                                    //对offset区域进行中心点坐标的的计算
                                    //CenterPointCalculate(pfeature_save, out x, out y);
                                    CenterPointCalculate(pfeat, out x, out y);
                                    isdiff = "1";
                                    //pFeature.set_Value(fieldIndex, isdiff);
                                    ////pFeatCursor.UpdateFeature(pfeat);
                                    //pFeature.Store();
                                }
                                else
                                {
                                    //没查询到的差异不显著部分用几何中心作为截图中心
                                    CenterPointCalculate(pfeat, out x, out y);
                                    isdiff = "0";
                                    //pFeature.set_Value(fieldIndex, isdiff);
                                    ////pFeatCursor.UpdateFeature(pfeat);
                                    //pFeature.Store();
                                }

                                //图像生成
                                if (sql != null&& groupid != "-1"&&area>300)
                                {
                                    if ( isdiff == "1")
                                    {
                                        /* string path = @"D:\实验\experiment\isdiffer\train_diff\1\" + "groupid"+groupid+"diff" + isdiff +"x"+ (int)x + ".jpg";  //图像存储路径*/                                                                                                                                                                                         //string path = savepath + @"\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                                        string path = @"D:\test\" + "groupid" + groupid + "diff" + isdiff + "x" + (int)x + ".jpg";  //图像存储路径
                                        ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);
                                    }
                                    else
                                    {
                                        //string path = @"D:\实验\experiment\isdiffer\train_diff\0\" + "groupid" + groupid + "diff" + isdiff+ "x" + (int)x + ".jpg";  //图像存储路径                                                                                                                                                                                         //string path = savepath + @"\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                                        string path = @"D:\test\" + "groupid" + groupid + "diff" + isdiff + "x" + (int)x + ".jpg";  //图像存储路径
                                        ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);
                                    }
                                    //    string path = @"E:\Naraku\experiment\isdiffer\test5\" + "diff" + isdiff + "topid" + ptopid + "osmid" + posmid  +"x"+ (int)x + ".jpg";  //图像存储路径                                                                                                                                                                                         //string path = savepath + @"\" + "ex" + expert + "topid" + topid + "osmid" + osmid + ".jpg";  //图像存储路径
                                    //ImageDatesetProduce(sql, pFeatSet, pTocFeatureLayer, x, y, 1202, path, 500, 500, 2);
                                    //ClipProduce(1000000, unionFeature,sql);             //如果只需要集合部分的裁剪就采用这一函数
                                    //pfeat = pFeatCursor.NextFeature();
                                }
                                //else             //进行下一步
                                //    pfeat = pFeatCursor.NextFeature();
                                pfeat = pFeatCursor.NextFeature();
                            }
                            else
                                pfeat = pFeatCursor.NextFeature();
                        }
                    }
                    pFeature = pFeatureCursor.NextFeature();
                }
                else
                    pFeature = pFeatureCursor.NextFeature();
                //添加进度条
                //progress.Addprogess(count, sp);
                //sp++;
            }
            pEnvelope.Expand(1.1, 1.1, true);
            mainMapControl.ActiveView.Extent = pEnvelope;
            mainMapControl.ActiveView.Refresh();
            //if (MessageBox.Show("要保存吗？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            //{
            //    bool isSave = true;
            //    otherSave(isSave, @"C:\Users\11853\Desktop\Screenshot\work");
            //}
            //progress.Close();


        }

        private void 属性表标注ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1.添加差异显著与否字段
            //2.旧建筑物expert值的修改，以适应差异样本生成
            ILayer pLayer = null;
            pLayer = mainMapControl.get_Layer(0);
            pTocFeatureLayer = pLayer as IFeatureLayer;
            IActiveView pActiveView = mainMapControl.ActiveView;
            IFeatureClass pFeatureClass = pTocFeatureLayer.FeatureClass;
            ITable pTable = pFeatureClass as ITable;
            ////开启编辑状态
            //IWorkspace workspace = ((IDataset)pFeatureClass).Workspace;
            //IWorkspaceEdit workspaceEdit = workspace as IWorkspaceEdit;
            //bool startEdit = workspaceEdit.IsBeingEdited();
            //if (!startEdit)
            //{
            //    workspaceEdit.StartEditing(false);
            //}
            //workspaceEdit.StartEditOperation();
            //查询模块
            IFeature pFeature = null;
            IFeatureCursor pFeatureCursor = pTocFeatureLayer.Search(null, false);
            //统计feature个数
            string sql_count = "";
            IQueryFilter pQueryFilter_count1 = new QueryFilterClass();
            pQueryFilter_count1.WhereClause = sql_count;
            int count = pTocFeatureLayer.FeatureClass.FeatureCount(pQueryFilter_count1);
            int fieldIndex = pFeatureClass.FindField("expert");
            IEnvelope pEnvelope = new EnvelopeClass();
            pFeature = pFeatureCursor.NextFeature();                //设置指针
            int sp = 1;                                                 //处理步骤计数
            ProgressForm progress = new ProgressForm();
            progress.Show();
            while (pFeature != null)
            {
                //Console.WriteLine(feature_count++);
                //pEnvelope.Union(pFeature.Extent);
                string isunion = null;
                pEnvelope.Union(pFeature.Extent);
                isunion = pFeature.get_Value(4).ToString();
                if (isunion == "1")
                {
                    string osmid = null;
                    string topid = null;
                    string expert = null;
                    string isdiff = null;
                    osmid = pFeature.get_Value(3).ToString();
                    topid = pFeature.get_Value(2).ToString();
                    expert = pFeature.get_Value(5).ToString();
                    //定义查询模块
                    IQueryFilter pQueryFilter;
                    pQueryFilter = new QueryFilterClass();
                    //IFeatureSelection pFeatSel;
                    IFeatureLayerDefinition pFeatLyrdef;
                    pFeatLyrdef = pTocFeatureLayer as IFeatureLayerDefinition;
                    //定义查询指针
                    IFeatureCursor pFeatCursor;
                    //  ICursor pCursor;
                  
                    ISelectionSet pFeatSet=null;
                    //开始查询
                    //string sql = MultiFeatureQuery(pFeature, ref pFeatSet, pTocFeatureLayer);
                    string sql = "TOP10_ID = " + topid + "OR osm_id = " + osmid;
                    pQueryFilter.WhereClause = sql;
                    //pFeatCursor = pFeatureClass.Update(pQueryFilter, false);
                    //pFeatSel = pTocFeatureLayer as IFeatureSelection;
                    //pFeatSel.SelectFeatures(pQueryFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                    //pFeatSet = pFeatSel.SelectionSet;
                    //pFeatSet.Search(null, false, out pCursor);
                    // pCursor = pTable.Update(null, false);
                    //pFeatCursor = pCursor as IFeatureCursor;
                    //IFeature pfeat;
                    //pfeat = pFeatCursor.NextFeature();
                    ICursor pCursor = pTable.Update(pQueryFilter, false);
                    IRow pRow = pCursor.NextRow();
                    while (pRow != null)
                    {
                        string pexpert = pRow.get_Value(fieldIndex).ToString();
                        //string pexpert = pfeat.get_Value(fieldIndex).ToString();
                        if (pexpert == "-3")
                        {
                            pRow.set_Value(fieldIndex, expert);
                            //pRow.set_Value(fieldIndex, "1");
                            pCursor.UpdateRow(pRow);
                            Console.WriteLine("修改后expert:" + pRow.get_Value(fieldIndex).ToString());
                            //pfeat.set_Value(fieldIndex, expert);
                            ////pFeatCursor.UpdateFeature(pfeat);
                            //pfeat.Store();
                        }
                        pRow = pCursor.NextRow();
                        
                        //释放当前feature，再进行接下来的修改
                        //System.Runtime.InteropServices.Marshal.ReleaseComObject(pfeat);
                        //pfeat = null;
                        //pfeat = pFeatCursor.NextFeature();
                    }

                }
                else
                    pFeature = pFeatureCursor.NextFeature();

                    //添加进度条
                 progress.Addprogess(count, sp);
                 sp++;
                
            }

            pEnvelope.Expand(1.1, 1.1, true);
            mainMapControl.ActiveView.Extent = pEnvelope;
            mainMapControl.ActiveView.Refresh();
            pFeatureCursor.Flush();
            progress.Close();
            ////停止编辑
            //workspaceEdit.StopEditOperation();
            //startEdit = workspaceEdit.IsBeingEdited();
            //if(!startEdit)
            //{
            //    workspaceEdit.StopEditing(true);
            //}
        }

 
    }
}
