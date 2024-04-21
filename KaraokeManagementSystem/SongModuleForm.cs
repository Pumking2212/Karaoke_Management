using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace KaraokeManagementSystem
{
    public partial class SongModuleForm : Form
    {
        SqlConnection con = DBConnection.GetConnection();
        SqlCommand cm = new SqlCommand();

        public SongModuleForm()
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
                string songName = txtSongName.Text;
                string songType = cmb_Type.Text;
                string songArtist = txtArtist.Text;

                if (string.IsNullOrEmpty(songName) || string.IsNullOrEmpty(songType) || string.IsNullOrEmpty(songArtist))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                cm = new SqlCommand("INSERT INTO [Karaoke1].[dbo].[song] (songname, song_type, song_artist) VALUES (@songName, @songType, @songArtist)", con);
                cm.Parameters.AddWithValue("@songName", songName);
                cm.Parameters.AddWithValue("@songType", songType);
                cm.Parameters.AddWithValue("@songArtist", songArtist);

                con.Open();
                cm.ExecuteNonQuery();
                con.Close();

                MessageBox.Show("Song has been successfully saved.");
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
                if (MessageBox.Show("Are you sure you want to update this song?", "Update Record", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string songName = txtSongName.Text;
                    string songType = cmb_Type.Text;
                    string songArtist = txtArtist.Text;

                    if (string.IsNullOrEmpty(songName) || string.IsNullOrEmpty(songType) || string.IsNullOrEmpty(songArtist))
                    {
                        MessageBox.Show("Please fill in all fields.");
                        return;
                    }

                    cm = new SqlCommand("UPDATE [Karaoke1].[dbo].[song] SET songname = @songName, song_type = @songType, song_artist = @songArtist WHERE id_song = '" + txtID.Text + "'", con);
                    cm.Parameters.AddWithValue("@songName", songName);
                    cm.Parameters.AddWithValue("@songType", songType);
                    cm.Parameters.AddWithValue("@songArtist", songArtist);

                    con.Open();
                    cm.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Song has been successfully updated.");
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
            txtSongName.Clear();
            cmb_Type.SelectedIndex = -1;
            txtArtist.Clear();
        }
    }
}
