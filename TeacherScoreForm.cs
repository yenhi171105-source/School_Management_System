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
using static SchoolMangementSystem.ManagerAssignForm;

namespace SchoolMangementSystem
{
    public partial class TeacherScoreForm : UserControl
    {
        SqlConnection connect = new SqlConnection(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");

        int teacherId;
        bool isManager = false;

        public TeacherScoreForm(bool managerMode = false)
        {
            InitializeComponent();
            teacherId = GetTeacherIdByUser();
            isManager = managerMode;
            
            LoadSemesters();
            LoadClasses();

            cbClass.SelectedIndexChanged += cbClass_SelectedIndexChanged;
            cbSemester.SelectedIndexChanged += cbSemester_SelectedIndexChanged;
            btnSave.Click += btnSave_Click;
            btnClear.Click += btnClear_Click;

            ApplyPermission();
        }

        void LoadSemesters()
        {
            cbSemester.Items.Clear();
            cbSemester.Items.Add("1");
            cbSemester.Items.Add("2");
            cbSemester.SelectedIndex = -1;
        }

        void LoadClasses()
        {
            string sql;

            if (isManager)
            {
                sql = "SELECT class_id, class_name FROM classes ORDER BY class_name";
            }
            else
            {
                sql = @"SELECT DISTINCT c.class_id, c.class_name
                FROM teaching_assignments ta
                JOIN classes c ON ta.class_id = c.class_id
                WHERE ta.teacher_id = @tid";
            }

            using (SqlDataAdapter da = new SqlDataAdapter(sql, connect))
            {
                if (!isManager)
                    da.SelectCommand.Parameters.AddWithValue("@tid", teacherId);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbClass.DataSource = dt;
                cbClass.DisplayMember = "class_name";
                cbClass.ValueMember = "class_id";
                cbClass.SelectedIndex = -1;
            }
        }

        void LoadSubject(int classId)
        {
            cbSubject.Items.Clear();
            bool isHomeroom = IsHomeroomTeacher(classId);
            if (isManager || isHomeroom)
            {
                LoadAllSubjects();
                cbSubject.Enabled = true;
            }
            else
            {
                LoadOnlyTeacherSubject(classId);
                cbSubject.Enabled = false;
            }
        }

        void LoadStudents()
        {
            if (cbClass.SelectedIndex == -1 || cbSemester.SelectedIndex == -1)
                return;

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            int semester = Convert.ToInt32(cbSemester.Text);
            bool isHomeroom = IsHomeroomTeacher(classId);
            string sql;

            if (isManager || isHomeroom)
            {
                if (cbSubject.SelectedItem == null)
                {
                    sql = @"
                        SELECT 
                            s.student_id,
                            s.student_name,
                            sub.subject_name,
                            sc.score
                        FROM students s
                        CROSS JOIN subjects sub
                        LEFT JOIN scores sc
                            ON s.student_id = sc.student_id
                           AND sc.class_id = @cid
                           AND sc.subject_id = sub.subject_id
                           AND sc.semester = @sem
                        WHERE s.class_id = @cid
                          AND s.date_delete IS NULL
                        ORDER BY s.student_name, sub.subject_name";
                }
                else
                {
                    sql = @"
                        SELECT 
                            s.student_id,
                            s.student_name,
                            sub.subject_name,
                            sc.score
                        FROM students s
                        CROSS JOIN subjects sub
                        LEFT JOIN scores sc
                            ON s.student_id = sc.student_id
                           AND sc.class_id = @cid
                           AND sc.subject_id = @sid
                           AND sc.semester = @sem
                        WHERE s.class_id = @cid
                          AND s.date_delete IS NULL
                          AND sub.subject_id = @sid
                        ORDER BY s.student_name";
                }
            }
            else
            {
                if (cbSubject.SelectedItem == null)
                    return;

                int subjectId = (int)((ComboboxItem)cbSubject.SelectedItem).Value;

                sql = @"
                    SELECT 
                        s.student_id,
                        s.student_name,
                        sc.score
                    FROM students s
                    LEFT JOIN scores sc
                        ON s.student_id = sc.student_id
                       AND sc.class_id = @cid
                       AND sc.subject_id = @sid
                       AND sc.semester = @sem
                       AND sc.teacher_id = @tid
                    WHERE s.class_id = @cid
                      AND s.date_delete IS NULL
                    ORDER BY s.student_name";
            }

            using (SqlDataAdapter da = new SqlDataAdapter(sql, connect))
            {
                da.SelectCommand.Parameters.AddWithValue("@cid", classId);
                da.SelectCommand.Parameters.AddWithValue("@sem", semester);

                if (cbSubject.SelectedItem != null)
                {
                    da.SelectCommand.Parameters.AddWithValue("@sid",
                        ((ComboboxItem)cbSubject.SelectedItem).Value);
                }

                if (!isManager && !isHomeroom)
                {
                    da.SelectCommand.Parameters.AddWithValue("@tid", teacherId);
                }

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvScores.DataSource = dt;

            }
        }

        private void cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClass.SelectedIndex == -1) return;

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            LoadSubject(classId);

            if (cbSemester.SelectedIndex != -1)
                LoadStudents();
        }

