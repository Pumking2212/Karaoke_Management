using System.Data.SqlClient;
using System.Windows.Forms;
using System;
using System.Linq;

namespace KaraokeManagementSystem
{
    public partial class EmployeeModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();

        public EmployeeModuleForm()
        {
            InitializeComponent();
        }
        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            
        }

        private void ClearFields()
        {
            txtFullName.Clear();
            txtPhone.Clear();
            rbtn_M.Checked = false;
            rbtn_F.Checked = false;
        }
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (txtPass.Text.Length < 6)
                {
                    MessageBox.Show("Passcode must be at least 6 characters long.");
                    return;
                }

                if (txtPhone.Text.Length != 10 || !txtPhone.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Phone number must be 10 digits long and contain only numbers.");
                    //txtPhone.Clear();
                    return;
                }

                con.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM employee WHERE phonenumber = @phonenum", con);
                checkCmd.Parameters.AddWithValue("@phonenum", txtPhone.Text);
                int phoneExists = (int)checkCmd.ExecuteScalar();
                con.Close();

                if (phoneExists > 0)
                {
                    MessageBox.Show("This phone number already exists in the database.");
                    return;
                }

                string fullName = string.IsNullOrEmpty(txtFullName.Text) ? "Employee" : txtFullName.Text;

                if (!rbtn_M.Checked && !rbtn_F.Checked)
                {
                    MessageBox.Show("Please select gender.");
                    return;
                }

                string gender = rbtn_M.Checked ? "M" : "F";

                cm = new SqlCommand("INSERT INTO employee(phonenumber, fullname, gender, passcode) VALUES (@phonenum,@fullname,  @gender, @passcode)", con);
                cm.Parameters.AddWithValue("@fullname", fullName);
                cm.Parameters.AddWithValue("@phonenum", txtPhone.Text);
                cm.Parameters.AddWithValue("@gender", gender);
                cm.Parameters.AddWithValue("@passcode", txtPass.Text);
                con.Open();
                cm.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Employee has been successfully saved.");
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }


        private void pictureBoxClose_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUpdate_Click_1(object sender, EventArgs e)
        {
            try
            {
                
                if (MessageBox.Show("Are you sure you want to update this employee?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (txtPass.Text.Length < 6)
                    {
                        MessageBox.Show("Passcode must be at least 6 characters long.");
                        return;
                    }
                    string fullName = string.IsNullOrEmpty(txtFullName.Text) ? "Employee" : txtFullName.Text;

                    if (!rbtn_M.Checked && !rbtn_F.Checked)
                    {
                        MessageBox.Show("Please select gender.");
                        return;
                    }
                    string gender = rbtn_M.Checked ? "M" : "F";

                    cm = new SqlCommand("UPDATE [Karaoke1].[dbo].[employee] SET fullname = @fullname, passcode = @passcode, gender = @gender WHERE phonenumber = @phonenumber", con);
                    cm.Parameters.AddWithValue("@phonenumber", txtPhone.Text);
                    cm.Parameters.AddWithValue("@passcode", txtPass.Text);
                    cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                    cm.Parameters.AddWithValue("@gender", gender);

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Employee has been successfully updated.");
                    this.Dispose();
                   
                }    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPass.Clear();
            txtFullName.Clear();
            //txtPhone.Clear();
            rbtn_M.Checked = false;
            rbtn_F.Checked = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void txtPass_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void rbtn_F_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rbtn_M_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtFullName_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
