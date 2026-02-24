using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolMangementSystem
{
    public partial class TeacherClassesForm : UserControl
    {
        public TeacherClassesForm()
        {
            InitializeComponent();
            SetupStudentGrid();
            LoadTeacherClasses();
        }

        private void LoadTeacherClasses()
        {
            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                string sql = @"
            SELECT DISTINCT c.class_id, c.class_name
            FROM teaching_assignments ta
            INNER JOIN classes c 
                ON ta.class_id = c.class_id
            WHERE ta.teacher_id = @tid";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tid", Session.TeacherDbId);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbClass.DataSource = dt;
                cbClass.DisplayMember = "class_name";
                cbClass.ValueMember = "class_id";
            }
        }
        private void cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClass.SelectedValue == null || cbClass.SelectedValue is DataRowView)
                return;

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            LoadStudents(classId);
        }
        private void LoadStudents(int classId)
        {
            dgvStudents.Rows.Clear();

            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                string sql = @"
                    SELECT student_id,
                           student_name,
                           student_gender,
                           student_address
                    FROM students
                    WHERE class_id = @cid";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", classId);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dgvStudents.Rows.Add(
                        dr["student_id"],
                        dr["student_name"],
                        dr["student_gender"],
                        dr["student_address"]
                    );
                }

                dr.Close();
            }
        }
        private void SetupStudentGrid()
        {
            dgvStudents.Columns.Clear();
            dgvStudents.Columns.Add("colId", "Student ID");
            dgvStudents.Columns.Add("colName", "Student Name");
            dgvStudents.Columns.Add("colGender", "Gender");
            dgvStudents.Columns.Add("colAddress", "Address");

            dgvStudents.ColumnHeadersHeight = 45;
            dgvStudents.EnableHeadersVisualStyles = false;

            dgvStudents.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(4, 87, 122);

            dgvStudents.ColumnHeadersDefaultCellStyle.ForeColor =
                System.Drawing.Color.White;

            dgvStudents.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);

            dgvStudents.DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
        }
    }
}
