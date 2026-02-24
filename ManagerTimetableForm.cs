using System; 
using System.Collections.Generic; 
using System.ComponentModel; 
using System.Data; 
using System.Data.SqlClient; 
using System.Drawing; 
using System.Drawing.Drawing2D; 
using System.Linq; 
using System.Text; 
using System.Threading.Tasks; 
using System.Windows.Forms; 
namespace SchoolMangementSystem 
{ 
    public partial class ManagerTimetableForm : UserControl 
    {
        public ManagerTimetableForm()
        {
            InitializeComponent();

            txtYear.Enter += TxtYear_Enter;
            txtYear.Leave += TxtYear_Leave;
            RoundButton(btnDelete, 20);

            dgvTimetable.CellClick += DgvTimetable_CellClick;

            ApplyPermission();
            CreateSchoolTimetableGrid();

            if (Session.Role.ToLower() == "manager")
            {
                LoadClasses();
                if (cbClass.Items.Count > 0)
                    cbClass.SelectedIndex = 0;
            }
            else
            {
                LoadTeacherTimetable();
            }
        }
        private void TxtYear_Enter(object sender, EventArgs e) 
        { 
            if (txtYear.Text == "2026-2027") 
            { 
                txtYear.Text = ""; 
                txtYear.ForeColor = Color.Black; 
            } 
        } 
        private void TxtYear_Leave(object sender, EventArgs e) 
        { 
            if (string.IsNullOrWhiteSpace(txtYear.Text)) 
            { txtYear.Text = "2026-2027"; 
                txtYear.ForeColor = Color.Gray; 
            } 
        } 
        private void RoundButton(Button btn, int radius) 
        { 
            GraphicsPath path = new GraphicsPath(); 
            path.AddArc(0, 0, radius, radius, 180, 90); 
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90); 
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90); 
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90); 
            path.CloseFigure(); 
            btn.Region = new Region(path); 
        } 
        protected override void OnPaint(PaintEventArgs e) 
        { 
            base.OnPaint(e); 
            using (LinearGradientBrush brush = new LinearGradientBrush( 
                this.ClientRectangle, Color.FromArgb(0, 90, 170), Color.FromArgb(0, 150, 255), 90F)) 
            { 
                e.Graphics.FillRectangle(brush, 0, 0, this.Width, 70); 
            } 
        } 
        void ApplyPermission() 
        { 
            string role = Session.Role?.ToLower(); 
            if (role == "manager") 
            { 

                btnDelete.Visible = true; 
                dgvTimetable.ReadOnly = false;
                tkb.Visible = false;
            }
            else if (role == "teacher")
            {
                btnDelete.Visible = false;
                dgvTimetable.ReadOnly = true;

                cbClass.Visible = false;
                lblClass.Visible = false;

                txtYear.Visible = false;        
                lblYear.Visible = false;
                tkb.Visible = true;
            }
            else 
            { 
                this.Visible = false; 
            } 
        } 
        private void CreateSchoolTimetableGrid() 
        { 
            dgvTimetable.Columns.Clear(); 
            dgvTimetable.Rows.Clear(); 
            dgvTimetable.Columns.Add("colPeriod", "Period");
            dgvTimetable.Columns["colPeriod"].SortMode = DataGridViewColumnSortMode.NotSortable;
            string[] days = { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6" }; 
            foreach (var day in days) 
            { 
                dgvTimetable.Columns.Add("col" + day, day); 
            } 
            for (int i = 1; i <= 10; i++) 
            { 
                int rowIndex = dgvTimetable.Rows.Add(); 
                dgvTimetable.Rows[rowIndex].Cells[0].Value = "Tiết " + i; 
                if (i == 6) 
                { 
                    dgvTimetable.Rows[rowIndex - 1].DefaultCellStyle.BackColor = Color.LightGray; 
                } 
            } 
            dgvTimetable.RowHeadersVisible = false; 
            dgvTimetable.ColumnHeadersHeight = 45; 
            dgvTimetable.RowTemplate.Height = 45; 
            dgvTimetable.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dgvTimetable.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; 
            dgvTimetable.Rows[4].DividerHeight = 5; dgvTimetable.Rows[4].DefaultCellStyle.BackColor = Color.WhiteSmoke; 
        } 
        private void LoadTimetableData(int classId) 
        { 
            using (SqlConnection conn = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True")) 
            { 
                conn.Open(); 
                
                string schoolYear = string.IsNullOrWhiteSpace(txtYear.Text)
                    ? "2026-2027"
                    : txtYear.Text.Trim();

                string sql = @"
                    SELECT t.day_of_week,
                           t.period,
                           s.subject_name
                    FROM Timetable t
                    INNER JOIN Subjects s ON t.subject_id = s.subject_id
                    WHERE t.class_id = @cid
                    AND t.school_year = @year";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cid", classId);
                cmd.Parameters.AddWithValue("@year", schoolYear);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int dayNumber = Convert.ToInt32(dr["day_of_week"]);
                    string day = "Thứ " + dayNumber;
                    int period = Convert.ToInt32(dr["period"]);

                    string cellValue;

                    if (Session.Role.ToLower() == "manager")
                        cellValue = dr["subject_name"].ToString();
                    else
                        cellValue = dr["class_name"].ToString();

                    int columnIndex = dgvTimetable.Columns
                        .Cast<DataGridViewColumn>()
                        .FirstOrDefault(c => c.HeaderText == day)?.Index ?? -1;

                    if (columnIndex > 0 && period >= 1 && period <= 10)
                    {
                        dgvTimetable.Rows[period - 1]
                            .Cells[columnIndex].Value = cellValue;
                    }
                }
                dr.Close(); 
            } 
        } 
        private void DgvTimetable_CellClick(object sender, DataGridViewCellEventArgs e) 
        { 
            if (e.RowIndex < 0 || e.ColumnIndex <= 0) return; 
            if (Session.Role?.ToLower() != "manager") return; 
            string day = dgvTimetable.Columns[e.ColumnIndex].HeaderText; 
            int period = e.RowIndex + 1; 
            string subject = dgvTimetable.Rows[e.RowIndex] .Cells[e.ColumnIndex].Value?.ToString();
            EditTimetableForm frm = new EditTimetableForm(day, period, subject); 
            if (frm.ShowDialog() == DialogResult.OK) 
            { 
                UpdateTimetable(day, period, frm.SelectedSubject); 
                if (cbClass.SelectedValue != null) 
                { 
                    int classId = Convert.ToInt32(cbClass.SelectedValue); 
                    CreateSchoolTimetableGrid(); 
                    LoadTimetableData(classId); 
                } 
            } 
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvTimetable.CurrentCell == null)
                return;

            int row = dgvTimetable.CurrentCell.RowIndex;
            int col = dgvTimetable.CurrentCell.ColumnIndex;

            if (col <= 0) return;

            string day = dgvTimetable.Columns[col].HeaderText;
            int period = row + 1;

            int classId = Convert.ToInt32(cbClass.SelectedValue);
            int dayNumber = ConvertDayToInt(day);

            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    DELETE FROM Timetable
                    WHERE class_id=@cid
                    AND day_of_week=@day
                    AND period=@period", conn);

                cmd.Parameters.AddWithValue("@cid", classId);
                cmd.Parameters.AddWithValue("@day", dayNumber);
                cmd.Parameters.AddWithValue("@period", period);

                cmd.ExecuteNonQuery();
            }

            dgvTimetable.Rows[row].Cells[col].Value = null;
        }
        private void LoadClasses()
        {
            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                string sql;

                if (Session.Role.ToLower() == "manager")
                {
                    sql = "SELECT class_id, class_name FROM classes";
                }
                else // teacher
                {
                    sql = @"
                SELECT DISTINCT c.class_id, c.class_name
                FROM classes c
                INNER JOIN teaching_assignments ta 
                    ON c.class_id = ta.class_id
                INNER JOIN teachers t
                    ON ta.teacher_id = t.id
                WHERE t.user_id = @uid";
                }

                SqlCommand cmd = new SqlCommand(sql, conn);

                if (Session.Role.ToLower() == "teacher")
                {
                    cmd.Parameters.AddWithValue("@uid", Session.UserId);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cbClass.DataSource = dt;
                cbClass.DisplayMember = "class_name";
                cbClass.ValueMember = "class_id";

                if (dt.Rows.Count > 0)
                    cbClass.SelectedIndex = 0;
            }
        }
        private void UpdateTimetable(string day, int period, string subjectName)
        {
            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();
                string schoolYear = string.IsNullOrWhiteSpace(txtYear.Text)
                    ? "2026-2027"
                    : txtYear.Text.Trim();
                // Lấy subject_id
                SqlCommand getSubject = new SqlCommand(
                    "SELECT subject_id FROM Subjects WHERE subject_name = @name",
                    conn);

                getSubject.Parameters.Add("@name", SqlDbType.NVarChar).Value = subjectName;

                int subjectId = Convert.ToInt32(getSubject.ExecuteScalar());
                int classId = Convert.ToInt32(cbClass.SelectedValue);
                int dayNumber = ConvertDayToInt(day);

                // Lấy teacher_id từ bảng phân công
                SqlCommand getTeacher = new SqlCommand(@"
                    SELECT teacher_id
                    FROM teaching_assignments
                    WHERE class_id = @cid AND subject_id = @sid", conn);

                getTeacher.Parameters.AddWithValue("@cid", classId);
                getTeacher.Parameters.AddWithValue("@sid", subjectId);

                object teacherResult = getTeacher.ExecuteScalar();

                if (teacherResult == null)
                {
                    MessageBox.Show("This subject has not been assigned to any teacher for this class.");
                    return;
                }

                int teacherId = Convert.ToInt32(teacherResult);
                

                // Kiểm tra đã tồn tại chưa
                SqlCommand check = new SqlCommand(@"
                    SELECT COUNT(*) FROM Timetable
                    WHERE class_id=@cid AND day_of_week=@day AND period=@period", conn);

                check.Parameters.AddWithValue("@cid", classId);
                check.Parameters.AddWithValue("@day", dayNumber);
                check.Parameters.AddWithValue("@period", period);

                int count = (int)check.ExecuteScalar();

                if (count > 0)
                {
                    // UPDATE
                    SqlCommand update = new SqlCommand(@"
                        UPDATE Timetable
                        SET subject_id=@sid,
                            teacher_id=@tid,
                            school_year=@year
                        WHERE class_id=@cid
                        AND day_of_week=@day
                        AND period=@period", conn);

                    update.Parameters.AddWithValue("@sid", subjectId);
                    update.Parameters.AddWithValue("@tid", teacherId);
                    update.Parameters.AddWithValue("@year", schoolYear);
                    update.Parameters.AddWithValue("@cid", classId);
                    update.Parameters.AddWithValue("@day", dayNumber);
                    update.Parameters.AddWithValue("@period", period);

                    try
                    {
                        update.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            MessageBox.Show(
                                "This teacher already has another class at this period!",
                                "Schedule Conflict",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show("Database error: " + ex.Message);
                        }
                    }
                }
                else
                {
                    // INSERT
                    SqlCommand insert = new SqlCommand(@"
                            INSERT INTO Timetable
                            (class_id, day_of_week, period, subject_id, teacher_id, school_year)
                            VALUES
                            (@cid, @day, @period, @sid, @tid, @year)", conn);

                    insert.Parameters.AddWithValue("@cid", classId);
                    insert.Parameters.AddWithValue("@day", dayNumber);
                    insert.Parameters.AddWithValue("@period", period);
                    insert.Parameters.AddWithValue("@sid", subjectId);
                    insert.Parameters.AddWithValue("@tid", teacherId);
                    insert.Parameters.AddWithValue("@year", schoolYear);
                    try
                    {
                        insert.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 2601 || ex.Number == 2627)
                        {
                            MessageBox.Show(
                                "This teacher is already teaching another class at this time!",
                                "Schedule Conflict",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show(
                                "Database error: " + ex.Message,
                                "Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
        private int ConvertDayToInt(string day) { switch (day) 
            { 
                case "Thứ 2": return 2; 
                case "Thứ 3": return 3; 
                case "Thứ 4": return 4; 
                case "Thứ 5": return 5;
                case "Thứ 6": return 6; 
                default: return 0; 
            } 
        }
        private void cbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbClass.SelectedValue == null|| cbClass.SelectedValue is DataRowView)
                return;

            int classId = Convert.ToInt32(cbClass.SelectedValue);

            CreateSchoolTimetableGrid();
            LoadTimetableData(classId);
        }
        private void LoadTeacherTimetable()
        {
            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                string schoolYear = string.IsNullOrWhiteSpace(txtYear.Text)
                    ? "2026-2027"
                    : txtYear.Text.Trim();

                string sql = @"
                    SELECT t.day_of_week,
                           t.period,
                           c.class_name
                    FROM Timetable t
                    INNER JOIN classes c ON t.class_id = c.class_id
                    WHERE t.teacher_id = @tid
                    AND t.school_year = @year";

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@tid", Session.TeacherDbId);
                cmd.Parameters.AddWithValue("@year", schoolYear);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    int dayNumber = Convert.ToInt32(dr["day_of_week"]);
                    string day = "Thứ " + dayNumber;
                    int period = Convert.ToInt32(dr["period"]);
                    string className = dr["class_name"].ToString();

                    int columnIndex = dgvTimetable.Columns
                        .Cast<DataGridViewColumn>()
                        .FirstOrDefault(c => c.HeaderText == day)?.Index ?? -1;

                    if (columnIndex > 0 && period >= 1 && period <= 10)
                    {
                        dgvTimetable.Rows[period - 1]
                            .Cells[columnIndex].Value = className;
                    }
                }

                dr.Close();
            }
        }
    } 
}