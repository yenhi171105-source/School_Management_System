namespace SchoolMangementSystem
{
    partial class TeacherScoreForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.ComboBox cbClass;
        private System.Windows.Forms.ComboBox cbSubject;
        private System.Windows.Forms.ComboBox cbSemester;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblSemester;
        private System.Windows.Forms.DataGridView dgvScores;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.cbClass = new System.Windows.Forms.ComboBox();
            this.cbSubject = new System.Windows.Forms.ComboBox();
            this.cbSemester = new System.Windows.Forms.ComboBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblSemester = new System.Windows.Forms.Label();
            this.dgvScores = new System.Windows.Forms.DataGridView();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvScores)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.cbClass);
            this.panelTop.Controls.Add(this.cbSubject);
            this.panelTop.Controls.Add(this.cbSemester);
            this.panelTop.Controls.Add(this.lblClass);
            this.panelTop.Controls.Add(this.lblSubject);
            this.panelTop.Controls.Add(this.lblSemester);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1200, 60);
            this.panelTop.TabIndex = 3;
            // 
            // cbClass
            // 
            this.cbClass.Location = new System.Drawing.Point(65, 17);
            this.cbClass.Name = "cbClass";
            this.cbClass.Size = new System.Drawing.Size(121, 24);
            this.cbClass.TabIndex = 0;
            // 
            // cbSubject
            // 
            this.cbSubject.Enabled = false;
            this.cbSubject.Location = new System.Drawing.Point(283, 17);
            this.cbSubject.Name = "cbSubject";
            this.cbSubject.Size = new System.Drawing.Size(121, 24);
            this.cbSubject.TabIndex = 1;
            // 
            // cbSemester
            // 
            this.cbSemester.Location = new System.Drawing.Point(499, 17);
            this.cbSemester.Name = "cbSemester";
            this.cbSemester.Size = new System.Drawing.Size(121, 24);
            this.cbSemester.TabIndex = 2;
            // 
            // lblClass
            // 
            this.lblClass.Location = new System.Drawing.Point(20, 20);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(100, 23);
            this.lblClass.TabIndex = 3;
            this.lblClass.Text = "Class";
            // 
            // lblSubject
            // 
            this.lblSubject.Location = new System.Drawing.Point(220, 20);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(100, 23);
            this.lblSubject.TabIndex = 4;
            this.lblSubject.Text = "Subject";
            // 
            // lblSemester
            // 
            this.lblSemester.Location = new System.Drawing.Point(428, 20);
            this.lblSemester.Name = "lblSemester";
            this.lblSemester.Size = new System.Drawing.Size(100, 23);
            this.lblSemester.TabIndex = 5;
            this.lblSemester.Text = "Semester";
            // 
            // dgvScores
            // 
            this.dgvScores.AllowUserToAddRows = false;
            this.dgvScores.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvScores.ColumnHeadersHeight = 29;
            this.dgvScores.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvScores.Location = new System.Drawing.Point(0, 60);
            this.dgvScores.Name = "dgvScores";
            this.dgvScores.RowHeadersWidth = 51;
            this.dgvScores.Size = new System.Drawing.Size(1200, 400);
            this.dgvScores.TabIndex = 2;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btnSave.Location = new System.Drawing.Point(400, 480);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnClear.Location = new System.Drawing.Point(550, 480);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(120, 35);
            this.btnClear.TabIndex = 0;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // TeacherScoreForm
            // 
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dgvScores);
            this.Controls.Add(this.panelTop);
            this.Name = "TeacherScoreForm";
            this.Size = new System.Drawing.Size(1200, 600);
            this.panelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvScores)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
