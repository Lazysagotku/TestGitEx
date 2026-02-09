
namespace TimeReportV3
{
    partial class ReportForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.nudYear = new System.Windows.Forms.NumericUpDown();
            this.rbKvartal = new System.Windows.Forms.RadioButton();
            this.cmbKvartal = new System.Windows.Forms.ComboBox();
            this.cmbMonth = new System.Windows.Forms.ComboBox();
            this.rbMonth = new System.Windows.Forms.RadioButton();
            this.rbPrevWeek = new System.Windows.Forms.RadioButton();
            this.rbWeek = new System.Windows.Forms.RadioButton();
            this.rbPeriod = new System.Windows.Forms.RadioButton();
            this.rbToDay = new System.Windows.Forms.RadioButton();
            this.dtpBegin = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rdGroup = new System.Windows.Forms.RadioButton();
            this.rdList = new System.Windows.Forms.RadioButton();
            this.rbAllUsers = new System.Windows.Forms.RadioButton();
            this.rbActiveUsers = new System.Windows.Forms.RadioButton();
            this.cmbUser = new System.Windows.Forms.ComboBox();
            this.cbReportTimeByUser = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ReportName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.checkBoxSum = new System.Windows.Forms.CheckBox();
            this.rbMyDocuments = new System.Windows.Forms.RadioButton();
            this.rbTempFolder = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudYear);
            this.groupBox1.Controls.Add(this.rbKvartal);
            this.groupBox1.Controls.Add(this.cmbKvartal);
            this.groupBox1.Controls.Add(this.cmbMonth);
            this.groupBox1.Controls.Add(this.rbMonth);
            this.groupBox1.Controls.Add(this.rbPrevWeek);
            this.groupBox1.Controls.Add(this.rbWeek);
            this.groupBox1.Controls.Add(this.rbPeriod);
            this.groupBox1.Controls.Add(this.rbToDay);
            this.groupBox1.Controls.Add(this.dtpBegin);
            this.groupBox1.Controls.Add(this.dtpEnd);
            this.groupBox1.Location = new System.Drawing.Point(1, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 125);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Период отчета";
            // 
            // nudYear
            // 
            this.nudYear.Enabled = false;
            this.nudYear.Location = new System.Drawing.Point(252, 55);
            this.nudYear.Maximum = new decimal(new int[] {
            2012,
            0,
            0,
            0});
            this.nudYear.Minimum = new decimal(new int[] {
            2011,
            0,
            0,
            0});
            this.nudYear.Name = "nudYear";
            this.nudYear.Size = new System.Drawing.Size(54, 20);
            this.nudYear.TabIndex = 11;
            this.nudYear.Value = new decimal(new int[] {
            2012,
            0,
            0,
            0});
            this.nudYear.ValueChanged += new System.EventHandler(this.nudYear_ValueChanged);
            // 
            // rbKvartal
            // 
            this.rbKvartal.AutoSize = true;
            this.rbKvartal.Location = new System.Drawing.Point(6, 69);
            this.rbKvartal.Name = "rbKvartal";
            this.rbKvartal.Size = new System.Drawing.Size(81, 17);
            this.rbKvartal.TabIndex = 10;
            this.rbKvartal.Text = "за квартал";
            this.rbKvartal.UseVisualStyleBackColor = true;
            this.rbKvartal.CheckedChanged += new System.EventHandler(this.rbKvartal_CheckedChanged);
            // 
            // cmbKvartal
            // 
            this.cmbKvartal.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKvartal.Enabled = false;
            this.cmbKvartal.FormattingEnabled = true;
            this.cmbKvartal.Items.AddRange(new object[] {
            "I   Квартал",
            "II  Квартал",
            "III Квартал",
            "IV  Квартал"});
            this.cmbKvartal.Location = new System.Drawing.Point(157, 69);
            this.cmbKvartal.MaxDropDownItems = 12;
            this.cmbKvartal.Name = "cmbKvartal";
            this.cmbKvartal.Size = new System.Drawing.Size(89, 21);
            this.cmbKvartal.TabIndex = 9;
            this.cmbKvartal.SelectedIndexChanged += new System.EventHandler(this.cmbKvartal_SelectedIndexChanged);
            // 
            // cmbMonth
            // 
            this.cmbMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMonth.Enabled = false;
            this.cmbMonth.FormattingEnabled = true;
            this.cmbMonth.Items.AddRange(new object[] {
            "Январь",
            "Февраль",
            "Март",
            "Апрель",
            "Май",
            "Июнь",
            "Июль",
            "Август",
            "Сентябрь",
            "Октябрь",
            "Ноябрь",
            "Декабрь"});
            this.cmbMonth.Location = new System.Drawing.Point(157, 42);
            this.cmbMonth.MaxDropDownItems = 12;
            this.cmbMonth.Name = "cmbMonth";
            this.cmbMonth.Size = new System.Drawing.Size(89, 21);
            this.cmbMonth.TabIndex = 8;
            this.cmbMonth.SelectedIndexChanged += new System.EventHandler(this.cmbMonth_SelectedIndexChanged);
            // 
            // rbMonth
            // 
            this.rbMonth.AutoSize = true;
            this.rbMonth.Location = new System.Drawing.Point(6, 42);
            this.rbMonth.Name = "rbMonth";
            this.rbMonth.Size = new System.Drawing.Size(72, 17);
            this.rbMonth.TabIndex = 7;
            this.rbMonth.Text = "за месяц";
            this.rbMonth.UseVisualStyleBackColor = true;
            this.rbMonth.CheckedChanged += new System.EventHandler(this.rbMonth_CheckedChanged);
            // 
            // rbPrevWeek
            // 
            this.rbPrevWeek.AutoSize = true;
            this.rbPrevWeek.Location = new System.Drawing.Point(194, 19);
            this.rbPrevWeek.Name = "rbPrevWeek";
            this.rbPrevWeek.Size = new System.Drawing.Size(110, 17);
            this.rbPrevWeek.TabIndex = 3;
            this.rbPrevWeek.Text = "Прошлая неделя";
            this.rbPrevWeek.UseVisualStyleBackColor = true;
            this.rbPrevWeek.CheckedChanged += new System.EventHandler(this.rbPrevWeek_CheckedChanged);
            // 
            // rbWeek
            // 
            this.rbWeek.AutoSize = true;
            this.rbWeek.Checked = true;
            this.rbWeek.Location = new System.Drawing.Point(79, 19);
            this.rbWeek.Name = "rbWeek";
            this.rbWeek.Size = new System.Drawing.Size(109, 17);
            this.rbWeek.TabIndex = 2;
            this.rbWeek.TabStop = true;
            this.rbWeek.Text = "Текущая неделя";
            this.rbWeek.UseVisualStyleBackColor = true;
            this.rbWeek.CheckedChanged += new System.EventHandler(this.rbWeek_CheckedChanged);
            // 
            // rbPeriod
            // 
            this.rbPeriod.AutoSize = true;
            this.rbPeriod.Location = new System.Drawing.Point(6, 97);
            this.rbPeriod.Name = "rbPeriod";
            this.rbPeriod.Size = new System.Drawing.Size(77, 17);
            this.rbPeriod.TabIndex = 4;
            this.rbPeriod.Text = "За период";
            this.rbPeriod.UseVisualStyleBackColor = true;
            this.rbPeriod.CheckedChanged += new System.EventHandler(this.rbPeriod_CheckedChanged);
            // 
            // rbToDay
            // 
            this.rbToDay.AutoSize = true;
            this.rbToDay.Location = new System.Drawing.Point(6, 19);
            this.rbToDay.Name = "rbToDay";
            this.rbToDay.Size = new System.Drawing.Size(67, 17);
            this.rbToDay.TabIndex = 1;
            this.rbToDay.Text = "Сегодня";
            this.rbToDay.UseVisualStyleBackColor = true;
            this.rbToDay.CheckedChanged += new System.EventHandler(this.rbToDay_CheckedChanged);
            // 
            // dtpBegin
            // 
            this.dtpBegin.CustomFormat = "dd.mm.yyyy";
            this.dtpBegin.Enabled = false;
            this.dtpBegin.Location = new System.Drawing.Point(89, 97);
            this.dtpBegin.Name = "dtpBegin";
            this.dtpBegin.Size = new System.Drawing.Size(128, 20);
            this.dtpBegin.TabIndex = 5;
            this.dtpBegin.Value = new System.DateTime(2011, 11, 28, 0, 0, 0, 0);
            this.dtpBegin.ValueChanged += new System.EventHandler(this.dtpBegin_ValueChanged);
            // 
            // dtpEnd
            // 
            this.dtpEnd.CustomFormat = "dd.mm.yyyy";
            this.dtpEnd.Enabled = false;
            this.dtpEnd.Location = new System.Drawing.Point(221, 97);
            this.dtpEnd.Name = "dtpEnd";
            this.dtpEnd.Size = new System.Drawing.Size(128, 20);
            this.dtpEnd.TabIndex = 6;
            this.dtpEnd.Value = new System.DateTime(2011, 12, 2, 0, 0, 0, 0);
            this.dtpEnd.ValueChanged += new System.EventHandler(this.dtpEnd_ValueChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tabControl1);
            this.groupBox3.Location = new System.Drawing.Point(1, 134);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(359, 145);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Объект отчета";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(6, 19);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(343, 120);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.rbAllUsers);
            this.tabPage1.Controls.Add(this.rbActiveUsers);
            this.tabPage1.Controls.Add(this.cmbUser);
            this.tabPage1.Controls.Add(this.cbReportTimeByUser);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(335, 94);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "По пользователю";
            this.tabPage1.UseVisualStyleBackColor = true;
            this.tabPage1.Click += new System.EventHandler(this.tabPage1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rdGroup);
            this.panel1.Controls.Add(this.rdList);
            this.panel1.Location = new System.Drawing.Point(6, 61);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(323, 26);
            this.panel1.TabIndex = 16;
            // 
            // rdGroup
            // 
            this.rdGroup.AutoSize = true;
            this.rdGroup.Checked = true;
            this.rdGroup.Location = new System.Drawing.Point(25, 4);
            this.rdGroup.Name = "rdGroup";
            this.rdGroup.Size = new System.Drawing.Size(139, 17);
            this.rdGroup.TabIndex = 14;
            this.rdGroup.TabStop = true;
            this.rdGroup.Text = "Группировать по дням";
            this.rdGroup.UseVisualStyleBackColor = true;
            // 
            // rdList
            // 
            this.rdList.AutoSize = true;
            this.rdList.Location = new System.Drawing.Point(178, 4);
            this.rdList.Name = "rdList";
            this.rdList.Size = new System.Drawing.Size(109, 17);
            this.rdList.TabIndex = 15;
            this.rdList.Text = "Общим списком";
            this.rdList.UseVisualStyleBackColor = true;
            // 
            // rbAllUsers
            // 
            this.rbAllUsers.AutoSize = true;
            this.rbAllUsers.Location = new System.Drawing.Point(176, 9);
            this.rbAllUsers.Name = "rbAllUsers";
            this.rbAllUsers.Size = new System.Drawing.Size(44, 17);
            this.rbAllUsers.TabIndex = 13;
            this.rbAllUsers.Text = "Все";
            this.rbAllUsers.UseVisualStyleBackColor = true;
            this.rbAllUsers.CheckedChanged += new System.EventHandler(this.rbAllUsers_CheckedChanged);
            // 
            // rbActiveUsers
            // 
            this.rbActiveUsers.AutoSize = true;
            this.rbActiveUsers.Checked = true;
            this.rbActiveUsers.Location = new System.Drawing.Point(95, 9);
            this.rbActiveUsers.Name = "rbActiveUsers";
            this.rbActiveUsers.Size = new System.Drawing.Size(75, 17);
            this.rbActiveUsers.TabIndex = 12;
            this.rbActiveUsers.TabStop = true;
            this.rbActiveUsers.Text = "Активные";
            this.rbActiveUsers.UseVisualStyleBackColor = true;
            this.rbActiveUsers.CheckedChanged += new System.EventHandler(this.rbActiveUsers_CheckedChanged);
            // 
            // cmbUser
            // 
            this.cmbUser.DisplayMember = "fullname";
            this.cmbUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbUser.FormattingEnabled = true;
            this.cmbUser.Location = new System.Drawing.Point(6, 36);
            this.cmbUser.MaxDropDownItems = 11;
            this.cmbUser.Name = "cmbUser";
            this.cmbUser.Size = new System.Drawing.Size(218, 21);
            this.cmbUser.TabIndex = 7;
            this.cmbUser.ValueMember = "id";
            this.cmbUser.SelectedIndexChanged += new System.EventHandler(this.cmbUser_SelectedIndexChanged);
            // 
            // cbReportTimeByUser
            // 
            this.cbReportTimeByUser.Location = new System.Drawing.Point(229, 36);
            this.cbReportTimeByUser.Name = "cbReportTimeByUser";
            this.cbReportTimeByUser.Size = new System.Drawing.Size(100, 23);
            this.cbReportTimeByUser.TabIndex = 11;
            this.cbReportTimeByUser.Text = "Сформировать";
            this.cbReportTimeByUser.UseVisualStyleBackColor = true;
            this.cbReportTimeByUser.Click += new System.EventHandler(this.cbReportTimeByUser_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView1);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(335, 94);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "По всем пользователям / По задачам";
            this.tabPage2.UseVisualStyleBackColor = true;
            this.tabPage2.Click += new System.EventHandler(this.tabPage2_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ReportName});
            this.dataGridView1.Location = new System.Drawing.Point(1, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(336, 95);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // ReportName
            // 
            this.ReportName.HeaderText = "Отчеты";
            this.ReportName.Name = "ReportName";
            this.ReportName.ToolTipText = "ReportName";
            this.ReportName.Width = 300;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.groupBox2.Controls.Add(this.checkBoxSum);
            this.groupBox2.Controls.Add(this.rbMyDocuments);
            this.groupBox2.Controls.Add(this.rbTempFolder);
            this.groupBox2.Location = new System.Drawing.Point(0, 286);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(359, 69);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Сохранять отчеты";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // checkBoxSum
            // 
            this.checkBoxSum.AutoSize = true;
            this.checkBoxSum.Checked = true;
            this.checkBoxSum.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxSum.Location = new System.Drawing.Point(240, 30);
            this.checkBoxSum.Name = "checkBoxSum";
            this.checkBoxSum.Size = new System.Drawing.Size(83, 17);
            this.checkBoxSum.TabIndex = 2;
            this.checkBoxSum.Text = "Автосумма";
            this.checkBoxSum.UseVisualStyleBackColor = true;
            // 
            // rbMyDocuments
            // 
            this.rbMyDocuments.AutoSize = true;
            this.rbMyDocuments.Location = new System.Drawing.Point(15, 44);
            this.rbMyDocuments.Name = "rbMyDocuments";
            this.rbMyDocuments.Size = new System.Drawing.Size(146, 17);
            this.rbMyDocuments.TabIndex = 1;
            this.rbMyDocuments.TabStop = true;
            this.rbMyDocuments.Text = "в папку Мои документы";
            this.rbMyDocuments.UseVisualStyleBackColor = true;
            // 
            // rbTempFolder
            // 
            this.rbTempFolder.AutoSize = true;
            this.rbTempFolder.Location = new System.Drawing.Point(15, 20);
            this.rbTempFolder.Name = "rbTempFolder";
            this.rbTempFolder.Size = new System.Drawing.Size(129, 17);
            this.rbTempFolder.TabIndex = 0;
            this.rbTempFolder.TabStop = true;
            this.rbTempFolder.Text = "во временную папку";
            this.rbTempFolder.UseVisualStyleBackColor = true;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 362);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Name = "ReportForm";
            this.Text = "Reports";
            this.Deactivate += new System.EventHandler(this.ReportForm_Deactivate);
            this.Load += new System.EventHandler(this.ReportForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudYear)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown nudYear;
        private System.Windows.Forms.RadioButton rbKvartal;
        private System.Windows.Forms.ComboBox cmbKvartal;
        private System.Windows.Forms.ComboBox cmbMonth;
        private System.Windows.Forms.RadioButton rbMonth;
        private System.Windows.Forms.RadioButton rbPrevWeek;
        private System.Windows.Forms.RadioButton rbWeek;
        private System.Windows.Forms.RadioButton rbPeriod;
        private System.Windows.Forms.RadioButton rbToDay;
        private System.Windows.Forms.DateTimePicker dtpBegin;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cmbUser;
        private System.Windows.Forms.Button cbReportTimeByUser;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbMyDocuments;
        private System.Windows.Forms.RadioButton rbTempFolder;
        private System.Windows.Forms.RadioButton rbAllUsers;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton rbActiveUsers;
        private System.Windows.Forms.RadioButton rdList;
        private System.Windows.Forms.RadioButton rdGroup;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBoxSum;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ReportName;
    }
}