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
    public partial class RoomModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        public RoomModuleForm()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!rbtn_Vip.Checked && !rbtn_Normal.Checked)
                {
                    MessageBox.Show("Please select room type (VIP or Normal).");
                    return;
                }

                string priceText = txtPrice.Text.Trim();
                if (!int.TryParse(priceText, out int priceValue))
                {
                    MessageBox.Show("Price must be a valid integer value.");
                    return;
                }

                if (rbtn_Vip.Checked && priceValue <= 170000)
                {
                    MessageBox.Show("Price for VIP room must be greater than 170,000 VND/1h.");
                    return;
                }
                else if (rbtn_Normal.Checked && priceValue >= 150000)
                {
                    MessageBox.Show("Price for Normal room must be less than 150,000 VND/1h.");
                    return;
                }

                string roomType = rbtn_Vip.Checked ? "vip" : "normal";
                string roomStatus = rbtn_Avai.Checked ? "available" : "used";
                string query = "INSERT INTO [Karaoke1].[dbo].[room] (room_type, room_status, price) VALUES (@roomType, @roomStatus, @price)";
                cm = new SqlCommand(query, con);
                
                cm.Parameters.AddWithValue("@roomType", roomType);
                cm.Parameters.AddWithValue("@roomStatus", "available");
                cm.Parameters.AddWithValue("@price", priceText);

                con.Open();
                cm.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Room has been successfully saved.");
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }





        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure you want to update this room?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (!rbtn_Vip.Checked && !rbtn_Normal.Checked)
                    {
                        MessageBox.Show("Please select room type (VIP or Normal).");
                        return;
                    }

                    string priceText = txtPrice.Text.Trim();
                    if (!int.TryParse(priceText, out int priceValue))
                    {
                        MessageBox.Show("Price must be a valid integer value.");
                        return;
                    }

                    if (rbtn_Vip.Checked && priceValue <= 170000)
                    {
                        MessageBox.Show("Price for VIP room must be greater than 170,000 VND/1h.");
                        return;
                    }
                    else if (rbtn_Normal.Checked && priceValue >= 150000)
                    {
                        MessageBox.Show("Price for Normal room must be less than 150,000 VND/1h.");
                        return;
                    }

                    string roomType = rbtn_Vip.Checked ? "vip" : "normal";
                    string roomStatus = rbtn_Avai.Checked ? "available" : "used";
                    string query = "UPDATE [Karaoke1].[dbo].[room] SET room_type = @roomType, room_status = @roomStatus, price = @price WHERE id_room = @roomId";
                    cm = new SqlCommand(query, con);
                    cm.Parameters.AddWithValue("@roomType", roomType);
                    cm.Parameters.AddWithValue("@roomStatus", roomStatus);
                    cm.Parameters.AddWithValue("@price", priceText);
                    cm.Parameters.AddWithValue("@roomId", txtID.Text);

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Room has been successfully updated.");
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
            txtPrice.Clear();
            rbtn_Vip.Checked = false;
            rbtn_Normal.Checked = false;
            //rbtn_Avai.Checked = false;
            //rbtn_Used.Checked = false;
        }

    }
}
