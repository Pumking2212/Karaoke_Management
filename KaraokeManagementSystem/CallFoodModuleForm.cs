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
    public partial class CallFoodModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        public CallFoodModuleForm()
        {
            InitializeComponent();
            LoadFoodNames();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void LoadFoodNames()
        {
            string query = "SELECT foodname FROM food";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmb_Food.Items.Add(reader["foodname"].ToString());
                }
                con.Close();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem đã chọn thức ăn và nhập số lượng chưa
                if (cmb_Food.SelectedItem == null || string.IsNullOrEmpty(txtAmount.Text))
                {
                    MessageBox.Show("Please select a food and enter the quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra số lượng nhập vào có phải là số không
                if (!int.TryParse(txtAmount.Text, out int amount))
                {
                    MessageBox.Show("Quantity must be a valid integer.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy tên thức ăn và số lượng từ các control trên form
                string foodName = cmb_Food.SelectedItem.ToString();

                // Tìm id_food tương ứng với foodName trong bảng food
                string queryFoodID = "SELECT id_food FROM food WHERE foodname = @foodname";
                using (SqlCommand cmdFoodID = new SqlCommand(queryFoodID, con))
                {
                    cmdFoodID.Parameters.AddWithValue("@foodname", foodName);
                    con.Open();
                    int foodID = Convert.ToInt32(cmdFoodID.ExecuteScalar());
                    con.Close();

                    // Kiểm tra số lượng thức ăn có đủ không
                    string queryFoodAmount = "SELECT amount FROM food WHERE id_food = @foodID";
                    using (SqlCommand cmdFoodAmount = new SqlCommand(queryFoodAmount, con))
                    {
                        cmdFoodAmount.Parameters.AddWithValue("@foodID", foodID);
                        con.Open();
                        int foodAmount = Convert.ToInt32(cmdFoodAmount.ExecuteScalar());
                        con.Close();

                        if (amount > foodAmount)
                        {
                            MessageBox.Show("Not enough food available in stock.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Insert dữ liệu vào bảng bill_detail
                        string insertQuery = "INSERT INTO bill_detail (id_bill, id_food, amount) VALUES (@id_bill, @id_food, @amount)";
                        using (SqlCommand cmdInsert = new SqlCommand(insertQuery, con))
                        {
                            cmdInsert.Parameters.AddWithValue("@id_bill", int.Parse(txtBillID.Text));
                            cmdInsert.Parameters.AddWithValue("@id_food", foodID);
                            cmdInsert.Parameters.AddWithValue("@amount", amount);
                            con.Open();
                            cmdInsert.ExecuteNonQuery();
                            con.Close();
                        }

                        // Trừ đi số lượng đã đặt từ bảng food
                        string updateFoodAmountQuery = "UPDATE food SET amount = amount - @amount WHERE id_food = @foodID";
                        using (SqlCommand cmdUpdateFoodAmount = new SqlCommand(updateFoodAmountQuery, con))
                        {
                            cmdUpdateFoodAmount.Parameters.AddWithValue("@amount", amount);
                            cmdUpdateFoodAmount.Parameters.AddWithValue("@foodID", foodID);
                            con.Open();
                            cmdUpdateFoodAmount.ExecuteNonQuery();
                            con.Close();
                        }

                        MessageBox.Show("Food order sent successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            cmb_Food.SelectedIndex = -1;
            txtAmount.Clear();  
        }
    }
}
