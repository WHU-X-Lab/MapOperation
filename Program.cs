using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using _190820SmoothLine;

namespace MapOperation
{
    static class Program
    {
        private static LicenseInitializer m_AOLicenseInitializer = new _190820SmoothLine.LicenseInitializer();
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine))
            {
                if (!ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop))
                {
                    MessageBox.Show("不能绑定ArcGIS runtime，应用程序即将关闭.");
                    return;
                }
            }
            //初始化产品代码和扩展代码

            m_AOLicenseInitializer.InitializeApplication(
            new esriLicenseProductCode[] { esriLicenseProductCode.esriLicenseProductCodeStandard },
            new esriLicenseExtensionCode[] { esriLicenseExtensionCode.esriLicenseExtensionCode3DAnalyst,
            esriLicenseExtensionCode.esriLicenseExtensionCodeNetwork,
            esriLicenseExtensionCode.esriLicenseExtensionCodeSpatialAnalyst,
            esriLicenseExtensionCode.esriLicenseExtensionCodeDataInteroperability });

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());

            //关闭程序后关闭许可初始化
            m_AOLicenseInitializer.ShutdownApplication();


            ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.Engine);
            Application.EnableVisualStyles();
           // Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FormMain()); 
        }
    }
}
