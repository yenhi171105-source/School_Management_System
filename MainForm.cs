using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolMangementSystem
{
    public partial class MainForm : Form
    {
        public DashboardForm Dashboard => dashboardForm1;
        private ManagerAssignForm managerAssignForm1;
        private ManagerTimetableForm managerTimetableForm1;
        private TeacherClassesForm teacherClassesForm1;
        public MainForm()
        {
            InitializeComponent();
            label1.Text = $"Welcome, {Session.Username}";
            // tạo ManagerAssignForm
            managerAssignForm1 = new ManagerAssignForm();
            managerAssignForm1.Dock = DockStyle.Fill;
            managerAssignForm1.Visible = false;
            panel3.Controls.Add(managerAssignForm1);

            // tạo TeacherScoreForm
            bool isManager = Session.Role == "Manager";

            teacherScoreForm1 = new TeacherScoreForm(isManager);
            teacherScoreForm1.Dock = DockStyle.Fill;
            teacherScoreForm1.Visible = false;
            panel3.Controls.Add(teacherScoreForm1);

            teacherScoreForm1.Dock = DockStyle.Fill;
            teacherScoreForm1.Visible = false;
            panel3.Controls.Add(teacherScoreForm1);

            managerTimetableForm1 = new ManagerTimetableForm();
            managerTimetableForm1.Dock = DockStyle.Fill;
            managerTimetableForm1.Visible = false;
            panel3.Controls.Add(managerTimetableForm1);

            teacherClassesForm1 = new TeacherClassesForm();
            teacherClassesForm1.Dock = DockStyle.Fill;
            teacherClassesForm1.Visible = false;
            panel3.Controls.Add(teacherClassesForm1);

            ApplyMenuPermission();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult check = MessageBox.Show("Are you sure you want to logout?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (check == DialogResult.Yes)
            {
                LoginForm lForm = new LoginForm();
                lForm.Show();
                this.Hide();
            }
        }
        private void HideAllForms()
        {
            dashboardForm1.Visible = false;
            addStudentForm1.Visible = false;
            addTeachersForm1.Visible = false;
            addUsersForm1.Visible = false;
            managerAssignForm1.Visible = false;
            teacherScoreForm1.Visible = false;
            managerTimetableForm1.Visible = false;
        }
        private void HideAllMenus()
        {
            AddTeacher_btn.Visible = false;
            AddStudent_btn.Visible = false;
            btnAddUsers.Visible = false;
            btnManagerAssign.Visible = false;
            btnTeacherScore.Visible = false;
            btnTimetable.Visible = false;
            btnMyClasses.Visible = false;
        }
        private void btnTimetable_Click(object sender, EventArgs e)
        {
            HideAllForms();

            if (Session.Role != "Admin")
            {
                managerTimetableForm1.Visible = true;
                managerTimetableForm1.BringToFront();
            }
        }
        private void btnTeacherScore_Click(object sender, EventArgs e)
        {
            HideAllForms();
            teacherScoreForm1.Visible = true;
            teacherScoreForm1.BringToFront();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            dashboardForm1.Visible = false;
            addStudentForm1.Visible = false;
            addTeachersForm1.Visible = true;
            addTeachersForm1.Update();
            addTeachersForm1.BringToFront();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
          
            RefreshDashboardData();

            dashboardForm1.Visible = true;
            dashboardForm1.Update();
            addStudentForm1.Visible = false;
            addTeachersForm1.Visible = false;
            addUsersForm1.Visible = false;
            dashboardForm1.BringToFront();
        }

        private void AddStudent_btn_Click(object sender, EventArgs e)
        {
            dashboardForm1.Visible = false;
            addStudentForm1.Visible = true;
            addStudentForm1.Update();
            addTeachersForm1.Visible = false;
            addStudentForm1.BringToFront();
        }

        private void RefreshDashboardData()
        {
            dashboardForm1.LoadDashboard();
        }
        
        private void ApplyMenuPermission() 
        { 
            string role = Session.Role?.Trim().ToLower();
            HideAllMenus();

            // ADMIN
            if (role == "admin")
            {
                AddTeacher_btn.Visible = true;
                AddStudent_btn.Visible = true;
                btnAddUsers.Visible = true;

            }
            // MANAGER
            else if (role == "manager")
            {
                btnTeacherScore.Visible = true;
                AddStudent_btn.Visible = true;
                btnManagerAssign.Visible = true;
                btnTimetable.Visible = true;
            }
            // TEACHER
            else if (role == "teacher")
            {
                AddStudent_btn.Visible = true;
                btnTeacherScore.Visible = true;
                btnTimetable.Visible = true;
                btnMyClasses.Visible = true;
            }
        }


        private void addUsersForm1_Load(object sender, EventArgs e)
        {
            dashboardForm1.Visible = false;
            addStudentForm1.Visible = false;
            addTeachersForm1.Visible = false;
            addUsersForm1.Visible = true;
            addUsersForm1.BringToFront();
        }
        private void btnManagerAssign_Click(object sender, EventArgs e)
        {
            managerAssignForm1.Visible = true;
            managerAssignForm1.BringToFront();
        }
        public void ReloadUsers()
        {
            addUsersForm1.LoadUsers(); 
        }

        public void ReloadTeachers()
        {
            addTeachersForm1.ReloadTeachers();
        }

        private void btnMyClasses_Click(object sender, EventArgs e)
        {
            HideAllForms();
            teacherClassesForm1.Visible = true;
            teacherClassesForm1.BringToFront();
        }
    }
}