using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class AccountForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        LoginForm loginForm = new LoginForm();
        string username;
        public AccountForm(string username)
        {
            InitializeComponent();
            this.username = username;
            LoadAccount();
        }

        public void LoadAccount()
        {
            dgvAccount.Rows.Clear();
            cm = new SqlCommand("SELECT [id], [username], [password], [user_role] FROM [Karaoke1].[dbo].[users]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvAccount.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadAccountWithFilter(searchText);
            }
            else
            {
                LoadAccount();
            }
        }

        private void LoadAccountWithFilter(string searchText)
        {
            dgvAccount.Rows.Clear();
            string query = "SELECT [id], [username], [password], [user_role] FROM [Karaoke1].[dbo].[users] WHERE [id] LIKE @searchText OR [username] LIKE @searchText OR [user_role] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvAccount.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void dgvAccount_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvAccount.Columns["Edit"].Index)
            {
                string userID = dgvAccount.Rows[e.RowIndex].Cells[0].Value.ToString();
                string username = dgvAccount.Rows[e.RowIndex].Cells[1].Value.ToString();
                string password = dgvAccount.Rows[e.RowIndex].Cells[2].Value.ToString();
                string userRole = dgvAccount.Rows[e.RowIndex].Cells[3].Value.ToString();
                int adminID = GetUserID("admin1");
                if (Convert.ToInt32(userID) == adminID)
                {
                    MessageBox.Show("Can not edit Admin1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                AccountModuleForm accountModuleForm = new AccountModuleForm();
                accountModuleForm.txtID.Text = userID;
                accountModuleForm.txtUserName.Text = username;
                accountModuleForm.txtPass.Text = password;
                accountModuleForm.txtUserRole.Text = userRole;
                accountModuleForm.btnSave.Enabled = false;
                accountModuleForm.txtID.Enabled = false;
                accountModuleForm.txtRepass.Enabled = false;
                accountModuleForm.ShowDialog();
            }
            else if (e.RowIndex >= 0 && e.ColumnIndex == dgvAccount.Columns["Delete"].Index)
            {
                if (MessageBox.Show("Are you sure you want to delete this account?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string accountID = dgvAccount.Rows[e.RowIndex].Cells[0].Value.ToString();
                    int adminID = GetUserID("admin1");
                    con.Open();
                    if (GetUserID(username) == Convert.ToInt32(accountID))
                    {
                        MessageBox.Show("You are using this account so you cannot deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (Convert.ToInt32(accountID) == adminID) 
                    {
                        MessageBox.Show("Can not delete Admin1.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    } 
                    
                    cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[users] WHERE [id] = @accountID", con);
                    cm.Parameters.AddWithValue("@accountID", accountID); 
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Record has been successfully deleted!");
                }
                LoadAccount();
            }
        }

        private int GetUserID(string username)
        {
            int userID = -1;

            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    string query = "SELECT id FROM users WHERE username = @username";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userID = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return userID;
        }

        


        private void btnAdd_Click(object sender, EventArgs e)
        {
            AccountModuleForm accountModuleForm = new AccountModuleForm();
            accountModuleForm.btnUpdate.Enabled = false;
            accountModuleForm.btnSave.Enabled = true;
            accountModuleForm.txtID.Enabled = false;
            accountModuleForm.ShowDialog();
            LoadAccount();
            
        }

    }
}
