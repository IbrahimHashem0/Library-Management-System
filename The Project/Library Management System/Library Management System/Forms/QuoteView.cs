using Library_Management_System.Models;
using Library_Management_System.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Library_Management_System.Forms
{
    public partial class QuoteView : UserControl
    {

        List<Quote> quoteList = new List<Quote>();
        QuoteRepository quoteRepo = new QuoteRepository(); // Instantiate the repo
        Random random = new Random();

        // UI Controls
        Label lblQuote;
        Label lblAuthor;


        public QuoteView()
        {
            InitializeComponent();
            LoadQuotesFromDatabase();
            Initialize();
            ShowRandomQuote();
        }

        private void LoadQuotesFromDatabase()
        {
            
            quoteList = quoteRepo.GetAllQuotes();
   
        }
        private void ShowRandomQuote()
        {
            if (quoteList.Count > 0)
            {
                int index = random.Next(quoteList.Count);
                lblQuote.Text = "“" + quoteList[index].Text + "”";
                lblAuthor.Text = "– " + quoteList[index].Author;
            }
        }
        private void Initialize()
        {
            // 1. Configure the Form
            this.Text = "Quote of the Day";
            this.Size = new Size(600, 550);
            this.BackColor = Color.FromArgb(242, 242, 242); // Light Gray Background
           // this.StartPosition = FormStartPosition.CenterScreen;

            // 2. The Image (PictureBox)
            PictureBox pbIllustration = new PictureBox();
            // PLEASE NOTE: You must have an image file at this location or this line will crash.
            // If you don't have one yet, comment the next line out to test the text only.
            pbIllustration.Image = Image.FromFile("Screenshot_1.png"); 
            pbIllustration.BackColor = Color.Transparent; // Or match form color
            pbIllustration.SizeMode = PictureBoxSizeMode.Zoom;
            pbIllustration.Size = new Size(400, 250);

            // Center the image horizontally
            pbIllustration.Location = new Point((this.ClientSize.Width - pbIllustration.Width) / 2, 200);

            // Handle resizing to keep it centered
            this.Resize += (s, e) => {
                pbIllustration.Left = (this.ClientSize.Width - pbIllustration.Width) / 2;
            };

            // 3. The Quote Label
            lblQuote = new Label { 
            Font = new Font("Segoe UI", 14, FontStyle.Regular),
            ForeColor = Color.Black,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(this.ClientSize.Width - 40, 80), 
            Location = new Point(20, 500), 
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            // 4. The Author Label
            lblAuthor = new Label {
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                ForeColor = Color.DodgerBlue, // The blue color
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(this.ClientSize.Width - 40, 40),
                Location = new Point(20, 570), // Positioned below quote
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right 
            };

            // Add controls to the Form
            this.Controls.Add(pbIllustration);
            this.Controls.Add(lblQuote);
            this.Controls.Add(lblAuthor);
        }
    }
}