        private void cbSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClass.SelectedIndex != -1)
                LoadStudents();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Manager không được sửa
            if (isManager)
            {
                MessageBox.Show("Manager cannot edit scores.");
                return;
            }

            // 2. Kiểm tra dữ liệu
            if (dgvScores.DataSource == null)
            {
                MessageBox.Show("No data to save.");
                return;
            }

            if (cbClass.SelectedIndex == -1 || cbSemester.SelectedIndex == -1)
            {
                MessageBox.Show("Please select class and semester.");
                return;
            }

            if (cbSubject.SelectedItem == null)
            {
                MessageBox.Show("Please select a subject.");
                return;
            }

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            int semester = Convert.ToInt32(cbSemester.Text);
            int subjectId = (int)((ComboboxItem)cbSubject.SelectedItem).Value;

            // 3. Kiểm tra giáo viên có dạy môn này không
            if (!IsTeacherAssignedToSubject(classId, subjectId))
            {
                MessageBox.Show("You can only edit the subject you teach.");
                return;
            }

            try
            {
                connect.Open();

                foreach (DataGridViewRow row in dgvScores.Rows)
                {
                    if (row.IsNewRow) continue;
                    if (row.Cells["score"].Value == null) continue;

                    decimal score;
                    if (!decimal.TryParse(row.Cells["score"].Value.ToString(), out score))
                        continue;

                    if (score < 0 || score > 10)
                    {
                        MessageBox.Show("Score must be between 0 and 10.");
                        connect.Close();
                        return;
                    }

                    string studentId = row.Cells["student_id"].Value.ToString();

                    SqlCommand cmd = new SqlCommand(@"
                IF EXISTS (
                    SELECT 1 FROM scores
                    WHERE student_id = @sid
                      AND class_id = @cid
                      AND subject_id = @sub
                      AND semester = @sem
                )
                UPDATE scores
                SET score = @score,
                    teacher_id = @tid,
                    date_insert = GETDATE()
                WHERE student_id = @sid
                  AND class_id = @cid
                  AND subject_id = @sub
                  AND semester = @sem
                ELSE
                INSERT INTO scores
                (student_id, class_id, subject_id, semester, attempt, score, teacher_id, date_insert)
                VALUES
                (@sid, @cid, @sub, @sem, 1, @score, @tid, GETDATE())
            ", connect);

                    cmd.Parameters.AddWithValue("@sid", studentId);
                    cmd.Parameters.AddWithValue("@cid", classId);
                    cmd.Parameters.AddWithValue("@sub", subjectId);
                    cmd.Parameters.AddWithValue("@sem", semester);
                    cmd.Parameters.AddWithValue("@score", score);
                    cmd.Parameters.AddWithValue("@tid", teacherId);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Scores saved successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                connect.Close();
            }

            LoadStudents();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvScores.DataSource = null;
            cbClass.SelectedIndex = -1;
            cbSemester.SelectedIndex = -1;
            cbSubject.Text = "";
        }
        int GetTeacherIdByUser()
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT id FROM teachers WHERE user_id = @uid", connect))
            {
                cmd.Parameters.AddWithValue("@uid", Session.UserId);
                connect.Open();
                object result = cmd.ExecuteScalar();
                connect.Close();

                return result == null ? 0 : Convert.ToInt32(result);
            }
        }
        int GetTeacherSubjectId(int teacherDbId)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT subject_id FROM teachers WHERE id = @tid", connect))
            {
                cmd.Parameters.AddWithValue("@tid", teacherDbId);
                connect.Open();
                object result = cmd.ExecuteScalar();
                connect.Close();

                return result == null ? 0 : Convert.ToInt32(result);
            }
        }
        void ApplyPermission()
        {
            if (isManager)
            {
                btnSave.Visible = false;
                dgvScores.ReadOnly = true;
                cbSubject.Enabled = true;
            }
            else
            {
                btnSave.Visible = true;
                dgvScores.ReadOnly = false;
            }
        }
        bool IsHomeroomTeacher(int classId)
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT COUNT(*) FROM classes 
          WHERE class_id = @cid 
            AND homeroom_teacher_id = @tid", connect))
            {
                cmd.Parameters.AddWithValue("@cid", classId);
                cmd.Parameters.AddWithValue("@tid", teacherId);

                connect.Open();
                int count = (int)cmd.ExecuteScalar();
                connect.Close();

                return count > 0;
            }
        }
        void LoadAllSubjects()
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT subject_id, subject_name FROM subjects", connect))
            {
                connect.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    cbSubject.Items.Add(new ComboboxItem(
                        dr["subject_name"].ToString(),
                        dr["subject_id"]
                    ));
                }
                dr.Close();
                connect.Close();
            }
        }
        void LoadOnlyTeacherSubject(int classId)
        {
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT s.subject_id, s.subject_name
                  FROM teaching_assignments ta
                  JOIN subjects s ON ta.subject_id = s.subject_id
                  WHERE ta.teacher_id = @tid
                    AND ta.class_id = @cid", connect))
            {
                cmd.Parameters.AddWithValue("@tid", teacherId);
                cmd.Parameters.AddWithValue("@cid", classId);

                connect.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    var item = new ComboboxItem(
                        dr["subject_name"].ToString(),
                        dr["subject_id"]
                    );

                    cbSubject.Items.Add(item);
                    cbSubject.SelectedItem = item;   
                }

                dr.Close();
                connect.Close();
            }
        }
        bool IsTeacherAssignedToSubject(int classId, int subjectId)
        {
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM teaching_assignments
        WHERE teacher_id = @tid
          AND class_id = @cid
          AND subject_id = @sid", connect))
            {
                cmd.Parameters.AddWithValue("@tid", teacherId);
                cmd.Parameters.AddWithValue("@cid", classId);
                cmd.Parameters.AddWithValue("@sid", subjectId);

                connect.Open();
                int count = (int)cmd.ExecuteScalar();
                connect.Close();

                return count > 0;
            }
        }
    }
}
