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
    public partial class AddUsersForm : UserControl
    {
        int selectedUserId = 0;
        string selectedRole = "";

        SqlConnection connect = new SqlConnection(
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");

        public AddUsersForm()
        {
            InitializeComponent();
            LoadRoles();
            LoadUsers();
        }

        // ================= LOAD ROLES =================
        void LoadRoles()
        {
            try
            {
                connect.Open();
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT role_id, role_name FROM roles WHERE role_name IN ('Manager', 'Teacher')", connect);

                DataTable dt = new DataTable();
                da.Fill(dt);
            }
            finally
            {
                connect.Close();
            }
        }

        // ================= LOAD USERS =================
        public void LoadUsers()
        {
            string query = @"
                SELECT 
                    u.id,
                    u.username,
                    r.role_name,
                    u.is_active,
                    u.date_create
                FROM users u
                JOIN roles r ON u.role_id = r.role_id";

            SqlDataAdapter da = new SqlDataAdapter(query, connect);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvUsers.DataSource = dt;
            dgvUsers.Columns["is_active"].ReadOnly = false;

            ColorDisabledUsers();
        }
        void ColorDisabledUsers()
        {
            foreach (DataGridViewRow row in dgvUsers.Rows)
            {
                if (row.Cells["is_active"].Value != null &&
                    Convert.ToInt32(row.Cells["is_active"].Value) == 0)
                {
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }


        // ================= CREATE ACCOUNT =================
        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (Session.Role != "Admin")
            {
                MessageBox.Show("Only admin can create accounts.");
                return;
            }

            if (txtUsername.Text == "" || txtPassword.Text == "")
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            try
            {
                connect.Open();
                int ROLE_MANAGER = 2;
                SqlCommand checkManagerCmd = new SqlCommand(
                    @"SELECT COUNT(*) 
                      FROM users 
                      WHERE role_id = @roleId AND is_active = 1",
                    connect);

                checkManagerCmd.Parameters.AddWithValue("@roleId", ROLE_MANAGER);

                int managerCount = (int)checkManagerCmd.ExecuteScalar();

                if (managerCount >= 3)
                {
                    MessageBox.Show("Only 3 Manager accounts are allowed!");
                    connect.Close();
                    return;
                }

                // Check duplicate username
                SqlCommand check = new SqlCommand(
                    "SELECT COUNT(*) FROM users WHERE username=@u", connect);
                check.Parameters.AddWithValue("@u", txtUsername.Text.Trim());

                if ((int)check.ExecuteScalar() > 0)
                {
                    MessageBox.Show("Username already exists!");
                    return;
                }

                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO users (username, password, role_id, is_active, date_create)
                    VALUES (@u, @p, 2, 1, GETDATE())", connect);

                cmd.Parameters.AddWithValue("@u", txtUsername.Text.Trim());
                cmd.Parameters.AddWithValue("@p", txtPassword.Text.Trim()); 

                cmd.ExecuteNonQuery();

                MessageBox.Show("Account created successfully!");

                LoadUsers();
                ClearFields();
            }
            finally
            {
                connect.Close();
            }
        }

        // ================= GRID CLICK =================
        private void dgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            selectedUserId = Convert.ToInt32(row.Cells["id"].Value);
            selectedRole = row.Cells["role_name"].Value.ToString();

            txtUsername.Text = row.Cells["username"].Value.ToString();

        }
        private void dgvUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Chỉ xử lý cột is_active
            if (dgvUsers.Columns[e.ColumnIndex].Name != "is_active") return;

            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            int userId = Convert.ToInt32(row.Cells["id"].Value);
            string username = row.Cells["username"].Value.ToString();
            int currentStatus = Convert.ToInt32(row.Cells["is_active"].Value);

            // Nếu đang bị disable và user muốn bật lại
            if (currentStatus == 0)
            {
                DialogResult result = MessageBox.Show(
                    $"Do you want to restore account '{username}'?",
                    "Confirm Restore",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    RestoreUser(userId);
                    LoadUsers();
                    ColorDisabledUsers();

                    MainForm main = (MainForm)this.FindForm();
                    main.ReloadTeachers();

                }
                else
                {
                    // Hoàn tác checkbox
                    row.Cells["is_active"].Value = 0;
                }
            }
        }
        void RestoreUser(int userId)
        {
            connect.Open();

            SqlCommand cmd = new SqlCommand(
                "UPDATE users SET is_active = 1 WHERE id = @id",
                connect
            );
            cmd.Parameters.AddWithValue("@id", userId);

            cmd.ExecuteNonQuery();
            connect.Close();
        }

        // ================= CLEAR =================
        void ClearFields()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
        }
        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (selectedRole == "Admin")
            {
                MessageBox.Show("Admin account cannot be deleted");
                return;
            }

            if (selectedUserId == 0)
            {
                MessageBox.Show("Please select a user first");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this account?",
                "Confirm",
                MessageBoxButtons.YesNo
            );

            if (result != DialogResult.Yes) return;

            connect.Open();

            SqlCommand cmd = new SqlCommand(
                @"UPDATE users 
                  SET is_active = 0 
                  WHERE id = @id",
                connect);

            cmd.Parameters.AddWithValue("@id", selectedUserId);
            cmd.ExecuteNonQuery();

            connect.Close();

            LoadUsers(); // reload grid
            MainForm main = (MainForm)this.FindForm();
            main.ReloadTeachers();
            MessageBox.Show("User disabled successfully");
        }
        

    }
}
