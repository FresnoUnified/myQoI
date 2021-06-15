
namespace SpeedtestNetCli
{
    partial class ProjectInstaller
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.QOI_Service_Process_Installer = new System.ServiceProcess.ServiceProcessInstaller();
            this.QOI_Service_Installer = new System.ServiceProcess.ServiceInstaller();
            // 
            // QOI_Service_Process_Installer
            // 
            this.QOI_Service_Process_Installer.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.QOI_Service_Process_Installer.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.QOI_Service_Installer});
            this.QOI_Service_Process_Installer.Password = null;
            this.QOI_Service_Process_Installer.Username = null;
            // 
            // QOI_Service_Installer
            // 
            this.QOI_Service_Installer.DisplayName = "QOI Service";
            this.QOI_Service_Installer.ServiceName = "QOI_Service";
            this.QOI_Service_Installer.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.QOI_Service_Process_Installer});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller QOI_Service_Process_Installer;
        private System.ServiceProcess.ServiceInstaller QOI_Service_Installer;
    }
}