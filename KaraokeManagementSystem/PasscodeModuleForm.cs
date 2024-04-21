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

namespace KaraokeManagementSystem
{
    public partial class PasscodeModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        public PasscodeModuleForm()
        {
            InitializeComponent();
            LoadEmployeeNumbers();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose(); 
        }
        private void LoadEmployeeNumbers()
        {
            string query = "SELECT phonenumber FROM employee";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmb_ENum.Items.Add(reader["phonenumber"].ToString());
                }
                con.Close();
            }
        }
        private void txtPass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnCheck_Click(sender, e);
            }
        }
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string selectedPhoneNumber = cmb_ENum.SelectedItem?.ToString();
            string passcode = txtPass.Text;

            if (string.IsNullOrEmpty(selectedPhoneNumber))
            {
                MessageBox.Show("Please select a phone number.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(passcode))
            {
                MessageBox.Show("Please enter the passcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (CheckPasscode(selectedPhoneNumber, passcode))
            {
                MessageBox.Show("Login Successfully","ACCESS GRANTED", MessageBoxButtons.OK, MessageBoxIcon.Information);
                BillModuleForm billModuleForm = new BillModuleForm();
                billModuleForm.cmb_ENum.SelectedIndex = cmb_ENum.SelectedIndex;
                billModuleForm.cmb_ENum.Enabled = false;
                billModuleForm.txtBillID.Enabled = false;
                billModuleForm.ShowDialog();
                
                this.Dispose(); 
            }
            else
            {
                
                MessageBox.Show("Incorrect passcode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckPasscode(string phoneNumber, string passcode)
        {
            string query = "SELECT COUNT(*) FROM employee WHERE phonenumber = @phoneNumber AND passcode = @passcode";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@passcode", passcode);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                return count > 0;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            cmb_ENum.SelectedIndex = -1;
            txtPass.Clear();
        }
    }
}
