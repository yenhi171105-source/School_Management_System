using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SchoolMangementSystem
{
    class AddTeachersData
    {
        SqlConnection connect = new SqlConnection(
    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True");


        public int ID { set; get; }
        public string TeacherID { set; get; }
        public string TeacherName { set; get; }
        public string TeacherGender { set; get; }
        public string Subject { set; get; }
        public string TeacherImage { set; get; }
        public string DateInsert { set; get; }
        public int IsActive { get; set; }

        public List<AddTeachersData> teacherData()
        {
            List<AddTeachersData> listData = new List<AddTeachersData>();

            try
            {
                connect.Open();

                string sql = @"
                    SELECT
                        t.id AS ID,
                        t.teacher_id AS TeacherID,
                        t.teacher_name AS TeacherName,
                        t.teacher_gender AS TeacherGender,
                        s.subject_name AS Subject,
                        t.teacher_image AS TeacherImage,
                        t.date_insert AS DateInsert,
                        u.is_active AS IsActive
                    FROM teachers t
                    JOIN subjects s ON t.subject_id = s.subject_id
                    JOIN users u ON t.user_id = u.id
                ";

                using (SqlCommand cmd = new SqlCommand(sql, connect))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        listData.Add(new AddTeachersData
                        {
                            ID = Convert.ToInt32(reader["ID"]),
                            TeacherID = reader["TeacherID"].ToString(),
                            TeacherName = reader["TeacherName"].ToString(),
                            TeacherGender = reader["TeacherGender"].ToString(),
                            Subject = reader["Subject"].ToString(),
                            TeacherImage = reader["TeacherImage"].ToString(),
                            DateInsert = reader["DateInsert"].ToString(),
                            IsActive = Convert.ToInt32(reader["IsActive"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("TeacherData error:\n" + ex.Message);
            }
            finally
            {
                connect.Close();
            }

            return listData;
        }

    }
}
