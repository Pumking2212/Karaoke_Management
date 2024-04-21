using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class FoodForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public FoodForm()
        {
            InitializeComponent();
            LoadFoods();
        }

        public void LoadFoods()
        {
            dgvFood.Rows.Clear();
            cm = new SqlCommand("SELECT [id_food], [foodname], [food_type], [amount], [price] FROM [Karaoke1].[dbo].[food]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string idFood = dr[0].ToString();
                string foodName = dr[1].ToString();
                string foodType = dr[2].ToString();
                string amount = dr[3].ToString();
                string price = dr[4].ToString();

                // Xác định hình ảnh tương ứng với food_type
                Image foodImage = null;
                Image editBtn = Properties.Resources.edit_20px;
                Image delBtn = Properties.Resources.delete_20px;
                switch (foodType)
                {
                    case "snack":
                        foodImage = Properties.Resources.snack;
                        break;
                    case "drink":
                        foodImage = Properties.Resources.drink;
                        break;
                    case "nut":
                        foodImage = Properties.Resources.nut;
                        break;
                    case "fruit":
                        foodImage = Properties.Resources.fruit;
                        break;
                    default:
                        foodImage = Properties.Resources.other_type;
                        break;
                }

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvFood, idFood, foodName, foodImage, amount, price,editBtn,delBtn);

                row.Height = 100;

                dgvFood.Rows.Add(row);
            }
            dr.Close();
            con.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadFoodsWithFilter(searchText);
            }
            else
            {
                LoadFoods();
            }
        }

        private void LoadFoodsWithFilter(string searchText)
        {
            dgvFood.Rows.Clear();
            string query = "SELECT [id_food], [foodname], [food_type], [amount], [price] FROM [Karaoke1].[dbo].[food] WHERE [id_food] LIKE @searchText OR [foodname] LIKE @searchText OR [food_type] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                string idFood = dr[0].ToString();
                string foodName = dr[1].ToString();
                string foodType = dr[2].ToString();
                string amount = dr[3].ToString();
                string price = dr[4].ToString();

                // Xác định hình ảnh tương ứng với food_type
                Image foodImage = null;
                Image editBtn = Properties.Resources.edit_20px;
                Image delBtn = Properties.Resources.delete_20px;
                switch (foodType)
                {
                    case "snack":
                        foodImage = Properties.Resources.snack;
                        break;
                    case "drink":
                        foodImage = Properties.Resources.drink;
                        break;
                    case "nut":
                        foodImage = Properties.Resources.nut;
                        break;
                    case "fruit":
                        foodImage = Properties.Resources.fruit;
                        break;
                    default:
                        foodImage = Properties.Resources.other_type;
                        break;
                }

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvFood, idFood, foodName, foodImage, amount, price, editBtn, delBtn);

                row.Height = 100;

                dgvFood.Rows.Add(row);
            }
            dr.Close();
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FoodModuleForm foodModuleForm = new FoodModuleForm();
            foodModuleForm.btnSave.Enabled = true;
            foodModuleForm.btnUpdate.Enabled = false;
            foodModuleForm.txtID.Enabled = false;
            foodModuleForm.ShowDialog();
            LoadFoods();
        }

        private void dgvFood_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvFood.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                string foodID = dgvFood.Rows[e.RowIndex].Cells[0].Value.ToString();
                string foodName = dgvFood.Rows[e.RowIndex].Cells[1].Value.ToString();
                string foodType = dgvFood.Rows[e.RowIndex].Cells[2].Value.ToString();
                string amount = dgvFood.Rows[e.RowIndex].Cells[3].Value.ToString();
                string price = dgvFood.Rows[e.RowIndex].Cells[4].Value.ToString();

                FoodModuleForm foodModuleForm = new FoodModuleForm();

                foodModuleForm.txtID.Text = foodID;
                foodModuleForm.txtFoodName.Text = foodName;
                foodModuleForm.cmb_Type.Text = foodType;
                foodModuleForm.txtAmount.Text = amount;
                foodModuleForm.txtPrice.Text = price;

                foodModuleForm.btnSave.Enabled = false;
                foodModuleForm.btnUpdate.Enabled = true;
                foodModuleForm.txtID.Enabled = false;

                foodModuleForm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this food?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string foodID = dgvFood.Rows[e.RowIndex].Cells[0].Value.ToString();
                    con.Open();
                    cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[food] WHERE [id_food] = @foodID", con);
                    cm.Parameters.AddWithValue("@foodID", foodID);
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Record has been successfully deleted!");
                }
            }
            LoadFoods(); // Reload foods after editing or deleting
        }

    }
}
