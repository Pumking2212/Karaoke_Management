using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class FoodModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();

        public FoodModuleForm()
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
                string foodName = txtFoodName.Text;
                string foodType = cmb_Type.Text;
                string amount = txtAmount.Text;
                string price = txtPrice.Text;

                if (string.IsNullOrEmpty(foodName) || string.IsNullOrEmpty(foodType) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(price))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                double priceValue;
                if (!double.TryParse(price, out priceValue))
                {
                    MessageBox.Show("Price must be a number.");
                    return;
                }

                if (priceValue <= 1000)
                {
                    MessageBox.Show("Price must be greater than 1000.");
                    return;
                }

                cm = new SqlCommand("INSERT INTO [Karaoke1].[dbo].[food] (foodname, food_type, amount, price) VALUES (@foodName, @foodType, @amount, @price)", con);
                cm.Parameters.AddWithValue("@foodName", foodName);
                cm.Parameters.AddWithValue("@foodType", foodType);
                cm.Parameters.AddWithValue("@amount", amount);
                cm.Parameters.AddWithValue("@price", price);

                con.Open();
                cm.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Food has been successfully saved.");
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
                string foodName = txtFoodName.Text;
                string foodType = cmb_Type.Text;
                string amount = txtAmount.Text;
                string price = txtPrice.Text;

                if (string.IsNullOrEmpty(foodName) || string.IsNullOrEmpty(foodType) || string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(price))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                double priceValue;
                if (!double.TryParse(price, out priceValue))
                {
                    MessageBox.Show("Price must be a number.");
                    return;
                }

                if (priceValue <= 1000)
                {
                    MessageBox.Show("Price must be greater than 1000.");
                    return;
                }

                if (MessageBox.Show("Are you sure you want to update this food?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cm = new SqlCommand("UPDATE [Karaoke1].[dbo].[food] SET foodname = @foodName, food_type = @foodType, amount = @amount, price = @price WHERE id_food = '" + txtID.Text + "'", con);
                    cm.Parameters.AddWithValue("@foodName", foodName);
                    cm.Parameters.AddWithValue("@foodType", foodType);
                    cm.Parameters.AddWithValue("@amount", amount);
                    cm.Parameters.AddWithValue("@price", price);

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Food has been successfully updated.");
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
            txtFoodName.Clear();
            cmb_Type.SelectedIndex = -1;
            txtAmount.Clear();
            txtPrice.Clear();
        }
    }
}
