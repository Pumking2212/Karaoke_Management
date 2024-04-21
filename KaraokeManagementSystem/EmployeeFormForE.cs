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
    public partial class EmployeeFormForE : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;
        public EmployeeFormForE()
        {
            InitializeComponent();
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
                //
            }
        }
        private void LoadEmployeeWithFilter(string searchText)
        {
            dgvUser1.Rows.Clear();
            string query = "SELECT [phonenumber], [fullname], [gender] FROM [Karaoke1].[dbo].[employee] WHERE [fullname] LIKE @searchText OR [phonenumber] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvUser1.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
            }
            dr.Close();
            con.Close();
        }
        private void dgvUser1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string colName = dgvUser1.Columns[e.ColumnIndex].Name;
                if (colName == "Edit")
                {
                    string phoneNumber = dgvUser1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    string fullName = dgvUser1.Rows[e.RowIndex].Cells[1].Value.ToString();
                    string gender = dgvUser1.Rows[e.RowIndex].Cells[2].Value.ToString();
                    //string passcode = dgvUser1.Rows[e.RowIndex].Cells[3].Value.ToString();
                    EmployeeModuleFormForE employeeModule = new EmployeeModuleFormForE();

                    //employeeModule.txtPass.Text = passcode;
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
                    employeeModule.txtFullName.Enabled = false;
                    employeeModule.rbtn_M.Enabled = false;
                    employeeModule.rbtn_F.Enabled = false;
                    employeeModule.txtPhone.Enabled = false;
                    employeeModule.btnCheck.Enabled = true;
                    employeeModule.btnUpdate.Enabled = false;
                    employeeModule.btnClear.Enabled = false;


                    employeeModule.ShowDialog();
                }
                
                
            }
        }
    }
}
