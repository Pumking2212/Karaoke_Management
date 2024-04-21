using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;

using iTextSharp.text.pdf;
using System.IO;

namespace KaraokeManagementSystem
{
    public partial class BillModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        
        public BillModuleForm()
        {
            InitializeComponent();
            LoadRoomIDs();
            LoadEmployeeNumbers();
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        private void LoadRoomIDs()
        {
            string query = "SELECT id_room FROM room WHERE room_status = 'available'";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmb_RoomID.Items.Add(reader["id_room"].ToString());
                }
                con.Close();
            }
        }

        private void LoadEmployeeNumbers()
        {
            string query = "SELECT phonenumber FROM employee";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    cmb_ENum.Items.Add(reader["phonenumber"].ToString());
                }
                con.Close();
            }
        }
        private bool GuestExists(string phoneNumber)
        {
            string query = "SELECT COUNT(*) FROM guest WHERE phonenumber = @phoneNumber";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                return count > 0;
            }
        }
        private int GetMaxBillID()
        {
            int maxBillID = 0;

            string query = "SELECT TOP 1 id_bill FROM bill ORDER BY id_bill DESC";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                //con.Open();
                object result = cmd.ExecuteScalar();
                //con.Close();

                if (result != DBNull.Value)
                {
                    maxBillID = Convert.ToInt32(result);
                }
            }

            return maxBillID;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            if (cmb_RoomID.SelectedItem == null || string.IsNullOrEmpty(cmb_ENum.Text) || dtpTimeIn.Value == null)
            {
                MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            string guestPhoneNum = string.IsNullOrEmpty(txtGuestNum.Text) ? null : txtGuestNum.Text;
            if (!string.IsNullOrEmpty(guestPhoneNum) && guestPhoneNum.Length != 10)
            {
                MessageBox.Show("Guest phone number must contain 10 digits.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            DateTime timeIn = dtpTimeIn.Value;
            if (timeIn > DateTime.Now)
            {
                MessageBox.Show("Time in cannot be later than current time.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            string roomID = cmb_RoomID.SelectedItem.ToString();
            string employeePhoneNum = cmb_ENum.Text;
            decimal? total = null; 

            
            if (!string.IsNullOrEmpty(guestPhoneNum) && !GuestExists(guestPhoneNum))
            {
                
                string insertQuery = "INSERT INTO guest (phonenumber, fullname, dob, rank_type) VALUES (@phoneNumber, @fullname, @dob, @rankType)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@phoneNumber", guestPhoneNum);
                    insertCmd.Parameters.AddWithValue("@fullname", "Guest");
                    insertCmd.Parameters.AddWithValue("@dob", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@rankType", "normal");

                    try
                    {
                        con.Open();
                        insertCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while inserting guest record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }

            string query = "INSERT INTO bill (id_room, guest_phonenum, em_phonenum, time_in, total, is_pay) VALUES (@roomID, @guestPhoneNum, @employeePhoneNum, @timeIn, @total, @isPaid)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@roomID", roomID);
                cmd.Parameters.AddWithValue("@guestPhoneNum", (object)guestPhoneNum ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@employeePhoneNum", employeePhoneNum);
                cmd.Parameters.AddWithValue("@timeIn", timeIn);
                cmd.Parameters.AddWithValue("@total", (object)total ?? DBNull.Value); 
                cmd.Parameters.AddWithValue("@isPaid", false); 

                try
                {
                    con.Open();
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Bill saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    UpdateRoomStatus(roomID, "used");
                    this.Dispose();
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while saving bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    int billID = GetMaxBillID();

                    txtBillID.Text = billID.ToString();
                    con.Close();
                }

            }
            BillForm billForm = new BillForm(); 
            billForm.LoadBills();
        }


        private void UpdateRoomStatus(string roomID, string status)
        {
            string query = "UPDATE room SET room_status = @status WHERE id_room = @roomID";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@roomID", roomID);

                try
                {
                    
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while updating room status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    //con.Close();
                }
            }
        }



        private void btnCallFood_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem txtBillID và txtRoomID đã được nhập chưa
            if (string.IsNullOrEmpty(txtBillID.Text) || string.IsNullOrEmpty(cmb_RoomID.Text))
            {
                MessageBox.Show("Please enter Bill ID and Room ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra xem Bill ID đã tồn tại trong cơ sở dữ liệu hay chưa
            string billID = txtBillID.Text;
            string roomID = cmb_RoomID.Text;
            string checkBillQuery = "SELECT COUNT(*) FROM bill WHERE id_bill = @billID";
            using (SqlCommand checkBillCmd = new SqlCommand(checkBillQuery, con))
            {
                checkBillCmd.Parameters.AddWithValue("@billID", billID);
                con.Open();
                int billCount = (int)checkBillCmd.ExecuteScalar();
                con.Close();

                if (billCount == 0)
                {
                    MessageBox.Show("Bill ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // Hiển thị form CallFoodModuleForm và truyền giá trị txtBillID và txtRoomID sang
            CallFoodModuleForm callFoodForm = new CallFoodModuleForm();
            callFoodForm.txtBillID.Text = txtBillID.Text;
            callFoodForm.cmb_RoomID.Text = cmb_RoomID.Text;
            callFoodForm.txtBillID.Enabled = false;
            callFoodForm.cmb_RoomID.Enabled = false;
            callFoodForm.txtBillDetailID.Enabled = false;
            callFoodForm.ShowDialog();
        }


        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtBillID.Text))
                {
                    MessageBox.Show("Please enter Bill ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int billID = Convert.ToInt32(txtBillID.Text);

               
                if (!BillExists(billID))
                {
                    MessageBox.Show("Bill ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

               
                DialogResult result = MessageBox.Show("Are you sure to pay this bill?", "Confirm Payment", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                
                if (result == DialogResult.OK)
                {
                    
                    string query = "UPDATE bill SET is_pay = 1 WHERE id_bill = @billID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@billID", billID);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        UpdateRoomStatus(cmb_RoomID.Text, "available");
                        con.Close();
                    }
                    
                    MessageBox.Show("Bill has been paid successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    
                    PrintBillToPDF(billID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            // Kiểm tra số điện thoại khách hàng
            string guestPhoneNum = string.IsNullOrEmpty(txtGuestNum.Text) ? null : txtGuestNum.Text;
            if (!string.IsNullOrEmpty(guestPhoneNum) && guestPhoneNum.Length != 10)
            {
                MessageBox.Show("Guest phone number must contain 10 digits.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!string.IsNullOrEmpty(guestPhoneNum) && !GuestExists(guestPhoneNum))
            {
                // Nếu số điện thoại không tồn tại, thêm một row  mới vào bảng guest
                string insertQuery = "INSERT INTO guest (phonenumber, fullname, dob, rank_type) VALUES (@phoneNumber, @fullname, @dob, @rankType)";
                using (SqlCommand insertCmd = new SqlCommand(insertQuery, con))
                {
                    insertCmd.Parameters.AddWithValue("@phoneNumber", guestPhoneNum);
                    insertCmd.Parameters.AddWithValue("@fullname", "Guest");
                    insertCmd.Parameters.AddWithValue("@dob", DateTime.Now);
                    insertCmd.Parameters.AddWithValue("@rankType", "normal");

                    try
                    {
                        con.Open();
                        insertCmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while inserting guest record: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
                
                int billID = Convert.ToInt32(txtBillID.Text);

                
                if (!BillExists(billID))
                {
                    MessageBox.Show("Bill ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                
                string query = "UPDATE bill SET guest_phonenum = @guestPhoneNum WHERE id_bill = @billID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@guestPhoneNum", (object)guestPhoneNum ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@billID", billID);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Bill updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Dispose();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error while updating bill: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            
        }
        private bool BillExists(int billID)
        {
            string query = "SELECT COUNT(*) FROM bill WHERE id_bill = @billID";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@billID", billID);
                con.Open();
                int count = (int)cmd.ExecuteScalar();
                con.Close();
                return count > 0;
            }
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            //dtpTimeIn.Value = DateTime.Now;
            //cmb_ENum.SelectedIndex = -1; 
            //cmb_RoomID.SelectedIndex = -1; 
            txtGuestNum.Clear();
        }
        private void PrintBillToPDF(int billID)
        {
          
            Document doc = new Document();

            try
            {

                string billDirectory = Path.Combine(Application.StartupPath, "Bill");

                if (!Directory.Exists(billDirectory))
                {
                    Directory.CreateDirectory(billDirectory);
                }

              
                string fileName = Path.Combine(billDirectory, "Bill_" + billID + ".pdf");

                
                PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));



                doc.Open();

                // Thêm tiêu đề
                Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("PumKing Karaoke System", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                doc.Add(title);

                // Thêm thời gian thanh toán
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph timePaid = new Paragraph("Payment Time: " + DateTime.Now.ToString(), smallFont);
                timePaid.Alignment = Element.ALIGN_CENTER;
                doc.Add(timePaid);

                // Lấy thông tin nhân viên từ phonenumber
                string employeeName = GetEmployeeName(cmb_ENum.Text);
                Paragraph employeeInfo = new Paragraph("Employee: " + employeeName, smallFont);
                employeeInfo.Alignment = Element.ALIGN_CENTER;
                doc.Add(employeeInfo);

                // Thêm dòng chữ Bill
                Paragraph billHeader = new Paragraph("Bill", titleFont);
                billHeader.Alignment = Element.ALIGN_CENTER;
                doc.Add(billHeader);

                // Thêm thông tin khách hàng
                AddGuestInfo(doc, billID);

                // Thêm thông tin sử dụng phòng và thức ăn
                AddRoomAndFoodInfo(doc, billID);

                // Thêm thông tin discount và tổng cộng
                AddDiscountAndTotal(doc, billID);

                // Đóng tài liệu
                doc.Close();

                MessageBox.Show("Bill has been printed to PDF successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while printing bill to PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetEmployeeName(string phoneNumber)
        {
            string fullName = "";

            // Truy vấn để lấy thông tin nhân viên từ phonenumber
            string query = "SELECT fullname FROM employee WHERE phonenumber = @phoneNumber";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    fullName = reader["fullname"].ToString();
                }
                con.Close();
            }

            return fullName;
        }

        private void AddGuestInfo(Document doc, int billID)
        {
            // Lấy thông tin khách hàng từ bảng guest
            string guestName = "";
            string guestPhoneNum = "";
            DateTime guestDOB = DateTime.Now;
            string guestRankType = "";

            string query = "SELECT fullname, phonenumber, dob, rank_type FROM guest WHERE phonenumber IN (SELECT guest_phonenum FROM bill WHERE id_bill = @billID)";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@billID", billID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    guestName = reader["fullname"].ToString();
                    guestPhoneNum = reader["phonenumber"].ToString();
                    guestDOB = Convert.ToDateTime(reader["dob"]);
                    guestRankType = reader["rank_type"].ToString();
                }
                con.Close();
            }

            
            Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Paragraph guestInfo = new Paragraph();
            guestInfo.Add(new Chunk("Guest Name: " + guestName, smallFont));
            guestInfo.Add(Chunk.NEWLINE);
            guestInfo.Add(new Chunk("Guest Phonenumber: " + guestPhoneNum, smallFont));
            guestInfo.Add(Chunk.NEWLINE);
            guestInfo.Add(new Chunk("Date of Birth: " + guestDOB.ToString("dd/MM/yyyy"), smallFont));
            guestInfo.Add(Chunk.NEWLINE);
            guestInfo.Add(new Chunk("Rank Type: " + guestRankType, smallFont));
            guestInfo.Add(Chunk.NEWLINE);

            doc.Add(guestInfo);
        }

        private void AddRoomAndFoodInfo(Document doc, int billID)
        {
            try
            {
                
                string roomID = "";
                DateTime timeIn = DateTime.Now;
                DateTime timeOut = DateTime.Now;
                double totalHours = 0;

                
                string query = "SELECT id_room, time_in FROM bill WHERE id_bill = @billID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@billID", billID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        roomID = reader["id_room"].ToString();
                        timeIn = Convert.ToDateTime(reader["time_in"]);
                    }
                    con.Close();
                }

              
                TimeSpan hoursUsed = DateTime.Now.Subtract(timeIn);
                totalHours = Math.Ceiling(hoursUsed.TotalHours);

                
                timeOut = timeIn.AddHours(totalHours);

               
                decimal pricePerHour = 0;
                query = "SELECT price FROM room WHERE id_room = @roomID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@roomID", roomID);

                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        pricePerHour = Convert.ToDecimal(result);
                    }
                    con.Close();
                }

               
                Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                Paragraph timeInfo = new Paragraph();
                timeInfo.Add(new Chunk("Check-in time: " + timeIn.ToString("dd/MM/yyyy HH:mm:ss"), smallFont));
                timeInfo.Add(Chunk.NEWLINE);
                timeInfo.Add(new Chunk("Check-out time: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), smallFont));
                timeInfo.Add(Chunk.NEWLINE);
                doc.Add(timeInfo);

                
                Paragraph roomInfo = new Paragraph();
                roomInfo.Add(new Chunk("Number of hours used room: " + totalHours.ToString(), smallFont));
                roomInfo.Add(Chunk.NEWLINE);
                roomInfo.Add(new Chunk("Price per hour: " + pricePerHour.ToString() + "VND", smallFont));
                roomInfo.Add(Chunk.NEWLINE);
                doc.Add(roomInfo);

                
                query = "SELECT food.foodname, bill_detail.amount, food.price " +
                        "FROM bill_detail " +
                        "JOIN food ON bill_detail.id_food = food.id_food " +
                        "WHERE bill_detail.id_bill = @billID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@billID", billID);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string foodName = reader["foodname"].ToString();
                        int amount = Convert.ToInt32(reader["amount"]);
                        decimal price = Convert.ToDecimal(reader["price"]);

                        Font foodFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                        Paragraph foodInfo = new Paragraph();
                        foodInfo.Add(new Chunk("Food's Used: ", foodFont));
                        foodInfo.Add(new Chunk(foodName + " : " + amount + " : (Total: " + (price * amount).ToString() + "VND )", smallFont));
                        foodInfo.Add(Chunk.NEWLINE);
                        doc.Add(foodInfo);
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while adding room and food information to PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void AddDiscountAndTotal(Document doc, int billID)
        {
            string guestPhoneNum = "";
            string guestRankType = "";

            string query = "SELECT guest_phonenum FROM bill WHERE id_bill = @billID";
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@billID", billID);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    guestPhoneNum = reader["guest_phonenum"].ToString();
                }
                con.Close();
            }

            if (string.IsNullOrEmpty(guestPhoneNum))
            {
                guestRankType = "none";
            }
            else
            {
                query = "SELECT rank_type FROM guest WHERE phonenumber = @guestPhoneNum";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@guestPhoneNum", guestPhoneNum);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        guestRankType = reader["rank_type"].ToString();
                    }
                    con.Close();
                }
            }

            decimal discountRate = 0;
            switch (guestRankType)
            {
                case "normal":
                    discountRate = 0;
                    break;
                case "vip":
                    discountRate = 0.02m;
                    break;
                case "gold":
                    discountRate = 0.04m;
                    break;
                case "platinum":
                    discountRate = 0.08m;
                    break;
                case "diamond":
                    discountRate = 0.12m;
                    break;
                default:
                    discountRate = 0;
                    break;
            }

            decimal total = CalculateTotal(billID, discountRate);

            Font smallFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
            Paragraph discountAndTotalInfo = new Paragraph();
            discountAndTotalInfo.Add(new Chunk("Discount: " + (discountRate * 100).ToString() + "%", smallFont));
            discountAndTotalInfo.Add(Chunk.NEWLINE);
            discountAndTotalInfo.Add(new Chunk("Total: " + total.ToString() +" VND", smallFont));
            discountAndTotalInfo.Add(Chunk.NEWLINE);

            doc.Add(discountAndTotalInfo);
        }

        private decimal CalculateTotal(int billID, decimal discountRate)
        {
            decimal total = 0;

            string query = "SELECT SUM(room.price * DATEDIFF(HOUR, bill.time_in, GETDATE())) " +
                            "FROM room " +
                            "JOIN bill ON room.id_room = bill.id_room " +
                            "WHERE bill.id_bill = @billID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@billID", billID);

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    total += Convert.ToDecimal(result);
                }
                con.Close();
            }

            query = "SELECT SUM(bill_detail.amount * food.price) " +
                     "FROM bill_detail " +
                     "JOIN food ON bill_detail.id_food = food.id_food " +
                     "WHERE bill_detail.id_bill = @billID";

            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@billID", billID);

                con.Open();
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                {
                    total += Convert.ToDecimal(result);
                }
                con.Close();
            }

            
            total -= total * discountRate;

            string updateTotalQuery = "UPDATE bill SET total = @total WHERE id_bill = @billID";
            using (SqlCommand updateCmd = new SqlCommand(updateTotalQuery, con))
            {
                updateCmd.Parameters.AddWithValue("@total", total);
                updateCmd.Parameters.AddWithValue("@billID", billID);

                con.Open();
                updateCmd.ExecuteNonQuery();
                con.Close();
            }

            return total;
        }

    }
}
