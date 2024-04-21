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
    public partial class GuestModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();

        public GuestModuleForm()
        {
            InitializeComponent();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtPhone.Text.Length != 10 || !txtPhone.Text.All(char.IsDigit))
                {
                    MessageBox.Show("Phone number must be 10 digits long and contain only numbers.");
                    return;
                }

                string fullName = string.IsNullOrEmpty(txtFullName.Text) ? "Guest" : txtFullName.Text;

                string checkQuery = "SELECT COUNT(*) FROM [Karaoke1].[dbo].[guest] WHERE [phonenumber] = @phonenumber";
                cm = new SqlCommand(checkQuery, con);
                cm.Parameters.AddWithValue("@phonenumber", txtPhone.Text);

                con.Open();
                int count = Convert.ToInt32(cm.ExecuteScalar());
                con.Close();

                if (count > 0)
                {
                    MessageBox.Show("Phone number already exists in the database. Please enter a different phone number.");
                    txtPhone.Clear();
                    return;
                }

                string query = "INSERT INTO [Karaoke1].[dbo].[guest] (phonenumber, fullname, dob, rank_type) VALUES (@phonenumber, @fullname, @dob, @rank_type)";
                cm = new SqlCommand(query, con);
                cm.Parameters.AddWithValue("@phonenumber", txtPhone.Text);
                cm.Parameters.AddWithValue("@fullname", fullName);
                cm.Parameters.AddWithValue("@dob", dt_dobGuest.Value);
                cm.Parameters.AddWithValue("@rank_type", "normal");

                con.Open();
                cm.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Guest has been successfully saved.");
                this.Dispose();
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
                if (MessageBox.Show("Are you sure you want to update this guest?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string query = "UPDATE [Karaoke1].[dbo].[guest] SET fullname = @fullname, dob = @dob, rank_type = @rank_type WHERE phonenumber = @phonenumber";
                    cm = new SqlCommand(query, con);
                    cm.Parameters.AddWithValue("@phonenumber", txtPhone.Text);
                    cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                    cm.Parameters.AddWithValue("@dob", dt_dobGuest.Value);
                    cm.Parameters.AddWithValue("@rank_type", txtRank.Text);

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Guest has been successfully updated.");
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
            ClearFields();
        }

        private void ClearFields()
        {
            //txtPhone.Clear();
            txtFullName.Clear();
            //txtRank.Clear();
            dt_dobGuest.Value = DateTime.Now;
        }

        private void txtPhone_TextChanged(object sender, EventArgs e)
        {
            if (CheckPhoneNumberExists(txtPhone.Text))
            {
                MessageBox.Show("This phone number already exists in the database.");
                txtPhone.Clear();
            }
        }

        private bool CheckPhoneNumberExists(string phoneNumber)
        {
            string query = "SELECT COUNT(*) FROM [Karaoke1].[dbo].[guest] WHERE [phonenumber] = @phoneNumber";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@phoneNumber", phoneNumber);

            con.Open();
            int count = Convert.ToInt32(cm.ExecuteScalar());
            con.Close();

            return count > 0;
        }
    }
}
