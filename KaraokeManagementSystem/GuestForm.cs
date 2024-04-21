
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class GuestForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public GuestForm()
        {
            InitializeComponent();
            UpdateGuestRank();
            LoadGuest();
            //UpdateGuestRank();
        }

        public void LoadGuest()
        {
            dgvGuest.Rows.Clear();
            cm = new SqlCommand("SELECT [phonenumber], [fullname], [dob], [rank_type] FROM [Karaoke1].[dbo].[guest]", con);
            
            con.Open();
            dr = cm.ExecuteReader();
            //UpdateGuestRank();
            while (dr.Read())
            {
                dgvGuest.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            
            GuestModuleForm guestModule = new GuestModuleForm();
            guestModule.btnSave.Enabled = true;
            guestModule.btnUpdate.Enabled = false;
            guestModule.txtRank.Enabled = false;
            guestModule.ShowDialog();
            LoadGuest();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadGuestWithFilter(searchText);
            }
            else
            {
                LoadGuest();
            }
        }

        private void LoadGuestWithFilter(string searchText)
        {
            dgvGuest.Rows.Clear();
            string query = "SELECT [phonenumber], [fullname], [dob], [rank_type] FROM [Karaoke1].[dbo].[guest] WHERE [phonenumber] LIKE @searchText OR [fullname] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            //UpdateGuestRank();
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvGuest.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }
        private void dgvGuest_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvGuest.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                string phoneNumber = dgvGuest.Rows[e.RowIndex].Cells[0].Value.ToString();
                string fullname = dgvGuest.Rows[e.RowIndex].Cells[1].Value.ToString();
                string dob = dgvGuest.Rows[e.RowIndex].Cells[2].Value.ToString();
                string rankType = dgvGuest.Rows[e.RowIndex].Cells[3].Value.ToString();

                GuestModuleForm guestModule = new GuestModuleForm();

                guestModule.txtPhone.Text = phoneNumber;
                guestModule.txtFullName.Text = fullname;
                guestModule.dt_dobGuest.Value = DateTime.Parse(dob);
                guestModule.txtRank.Text = rankType;

                guestModule.btnSave.Enabled = false;
                guestModule.btnUpdate.Enabled = true;
                guestModule.txtPhone.Enabled = false;
                guestModule.txtRank.Enabled = false;
                guestModule.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this guest?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string phoneNumber = dgvGuest.Rows[e.RowIndex].Cells[0].Value.ToString();
                    con.Open();
                    cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[guest] WHERE [phonenumber] = @phoneNumber", con);
                    cm.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Record has been successfully deleted!");
                }
            }
            LoadGuest();
        }
        
        

        private string CalculateRankType(decimal totalSpent)
        {
            string rankType = "normal";
            if (totalSpent > 10000000)
            {
                rankType = "diamond";
            }
            else if (totalSpent > 4000000)
            {
                rankType = "platinum";
            }
            else if (totalSpent > 2000000)
            {
                rankType = "gold";
            }
            else if (totalSpent > 1000000)
            {
                rankType = "vip";
            }
            else
            {
                rankType = "normal";
            }
            return rankType;
        }

        private void UpdateGuestRank()
        {
            string query = "SELECT guest_phonenum, SUM(total) AS totalSpent " +
                   "FROM bill " +
                   "WHERE is_pay = 1 " +
                   "GROUP BY guest_phonenum";

            // Sử dụng kết nối mới trong phương thức này
            using (SqlConnection connection = DBConnection.GetConnection())
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            foreach (DataRow row in dataTable.Rows)
                            {
                                string guestPhoneNum = row["guest_phonenum"].ToString();
                                decimal totalSpent = Convert.ToDecimal(row["totalSpent"]);

                                string rankType = CalculateRankType(totalSpent);

                                // Sử dụng kết nối chính từ GuestForm
                                UpdateGuestRankType(guestPhoneNum, rankType, connection);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating guest rank type: " + ex.Message);
                }
            }
        }




        private void UpdateGuestRankType(string guestPhoneNum, string rankType, SqlConnection connection)
        {
           
            if (IsValidRankType(rankType))
            {
                string updateQuery = "UPDATE guest SET rank_type = @rankType WHERE phonenumber = @guestPhoneNum";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@rankType", rankType);
                    command.Parameters.AddWithValue("@guestPhoneNum", guestPhoneNum);

                    try
                    {
                        //connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show("Error updating guest rank type: " + ex.Message);
                    }
                    finally
                    {
                        //connection.Close();
                    }
                }
            }
            else
            {

                MessageBox.Show("Invalid rank_type value: " + rankType);
            }
        }



        private bool IsValidRankType(string rankType)
        {
            // Define a list of valid rank types
            List<string> validRankTypes = new List<string> { "normal", "vip", "gold", "platinum", "diamond" };

            // Check if the provided rankType is in the list of valid rank types
            return validRankTypes.Contains(rankType);
        }



    }
}
