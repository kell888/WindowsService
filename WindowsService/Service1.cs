using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WindowsService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 线程
        /// </summary>
        private Thread objThreadTime;
        /// <summary>
        /// 线程锁
        /// </summary>
        private object objThreadLock = new object();
        /// <summary>
        /// 应用程序所在目录
        /// </summary>
        private readonly string startPath = Application.StartupPath;

        protected override void OnStart(string[] args)
        {
            try
            {
                #region 启动线程
                ThreadStart ts = new ThreadStart(Thread_Start);
                objThreadTime = new Thread(ts);
                objThreadTime.Start();
                #endregion
                InLog("启动服务");
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        protected override void OnStop()
        {
            try
            {
                if (objThreadTime != null)
                {
                    objThreadTime.Abort();
                }
                InLog("关闭服务");
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        protected override void OnPause()
        {
            //base.OnPause();
            try
            {
                if (objThreadTime != null)
                {
                    objThreadTime.Suspend();
                }
                InLog("暂停服务");
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        protected override void OnContinue()
        {
            //base.OnContinue();
            try
            {
                if (objThreadTime != null)
                {
                    objThreadTime.Resume();
                }
                InLog("恢复服务");
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                InLog("测试运行");
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        /// <summary>
        /// 线程方法
        /// </summary>
        private void Thread_Start()
        {
            try
            {
                while (true)
                {
                    lock (objThreadLock)
                    {
                        Thread.Sleep(5000);
                        InLog("测试运行");
                        //Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                InLog(ex.Message);
            }
        }

        private static void InLog(string txt)
        {
            //const string log = "log.txt";
            //using (StreamWriter sw = new StreamWriter(log, true, Encoding.UTF8))
            //{
            //    sw.WriteLine(txt);
            //    sw.Close();
            //}

            string strMatter; //错误内容            
            string strPath; //错误文件的路径            
            DateTime dt = DateTime.Now;
            try
            {
                //Server.MapPath("./") + "File"; 服务器端路径                
                //strPath = Directory.GetCurrentDirectory() + "\\ErrorLog";   //winform工程\bin\目录下 创建日志文件夹 
                strPath = System.Windows.Forms.Application.StartupPath + "\\ErrorLog";
                //strPath = "c:" + "\\ErrorLog";//暂时放在c:下                
                if (Directory.Exists(strPath) == false)  //工程目录下 Log目录 '目录是否存在,为true则没有此目录   
                {
                    Directory.CreateDirectory(strPath); //建立目录　Directory为目录对象      
                }
                strPath = strPath + "\\" + dt.ToString("yyyyMM");
                if (Directory.Exists(strPath) == false)  //目录是否存在  '工程目录下 Log\月 目录   yyyymm    
                {
                    Directory.CreateDirectory(strPath);  //建立目录//日志文件，以 日 命名     
                } strPath = strPath + "\\" + dt.ToString("dd") + ".txt";
                //strMatter = strFunctionName + " , " + strErrorNum + " , " + strErrorDescription;//生成错误信息  
                strMatter = txt + "------" + dt.ToLongTimeString();
                StreamWriter FileWriter = new StreamWriter(strPath, true); //创建日志文件                
                FileWriter.WriteLine("Time: " + dt.ToString("HH:mm:ss") + "  Err: " + strMatter);
                FileWriter.Close(); //关闭StreamWriter对象            
            }
            catch (Exception ex)
            {
                //("写错误日志时出现问题，请与管理员联系！ 原错误:" + strMatter + "写日志错误:" + ex.Message.ToString());               
                string str = ex.Message.ToString();
            }
        }
    }
}
