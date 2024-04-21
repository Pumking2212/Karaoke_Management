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
    public partial class MainForm : Form
    {
        LoginForm loginForm = new LoginForm();
        public string username;
        


        public MainForm(string username)
        {
            InitializeComponent();
            this.username = username;
            
           
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
        //to show subform form in mainform
        private Form activeForm = null;
        private void openChildForm(Form childForm)
        {
            if (activeForm != null)
                activeForm.Close();
            activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelMain.Controls.Add(childForm);
            panelMain.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();

        }
        private void btnEmployee_Click(object sender, EventArgs e)
        {
            int userRole = GetUserRole(username); 

           
            if (userRole == 1)
            {
                openChildForm(new EmployeeForm());
            }
            else
            {
                openChildForm(new EmployeeFormForE());
            }
        }

        
        private int GetUserRole(string username)
        {
            int userRole = -1; 

            try
            {
                using (SqlConnection con = DBConnection.GetConnection())
                {
                    string query = "SELECT user_role FROM users WHERE username = @username";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@username", username);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userRole = Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return userRole;
        }

        private void btnGuest_Click(object sender, EventArgs e)
        {
            openChildForm(new GuestForm());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            openChildForm(new RoomForm());
        }

        private void btnSong_Click(object sender, EventArgs e)
        {
            openChildForm(new SongForm());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
               
                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Hide(); 
            }
        }

        private void btnFood_Click(object sender, EventArgs e)
        {
            openChildForm(new FoodForm());
        }

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnBill_Click(object sender, EventArgs e)
        {
            openChildForm(new BillForm());
        }

        private void btnAddBill_Click(object sender, EventArgs e)
        {
            int userRole = GetUserRole(username);
            if (userRole == 1) 
            {
                BillModuleForm billModule = new BillModuleForm();
                billModule.btnUpdate.Enabled = false;
                billModule.btnPay.Enabled = false;
                billModule.txtBillID.Enabled = false;
                billModule.ShowDialog();

            }
            else
            {
                PasscodeModuleForm passcodeModuleForm = new PasscodeModuleForm();
                passcodeModuleForm.ShowDialog();
            }    
            
        }

        private void btn_Account_Click(object sender, EventArgs e)
        {
            int userRole = GetUserRole(username);
            if (userRole == 1) 
            {
                openChildForm(new AccountForm(username));
            }
            else
            {
                MessageBox.Show("You can not access this feature with this account", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
