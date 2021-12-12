
namespace PostAgent.forms
{
    partial class MasterForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MasterForm));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabSend = new System.Windows.Forms.TabPage();
            this.textInfo = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();
            this.tabDbSetting = new System.Windows.Forms.TabPage();
            this.tabPostSetting = new System.Windows.Forms.TabPage();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl.SuspendLayout();
            this.tabSend.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabSend);
            this.tabControl.Controls.Add(this.tabDbSetting);
            this.tabControl.Controls.Add(this.tabPostSetting);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(933, 554);
            this.tabControl.TabIndex = 2;
            this.tabControl.TabStop = false;
            // 
            // tabSend
            // 
            this.tabSend.Controls.Add(this.textInfo);
            this.tabSend.Controls.Add(this.buttonSend);
            this.tabSend.Location = new System.Drawing.Point(4, 25);
            this.tabSend.Name = "tabSend";
            this.tabSend.Padding = new System.Windows.Forms.Padding(3);
            this.tabSend.Size = new System.Drawing.Size(925, 525);
            this.tabSend.TabIndex = 0;
            this.tabSend.Text = "Отправка почты";
            this.tabSend.UseVisualStyleBackColor = true;
            // 
            // textInfo
            // 
            this.textInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textInfo.Location = new System.Drawing.Point(7, 43);
            this.textInfo.Multiline = true;
            this.textInfo.Name = "textInfo";
            this.textInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textInfo.Size = new System.Drawing.Size(909, 467);
            this.textInfo.TabIndex = 2;
            // 
            // buttonSend
            // 
            this.buttonSend.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSend.Location = new System.Drawing.Point(7, 7);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(87, 29);
            this.buttonSend.TabIndex = 1;
            this.buttonSend.Text = "Отправить";
            this.buttonSend.UseVisualStyleBackColor = true;
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
            // 
            // tabDbSetting
            // 
            this.tabDbSetting.Location = new System.Drawing.Point(4, 25);
            this.tabDbSetting.Name = "tabDbSetting";
            this.tabDbSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabDbSetting.Size = new System.Drawing.Size(925, 525);
            this.tabDbSetting.TabIndex = 1;
            this.tabDbSetting.Text = "Настройки баз данных";
            this.tabDbSetting.UseVisualStyleBackColor = true;
            // 
            // tabPostSetting
            // 
            this.tabPostSetting.Location = new System.Drawing.Point(4, 25);
            this.tabPostSetting.Name = "tabPostSetting";
            this.tabPostSetting.Padding = new System.Windows.Forms.Padding(3);
            this.tabPostSetting.Size = new System.Drawing.Size(925, 525);
            this.tabPostSetting.TabIndex = 2;
            this.tabPostSetting.Text = "Настройки почты";
            this.tabPostSetting.UseVisualStyleBackColor = true;
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // MasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 554);
            this.Controls.Add(this.tabControl);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MasterForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Рассылка почты";
            this.Load += new System.EventHandler(this.MasterForm_Load);
            this.tabControl.ResumeLayout(false);
            this.tabSend.ResumeLayout(false);
            this.tabSend.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabSend;
        private System.Windows.Forms.TextBox textInfo;
        private System.Windows.Forms.Button buttonSend;
        private System.Windows.Forms.TabPage tabDbSetting;
        private System.Windows.Forms.TabPage tabPostSetting;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

