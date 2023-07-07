namespace MapOperation
{
    partial class FormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.AddData = new System.Windows.Forms.ToolStripMenuItem();
            this.AddMXD = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLoadMxFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnIMapDocument = new System.Windows.Forms.ToolStripMenuItem();
            this.btncontrolsOpenDocCommandClass = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddShapefile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddRaster = new System.Windows.Forms.ToolStripMenuItem();
            this.AddCAD = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddCADByLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddWholeCAD = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddRasterByCAD = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddPersonGeodatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddFileDatabase = new System.Windows.Forms.ToolStripMenuItem();
            this.AddSDE = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddSDEByService = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddSDEByDirect = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddTxt = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.另存为ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MapView = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoomIn = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoomOut = new System.Windows.Forms.ToolStripMenuItem();
            this.MapSel = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelFeature = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoomToSel = new System.Windows.Forms.ToolStripMenuItem();
            this.btnClearSel = new System.Windows.Forms.ToolStripMenuItem();
            this.平滑线ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.按中心区域截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.要素截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.差异样本生成ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.属性表标注ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.barCoorTxt = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.axTOCControl = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.EagleEyeMapControl = new ESRI.ArcGIS.Controls.AxMapControl();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.DataView = new System.Windows.Forms.TabPage();
            this.mainMapControl = new ESRI.ArcGIS.Controls.AxMapControl();
            this.PageLayoutView = new System.Windows.Forms.TabPage();
            this.axPageLayoutControl = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAttribute = new System.Windows.Forms.ToolStripMenuItem();
            this.btnZoomToLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemoveLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.截图ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.测试ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.多要素截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.多要素处理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.按固定面积截图ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EagleEyeMapControl)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.DataView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainMapControl)).BeginInit();
            this.PageLayoutView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl)).BeginInit();
            this.contextMenuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddData,
            this.MapView,
            this.MapSel,
            this.平滑线ToolStripMenuItem,
            this.按中心区域截图ToolStripMenuItem,
            this.要素截图ToolStripMenuItem,
            this.差异样本生成ToolStripMenuItem,
            this.属性表标注ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(734, 25);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // AddData
            // 
            this.AddData.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddMXD,
            this.btnAddShapefile,
            this.btnAddRaster,
            this.AddCAD,
            this.btnAddPersonGeodatabase,
            this.btnAddFileDatabase,
            this.AddSDE,
            this.btnAddTxt,
            this.保存ToolStripMenuItem,
            this.另存为ToolStripMenuItem});
            this.AddData.Name = "AddData";
            this.AddData.Size = new System.Drawing.Size(44, 21);
            this.AddData.Text = "文件";
            // 
            // AddMXD
            // 
            this.AddMXD.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnLoadMxFile,
            this.btnIMapDocument,
            this.btncontrolsOpenDocCommandClass});
            this.AddMXD.Name = "AddMXD";
            this.AddMXD.Size = new System.Drawing.Size(242, 22);
            this.AddMXD.Text = "加载MXD";
            this.AddMXD.Click += new System.EventHandler(this.AddMXD_Click);
            // 
            // btnLoadMxFile
            // 
            this.btnLoadMxFile.Name = "btnLoadMxFile";
            this.btnLoadMxFile.Size = new System.Drawing.Size(294, 22);
            this.btnLoadMxFile.Text = "LoadMxFile方法";
            this.btnLoadMxFile.Click += new System.EventHandler(this.btnLoadMxFile_Click);
            // 
            // btnIMapDocument
            // 
            this.btnIMapDocument.Name = "btnIMapDocument";
            this.btnIMapDocument.Size = new System.Drawing.Size(294, 22);
            this.btnIMapDocument.Text = "IMapDocument方法";
            this.btnIMapDocument.Click += new System.EventHandler(this.btnIMapDocument_Click);
            // 
            // btncontrolsOpenDocCommandClass
            // 
            this.btncontrolsOpenDocCommandClass.Name = "btncontrolsOpenDocCommandClass";
            this.btncontrolsOpenDocCommandClass.Size = new System.Drawing.Size(294, 22);
            this.btncontrolsOpenDocCommandClass.Text = "ControlsOpenDocCommandClass方法";
            this.btncontrolsOpenDocCommandClass.Click += new System.EventHandler(this.btncontrolsOpenDocCommandClass_Click);
            // 
            // btnAddShapefile
            // 
            this.btnAddShapefile.Name = "btnAddShapefile";
            this.btnAddShapefile.Size = new System.Drawing.Size(242, 22);
            this.btnAddShapefile.Text = "加载Shapefile数据";
            this.btnAddShapefile.Click += new System.EventHandler(this.btnAddShapefile_Click);
            // 
            // btnAddRaster
            // 
            this.btnAddRaster.Name = "btnAddRaster";
            this.btnAddRaster.Size = new System.Drawing.Size(242, 22);
            this.btnAddRaster.Text = "加载Raster数据";
            this.btnAddRaster.Click += new System.EventHandler(this.btnAddRaster_Click);
            // 
            // AddCAD
            // 
            this.AddCAD.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddCADByLayer,
            this.btnAddWholeCAD,
            this.btnAddRasterByCAD});
            this.AddCAD.Name = "AddCAD";
            this.AddCAD.Size = new System.Drawing.Size(242, 22);
            this.AddCAD.Text = "加载CAD数据";
            // 
            // btnAddCADByLayer
            // 
            this.btnAddCADByLayer.Name = "btnAddCADByLayer";
            this.btnAddCADByLayer.Size = new System.Drawing.Size(176, 22);
            this.btnAddCADByLayer.Text = "AddCADByLayer";
            this.btnAddCADByLayer.Click += new System.EventHandler(this.btnAddCADByLayer_Click);
            // 
            // btnAddWholeCAD
            // 
            this.btnAddWholeCAD.Name = "btnAddWholeCAD";
            this.btnAddWholeCAD.Size = new System.Drawing.Size(176, 22);
            this.btnAddWholeCAD.Text = "AddWholeCAD";
            this.btnAddWholeCAD.Click += new System.EventHandler(this.btnAddWholeCAD_Click);
            // 
            // btnAddRasterByCAD
            // 
            this.btnAddRasterByCAD.Name = "btnAddRasterByCAD";
            this.btnAddRasterByCAD.Size = new System.Drawing.Size(176, 22);
            this.btnAddRasterByCAD.Text = "AddRasterByCAD";
            this.btnAddRasterByCAD.Click += new System.EventHandler(this.btnAddRasterByCAD_Click);
            // 
            // btnAddPersonGeodatabase
            // 
            this.btnAddPersonGeodatabase.Name = "btnAddPersonGeodatabase";
            this.btnAddPersonGeodatabase.Size = new System.Drawing.Size(242, 22);
            this.btnAddPersonGeodatabase.Text = "加载PersonGeodatabase数据";
            this.btnAddPersonGeodatabase.Click += new System.EventHandler(this.btnAddPersonGeodatabase_Click);
            // 
            // btnAddFileDatabase
            // 
            this.btnAddFileDatabase.Name = "btnAddFileDatabase";
            this.btnAddFileDatabase.Size = new System.Drawing.Size(242, 22);
            this.btnAddFileDatabase.Text = "加载FileDatabase数据";
            this.btnAddFileDatabase.Click += new System.EventHandler(this.btnAddFileDatabase_Click);
            // 
            // AddSDE
            // 
            this.AddSDE.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddSDEByService,
            this.btnAddSDEByDirect});
            this.AddSDE.Name = "AddSDE";
            this.AddSDE.Size = new System.Drawing.Size(242, 22);
            this.AddSDE.Text = "加载SDE数据库";
            // 
            // btnAddSDEByService
            // 
            this.btnAddSDEByService.Name = "btnAddSDEByService";
            this.btnAddSDEByService.Size = new System.Drawing.Size(209, 22);
            this.btnAddSDEByService.Text = "AddSDEBaseOnService";
            this.btnAddSDEByService.Click += new System.EventHandler(this.btnAddSDEByService_Click);
            // 
            // btnAddSDEByDirect
            // 
            this.btnAddSDEByDirect.Name = "btnAddSDEByDirect";
            this.btnAddSDEByDirect.Size = new System.Drawing.Size(209, 22);
            this.btnAddSDEByDirect.Text = "AddSDEByDirect";
            this.btnAddSDEByDirect.Click += new System.EventHandler(this.btnAddSDEByDirect_Click);
            // 
            // btnAddTxt
            // 
            this.btnAddTxt.Name = "btnAddTxt";
            this.btnAddTxt.Size = new System.Drawing.Size(242, 22);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // 另存为ToolStripMenuItem
            // 
            this.另存为ToolStripMenuItem.Name = "另存为ToolStripMenuItem";
            this.另存为ToolStripMenuItem.Size = new System.Drawing.Size(242, 22);
            this.另存为ToolStripMenuItem.Text = "另存为";
            this.另存为ToolStripMenuItem.Click += new System.EventHandler(this.另存为ToolStripMenuItem_Click);
            // 
            // MapView
            // 
            this.MapView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnZoomIn,
            this.btnZoomOut});
            this.MapView.Name = "MapView";
            this.MapView.Size = new System.Drawing.Size(68, 21);
            this.MapView.Text = "放大缩小";
            // 
            // btnZoomIn
            // 
            this.btnZoomIn.Name = "btnZoomIn";
            this.btnZoomIn.Size = new System.Drawing.Size(124, 22);
            this.btnZoomIn.Text = "拉框放大";
            this.btnZoomIn.Click += new System.EventHandler(this.btnZoomIn_Click);
            // 
            // btnZoomOut
            // 
            this.btnZoomOut.Name = "btnZoomOut";
            this.btnZoomOut.Size = new System.Drawing.Size(124, 22);
            this.btnZoomOut.Text = "拉框缩小";
            this.btnZoomOut.Click += new System.EventHandler(this.btnZoomOut_Click);
            // 
            // MapSel
            // 
            this.MapSel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSelFeature,
            this.btnZoomToSel,
            this.btnClearSel});
            this.MapSel.Name = "MapSel";
            this.MapSel.Size = new System.Drawing.Size(68, 21);
            this.MapSel.Text = "要素选择";
            this.MapSel.Click += new System.EventHandler(this.MapSel_Click);
            // 
            // btnSelFeature
            // 
            this.btnSelFeature.Name = "btnSelFeature";
            this.btnSelFeature.Size = new System.Drawing.Size(136, 22);
            this.btnSelFeature.Text = "要素选择";
            this.btnSelFeature.Click += new System.EventHandler(this.btnSelFeature_Click);
            // 
            // btnZoomToSel
            // 
            this.btnZoomToSel.Name = "btnZoomToSel";
            this.btnZoomToSel.Size = new System.Drawing.Size(136, 22);
            this.btnZoomToSel.Text = "缩放至选择";
            this.btnZoomToSel.Click += new System.EventHandler(this.btnZoomToSel_Click);
            // 
            // btnClearSel
            // 
            this.btnClearSel.Name = "btnClearSel";
            this.btnClearSel.Size = new System.Drawing.Size(136, 22);
            this.btnClearSel.Text = "清除选择";
            this.btnClearSel.Click += new System.EventHandler(this.btnClearSel_Click);
            // 
            // 平滑线ToolStripMenuItem
            // 
            this.平滑线ToolStripMenuItem.Name = "平滑线ToolStripMenuItem";
            this.平滑线ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.平滑线ToolStripMenuItem.Text = "多要素截图";
            this.平滑线ToolStripMenuItem.Click += new System.EventHandler(this.多要素截图ToolStripMenuItem_Click);
            // 
            // 按中心区域截图ToolStripMenuItem
            // 
            this.按中心区域截图ToolStripMenuItem.Name = "按中心区域截图ToolStripMenuItem";
            this.按中心区域截图ToolStripMenuItem.Size = new System.Drawing.Size(104, 21);
            this.按中心区域截图ToolStripMenuItem.Text = "按中心区域截图";
            this.按中心区域截图ToolStripMenuItem.Click += new System.EventHandler(this.按中心区域截图ToolStripMenuItem_Click);
            // 
            // 要素截图ToolStripMenuItem
            // 
            this.要素截图ToolStripMenuItem.Name = "要素截图ToolStripMenuItem";
            this.要素截图ToolStripMenuItem.Size = new System.Drawing.Size(68, 21);
            this.要素截图ToolStripMenuItem.Text = "要素截图";
            this.要素截图ToolStripMenuItem.Click += new System.EventHandler(this.要素截图ToolStripMenuItem_Click);
            // 
            // 差异样本生成ToolStripMenuItem
            // 
            this.差异样本生成ToolStripMenuItem.Name = "差异样本生成ToolStripMenuItem";
            this.差异样本生成ToolStripMenuItem.Size = new System.Drawing.Size(92, 21);
            this.差异样本生成ToolStripMenuItem.Text = "差异样本生成";
            this.差异样本生成ToolStripMenuItem.Click += new System.EventHandler(this.差异样本生成ToolStripMenuItem_Click_1);
            // 
            // 属性表标注ToolStripMenuItem
            // 
            this.属性表标注ToolStripMenuItem.Name = "属性表标注ToolStripMenuItem";
            this.属性表标注ToolStripMenuItem.Size = new System.Drawing.Size(80, 21);
            this.属性表标注ToolStripMenuItem.Text = "属性表标注";
            this.属性表标注ToolStripMenuItem.Click += new System.EventHandler(this.属性表标注ToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.barCoorTxt});
            this.statusStrip1.Location = new System.Drawing.Point(0, 454);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(734, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // barCoorTxt
            // 
            this.barCoorTxt.Name = "barCoorTxt";
            this.barCoorTxt.Size = new System.Drawing.Size(80, 17);
            this.barCoorTxt.Text = "当前坐标为：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.splitContainer1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(734, 429);
            this.panel1.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
            this.splitContainer1.Size = new System.Drawing.Size(734, 429);
            this.splitContainer1.SplitterDistance = 186;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.axTOCControl);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.EagleEyeMapControl);
            this.splitContainer2.Size = new System.Drawing.Size(186, 429);
            this.splitContainer2.SplitterDistance = 189;
            this.splitContainer2.TabIndex = 1;
            // 
            // axTOCControl
            // 
            this.axTOCControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl.Location = new System.Drawing.Point(0, 0);
            this.axTOCControl.Name = "axTOCControl";
            this.axTOCControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl.OcxState")));
            this.axTOCControl.Size = new System.Drawing.Size(186, 189);
            this.axTOCControl.TabIndex = 0;
            this.axTOCControl.OnMouseDown += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseDownEventHandler(this.axTOCControl_OnMouseDown);
            this.axTOCControl.OnMouseUp += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseUpEventHandler(this.axTOCControl_OnMouseUp);
            // 
            // EagleEyeMapControl
            // 
            this.EagleEyeMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.EagleEyeMapControl.Location = new System.Drawing.Point(0, 0);
            this.EagleEyeMapControl.Name = "EagleEyeMapControl";
            this.EagleEyeMapControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("EagleEyeMapControl.OcxState")));
            this.EagleEyeMapControl.Size = new System.Drawing.Size(186, 236);
            this.EagleEyeMapControl.TabIndex = 0;
            this.EagleEyeMapControl.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.EagleEyeMapControl_OnMouseDown);
            this.EagleEyeMapControl.OnMouseUp += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseUpEventHandler(this.EagleEyeMapControl_OnMouseUp);
            this.EagleEyeMapControl.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.EagleEyeMapControl_OnMouseMove);
            // 
            // tabControl1
            // 
            this.tabControl1.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tabControl1.Controls.Add(this.DataView);
            this.tabControl1.Controls.Add(this.PageLayoutView);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(544, 429);
            this.tabControl1.TabIndex = 4;
            // 
            // DataView
            // 
            this.DataView.Controls.Add(this.listBox1);
            this.DataView.Controls.Add(this.mainMapControl);
            this.DataView.Location = new System.Drawing.Point(4, 4);
            this.DataView.Name = "DataView";
            this.DataView.Padding = new System.Windows.Forms.Padding(3);
            this.DataView.Size = new System.Drawing.Size(536, 403);
            this.DataView.TabIndex = 0;
            this.DataView.Text = "数据视图";
            this.DataView.UseVisualStyleBackColor = true;
            // 
            // mainMapControl
            // 
            this.mainMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainMapControl.Location = new System.Drawing.Point(3, 3);
            this.mainMapControl.Name = "mainMapControl";
            this.mainMapControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("mainMapControl.OcxState")));
            this.mainMapControl.Size = new System.Drawing.Size(530, 397);
            this.mainMapControl.TabIndex = 0;
            this.mainMapControl.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.mainMapControl_OnMouseDown);
            this.mainMapControl.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.mainMapControl_OnMouseMove);
            this.mainMapControl.OnDoubleClick += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnDoubleClickEventHandler(this.mainMapControl_OnDoubleClick);
            this.mainMapControl.OnAfterScreenDraw += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnAfterScreenDrawEventHandler(this.mainMapControl_OnAfterScreenDraw);
            this.mainMapControl.OnExtentUpdated += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnExtentUpdatedEventHandler(this.mainMapControl_OnExtentUpdated);
            this.mainMapControl.OnMapReplaced += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMapReplacedEventHandler(this.mainMapControl_OnMapReplaced);
            // 
            // PageLayoutView
            // 
            this.PageLayoutView.Controls.Add(this.axPageLayoutControl);
            this.PageLayoutView.Location = new System.Drawing.Point(4, 4);
            this.PageLayoutView.Name = "PageLayoutView";
            this.PageLayoutView.Padding = new System.Windows.Forms.Padding(3);
            this.PageLayoutView.Size = new System.Drawing.Size(536, 403);
            this.PageLayoutView.TabIndex = 1;
            this.PageLayoutView.Text = "布局视图";
            this.PageLayoutView.UseVisualStyleBackColor = true;
            // 
            // axPageLayoutControl
            // 
            this.axPageLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axPageLayoutControl.Location = new System.Drawing.Point(3, 3);
            this.axPageLayoutControl.Name = "axPageLayoutControl";
            this.axPageLayoutControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPageLayoutControl.OcxState")));
            this.axPageLayoutControl.Size = new System.Drawing.Size(530, 397);
            this.axPageLayoutControl.TabIndex = 0;
            this.axPageLayoutControl.OnMouseDown += new ESRI.ArcGIS.Controls.IPageLayoutControlEvents_Ax_OnMouseDownEventHandler(this.axPageLayoutControl_OnMouseDown);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAttribute,
            this.btnZoomToLayer,
            this.btnRemoveLayer,
            this.截图ToolStripMenuItem1,
            this.测试ToolStripMenuItem,
            this.多要素截图ToolStripMenuItem,
            this.多要素处理ToolStripMenuItem,
            this.按固定面积截图ToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(161, 180);
            // 
            // btnAttribute
            // 
            this.btnAttribute.Name = "btnAttribute";
            this.btnAttribute.Size = new System.Drawing.Size(160, 22);
            this.btnAttribute.Text = "属性表";
            this.btnAttribute.Click += new System.EventHandler(this.btnAttribute_Click);
            // 
            // btnZoomToLayer
            // 
            this.btnZoomToLayer.Name = "btnZoomToLayer";
            this.btnZoomToLayer.Size = new System.Drawing.Size(160, 22);
            this.btnZoomToLayer.Text = "缩放到图层";
            this.btnZoomToLayer.Click += new System.EventHandler(this.btnZoomToLayer_Click);
            // 
            // btnRemoveLayer
            // 
            this.btnRemoveLayer.Name = "btnRemoveLayer";
            this.btnRemoveLayer.Size = new System.Drawing.Size(160, 22);
            this.btnRemoveLayer.Text = "移除图层";
            this.btnRemoveLayer.Click += new System.EventHandler(this.btnRemoveLayer_Click);
            // 
            // 截图ToolStripMenuItem1
            // 
            this.截图ToolStripMenuItem1.Name = "截图ToolStripMenuItem1";
            this.截图ToolStripMenuItem1.Size = new System.Drawing.Size(160, 22);
            this.截图ToolStripMenuItem1.Text = "截图";
            this.截图ToolStripMenuItem1.Click += new System.EventHandler(this.截图ToolStripMenuItem1_Click);
            // 
            // 测试ToolStripMenuItem
            // 
            this.测试ToolStripMenuItem.Name = "测试ToolStripMenuItem";
            this.测试ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.测试ToolStripMenuItem.Text = "测试";
            this.测试ToolStripMenuItem.Click += new System.EventHandler(this.测试ToolStripMenuItem_Click);
            // 
            // 多要素截图ToolStripMenuItem
            // 
            this.多要素截图ToolStripMenuItem.Name = "多要素截图ToolStripMenuItem";
            this.多要素截图ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.多要素截图ToolStripMenuItem.Text = "多要素截图";
            this.多要素截图ToolStripMenuItem.Click += new System.EventHandler(this.多要素截图ToolStripMenuItem_Click);
            // 
            // 多要素处理ToolStripMenuItem
            // 
            this.多要素处理ToolStripMenuItem.Name = "多要素处理ToolStripMenuItem";
            this.多要素处理ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.多要素处理ToolStripMenuItem.Text = "多要素处理";
            this.多要素处理ToolStripMenuItem.Click += new System.EventHandler(this.多要素处理ToolStripMenuItem_Click);
            // 
            // 按固定面积截图ToolStripMenuItem
            // 
            this.按固定面积截图ToolStripMenuItem.Name = "按固定面积截图ToolStripMenuItem";
            this.按固定面积截图ToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.按固定面积截图ToolStripMenuItem.Text = "按固定面积截图";
            this.按固定面积截图ToolStripMenuItem.Click += new System.EventHandler(this.按固定面积截图ToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.toolStrip1);
            this.panel2.Location = new System.Drawing.Point(532, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(198, 26);
            this.panel2.TabIndex = 1;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(198, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 12;
            this.listBox1.Location = new System.Drawing.Point(236, 216);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(297, 184);
            this.listBox1.TabIndex = 1;
            this.listBox1.Visible = false;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 476);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormMain";
            this.Text = "test";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EagleEyeMapControl)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.DataView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainMapControl)).EndInit();
            this.PageLayoutView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl)).EndInit();
            this.contextMenuStrip.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem AddData;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem AddMXD;
        private System.Windows.Forms.ToolStripMenuItem btnAddShapefile;
        private System.Windows.Forms.ToolStripMenuItem MapView;
        private System.Windows.Forms.ToolStripMenuItem btnZoomIn;
        private System.Windows.Forms.ToolStripMenuItem btnZoomOut;
        private System.Windows.Forms.ToolStripMenuItem MapSel;
        private System.Windows.Forms.ToolStripMenuItem btnSelFeature;
        private System.Windows.Forms.ToolStripMenuItem btnZoomToSel;
        private System.Windows.Forms.ToolStripMenuItem btnClearSel;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl;
        private ESRI.ArcGIS.Controls.AxMapControl EagleEyeMapControl;
        private System.Windows.Forms.ToolStripMenuItem btnLoadMxFile;
        private System.Windows.Forms.ToolStripMenuItem btnIMapDocument;
        private System.Windows.Forms.ToolStripMenuItem btnAddRaster;
        private System.Windows.Forms.ToolStripMenuItem AddCAD;
        private System.Windows.Forms.ToolStripMenuItem btnAddPersonGeodatabase;
        private System.Windows.Forms.ToolStripMenuItem btnAddFileDatabase;
        private System.Windows.Forms.ToolStripMenuItem AddSDE;
        private System.Windows.Forms.ToolStripMenuItem btnAddCADByLayer;
        private System.Windows.Forms.ToolStripMenuItem btnAddWholeCAD;
        private System.Windows.Forms.ToolStripStatusLabel barCoorTxt;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem btnAttribute;
        private System.Windows.Forms.ToolStripMenuItem btnZoomToLayer;
        private System.Windows.Forms.ToolStripMenuItem btnRemoveLayer;
        private System.Windows.Forms.ToolStripMenuItem btncontrolsOpenDocCommandClass;
        private System.Windows.Forms.ToolStripMenuItem btnAddTxt;
        private System.Windows.Forms.ToolStripMenuItem btnAddRasterByCAD;
        private System.Windows.Forms.ToolStripMenuItem btnAddSDEByService;
        private System.Windows.Forms.ToolStripMenuItem btnAddSDEByDirect;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 另存为ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 平滑线ToolStripMenuItem;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage DataView;
        public ESRI.ArcGIS.Controls.AxMapControl mainMapControl;
        private System.Windows.Forms.TabPage PageLayoutView;
        private ESRI.ArcGIS.Controls.AxPageLayoutControl axPageLayoutControl;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripMenuItem 截图ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 测试ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 多要素截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 多要素处理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 按固定面积截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 按中心区域截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 要素截图ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 差异样本生成ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 属性表标注ToolStripMenuItem;
        private System.Windows.Forms.ListBox listBox1;
    }
}

