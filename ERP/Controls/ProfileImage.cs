using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ERP.Controls
{
    public partial class ProfileImage : UserControl
    {
        public ProfileImage()
        {
            InitializeComponent();
        }
        public Image  Image { get; set; }
        private void ProfileImage_Load(object sender, EventArgs e)
        {

        }
    }
}
