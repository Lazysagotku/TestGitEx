using TrfCommonUtility;

namespace TimeReportV3
{
    partial class DetailsForm
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
            this.dgvDetailTable = new TrfCommonUtility.CustomDataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailTable)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDetailTable
            // 
            this.dgvDetailTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetailTable.Location = new System.Drawing.Point(12, 12);
            this.dgvDetailTable.Name = "dgvDetailTable";
            this.dgvDetailTable.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvDetailTable.Size = new System.Drawing.Size(832, 309);
            this.dgvDetailTable.TabIndex = 0;
            this.dgvDetailTable.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDetailTable_CellContentClick);
            // 
            // DetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(854, 342);
            this.Controls.Add(this.dgvDetailTable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailsForm";
            this.Text = "Детализация";
            this.Deactivate += new System.EventHandler(this.DetailsForm_Deactivate);
            this.Load += new System.EventHandler(this.DetailsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetailTable)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomDataGridView dgvDetailTable;
    }
}