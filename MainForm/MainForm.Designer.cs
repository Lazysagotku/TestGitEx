using System.Windows.Forms;
using TrfCommonUtility;

namespace TimeReportV3
{
    partial class MainForm
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
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.dgvMainTable = new System.Windows.Forms.DataGridView();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.dgvTimeUserTable = new TrfCommonUtility.CustomDataGridView();
            this.dgvIdTasksTable = new TrfCommonUtility.CustomDataGridView();
            this.rbAllUsers = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeUserTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdTasksTable)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // dgvMainTable
            // 
            this.dgvMainTable.Location = new System.Drawing.Point(12, 29);
            this.dgvMainTable.Name = "dgvMainTable";
            this.dgvMainTable.Size = new System.Drawing.Size(577, 288);
            this.dgvMainTable.TabIndex = 1;
            this.dgvMainTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMainTable_CellContentClick);
            // 
            // timerRefresh
            // 
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // dgvTimeUserTable
            // 
            this.dgvTimeUserTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvTimeUserTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTimeUserTable.Location = new System.Drawing.Point(595, 29);
            this.dgvTimeUserTable.Name = "dgvTimeUserTable";
            this.dgvTimeUserTable.RowHeadersWidth = 35;
            this.dgvTimeUserTable.Size = new System.Drawing.Size(163, 177);
            this.dgvTimeUserTable.TabIndex = 2;
            this.dgvTimeUserTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTimeUserTable_CellContentClick);
            // 
            // dgvIdTasksTable
            // 
            this.dgvIdTasksTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvIdTasksTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvIdTasksTable.Location = new System.Drawing.Point(764, 29);
            this.dgvIdTasksTable.Name = "dgvIdTasksTable";
            this.dgvIdTasksTable.Size = new System.Drawing.Size(340, 178);
            this.dgvIdTasksTable.TabIndex = 3;
            this.dgvIdTasksTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvIdTasksTable_CellContentClick);
            // 
            // rbAllUsers
            // 
            this.rbAllUsers.AutoSize = true;
            this.rbAllUsers.Location = new System.Drawing.Point(12, 6);
            this.rbAllUsers.Name = "rbAllUsers";
            this.rbAllUsers.Size = new System.Drawing.Size(35, 17);
            this.rbAllUsers.TabIndex = 14;
            this.rbAllUsers.Text = "IS";
            this.rbAllUsers.UseVisualStyleBackColor = true;
            this.rbAllUsers.CheckedChanged += new System.EventHandler(this.rbAllUsers_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(62, 6);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(41, 17);
            this.radioButton1.TabIndex = 15;
            this.radioButton1.Text = "Jira";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(112, 6);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(44, 17);
            this.radioButton2.TabIndex = 16;
            this.radioButton2.Text = "Все";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 337);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.rbAllUsers);
            this.Controls.Add(this.dgvIdTasksTable);
            this.Controls.Add(this.dgvTimeUserTable);
            this.Controls.Add(this.dgvMainTable);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainForm_Paint);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMainTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTimeUserTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvIdTasksTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.DataGridView dgvMainTable;
        private System.Windows.Forms.Timer timerRefresh;
        private CustomDataGridView dgvTimeUserTable;
        private CustomDataGridView dgvIdTasksTable;
        private System.Windows.Forms.RadioButton rbAllUsers;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
    }
}


