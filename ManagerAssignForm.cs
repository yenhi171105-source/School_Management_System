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
    public partial class ManagerAssignForm : UserControl
    {
        int selectedAssignmentId = 0;
        public ManagerAssignForm()
        {
            InitializeComponent();

            LoadTeachers();
            LoadClasses();
            LoadSubjects();
            LoadAssignments();

            cbClass.SelectedIndexChanged += cbClass_SelectedIndexChanged;
            cbTeacher.SelectedIndexChanged += cbTeacher_SelectedIndexChanged;
            dgvAssign.CellClick += dgvAssign_CellClick;

            this.Click += ManagerAssignForm_Click;
            panelTop.Click += ManagerAssignForm_Click;
            panelBottom.Click += ManagerAssignForm_Click;

            cbHomeroomTeacher.Enabled = false;
            btnAssignHomeroom.Enabled = false;

            btnAssign.Visible = true;
            btnDeleteAssign.Visible = false;
        }
        SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");
        void LoadTeachers()
        {
            cbTeacher.Items.Clear();
            connect.Open();

            SqlCommand cmd = new SqlCommand(
                @"SELECT t.id, teacher_name FROM teachers t
                JOIN users u ON t.user_id = u.id
                WHERE t.date_delete IS NULL
                AND u.is_active = 1
                ", connect);

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbTeacher.Items.Add(new ComboboxItem(
                    dr["teacher_name"].ToString(),   // hiển thị
                    dr["id"]
                ));
            }
            connect.Close();
        }
        void LoadClasses()
        {
            using (SqlConnection conn = new SqlConnection(connect.ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT class_id, class_name FROM classes ORDER BY class_name", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbClass.DataSource = dt;
                cbClass.DisplayMember = "class_name"; // 6A, 7B...
                cbClass.ValueMember = "class_id";     // ID
                cbClass.SelectedIndex = -1;
            }
        }

        void LoadSubjects()
        {
            cbSubject.Items.Clear();
            connect.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT subject_id, subject_name FROM subjects", connect);

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbSubject.Items.Add(
                    new ComboboxItem(dr["subject_name"].ToString(), dr["subject_id"]));
            }
            cbSubject.DropDownStyle = ComboBoxStyle.DropDownList;
            cbSubject.Enabled = false;
            connect.Close();
        }
        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public ComboboxItem(string text, object value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }
        private void btnAssign_Click(object sender, EventArgs e)
        {
            if (cbTeacher.SelectedItem == null || cbClass.SelectedIndex == -1)
            {
                MessageBox.Show("Please select Teacher and Class");
                return;
            }

            int teacherDbId = (int)((ComboboxItem)cbTeacher.SelectedItem).Value;
            int subjectId = GetTeacherSubjectId(teacherDbId);
            int classId = Convert.ToInt32(cbClass.SelectedValue);

            connect.Open();
            SqlCommand checkCmd = new SqlCommand(
                @"SELECT COUNT(*) 
                  FROM teaching_assignments 
                  WHERE teacher_id = @t AND class_id = @c",
                connect);

            checkCmd.Parameters.AddWithValue("@t", teacherDbId);
            checkCmd.Parameters.AddWithValue("@c", classId);

            int exists = (int)checkCmd.ExecuteScalar();

            if (exists > 0)
            {
                MessageBox.Show("This teacher is already assigned to this class!");
                connect.Close();
                return;
            }
            SqlCommand checkSubjectClassCmd = new SqlCommand(
                @"SELECT COUNT(*) 
                  FROM teaching_assignments 
                  WHERE class_id = @c AND subject_id = @s",
                connect);

            checkSubjectClassCmd.Parameters.AddWithValue("@c", classId);
            checkSubjectClassCmd.Parameters.AddWithValue("@s", subjectId);

            int subjectExists = (int)checkSubjectClassCmd.ExecuteScalar();

            if (subjectExists > 0)
            {
                MessageBox.Show(
                    "This class already has a teacher for this subject!",
                    "Invalid Assignment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                connect.Close();
                return;
            }
            SqlCommand countCmd = new SqlCommand(
                @"SELECT COUNT(*) 
                  FROM teaching_assignments 
                  WHERE teacher_id = @t",
                connect);

            countCmd.Parameters.AddWithValue("@t", teacherDbId);

            int classCount = (int)countCmd.ExecuteScalar();

            if (classCount >= 5)
            {
                MessageBox.Show("This teacher has reached the maximum number of classes!");
                connect.Close();
                return;
            }

            SqlCommand cmd = new SqlCommand(
                @"INSERT INTO teaching_assignments (teacher_id, class_id, subject_id)
                VALUES (@t, @c, @s)", connect);

            cmd.Parameters.AddWithValue("@t", teacherDbId);
            cmd.Parameters.AddWithValue("@c", classId);
            cmd.Parameters.AddWithValue("@s", subjectId);

            cmd.ExecuteNonQuery();
            connect.Close();
            btnAssign.Enabled = cbTeacher.SelectedItem != null && cbClass.SelectedIndex != -1;

            LoadAssignments();
            MessageBox.Show("Assigned successfully");
        }
        private void btnDeleteAssign_Click(object sender, EventArgs e)
        {
            if (selectedAssignmentId == 0)
            {
                MessageBox.Show("Please select an assignment first.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Are you sure you want to delete this assignment?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            connect.Open();
            // Lấy thông tin assignment đang chọn
            DataGridViewRow row = dgvAssign.Rows[dgvAssign.CurrentRow.Index];

            int teacherId = GetTeacherIdByName(row.Cells["teacher_name"].Value.ToString());
            int classId = GetClassIdByName(row.Cells["class_name"].Value.ToString());

            // Kiểm tra nếu teacher là GVCN lớp đó
            SqlCommand checkHomeroom = new SqlCommand(
                @"SELECT homeroom_teacher_id 
                  FROM classes 
                  WHERE class_id = @cid", connect);

            checkHomeroom.Parameters.AddWithValue("@cid", classId);

            object result = checkHomeroom.ExecuteScalar();

            if (result != null && Convert.ToInt32(result) == teacherId)
            {
                MessageBox.Show(
                    "Cannot delete this assignment.\nThis teacher is currently homeroom teacher of this class.",
                    "Invalid Operation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                connect.Close();
                return;
            }
            SqlCommand cmd = new SqlCommand(
                "DELETE FROM teaching_assignments WHERE assignment_id = @id", connect);
            cmd.Parameters.AddWithValue("@id", selectedAssignmentId);
            cmd.ExecuteNonQuery();
            SqlCommand checkRemain = new SqlCommand(
            @"SELECT COUNT(*) 
              FROM teaching_assignments
              WHERE teacher_id = @tid
                AND class_id = @cid", connect);

            checkRemain.Parameters.AddWithValue("@tid", teacherId);
            checkRemain.Parameters.AddWithValue("@cid", classId);

            int remain = (int)checkRemain.ExecuteScalar();

            if (remain == 0)
            {
                SqlCommand removeHomeroom = new SqlCommand(
                    @"UPDATE classes
                      SET homeroom_teacher_id = NULL
                      WHERE class_id = @cid", connect);

                removeHomeroom.Parameters.AddWithValue("@cid", classId);
                removeHomeroom.ExecuteNonQuery();
            }
            connect.Close();

            selectedAssignmentId = 0;
            btnAssign.Visible = true;
            btnDeleteAssign.Visible = false;

            LoadAssignments();

            MessageBox.Show("Assignment deleted successfully!");
        }
        int GetTeacherSubjectId(int teacherDbId)
        {
            int subjectId = 0;
            if (connect.State != ConnectionState.Open)
                connect.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT subject_id FROM teachers WHERE id = @tid", connect);

            cmd.Parameters.AddWithValue("@tid", teacherDbId);

            object result = cmd.ExecuteScalar();
            if (result != null)
                subjectId = Convert.ToInt32(result);

            connect.Close();
            return subjectId;
        }
        int GetTeacherIdByName(string teacherName)
        {
            int id = 0;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT id FROM teachers WHERE teacher_name = @name", connect))
            {
                cmd.Parameters.AddWithValue("@name", teacherName);

                object result = cmd.ExecuteScalar();
                if (result != null)
                    id = Convert.ToInt32(result);
            }
            return id;
        }

        int GetClassIdByName(string className)
        {
            int id = 0;
            using (SqlCommand cmd = new SqlCommand(
                "SELECT class_id FROM classes WHERE class_name = @name", connect))
            {
                cmd.Parameters.AddWithValue("@name", className);

                object result = cmd.ExecuteScalar();
                if (result != null)
                    id = Convert.ToInt32(result);
            }
            return id;
        }
        void LoadAssignments()
        {
            connect.Open();
            SqlDataAdapter da = new SqlDataAdapter(
                @"SELECT a.assignment_id,t.teacher_name, c.class_name, s.subject_name, 
                  CASE 
                    WHEN c.homeroom_teacher_id = t.id 
                    THEN 'GVCN' 
                    ELSE '' 
                  END AS GVCN,
                  a.date_assign
                  FROM teaching_assignments a
                  JOIN teachers t ON a.teacher_id = t.id
                  JOIN classes c ON a.class_id = c.class_id
                  JOIN subjects s ON a.subject_id = s.subject_id",
                connect);
            
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvAssign.DataSource = dt;

            dgvAssign.Columns["assignment_id"].Visible = false;
            dgvAssign.ReadOnly = true;
            dgvAssign.AllowUserToAddRows = false;

            btnAssign.Visible = true;
            btnDeleteAssign.Visible = false;
            selectedAssignmentId = 0;

            connect.Close();
        }
        private void dgvAssign_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                ResetAssignMode();
                return;
            }

            DataGridViewRow row = dgvAssign.Rows[e.RowIndex];
            selectedAssignmentId = Convert.ToInt32(row.Cells["assignment_id"].Value);

            btnAssign.Visible = false;
            btnDeleteAssign.Visible = true;
        }
        private void cbTeacher_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAssignMode();
            if (cbTeacher.SelectedItem == null) return;

            int teacherDbId = (int)((ComboboxItem)cbTeacher.SelectedItem).Value;

            LoadTeacherSubject(teacherDbId);
            UpdateAssignButton();
        }
        void LoadTeacherSubject(int teacherDbId)
        {
            cbSubject.Enabled = false; 

            connect.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT s.subject_id, s.subject_name
                  FROM teachers t
                  JOIN subjects s ON t.subject_id = s.subject_id
                  WHERE t.id = @tid", connect);

            cmd.Parameters.AddWithValue("@tid", teacherDbId);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                int subjectId = (int)dr["subject_id"];

                // set subject
                foreach (ComboboxItem item in cbSubject.Items)
                {
                    if ((int)item.Value == subjectId)
                    {
                        cbSubject.SelectedItem = item;
                        break;
                    }
                }
            }

            dr.Close();
            connect.Close();
        }
        void UpdateAssignButton()
        {
            btnAssign.Enabled =
                cbTeacher.SelectedItem != null &&
                cbClass.SelectedIndex != -1;
        }
        private void cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            ResetAssignMode();
            UpdateAssignButton();
            if (cbClass.SelectedIndex == -1)
            {
                cbHomeroomTeacher.Enabled = false;
                btnAssignHomeroom.Enabled = false;
                return;
            }
            int classId = Convert.ToInt32(cbClass.SelectedValue);
            if (cbTeacher.SelectedItem != null)
            {
                int teacherId = (int)((ComboboxItem)cbTeacher.SelectedItem).Value;
                
                connect.Open();
                SqlCommand cmd = new SqlCommand(
                    @"SELECT COUNT(*) 
                      FROM teaching_assignments
                      WHERE teacher_id = @tid
                        AND class_id = @cid", connect);

                cmd.Parameters.AddWithValue("@tid", teacherId);
                cmd.Parameters.AddWithValue("@cid", classId);

                int count = (int)cmd.ExecuteScalar();
                connect.Close();

                btnAssignHomeroom.Enabled = count > 0;
            }
            
            LoadHomeroomTeachers(classId);
            LoadCurrentHomeroomTeacher(classId);

            cbHomeroomTeacher.Enabled = true;
            btnAssignHomeroom.Enabled = true;
        }
        void LoadCurrentHomeroomTeacher(int classId)
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT homeroom_teacher_id 
          FROM classes 
          WHERE class_id = @cid",
                connect);

            cmd.Parameters.AddWithValue("@cid", classId);

            object result = cmd.ExecuteScalar();
            connect.Close();

            if (result == DBNull.Value || result == null) return;

            int teacherId = Convert.ToInt32(result);

            foreach (ComboboxItem item in cbHomeroomTeacher.Items)
            {
                if ((int)item.Value == teacherId)
                {
                    cbHomeroomTeacher.SelectedItem = item;
                    break;
                }
            }
        }
        void LoadHomeroomTeachers(int classId)
        {
            cbHomeroomTeacher.Items.Clear();

            connect.Open();
            SqlCommand cmd = new SqlCommand(
                @"SELECT DISTINCT t.id, t.teacher_name
          FROM teaching_assignments a
          JOIN teachers t ON a.teacher_id = t.id
          WHERE a.class_id = @cid",
                connect);

            cmd.Parameters.AddWithValue("@cid", classId);

            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbHomeroomTeacher.Items.Add(
                    new ComboboxItem(
                        dr["teacher_name"].ToString(),
                        dr["id"]
                    ));
            }

            dr.Close();
            connect.Close();
        }
        private void btnAssignHomeroom_Click(object sender, EventArgs e)
        {
            if (cbClass.SelectedIndex == -1 || cbHomeroomTeacher.SelectedItem == null)
            {
                MessageBox.Show("Please select Class and Homeroom Teacher");
                return;
            }

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            int teacherId = (int)((ComboboxItem)cbHomeroomTeacher.SelectedItem).Value;

            connect.Open();
            SqlCommand checkTeach = new SqlCommand(
            @"SELECT COUNT(*)
              FROM teaching_assignments
              WHERE teacher_id = @tid
                AND class_id = @cid", connect);

            checkTeach.Parameters.AddWithValue("@tid", teacherId);
            checkTeach.Parameters.AddWithValue("@cid", classId);

            int teachCount = (int)checkTeach.ExecuteScalar();

            if (teachCount == 0)
            {
                connect.Close();
                MessageBox.Show(
                    "This teacher is not teaching this class.\nHomeroom teacher must teach at least 1 subject in this class.",
                    "Invalid Assignment",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SqlCommand removeOld = new SqlCommand(
                @"UPDATE classes
                  SET homeroom_teacher_id = NULL
                  WHERE homeroom_teacher_id = @tid", connect);

            removeOld.Parameters.AddWithValue("@tid", teacherId);
            removeOld.ExecuteNonQuery();

            SqlCommand assignCmd = new SqlCommand(
                @"UPDATE classes
                  SET homeroom_teacher_id = @tid
                  WHERE class_id = @cid", connect);

            assignCmd.Parameters.AddWithValue("@tid", teacherId);
            assignCmd.Parameters.AddWithValue("@cid", classId);

            assignCmd.ExecuteNonQuery();

            connect.Close();

            LoadAssignments();
            MessageBox.Show("Homeroom teacher updated successfully!");
        }
        void ResetAssignMode()
        {
            selectedAssignmentId = 0;
            btnAssign.Visible = true;
            btnDeleteAssign.Visible = false;
            dgvAssign.ClearSelection();
        }
        private void ManagerAssignForm_Click(object sender, EventArgs e)
        {
            ResetAssignMode();
        }

    }
}
