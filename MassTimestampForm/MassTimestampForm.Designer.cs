
using TrfCommonUtility;

namespace TimeReportV3
{
    partial class MassTimestampForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSaveAndExit = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxTaskId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbxMinutes = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxHours = new System.Windows.Forms.TextBox();
            this.cbxSetTaskStatusCompleted = new System.Windows.Forms.CheckBox();
            this.dgvMassTasks = new TrfCommonUtility.CustomDataGridView();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMassTasks)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSaveAndExit
            // 
            this.btnSaveAndExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnSaveAndExit.Location = new System.Drawing.Point(629, 326);
            this.btnSaveAndExit.Name = "btnSaveAndExit";
            this.btnSaveAndExit.Size = new System.Drawing.Size(146, 30);
            this.btnSaveAndExit.TabIndex = 8;
            this.btnSaveAndExit.Text = "Списать часы и выйти";
            this.btnSaveAndExit.UseVisualStyleBackColor = true;
            this.btnSaveAndExit.Click += new System.EventHandler(this.btnSaveAndExit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnCancel.Location = new System.Drawing.Point(810, 326);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.tbxTaskId);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbxMinutes);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tbxHours);
            this.groupBox1.Controls.Add(this.cbxSetTaskStatusCompleted);
            this.groupBox1.Location = new System.Drawing.Point(25, 217);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 82);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "исходные данные";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkBox1.Location = new System.Drawing.Point(463, 11);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(228, 20);
            this.checkBox1.TabIndex = 16;
            this.checkBox1.Text = "Так же списать на Инициатора";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(28, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 16);
            this.label4.TabIndex = 15;
            this.label4.Text = "Номер задачи:";
            // 
            // tbxTaskId
            // 
            this.tbxTaskId.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbxTaskId.Location = new System.Drawing.Point(136, 34);
            this.tbxTaskId.Name = "tbxTaskId";
            this.tbxTaskId.Size = new System.Drawing.Size(72, 22);
            this.tbxTaskId.TabIndex = 14;
            this.tbxTaskId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(301, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 15);
            this.label3.TabIndex = 13;
            this.label3.Text = "мин";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(257, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 15);
            this.label2.TabIndex = 12;
            this.label2.Text = "часы";
            // 
            // tbxMinutes
            // 
            this.tbxMinutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbxMinutes.Location = new System.Drawing.Point(301, 35);
            this.tbxMinutes.MaxLength = 2;
            this.tbxMinutes.Name = "tbxMinutes";
            this.tbxMinutes.Size = new System.Drawing.Size(31, 22);
            this.tbxMinutes.TabIndex = 11;
            this.tbxMinutes.Text = "00";
            this.tbxMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxMinutes_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(289, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(11, 16);
            this.label1.TabIndex = 10;
            this.label1.Text = ":";
            // 
            // tbxHours
            // 
            this.tbxHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbxHours.Location = new System.Drawing.Point(257, 35);
            this.tbxHours.MaxLength = 2;
            this.tbxHours.Name = "tbxHours";
            this.tbxHours.Size = new System.Drawing.Size(31, 22);
            this.tbxHours.TabIndex = 9;
            this.tbxHours.Text = "00";
            this.tbxHours.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxHours.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbxHours_KeyPress);
            // 
            // cbxSetTaskStatusCompleted
            // 
            this.cbxSetTaskStatusCompleted.AutoSize = true;
            this.cbxSetTaskStatusCompleted.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbxSetTaskStatusCompleted.Location = new System.Drawing.Point(463, 56);
            this.cbxSetTaskStatusCompleted.Name = "cbxSetTaskStatusCompleted";
            this.cbxSetTaskStatusCompleted.Size = new System.Drawing.Size(287, 20);
            this.cbxSetTaskStatusCompleted.TabIndex = 8;
            this.cbxSetTaskStatusCompleted.Text = "установить статус задачи \"Выполнена\"";
            this.cbxSetTaskStatusCompleted.UseVisualStyleBackColor = true;
            this.cbxSetTaskStatusCompleted.CheckedChanged += new System.EventHandler(this.cbxSetTaskStatusCompleted_CheckedChanged);
            // 
            // dgvMassTasks
            // 
            this.dgvMassTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMassTasks.Location = new System.Drawing.Point(25, 30);
            this.dgvMassTasks.Name = "dgvMassTasks";
            this.dgvMassTasks.Size = new System.Drawing.Size(860, 165);
            this.dgvMassTasks.TabIndex = 11;
            this.dgvMassTasks.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMassTasks_CellContentClick);
            // 
            // MassTimestampForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(897, 368);
            this.Controls.Add(this.dgvMassTasks);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveAndExit);
            this.Name = "MassTimestampForm";
            this.Text = "Массовая отметка времени";
            this.Load += new System.EventHandler(this.MassTimestampForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMassTasks)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSaveAndExit;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxTaskId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbxMinutes;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbxHours;
        private System.Windows.Forms.CheckBox cbxSetTaskStatusCompleted;
        private CustomDataGridView dgvMassTasks;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}