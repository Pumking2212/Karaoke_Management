using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class SongForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();
        SqlDataReader dr;

        public SongForm()
        {
            InitializeComponent();
            LoadSongs();
        }

        public void LoadSongs()
        {
            dgvSong.Rows.Clear();
            cm = new SqlCommand("SELECT [id_song], [songname], [song_type], [song_artist] FROM [Karaoke1].[dbo].[song]", con);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvSong.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchText))
            {
                LoadSongsWithFilter(searchText);
            }
            else
            {
                LoadSongs();
            }
        }

        private void LoadSongsWithFilter(string searchText)
        {
            dgvSong.Rows.Clear();
            string query = "SELECT [id_song], [songname], [song_type], [song_artist] FROM [Karaoke1].[dbo].[song] WHERE [id_song] LIKE @searchText OR [songname] LIKE @searchText OR [song_type] LIKE @searchText OR [song_artist] LIKE @searchText";
            cm = new SqlCommand(query, con);
            cm.Parameters.AddWithValue("@searchText", "%" + searchText + "%");
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                dgvSong.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString());
            }
            dr.Close();
            con.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            SongModuleForm songModuleForm = new SongModuleForm();
            songModuleForm.btnSave.Enabled = true;
            songModuleForm.btnUpdate.Enabled = false;
            songModuleForm.txtID.Enabled = false;
            songModuleForm.ShowDialog();
            LoadSongs(); 
        }
        private void dgvSong_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colName = dgvSong.Columns[e.ColumnIndex].Name;
            if (colName == "Edit")
            {
                string songID = dgvSong.Rows[e.RowIndex].Cells[0].Value.ToString();
                string songName = dgvSong.Rows[e.RowIndex].Cells[1].Value.ToString();
                string songType = dgvSong.Rows[e.RowIndex].Cells[2].Value.ToString();
                string songArtist = dgvSong.Rows[e.RowIndex].Cells[3].Value.ToString();

                SongModuleForm songModuleForm = new SongModuleForm();

                songModuleForm.txtID.Text = songID;
                songModuleForm.txtSongName.Text = songName;
                songModuleForm.cmb_Type.Text = songType;
                songModuleForm.txtArtist.Text = songArtist;

                songModuleForm.btnSave.Enabled = false;
                songModuleForm.btnUpdate.Enabled = true;
                songModuleForm.txtID.Enabled = false;

                songModuleForm.ShowDialog();
            }
            else if (colName == "Delete")
            {
                if (MessageBox.Show("Are you sure you want to delete this song?", "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string songID = dgvSong.Rows[e.RowIndex].Cells[0].Value.ToString();
                    con.Open();
                    cm = new SqlCommand("DELETE FROM [Karaoke1].[dbo].[song] WHERE [id_song] = @songID", con);
                    cm.Parameters.AddWithValue("@songID", songID);
                    cm.ExecuteNonQuery();
                    con.Close();
                    MessageBox.Show("Record has been successfully deleted!");
                }
            }
            LoadSongs(); // Reload songs after editing or deleting
        }

    }
}
