using System.Drawing;
using System.Windows.Forms;

namespace SchoolMangementSystem
{
    partial class ManagerAssignForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.ComboBox cbTeacher;
        private System.Windows.Forms.ComboBox cbClass;
        private System.Windows.Forms.ComboBox cbSubject;
        private System.Windows.Forms.Button btnAssign;
        private System.Windows.Forms.DataGridView dgvAssign;
        private System.Windows.Forms.Label lblTeacher;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblSubject;

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
            this.panelTop = new System.Windows.Forms.Panel();
            this.btnDeleteAssign = new System.Windows.Forms.Button();
            this.cbTeacher = new System.Windows.Forms.ComboBox();
            this.cbClass = new System.Windows.Forms.ComboBox();
            this.cbSubject = new System.Windows.Forms.ComboBox();
            this.btnAssign = new System.Windows.Forms.Button();
            this.lblTeacher = new System.Windows.Forms.Label();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.lblHomeroom = new System.Windows.Forms.Label();
            this.cbHomeroomTeacher = new System.Windows.Forms.ComboBox();
            this.btnAssignHomeroom = new System.Windows.Forms.Button();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.dgvAssign = new System.Windows.Forms.DataGridView();
            this.panelTop.SuspendLayout();
            this.panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssign)).BeginInit();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelTop.Controls.Add(this.btnDeleteAssign);
            this.panelTop.Controls.Add(this.cbTeacher);
            this.panelTop.Controls.Add(this.cbClass);
            this.panelTop.Controls.Add(this.cbSubject);
            this.panelTop.Controls.Add(this.btnAssign);
            this.panelTop.Controls.Add(this.lblTeacher);
            this.panelTop.Controls.Add(this.lblClass);
            this.panelTop.Controls.Add(this.lblSubject);
            this.panelTop.Controls.Add(this.lblHomeroom);
            this.panelTop.Controls.Add(this.cbHomeroomTeacher);
            this.panelTop.Controls.Add(this.btnAssignHomeroom);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1852, 150);
            this.panelTop.TabIndex = 1;
            // 
            // btnDeleteAssign
            // 
            this.btnDeleteAssign.BackColor = System.Drawing.Color.Maroon;
            this.btnDeleteAssign.ForeColor = System.Drawing.Color.White;
            this.btnDeleteAssign.Location = new System.Drawing.Point(672, 24);
            this.btnDeleteAssign.Name = "btnDeleteAssign";
            this.btnDeleteAssign.Size = new System.Drawing.Size(120, 35);
            this.btnDeleteAssign.TabIndex = 7;
            this.btnDeleteAssign.Text = "Delete";
            this.btnDeleteAssign.UseVisualStyleBackColor = false;
            this.btnDeleteAssign.Click += new System.EventHandler(this.btnDeleteAssign_Click);
            // 
            // cbTeacher
            // 
            this.cbTeacher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTeacher.Location = new System.Drawing.Point(20, 30);
            this.cbTeacher.Name = "cbTeacher";
            this.cbTeacher.Size = new System.Drawing.Size(200, 24);
            this.cbTeacher.TabIndex = 0;
            // 
            // cbClass
            // 
            this.cbClass.Location = new System.Drawing.Point(250, 30);
            this.cbClass.Name = "cbClass";
            this.cbClass.Size = new System.Drawing.Size(157, 24);
            this.cbClass.TabIndex = 1;
            // 
            // cbSubject
            // 
            this.cbSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubject.Enabled = false;
            this.cbSubject.Location = new System.Drawing.Point(440, 30);
            this.cbSubject.Name = "cbSubject";
            this.cbSubject.Size = new System.Drawing.Size(200, 24);
            this.cbSubject.TabIndex = 2;
            // 
            // btnAssign
            // 
            this.btnAssign.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(4)))), ((int)(((byte)(87)))), ((int)(((byte)(122)))));
            this.btnAssign.ForeColor = System.Drawing.Color.White;
            this.btnAssign.Location = new System.Drawing.Point(672, 24);
            this.btnAssign.Name = "btnAssign";
            this.btnAssign.Size = new System.Drawing.Size(120, 35);
            this.btnAssign.TabIndex = 3;
            this.btnAssign.Text = "Assign";
            this.btnAssign.UseVisualStyleBackColor = false;
            this.btnAssign.Click += new System.EventHandler(this.btnAssign_Click);
            // 
            // lblTeacher
            // 
            this.lblTeacher.AutoSize = true;
            this.lblTeacher.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTeacher.Location = new System.Drawing.Point(20, 10);
            this.lblTeacher.Name = "lblTeacher";
            this.lblTeacher.Size = new System.Drawing.Size(63, 20);
            this.lblTeacher.TabIndex = 4;
            this.lblTeacher.Text = "Teacher";
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Font = this.lblTeacher.Font;
            this.lblClass.Location = new System.Drawing.Point(250, 10);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(44, 20);
            this.lblClass.TabIndex = 5;
            this.lblClass.Text = "Class";
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Font = this.lblTeacher.Font;
            this.lblSubject.Location = new System.Drawing.Point(440, 10);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(60, 20);
            this.lblSubject.TabIndex = 6;
            this.lblSubject.Text = "Subject";
            // 
            // lblHomeroom
            // 
            this.lblHomeroom.AutoSize = true;
            this.lblHomeroom.Font = this.lblTeacher.Font;
            this.lblHomeroom.Location = new System.Drawing.Point(129, 78);
            this.lblHomeroom.Name = "lblHomeroom";
            this.lblHomeroom.Size = new System.Drawing.Size(147, 20);
            this.lblHomeroom.TabIndex = 8;
            this.lblHomeroom.Text = "Homeroom Teacher";
            // 
            // cbHomeroomTeacher
            // 
            this.cbHomeroomTeacher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbHomeroomTeacher.Location = new System.Drawing.Point(129, 98);
            this.cbHomeroomTeacher.Name = "cbHomeroomTeacher";
            this.cbHomeroomTeacher.Size = new System.Drawing.Size(220, 24);
            this.cbHomeroomTeacher.TabIndex = 9;
            // 
            // btnAssignHomeroom
            // 
            this.btnAssignHomeroom.BackColor = System.Drawing.Color.DarkGreen;
            this.btnAssignHomeroom.ForeColor = System.Drawing.Color.White;
            this.btnAssignHomeroom.Location = new System.Drawing.Point(379, 93);
            this.btnAssignHomeroom.Name = "btnAssignHomeroom";
            this.btnAssignHomeroom.Size = new System.Drawing.Size(140, 35);
            this.btnAssignHomeroom.TabIndex = 10;
            this.btnAssignHomeroom.Text = "Assign GVCN";
            this.btnAssignHomeroom.UseVisualStyleBackColor = false;
            this.btnAssignHomeroom.Click += new System.EventHandler(this.btnAssignHomeroom_Click);
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.dgvAssign);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(0, 150);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1852, 599);
            this.panelBottom.TabIndex = 0;
            // 
            // dgvAssign
            // 
            this.dgvAssign.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAssign.ColumnHeadersHeight = 29;
            this.dgvAssign.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAssign.Location = new System.Drawing.Point(0, 0);
            this.dgvAssign.Name = "dgvAssign";
            this.dgvAssign.RowHeadersVisible = false;
            this.dgvAssign.RowHeadersWidth = 51;
            this.dgvAssign.Size = new System.Drawing.Size(1852, 599);
            this.dgvAssign.TabIndex = 0;
            // 
            // ManagerAssignForm
            // 
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.panelTop);
            this.Name = "ManagerAssignForm";
            this.Size = new System.Drawing.Size(1852, 749);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAssign)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Button btnDeleteAssign;
        private ComboBox cbHomeroomTeacher;
        private Button btnAssignHomeroom;
        private Label lblHomeroom;
    }
}
