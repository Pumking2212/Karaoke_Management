using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class BillForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        
        public BillForm()
        {
            InitializeComponent();
            LoadBills();
        }
        
        public void LoadBills()
        {
            dgvBill.Rows.Clear();
            cm = new SqlCommand("SELECT [id_bill], [id_room], [guest_phonenum], [em_phonenum], [time_in], [total] FROM [Karaoke1].[dbo].[bill]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string total = (dr.IsDBNull(5)) ? "unpaid" : dr[5].ToString();
                dgvBill.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), total);
            }
            dr.Close();
            con.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadBillsWithFilter(searchText);
            }
            else
            {
                LoadBills();
            }
        }

        private void LoadBillsWithFilter(string searchText)
        {
            dgvBill.Rows.Clear();
            string query = "SELECT [id_bill], [id_room], [guest_phonenum], [em_phonenum], [time_in], [total] FROM [Karaoke1].[dbo].[bill] WHERE [id_bill] LIKE @searchText OR [id_room] LIKE @searchText OR [guest_phonenum] LIKE @searchText OR [em_phonenum] LIKE @searchText OR [time_in] LIKE @searchText OR [total] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string total = (dr.IsDBNull(5)) ? "unpaid" : dr[5].ToString();
                dgvBill.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), total);
            }
            dr.Close();
            con.Close();
        }
        private void dgvBill_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            

            string colName = dgvBill.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                string isPayQuery = "SELECT [is_pay] FROM [Karaoke1].[dbo].[bill] WHERE [id_bill] = @billID";
                string billID = dgvBill.Rows[e.RowIndex].Cells[0].Value.ToString();
                //string roomID = dgvBill.Rows[e.RowIndex].Cells[1].Value.ToString();
                SqlCommand isPayCmd = new SqlCommand(isPayQuery, con);
                isPayCmd.Parameters.AddWithValue("@billID", billID);
                con.Open();
                bool isPaid = (bool)isPayCmd.ExecuteScalar();
                con.Close();

                if (isPaid)
                {
                    MessageBox.Show("This bill has already been paid and cannot be edited.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                BillModuleForm billModule = new BillModuleForm();
                billModule.txtBillID.Text = dgvBill.Rows[e.RowIndex].Cells[0].Value.ToString();
                billModule.cmb_RoomID.Text = dgvBill.Rows[e.RowIndex].Cells[1].Value.ToString();
                billModule.txtGuestNum.Text = dgvBill.Rows[e.RowIndex].Cells[2].Value.ToString();
                billModule.cmb_ENum.Text = dgvBill.Rows[e.RowIndex].Cells[3].Value.ToString();
                billModule.dtpTimeIn.Value = DateTime.Parse(dgvBill.Rows[e.RowIndex].Cells[4].Value.ToString());
                //billModule.Total = dgvBill.Rows[e.RowIndex].Cells[5].Value.ToString();

                billModule.txtBillID.Enabled = false;
                billModule.cmb_RoomID.Enabled = false;
                billModule.cmb_ENum.Enabled = false;
                billModule.dtpTimeIn.Enabled = false;
                billModule.btnSave.Enabled = false;
                billModule.btnUpdate.Enabled = true;
                billModule.ShowDialog();

                
                LoadBills();
            }
            else if (colName == "Delete")

            {
                
                string billID = dgvBill.Rows[e.RowIndex].Cells[0].Value.ToString();
                int isPaid = 0;

                string isPayQuery = "SELECT [is_pay] FROM [Karaoke1].[dbo].[bill] WHERE [id_bill] = @billID";
                using (SqlCommand isPayCmd = new SqlCommand(isPayQuery, con))
                {
                    isPayCmd.Parameters.AddWithValue("@billID", billID);
                    con.Open();
                    isPaid = Convert.ToInt32(isPayCmd.ExecuteScalar());
                    con.Close();
                }

                if (isPaid == 0) 
                {
                   
                    string roomID = dgvBill.Rows[e.RowIndex].Cells[1].Value.ToString();

                    
                    if (MessageBox.Show("Are you sure you want to delete this bill?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        con.Open();
                        cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[bill] WHERE [id_bill] = @billID", con);
                        cm.Parameters.AddWithValue("@billID", billID);
                        cm.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Bill has been successfully deleted!");

                       
                        string updateRoomStatusQuery = "UPDATE room SET room_status = 'available' WHERE id_room = @roomID";
                        using (SqlCommand updateRoomStatusCmd = new SqlCommand(updateRoomStatusQuery, con))
                        {
                            updateRoomStatusCmd.Parameters.AddWithValue("@roomID", roomID);
                            con.Open();
                            updateRoomStatusCmd.ExecuteNonQuery();
                            con.Close();
                        }

                        LoadBills();
                    }
                }
                else
                {
                    MessageBox.Show("This bill has already been paid and cannot be deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            else if (colName == "View")
            {
                BillModuleForm billModule = new BillModuleForm();
                billModule.txtBillID.Text = dgvBill.Rows[e.RowIndex].Cells[0].Value.ToString();
                billModule.cmb_RoomID.Text = dgvBill.Rows[e.RowIndex].Cells[1].Value.ToString();
                billModule.txtGuestNum.Text = dgvBill.Rows[e.RowIndex].Cells[2].Value.ToString();
                billModule.cmb_ENum.Text = dgvBill.Rows[e.RowIndex].Cells[3].Value.ToString();
                billModule.dtpTimeIn.Value = DateTime.Parse(dgvBill.Rows[e.RowIndex].Cells[4].Value.ToString());


                billModule.txtBillID.Enabled = false;
                billModule.cmb_RoomID.Enabled = false;
                billModule.cmb_ENum.Enabled = false;
                billModule.txtGuestNum.Enabled = false;
                billModule.dtpTimeIn.Enabled = false;
                billModule.btnSave.Enabled = false;
                billModule.btnCallFood.Enabled = false;
                billModule.btnClear.Enabled = false;
                billModule.btnUpdate.Enabled = false;
                billModule.btnPay.Enabled = false;
                billModule.ShowDialog();
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }
    }
}
