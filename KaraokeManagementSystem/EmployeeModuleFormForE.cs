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
using System.Xml.Linq;

namespace KaraokeManagementSystem
{
    public partial class EmployeeModuleFormForE : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        public EmployeeModuleFormForE()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM employee WHERE phonenumber = @phonenumber AND passcode = @passcode", con);
                checkCmd.Parameters.AddWithValue("@phonenumber", txtPhone.Text);
                checkCmd.Parameters.AddWithValue("@passcode", txtPass.Text);
                int employeeCount = (int)checkCmd.ExecuteScalar();
                con.Close();

                if (employeeCount > 0)
                {
                    MessageBox.Show("Login successful!");
                    txtFullName.Enabled = true;
                    txtPass.Clear();
                    rbtn_M.Enabled = true;
                    rbtn_F.Enabled = true;
                    btnUpdate.Enabled = true;
                    btnClear.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Invalid phone number or passcode. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {

                if (MessageBox.Show("Are you sure you want to update your information", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
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
    }
}
