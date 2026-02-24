using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SchoolMangementSystem
{
    public partial class AddTeachersForm : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");
        string imagePath = "";
        public AddTeachersForm()
        {
            InitializeComponent();
            teacher_gridData.AutoGenerateColumns = false;
            LoadSubjects();
            teacherDisplayData();
            
        }

        public void teacherDisplayData()
        {
            AddTeachersData addTD = new AddTeachersData();
            teacher_gridData.DataSource = addTD.teacherData();

            foreach (DataGridViewRow row in teacher_gridData.Rows)
            {
                row.ReadOnly = false;
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;

                if (row.Cells["colIsActive"].Value != null &&
                    Convert.ToInt32(row.Cells["colIsActive"].Value) == 0)
                {
                    row.ReadOnly = true;
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        bool IsValidInput()
        {
            return !(string.IsNullOrWhiteSpace(teacher_id.Text)
                || string.IsNullOrWhiteSpace(teacher_name.Text)
                || cbSubject.SelectedIndex == -1
                || string.IsNullOrEmpty(imagePath)
                || teacher_gender.SelectedIndex == -1);
        } 
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image files (*.jpg; *.png)|*.jpg;*.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                imagePath = open.FileName;
                teacher_image.Image = Image.FromFile(imagePath);
            }
        }
        private void teacher_addBtn_Click(object sender, EventArgs e)
        {
            if (Session.Role != "Admin" && Session.Role != "Manager")
            {
                MessageBox.Show("You do not have permission to perform this action.");
                return;
            }
            if (!IsValidInput())
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if(connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        string checkUser = "SELECT COUNT(*) FROM users WHERE username = @u";// them
                        string checkTeacherID = "SELECT COUNT(*) FROM teachers WHERE teacher_id = @teacherID";

                        using(SqlCommand checkTID = new SqlCommand(checkTeacherID, connect))
                        {
                            checkTID.Parameters.AddWithValue("@teacherID", teacher_id.Text.Trim());
                            int count = (int)checkTID.ExecuteScalar();

                            if(count >= 1)
                            {
                                MessageBox.Show("Teacher ID: " + teacher_id.Text.Trim() + " is already exist"
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
                            else
                            {
                                int subjectId = Convert.ToInt32(cbSubject.SelectedValue);


                                DateTime today = DateTime.Today;

                                // Save image
                                string imgFolder = Path.Combine(Application.StartupPath, "Images", "Teachers");
                                Directory.CreateDirectory(imgFolder);

                                string imgName = teacher_id.Text + "_" + DateTime.Now.Ticks + ".jpg";
                                string imgFullPath = Path.Combine(imgFolder, imgName);
                                File.Copy(imagePath, imgFullPath, true);

                                string imgPathDB = @"Images\Teachers\" + imgName;
                                // 1. Tạo user cho teacher
                                string insertUserSql = @"
                                    INSERT INTO users (username, password, role_id, is_active, date_create)
                                    OUTPUT INSERTED.id
                                    VALUES (@username, @password, @roleId, 1, GETDATE())
                                ";

                                int newUserId;

                                using (SqlCommand userCmd = new SqlCommand(insertUserSql, connect))
                                {
                                    userCmd.Parameters.AddWithValue("@username", teacher_id.Text.Trim());
                                    userCmd.Parameters.AddWithValue("@password", "123456"); // password mặc định
                                    userCmd.Parameters.AddWithValue("@roleId", 3); // 3 = Teacher

                                    newUserId = (int)userCmd.ExecuteScalar(); // 
                                }

                                //them teacher
                                string insertTeacher = @"
                                    INSERT INTO teachers
                                    (teacher_id, teacher_name, teacher_gender, subject_id, user_id, teacher_image, date_insert)
                                    VALUES
                                    (@teacherID, @name, @gender, @subject, @userId, @image, GETDATE())";
                                int teacherDbId;
                                using (SqlCommand cmd = new SqlCommand(insertTeacher, connect))
                                {
                                    cmd.Parameters.AddWithValue("@teacherID", teacher_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@name", teacher_name.Text.Trim());
                                    cmd.Parameters.AddWithValue("@gender", teacher_gender.Text);
                                    cmd.Parameters.AddWithValue("@subject", cbSubject.SelectedValue);
                                    cmd.Parameters.AddWithValue("@image", imgPathDB);
                                    cmd.Parameters.AddWithValue("@userId", newUserId);

                                    cmd.ExecuteNonQuery();
                                    teacherDisplayData();

                                    MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    clearFields();
                                }
                                MainForm main = (MainForm)this.ParentForm;
                                main.ReloadUsers();
                                
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
        }
        private void teacher_updateBtn_Click(object sender, EventArgs e)
        {
            if (Session.Role != "Admin" && Session.Role != "Manager")
            {
                MessageBox.Show("Permission denied.");
                return;
            }

            if (string.IsNullOrWhiteSpace(teacher_id.Text))
            {
                MessageBox.Show("Please select item first", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        DialogResult check = MessageBox.Show("Are you sure you want to Update Teacher ID: "
                            + teacher_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (check == DialogResult.Yes)
                        {
                            DateTime today = DateTime.Today;
                            string finalImagePath = imagePath;

                            if (!string.IsNullOrEmpty(imagePath) && !imagePath.StartsWith("Images\\"))
                            {
                                string imgFolder = Path.Combine(Application.StartupPath, "Images", "Teachers");
                                Directory.CreateDirectory(imgFolder);

                                string imgName = teacher_id.Text.Trim() + "_" + DateTime.Now.Ticks + ".jpg";
                                string imgFullPath = Path.Combine(imgFolder, imgName);

                                File.Copy(imagePath, imgFullPath, true);

                                finalImagePath = @"Images\Teachers\" + imgName;
                            }

                            String updateData = @"
                                UPDATE teachers SET
                                    teacher_name = @teacherName,
                                    teacher_gender = @teacherGender,
                                    subject_id = @subject,
                                    teacher_image = @teacherImage
                                WHERE teacher_id = @teacherID";

                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@teacherName", teacher_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@teacherGender", teacher_gender.Text.Trim());
                                cmd.Parameters.AddWithValue("@subject", cbSubject.SelectedValue);
                                cmd.Parameters.AddWithValue("@teacherID", teacher_id.Text.Trim());
                                cmd.Parameters.AddWithValue("@teacherImage", finalImagePath);

                                cmd.ExecuteNonQuery();

                                teacherDisplayData();

                                MessageBox.Show("Updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cancelled.", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            clearFields();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error connecting Database: " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }
        private void teacher_deleteBtn_Click(object sender, EventArgs e)
        {
            if (Session.Role != "Admin" && Session.Role != "Manager")
            {
                MessageBox.Show("Permission denied.");
                return;
            }

            if (teacher_id.Text == "")
            {
                MessageBox.Show("Please select item first", "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to Delete Teacher ID: "
                        + teacher_id.Text + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {

                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Today;

                            string deleteData = @"
                                DECLARE @uid INT;

                                SELECT @uid = user_id
                                FROM teachers
                                WHERE teacher_id = @teacherID;

                                DELETE FROM teaching_assignments
                                WHERE teacher_id = (
                                    SELECT id FROM teachers WHERE teacher_id = @teacherID
                                );

                                DELETE FROM teachers
                                WHERE teacher_id = @teacherID;

                                DELETE FROM users
                                WHERE id = @uid;
                                ";

                            using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                            {
                                cmd.Parameters.AddWithValue("@dateDelete", today);
                                cmd.Parameters.AddWithValue("@teacherID", teacher_id.Text.Trim());

                                cmd.ExecuteNonQuery();

                                teacherDisplayData();

                                MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error  connecting Database: " + ex, "Error Message"
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            connect.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Cancelled.", "Information Message"
                        , MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }
        private void teacher_gridData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = teacher_gridData.Rows[e.RowIndex];

            if (Convert.ToInt32(row.Cells["colIsActive"].Value) == 0)
            {
                teacher_gridData.ClearSelection();
                MessageBox.Show(
                    "This teacher account is inactive and cannot be selected.",
                    "Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            teacher_id.Text = row.Cells["colTeacherID"].Value.ToString();
            teacher_name.Text = row.Cells["colTeacherName"].Value.ToString();
            teacher_gender.Text = row.Cells["colTeacherGender"].Value.ToString();
            cbSubject.Text = row.Cells["colSubject"].Value.ToString();
         
            teacher_image.Image = null;

            string imageData = row.Cells["colTeacherImage"].Value?.ToString();

            if (!string.IsNullOrEmpty(imageData))
            {
                string fullPath = Path.Combine(Application.StartupPath, imageData);

                if (File.Exists(fullPath))
                {
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                    {
                        teacher_image.Image = Image.FromStream(fs);
                    }
                }
            }
        }
        void LoadSubjects()
        {
            using (SqlConnection conn = new SqlConnection(connect.ConnectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT subject_id, subject_name FROM subjects", conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbSubject.DataSource = dt;
                cbSubject.DisplayMember = "subject_name";
                cbSubject.ValueMember = "subject_id";
                cbSubject.SelectedIndex = -1;
            }
        }

        public void clearFields()
        {
            teacher_id.Text = "";
            teacher_name.Text = "";
            teacher_gender.SelectedIndex = -1;
            cbSubject.SelectedIndex = -1;
            teacher_image.Image = null;
            imagePath = "";
        }

        private void teacher_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        public void ReloadTeachers()
        {
            teacherDisplayData();
        }
    }
}
