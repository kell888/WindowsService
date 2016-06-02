using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

//using System.ServiceProcess;
//using System.Configuration.Install;

namespace WindowsService.UI
{
    public class WService
    {
        /// <summary>
        /// 获取系统中所有服务
        /// </summary>
        public static System.ServiceProcess.ServiceController[] GetAllServices
        {
            //获取系统中所有服务
            get { return System.ServiceProcess.ServiceController.GetServices(); }
        }

        /// <summary>
        /// 安装Windows服务
        /// </summary>
        /// <param name="savedState">集合</param>
        /// <param name="serviceName">服务名称</param>
        /// <param name="filepath">程序文件路径</param>
        public static void InstallService(IDictionary savedState, string serviceName, string filepath)
        {
            try
            {
                System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName);

                //判断服务是否存在
                if (!ServicesExisted(serviceName))
                {
                    //安装服务
                    System.Configuration.Install.AssemblyInstaller myAssemblyInstaller = new System.Configuration.Install.AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Install(savedState);
                    myAssemblyInstaller.Commit(savedState);
                    myAssemblyInstaller.Dispose();

                    //启动服务
                    service.Start();
                }
                else
                {
                    //判断服务的运行状态(判断服务是否正在运行和正在启动)
                    if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running &&
                        service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                    {
                        //启动服务
                        service.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("InstallService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 卸载Windows服务
        /// </summary>
        /// <param name="serviceName">服务名称</param>
        /// <param name="filepath">程序文件路径</param>
        public static void UnInstallService(string serviceName, string filepath)
        {
            try
            {
                //判断服务是否存在
                if (ServicesExisted(serviceName))
                {
                    System.Configuration.Install.AssemblyInstaller myAssemblyInstaller = new System.Configuration.Install.AssemblyInstaller();
                    myAssemblyInstaller.UseNewContext = true;
                    myAssemblyInstaller.Path = filepath;
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("UnInstallService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 判断Window服务是否存在
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>存在返回 true,否则返回 false;</returns>
        public static bool ServicesExisted(string serviceName)
        {
            try
            {
                //获取系统中所有服务
                System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
                foreach (System.ServiceProcess.ServiceController sc in services)
                {
                    //判断服务是否相等
                    if (sc.ServiceName.ToLower().Equals(serviceName.ToLower()))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 启动Windows服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>启动成功返回 true,否则返回 false;</returns>
        public static bool StartService(string serviceName)
        {
            try
            {
                //判断服务是否存在
                if (ServicesExisted(serviceName))
                {
                    using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName))
                    {
                        //判断服务的运行状态(判断服务是否正在运行和正在启动)
                        if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running &&
                            service.Status != System.ServiceProcess.ServiceControllerStatus.StartPending)
                        {
                            //启动服务
                            service.Start();

                            #region 方案一
                            ////设置等待服务达到指定状态的超时时间
                            //service.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, new TimeSpan(0, 0, 10));

                            ////刷新属性值
                            //service.Refresh();
                            //if (service.Status != System.ServiceProcess.ServiceControllerStatus.Running)
                            //{
                            //    throw new Exception(string.Format("StartService Error\n服务：{0} 启动时超时...", serviceName));
                            //}
                            #endregion

                            #region 方案二
                            for (int i = 0; i < 60; i++)
                            {
                                System.Threading.Thread.Sleep(1000);
                                service.Refresh();
                                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                                {
                                    break;
                                }
                                if (i == 59)
                                {
                                    throw new Exception(string.Format("服务：{0} 启动时超时...", serviceName));
                                }
                            }
                            #endregion
                        }
                        //service.Close();
                        //service.Dispose();
                    }
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("服务：{0} 不存在...", serviceName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("StartService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 暂停Windows服务
        /// </summary>
        /// <param name="serviceName"></param>
        public static bool PauseService(string serviceName)
        {
            try
            {
                //判断服务是否存在
                if (ServicesExisted(serviceName))
                {
                    using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName))
                    {
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running ||
                            service.Status == System.ServiceProcess.ServiceControllerStatus.StartPending)
                        {
                            service.Pause();

                            for (int i = 0; i < 60; i++)
                            {
                                System.Threading.Thread.Sleep(1000);
                                service.Refresh();
                                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Paused)
                                {
                                    break;
                                }

                                if (i == 59)
                                {
                                    throw new Exception("暂停Windows服务操作超时...");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("服务：{0} 未运行...", serviceName));
                        }
                    }
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("服务：{0} 不存在...", serviceName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("PauseService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 恢复Windows服务
        /// </summary>
        /// <param name="serviceName"></param>
        public static bool ResumeService(string serviceName)
        {
            try
            {
                //判断服务是否存在
                if (ServicesExisted(serviceName))
                {
                    using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName))
                    {
                        if (service.Status == System.ServiceProcess.ServiceControllerStatus.Paused ||
                            service.Status == System.ServiceProcess.ServiceControllerStatus.PausePending)
                        {
                            //继续服务
                            service.Continue();

                            for (int i = 0; i < 60; i++)
                            {
                                System.Threading.Thread.Sleep(1000);
                                service.Refresh();
                                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                                {
                                    break;
                                }

                                if (i == 59)
                                {
                                    throw new Exception("暂停Windows服务操作超时...");
                                }
                            }
                        }
                        else
                        {
                            throw new Exception(string.Format("服务：{0} 未在暂停状态...", serviceName));
                        }
                    }
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("服务：{0} 不存在...", serviceName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("ResumeService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 停止Windows服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>停止成功返回 true,否则返回 false;</returns>
        public static bool StopService(string serviceName)
        {
            try
            {
                //判断服务是否存在
                if (ServicesExisted(serviceName))
                {
                    using (System.ServiceProcess.ServiceController service = new System.ServiceProcess.ServiceController(serviceName))
                    {
                        //判断服务的运行状态(判断服务是否已经停止和正在停止)
                        if (service.Status != System.ServiceProcess.ServiceControllerStatus.Stopped &&
                            service.Status != System.ServiceProcess.ServiceControllerStatus.StopPending)
                        {
                            //停止服务
                            service.Stop();

                            //判断是否超时
                            for (int i = 0; i < 60; i++)
                            {
                                System.Threading.Thread.Sleep(1000);
                                service.Refresh();
                                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                                {
                                    break;
                                }
                                if (i == 59)
                                {
                                    throw new Exception(string.Format("服务：{0} 停止时超时...", serviceName));
                                }
                            }
                        }
                        //service.Close();
                        //service.Dispose();
                    }
                    return true;
                }
                else
                {
                    throw new Exception(string.Format("服务：{0} 不存在...", serviceName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("StopService Error\n" + ex.Message);
            }
        }

        /// <summary>
        /// 重启Windows服务
        /// </summary>
        /// <param name="serviceName">服务名</param>
        /// <returns>重启成功返回 true,否则返回 false;</returns>
        public static bool RefreshService(string serviceName)
        {
            return StopService(serviceName) && StartService(serviceName);
        }
    }
}
