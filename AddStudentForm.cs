using System;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;

namespace SchoolMangementSystem
{
    public partial class AddStudentForm : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");
        string imagePath = "";
        string oldGrade = "";
        string oldSection = "";

        public AddStudentForm()
        {
            InitializeComponent();
            LoadGradeAndSection();
            displayStudentData();
        }

        public void displayStudentData()
        {
            AddStudentData adData = new AddStudentData();

            student_studentData.DataSource = adData.studentData();

            if (student_studentData.Columns.Contains("StudentSection"))
                student_studentData.Columns["StudentSection"].Visible = false;

            if (student_studentData.Columns.Contains("StudentGrade"))
                student_studentData.Columns["StudentGrade"].Visible = false;

        }
        bool IsValidInput()
        {
            return !(string.IsNullOrWhiteSpace(student_id.Text)
                || string.IsNullOrWhiteSpace(student_name.Text)
                || string.IsNullOrWhiteSpace(student_address.Text)
                || string.IsNullOrEmpty(imagePath)
                || student_gender.SelectedIndex == -1
                || student_grade.SelectedIndex == -1
                || student_section.SelectedIndex == -1
                || student_status.SelectedIndex == -1);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image files (*.jpg; *.png)|*.jpg;*.png";

            if (open.ShowDialog() == DialogResult.OK)
            {
                imagePath = open.FileName;
                student_image.Image = Image.FromFile(imagePath);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!IsValidInput())
            {
                MessageBox.Show("Please fill all blank fields", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    try
                    {
                        connect.Open();

                        // WE DON'T LIKE THE DUPLICATE STUDENT ID SO, WE NEED TO CHECK IF ON THE DATABASE HAS ALREADY Student ID VALUE THAT SAME TO THE USER THAT WANT TO INSERT 
                        string checkStudentID = "SELECT COUNT(*) FROM students WHERE student_id = @studentID";

                        using (SqlCommand checkSID = new SqlCommand(checkStudentID, connect))
                        {
                            checkSID.Parameters.AddWithValue("@studentID", student_id.Text.Trim());
                            int count = (int)checkSID.ExecuteScalar();

                            if (count >= 1)
                            {
                                MessageBox.Show("Student ID: " + student_id.Text.Trim() + " is already exist"
                                    , "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);

                            }
                            else
                            {
                                int classId = 0;

                                SqlCommand getClassCmd = new SqlCommand(
                                    "SELECT class_id FROM classes WHERE class_name = @name",
                                    connect);

                                getClassCmd.Parameters.AddWithValue(
                                    "@name",
                                    student_grade.Text.Trim() + student_section.Text.Trim()
                                );

                                object result = getClassCmd.ExecuteScalar();
                                if (result != null)
                                    classId = Convert.ToInt32(result);

                                string insertData = "INSERT INTO students (student_id, student_name" +
                                    ", student_gender, student_address, student_grade, student_section, class_id" +
                                    ", student_image, student_status, date_insert) " +
                                    "VALUES(@studentID, @studentName, @studentGender, @studentAddress" +
                                    ", @studentGrade, @studentSection, @classId, @studentImage, @studentStatus" +
                                    ", @dateInsert)";

                                // Save image
                                string imgFolder = Path.Combine(Application.StartupPath, "Images", "Students");
                                Directory.CreateDirectory(imgFolder);

                                string imgName = student_id.Text + "_" + DateTime.Now.Ticks + ".jpg";

                                string imgFullPath = Path.Combine(imgFolder, imgName);
                                File.Copy(imagePath, imgFullPath, true);

                                string imgPathDB = @"Images\Students\" + imgName;

                                using (SqlCommand cmd = new SqlCommand(insertData, connect))
                                {
                                    cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());
                                    cmd.Parameters.AddWithValue("@studentName", student_name.Text.Trim());
                                    cmd.Parameters.AddWithValue("@studentGender", student_gender.Text.Trim());
                                    cmd.Parameters.AddWithValue("@studentAddress", student_address.Text.Trim());
                                    cmd.Parameters.AddWithValue("@studentGrade", student_grade.Text.Trim());
                                    cmd.Parameters.AddWithValue("@studentSection", student_section.Text.Trim());
                                    cmd.Parameters.AddWithValue("@classId", classId);
                                    cmd.Parameters.AddWithValue("@studentImage", imgPathDB);
                                    cmd.Parameters.AddWithValue("@dateInsert", DateTime.Now);
                                    cmd.Parameters.AddWithValue("@studentStatus", student_status.Text.Trim());

                                    cmd.ExecuteNonQuery();
                                    ((MainForm)this.FindForm()).Dashboard.LoadDashboard();

                                    displayStudentData();

                                    MessageBox.Show("Added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    clearFields();
                                }
                            }
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

        public void clearFields()
        {
            student_id.Text = "";
            student_name.Text = "";
            student_gender.SelectedIndex = -1;
            student_address.Text = "";
            student_grade.SelectedIndex = -1;
            student_section.SelectedIndex = -1;
            student_status.SelectedIndex = -1;
            student_image.Image = null;
            imagePath = "";
        }

        private void student_updateBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(student_id.Text))
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

                        DialogResult check = MessageBox.Show("Are you sure you want to Update Student ID: "
                            + student_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                        if (check == DialogResult.Yes)
                        {
                            int classId = 0;
                            SqlCommand checkScore = new SqlCommand(
                                "SELECT COUNT(*) FROM scores WHERE student_id = @sid",
                                connect);
                            checkScore.Parameters.AddWithValue("@sid", student_id.Text.Trim());

                            int scoreCount = (int)checkScore.ExecuteScalar();
                            
                            if (scoreCount > 0 &&
                                (student_grade.Text != oldGrade || student_section.Text != oldSection))
                            {
                                MessageBox.Show(
                                    "This student already has scores. Class cannot be changed.",
                                    "Warning",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Warning
                                );
                                return;
                            }

                            SqlCommand getClassCmd = new SqlCommand(
                                "SELECT class_id FROM classes WHERE class_name = @name",
                                connect
                            );

                            getClassCmd.Parameters.AddWithValue(
                                "@name",
                                student_grade.Text.Trim() + student_section.Text.Trim()
                            );

                            object result = getClassCmd.ExecuteScalar();
                            if (result != null)
                                classId = Convert.ToInt32(result);
                            string finalImagePath = imagePath;

                            if (!string.IsNullOrEmpty(imagePath) && !imagePath.StartsWith("Images\\"))
                            {
                                string imgFolder = Path.Combine(Application.StartupPath, "Images", "Students");
                                Directory.CreateDirectory(imgFolder);

                                string imgName = student_id.Text.Trim() + "_" + DateTime.Now.Ticks + ".jpg";
                                string imgFullPath = Path.Combine(imgFolder, imgName);

                                File.Copy(imagePath, imgFullPath, true);

                                finalImagePath = @"Images\Students\" + imgName;
                            }

                            String updateData = "UPDATE students SET student_name = @studentName, " +
                                "student_gender = @studentGender, student_address = @studentAddress, " +
                                "student_grade = @studentGrade, student_section = @studentSection, " +
                                "student_status = @studentStatus, class_id = @classId, student_image = @studentImage " +
                                "WHERE student_id = @studentID";


                            using (SqlCommand cmd = new SqlCommand(updateData, connect))
                            {
                                cmd.Parameters.AddWithValue("@studentName", student_name.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentGender", student_gender.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentAddress", student_address.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentGrade", student_grade.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentSection", student_section.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentStatus", student_status.Text.Trim());
                                cmd.Parameters.AddWithValue("@classId", classId);
                                cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());
                                cmd.Parameters.AddWithValue("@studentImage", finalImagePath);

                                cmd.ExecuteNonQuery();
                                ((MainForm)this.FindForm()).Dashboard.LoadDashboard();


                                displayStudentData();

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
        private void student_deleteBtn_Click(object sender, EventArgs e)
        {
            if (student_id.Text == "")
            {
                MessageBox.Show("Please select item first", "Error Message"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State != ConnectionState.Open)
                {
                    DialogResult check = MessageBox.Show("Are you sure you want to Delete Student ID: "
                        + student_id.Text + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (check == DialogResult.Yes)
                    {

                        try
                        {
                            connect.Open();
                            DateTime today = DateTime.Now;

                            string deleteData = "UPDATE students SET date_delete = @dateDelete " +
                                "WHERE student_id = @studentID";

                            using (SqlCommand cmd = new SqlCommand(deleteData, connect))
                            {
                                cmd.Parameters.AddWithValue("@dateDelete", today);
                                cmd.Parameters.AddWithValue("@studentID", student_id.Text.Trim());

                                cmd.ExecuteNonQuery();
                                ((MainForm)this.FindForm()).Dashboard.LoadDashboard();


                                displayStudentData();

                                MessageBox.Show("Deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                clearFields();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error connecting Database: " + ex, "Error Message"
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

        private void student_studentData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = student_studentData.Rows[e.RowIndex];
                student_id.Text = row.Cells["StudentID"].Value?.ToString();
                student_name.Text = row.Cells["StudentName"].Value?.ToString();
                student_gender.Text = row.Cells["StudentGender"].Value?.ToString();
                student_address.Text = row.Cells["StudentAddress"].Value?.ToString();
                string className = row.Cells["ClassName"].Value?.ToString(); // ví dụ: 6A

                if (!string.IsNullOrWhiteSpace(className) && className.Length >= 2)
                {
                    string grade = className.Substring(0, className.Length - 1); // 6
                    string section = className.Substring(className.Length - 1);  // A

                    student_grade.SelectedItem = grade;
                    student_section.SelectedItem = section;
                }
                else
                {
                    student_grade.SelectedIndex = -1;
                    student_section.SelectedIndex = -1;
                }
                oldGrade = student_grade.Text;
                oldSection = student_section.Text;
                student_status.Text = row.Cells["Status"].Value?.ToString();
                imagePath = row.Cells["StudentImage"].Value?.ToString();

                student_image.Image = null;

                if (!string.IsNullOrWhiteSpace(imagePath))
                {
                    string fullPath = Path.Combine(Application.StartupPath, imagePath);

                    if (File.Exists(fullPath))
                    {
                        using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
                        {
                            student_image.Image = Image.FromStream(fs);
                        }
                    }
                }

            }
        }
        private void student_clearBtn_Click(object sender, EventArgs e)
        {
            clearFields();
        }
        void LoadGradeAndSection()
        {
            student_grade.Items.Clear();
            student_grade.Items.AddRange(new object[] { "6", "7", "8", "9" });

            student_section.Items.Clear();
            student_section.Items.AddRange(new object[] { "A", "B", "C" });

            student_grade.DropDownStyle = ComboBoxStyle.DropDownList;
            student_section.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        bool HasScore(string studentId)
        {
            connect.Open();
            SqlCommand cmd = new SqlCommand(
                "SELECT COUNT(*) FROM scores sc JOIN students s ON sc.student_id = s.id WHERE s.student_id = @sid",
                connect);

            cmd.Parameters.AddWithValue("@sid", studentId);
            int count = (int)cmd.ExecuteScalar();
            connect.Close();

            return count > 0;
        }
        
    }
}
