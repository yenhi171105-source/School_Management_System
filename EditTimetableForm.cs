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
    public partial class EditTimetableForm : Form
    {
        public string SelectedSubject { get; private set; }

        public EditTimetableForm(string day, int period, string subject)
        {
            InitializeComponent();

            lblInfo.Text = $"{day} - Tiết {period}";
            LoadSubjects();
            cbSubject.Text = subject;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbSubject.SelectedItem == null)
            {
                MessageBox.Show("Please select a subject.");
                return;
            }

            SelectedSubject = cbSubject.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void LoadSubjects()
        {
            using (SqlConnection conn =
                new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SchoolDB;Integrated Security=True"))
            {
                conn.Open();

                SqlDataAdapter da = new SqlDataAdapter(
                    "SELECT subject_name FROM Subjects",
                    conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                cbSubject.DataSource = dt;
                cbSubject.DisplayMember = "subject_name";
            }
        }
    }
}
