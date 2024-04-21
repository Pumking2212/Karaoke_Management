using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace KaraokeManagementSystem
{
    public partial class EmployeeForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public EmployeeForm()
        {
            InitializeComponent();
            LoadEmployee();
        }

        public void LoadEmployee()
        {
            dgvUser.Rows.Clear();
            cm = new SqlCommand("SELECT [phonenumber], [fullname], [gender], [passcode] FROM [Karaoke1].[dbo].[employee]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvUser.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            EmployeeModuleForm employeeModule = new EmployeeModuleForm();
            employeeModule.btnSave.Enabled = true;
            employeeModule.btnUpdate.Enabled = false;
            employeeModule.ShowDialog();
            LoadEmployee();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadEmployeeWithFilter(searchText);
            }
            else
            {
                LoadEmployee();
            }
        }

        private void LoadEmployeeWithFilter(string searchText)
        {
            dgvUser.Rows.Clear();
            string query = "SELECT [phonenumber], [fullname], [gender], [passcode] FROM [Karaoke1].[dbo].[employee] WHERE [fullname] LIKE @searchText OR [phonenumber] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvUser.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void dgvUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string colName = dgvUser.Columns[e.ColumnIndex].Name;
                if (colName == "Edit")
                {
                    string phoneNumber = dgvUser.Rows[e.RowIndex].Cells[0].Value.ToString();
                    string fullName = dgvUser.Rows[e.RowIndex].Cells[1].Value.ToString();
                    string gender = dgvUser.Rows[e.RowIndex].Cells[2].Value.ToString();
                    string passcode = dgvUser.Rows[e.RowIndex].Cells[3].Value.ToString();
                    EmployeeModuleForm employeeModule = new EmployeeModuleForm();

                    employeeModule.txtPass.Text = passcode;
                    employeeModule.txtFullName.Text = fullName;
                    employeeModule.txtPhone.Text = phoneNumber;

                    if (gender == "M")
                    {
                        employeeModule.rbtn_M.Checked = true;
                    }
                    else if (gender == "F")
                    {
                        employeeModule.rbtn_F.Checked = true;
                    }
                    employeeModule.txtPhone.Enabled = false;
                    employeeModule.btnSave.Enabled = false;
                    employeeModule.btnUpdate.Enabled = true;
                    

                    employeeModule.ShowDialog();
                }
                else if (colName == "Delete")
                {
                    if (MessageBox.Show("Are you sure you want to delete this employee?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        string phoneNumber = dgvUser.Rows[e.RowIndex].Cells[0].Value.ToString();
                        con.Open();
                        cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[employee] WHERE [phonenumber] = @phoneNumber", con);
                        cm.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                        cm.ExecuteNonQuery();
                        con.Close();
                        MessageBox.Show("Record has been successfully deleted!");
                        
                    }
                }
                LoadEmployee();
            }
        }

    }
}
