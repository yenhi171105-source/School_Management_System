using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SchoolMangementSystem
{
    public partial class LoginForm : Form
    {
        SqlConnection connect = new SqlConnection(
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");

        public LoginForm()
        {
            InitializeComponent();
           
        }

        private void label1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            if (username.Text == "" || password.Text == "")
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    connect.Open();

                    String selectData = @"
                        SELECT 
                            u.id AS user_id,
                            u.username,
                            r.role_name,
                            t.id AS teacher_db_id,
                            t.teacher_id
                        FROM users u
                        JOIN roles r ON u.role_id = r.role_id
                        LEFT JOIN teachers t ON u.id = t.user_id
                        WHERE u.username = @username
                          AND u.password = @password
                          AND u.is_active = 1
                        ";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        cmd.Parameters.AddWithValue("@username", username.Text.Trim());
                        cmd.Parameters.AddWithValue("@password", password.Text.Trim());
                        SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                        DataTable table = new DataTable();
                        adapter.Fill(table);

                        if (table.Rows.Count >= 1)
                        {
                            Session.Username = table.Rows[0]["username"].ToString();
                            Session.Role = table.Rows[0]["role_name"].ToString();
                            Session.UserId = Convert.ToInt32(table.Rows[0]["user_id"]);
                            if (Session.Role == "Teacher")
                            {
                                Session.TeacherDbId = table.Rows[0]["teacher_db_id"] == DBNull.Value
                                    ? 0
                                    : Convert.ToInt32(table.Rows[0]["teacher_db_id"]);

                                Session.TeacherID = table.Rows[0]["teacher_id"]?.ToString();
                            }

                            MessageBox.Show("Login Successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            

                            MainForm mForm = new MainForm();
                            mForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Username/Password", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }

                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
                finally
                {
                    connect.Close();
                }
                

            }
        }

        private void showPass_CheckedChanged(object sender, EventArgs e)
        {
            password.PasswordChar = showPass.Checked ? '\0' : '*';
        }
    }
}
