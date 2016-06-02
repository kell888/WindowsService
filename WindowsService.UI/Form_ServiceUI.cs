using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsService.UI
{
    public partial class Form_ServiceUI : Form
    {
        public Form_ServiceUI()
        {
            InitializeComponent();
        }

        private void btn_Select_Click(object sender, EventArgs e)
        {
            txt_ExePath.Text = "";
            using (OpenFileDialog ofd = new OpenFileDialog() { DefaultExt = ".exe", Filter = "exe文件(.exe)|*.exe" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txt_ExePath.Text = ofd.FileName;
                }
            }
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                string txtpath = txt_ExePath.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }
                if (txtpath == "")
                {
                    MessageBox.Show("服务文件不能为空！");
                    return;
                }

                WService.InstallService(new System.Collections.Hashtable(), serviceName, txtpath);
                MessageBox.Show("服务安装完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnUnInstall_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                string txtpath = txt_ExePath.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }
                if (txtpath == "")
                {
                    MessageBox.Show("服务文件不能为空！");
                    return;
                }

                WService.UnInstallService(serviceName, txtpath);
                MessageBox.Show("服务卸载完成！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }


                bool ret = WService.StartService(serviceName);
                if (ret)
                    MessageBox.Show("服务启动成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }


                bool ret = WService.RefreshService(serviceName);
                if (ret)
                    MessageBox.Show("服务重启成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }


                bool ret = WService.PauseService(serviceName);
                if (ret)
                    MessageBox.Show("服务暂停成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }


                bool ret = WService.ResumeService(serviceName);
                if (ret)
                    MessageBox.Show("服务恢复成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                string serviceName = txt_ServiceName.Text.Trim();
                if (serviceName == "")
                {
                    MessageBox.Show("服务名不能为空！");
                    return;
                }


                if (WService.StopService(serviceName))
                    MessageBox.Show("服务停止成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnRefreshServices_Click(object sender, EventArgs e)
        {
            System.ServiceProcess.ServiceController[] services = WService.GetAllServices;

            listView_Services.Items.Clear();
            ListViewItem item;
            foreach (var service in services)
            {
                string[] items = { service.ServiceName, "", service.Status.ToString(), service.ServiceType.ToString() };
                item = new ListViewItem(items);
                listView_Services.Items.Add(item);
            }
        }



    }
}
