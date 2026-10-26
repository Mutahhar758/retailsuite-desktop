using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace ERP.Classes
{
    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuRenderer() : base(new ModernMenuColorTable())
        {
            RoundedEdges = false;
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.IsOnDropDown)
            {
                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle rect = new Rectangle(2, 0, e.Item.Width - 4, e.Item.Height);
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(239, 246, 255))) // Tailwind Blue-50
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                    using (SolidBrush barBrush = new SolidBrush(Color.FromArgb(37, 99, 235))) // Tailwind Blue-600 indicator
                    {
                        e.Graphics.FillRectangle(barBrush, new Rectangle(2, 2, 3, e.Item.Height - 4));
                    }
                }
            }
            else
            {
                // Top-level menu bar item
                if (e.Item.Selected || e.Item.Pressed)
                {
                    Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(30, 41, 59))) // Slate-800
                    {
                        e.Graphics.FillRectangle(brush, rect);
                    }
                    using (SolidBrush barBrush = new SolidBrush(Color.FromArgb(56, 189, 248))) // Sky-400 accent
                    {
                        e.Graphics.FillRectangle(barBrush, new Rectangle(0, e.Item.Height - 3, e.Item.Width, 3));
                    }
                }
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            int y = e.Item.Height / 2;
            using (Pen pen = new Pen(Color.FromArgb(226, 232, 240))) // Slate-200
            {
                e.Graphics.DrawLine(pen, 28, y, e.Item.Width - 4, y);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (e.Item.IsOnDropDown)
            {
                e.TextColor = (e.Item.Selected || e.Item.Pressed)
                    ? Color.FromArgb(29, 78, 216)  // Tailwind Blue-700
                    : Color.FromArgb(15, 23, 42);  // Slate-900
            }
            else
            {
                e.TextColor = (e.Item.Selected || e.Item.Pressed)
                    ? Color.FromArgb(56, 189, 248) // Sky-400
                    : Color.FromArgb(248, 250, 252); // Slate-50
            }
            base.OnRenderItemText(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            Rectangle rect = new Rectangle(0, 0, 28, e.ToolStrip.Height);
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(248, 250, 252))) // Slate-50 gutter
            {
                e.Graphics.FillRectangle(brush, rect);
            }
            using (Pen pen = new Pen(Color.FromArgb(226, 232, 240)))
            {
                e.Graphics.DrawLine(pen, 28, 0, 28, e.ToolStrip.Height);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is ToolStripDropDown)
            {
                using (Pen pen = new Pen(Color.FromArgb(203, 213, 225))) // Slate-300
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, e.ToolStrip.Width - 1, e.ToolStrip.Height - 1);
                }
            }
            else
            {
                // Bottom border on main menu strip
                using (Pen pen = new Pen(Color.FromArgb(30, 41, 59))) // Slate-800
                {
                    e.Graphics.DrawLine(pen, 0, e.ToolStrip.Height - 1, e.ToolStrip.Width, e.ToolStrip.Height - 1);
                }
            }
        }
    }

    public class ModernMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => Color.FromArgb(15, 23, 42); // Slate-900
        public override Color MenuStripGradientEnd => Color.FromArgb(15, 23, 42);
        public override Color ToolStripDropDownBackground => Color.White;
        public override Color ImageMarginGradientBegin => Color.FromArgb(248, 250, 252);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(248, 250, 252);
        public override Color ImageMarginGradientEnd => Color.FromArgb(248, 250, 252);
        public override Color MenuBorder => Color.FromArgb(203, 213, 225);
        public override Color MenuItemBorder => Color.Transparent;
        public override Color MenuItemSelected => Color.FromArgb(239, 246, 255);
        public override Color MenuItemSelectedGradientBegin => Color.FromArgb(239, 246, 255);
        public override Color MenuItemSelectedGradientEnd => Color.FromArgb(239, 246, 255);
        public override Color MenuItemPressedGradientBegin => Color.FromArgb(30, 41, 59);
        public override Color MenuItemPressedGradientEnd => Color.FromArgb(30, 41, 59);
        public override Color StatusStripGradientBegin => Color.FromArgb(15, 23, 42);
        public override Color StatusStripGradientEnd => Color.FromArgb(15, 23, 42);
        public override Color SeparatorDark => Color.FromArgb(226, 232, 240);
        public override Color SeparatorLight => Color.Transparent;
    }
}
