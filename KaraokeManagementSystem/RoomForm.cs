using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class RoomForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public RoomForm()
        {
            InitializeComponent();
            LoadRooms();
        }

        public void LoadRooms()
        {
            dgvRooms.Rows.Clear();
            cm = new SqlCommand("SELECT [id_room], [room_type], [room_status], [price] FROM [Karaoke1].[dbo].[room]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string idRoom = dr[0].ToString();
                string roomType = dr[1].ToString();
                string roomStatus = dr[2].ToString();
                string price = dr[3].ToString() + " VND/1h"; // Thêm đơn vị VND/1h vào giá

                // Xác định hình ảnh tương ứng với room_type
                Image roomImage = null;
                if (roomType == "vip")
                {
                    roomImage = Properties.Resources.vip_room;
                }
                else if (roomType == "normal")
                {
                    roomImage = Properties.Resources.normal_room;
                }

                Color textColor = roomStatus == "available" ? Color.Green : Color.Red;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvRooms, idRoom, roomImage, roomStatus, price);

                row.Cells[2].Style.ForeColor = textColor;

                row.Height = 220;

                dgvRooms.Rows.Add(row);
            }
            dr.Close();
            con.Close();
        }




        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadRoomsWithFilter(searchText);
            }
            else
            {
                LoadRooms();
            }
        }

        private void LoadRoomsWithFilter(string searchText)
        {
            dgvRooms.Rows.Clear();
            string query = "SELECT [id_room], [room_type], [room_status], [price] FROM [Karaoke1].[dbo].[room] WHERE [id_room] LIKE @searchText OR [room_type] LIKE @searchText OR [room_status] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string idRoom = dr[0].ToString();
                string roomType = dr[1].ToString();
                string roomStatus = dr[2].ToString();
                string price = dr[3].ToString() + " VND/1h"; 

                
                Image roomImage = null;
                if (roomType == "vip")
                {
                    roomImage = Properties.Resources.vip_room;
                }
                else if (roomType == "normal")
                {
                    roomImage = Properties.Resources.normal_room;
                }

               
                Color textColor = roomStatus == "available" ? Color.Green : Color.Red;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvRooms, idRoom, roomImage, roomStatus, price);

                row.Cells[2].Style.ForeColor = textColor;

                row.Height = 220;

                dgvRooms.Rows.Add(row);
            }
            dr.Close();
            con.Close();
        }


        private void btnAdd_Click(object sender, EventArgs e)
        {
            RoomModuleForm roomModule = new RoomModuleForm();
            roomModule.btnSave.Enabled = true;
            roomModule.btnUpdate.Enabled = false;
            roomModule.txtID.Enabled = false;
            roomModule.rbtn_Avai.Enabled = false;
            roomModule.rbtn_Used.Enabled = false;
            roomModule.ShowDialog();
            LoadRooms();
        }

        private void dgvRooms_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvRooms.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                string roomId = dgvRooms.Rows[e.RowIndex].Cells[0].Value.ToString();
                string roomType = dgvRooms.Rows[e.RowIndex].Cells[1].Value.ToString();
                string roomStatus = dgvRooms.Rows[e.RowIndex].Cells[2].Value.ToString();
                string price = dgvRooms.Rows[e.RowIndex].Cells[3].Value.ToString();

                RoomModuleForm roomModule = new RoomModuleForm();
                roomModule.txtID.Text = roomId;
                price = price.Split(' ')[0];
                roomModule.txtPrice.Text = price;
                
                if (roomType == "vip")
                {
                    roomModule.rbtn_Vip.Checked = true;
                }
                else
                {
                    roomModule.rbtn_Normal.Checked = true;
                }

                if (roomStatus == "available")
                {
                    roomModule.rbtn_Avai.Checked = true;
                }
                else
                {
                    roomModule.rbtn_Used.Checked = true;
                }
                roomModule.rbtn_Avai.Enabled = false;
                roomModule.rbtn_Used.Enabled = false;
                roomModule.txtID.Enabled = false;
                roomModule.btnSave.Enabled = false;
                roomModule.btnUpdate.Enabled = true;
                roomModule.txtID.Enabled = false;
                roomModule.ShowDialog();
                LoadRooms();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this room?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string roomId = dgvRooms.Rows[e.RowIndex].Cells[0].Value.ToString();
                    con.Open();
                    cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[room] WHERE [id_room] = @roomId", con);
                    cm.Parameters.AddWithValue("@roomId", roomId);
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Record has been successfully deleted!");
                    LoadRooms();
                }
            }
        }
    }
}
