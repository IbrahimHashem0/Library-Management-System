using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Library_Management_System.Repositories;

namespace Library_Management_System.Forms
{
    public partial class AdminDashboardView : UserControl
    {
        private Label totalBooksLabel;
        private Label totalUsersLabel;
        private Label totalBorrowedLabel;
        public AdminDashboardView()
        {
            InitializeComponent();
            InitializeUI();
            LoadDashboardData();
        }

        private void InitializeUI()
        {
            this.Dock = DockStyle.Fill;

            Label title = new Label
            {
                Text = "Admin Dashboard" ,
                Font = new Font("Segoe UI" , 22 , FontStyle.Bold) ,
                Location = new Point(20 , 20) ,
                ForeColor = Color.FromArgb(63 , 81 , 181) ,
                AutoSize = true 
            };
            this.Controls.Add(title);

            FlowLayoutPanel cardsPanel = new FlowLayoutPanel
            {
                Location = new Point(20 , 90) ,
                Size = new Size(this.Width - 40 , 200) ,
                AutoSize = false ,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(cardsPanel);

            totalBooksLabel = CreateCard("Total Books");
            totalUsersLabel = CreateCard("Total Users");
            totalBorrowedLabel = CreateCard("Total Borrowed");

            cardsPanel.Controls.Add(totalBooksLabel.Parent);
            cardsPanel.Controls.Add(totalUsersLabel.Parent);
            cardsPanel.Controls.Add(totalBorrowedLabel.Parent);
        }

        private Label CreateCard(string title)
        {
            Panel card = new Panel
            {
                Size = new Size(230 , 130) ,
                BackColor = Color.White ,
                BorderStyle = BorderStyle.FixedSingle ,
                Margin = new Padding(15)
            };

            Label titleLabel = new Label
            {
                Text = title ,
                Font = new Font("Segoe UI" , 11 , FontStyle.Bold) ,
                Location = new Point(15 , 20) ,
                AutoSize = true,
            };

            Label valueLabel = new Label
            {
                Text = "0" ,
                Font = new Font("Segoe UI" , 20 , FontStyle.Bold) ,
                ForeColor = Color.FromArgb(79 , 70 , 229) ,
                Location = new Point(15 , 60) ,
                AutoSize = true
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return valueLabel;
        }

        private void LoadDashboardData()
        {
            var repo = new AdminDashboardRepository();

            totalBooksLabel.Text = repo.GetTotalBooks().ToString();
            totalUsersLabel.Text = repo.GetTotalUsers().ToString();
            totalBorrowedLabel.Text = repo.GetTotalBorrowed().ToString();
        }


    }
}
