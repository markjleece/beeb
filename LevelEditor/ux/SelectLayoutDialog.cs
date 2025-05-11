// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LevelEditor.ux
{
    public partial class SelectLayoutDialog : Form
    {
        internal SelectLayoutDialog(int layoutWidth, int layoutHeight)
        {
            LayoutWidth = layoutWidth;
            LayoutHeight = layoutHeight;

            SelectedLayoutIndex = (LayoutWidth / 16) switch
            {
                8 => 0,
                4 => 1,
                2 => 2,
                1 => 3,
                _ => 0,
            };

            InitializeComponent();

        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            switch (SelectedLayoutIndex)
            {
                default:
                case 0:
                    LayoutWidth = 8 * 16;
                    LayoutHeight = 1 * 16;
                    break;

                case 1:
                    LayoutWidth = 4 * 16;
                    LayoutHeight = 2 * 16;
                    break;

                case 2:
                    LayoutWidth = 2 * 16;
                    LayoutHeight = 4 * 16;
                    break;

                case 3:
                    LayoutWidth = 1 * 16;
                    LayoutHeight = 8 * 16;
                    break;
            }
        }

        private void LayoutSelectorPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int index = LayoutSelectedPanel_HitTest(e.X, e.Y);
            if (index != -1 && index != SelectedLayoutIndex)
            {
                Rectangle rect = GetLayoutSelectorPanelRect(SelectedLayoutIndex);
                layoutSelectorPanel.Invalidate(rect);

                rect = GetLayoutSelectorPanelRect(index);
                layoutSelectorPanel.Invalidate(rect);

                SelectedLayoutIndex = index;
            }
        }

        private void LayoutSelectorPanel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clippingRect = e.ClipRectangle;

            for (int i = 0; i < 4; i++)
            {
                Rectangle bounds = GetLayoutSelectorPanelRect(i);
                if (!clippingRect.IntersectsWith(bounds)) continue;

                // draw page layout
                double[,] pageCoords = i switch
                {
                    1 => new double[,] {
                            { -2.0, -1.0 }, { -1.0, -1.0 }, { 0.0, -1.0 }, { 1.0, -1.0 },
                            { -2.0,  0.0 }, { -1.0,  0.0 }, { 0.0,  0.0 }, { 1.0,  0.0 } },
                    2 => new double[,] {
                            { -1.0, -2.0 }, { -1.0, -1.0 }, { -1.0, 0.0 }, { -1.0, 1.0 },
                            {  0.0, -2.0 }, {  0.0, -1.0 }, {  0.0, 0.0 }, {  0.0, 1.0 } },
                    3 => new double[,] {
                            { -0.5, -4.0 }, { -0.5, -3.0 }, { -0.5, -2.0 }, { -0.5, -1.0 },
                            { -0.5,  0.0 }, { -0.5,  1.0 }, { -0.5,  2.0 }, { -0.5,  3.0 } },
                    _ => new double[,] {
                            { -4.0, -0.5 }, { -3.0, -0.5 }, { -2.0, -0.5 }, { -1.0, -0.5 },
                            {  0.0, -0.5 }, {  1.0, -0.5 }, {  2.0, -0.5 }, {  3.0, -0.5 } },
                };
                LayoutSelectorDrawLayout(bounds, pageCoords, e.Graphics);

                // draw selection border
                if (i == SelectedLayoutIndex)
                {
                    Pen pen = new(Color.Gray, 4);
                    bounds.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(pen, bounds);
                }

                // draw border
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Top), new Point(bounds.Left, bounds.Bottom));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Bottom));
            }
        }

        private static void LayoutSelectorDrawLayout(Rectangle bounds, double[,] pageCoords, Graphics graphics)
        {
            // find max value
            double maxValue = 0.0;
            for (int i = 0; i < 8; i++)
            {
                maxValue = Math.Max(maxValue, Math.Abs(pageCoords[i, 0]));
                maxValue = Math.Max(maxValue, Math.Abs(pageCoords[i, 1]));
            }

            // draw page rects
            bounds.Inflate(-10, -10);

            double scale = 0.5 * bounds.Width / maxValue;

            Point boundsCenter = new(
                (bounds.Left + bounds.Right) / 2, (bounds.Top + bounds.Bottom) / 2);

            for (int i = 0; i < 8; i++)
            {
                Point topLeft = new(
                    boundsCenter.X + (int)(scale * pageCoords[i, 0]),
                    boundsCenter.Y + (int)(scale * pageCoords[i, 1]));

                Point bottomRight = new(
                    boundsCenter.X + (int)(scale * (pageCoords[i, 0] + 1.0)),
                    boundsCenter.Y + (int)(scale * (pageCoords[i, 1] + 1.0)));

                Rectangle pageRectangle = new(
                    topLeft.X, topLeft.Y, (bottomRight.X - topLeft.X), (bottomRight.Y - topLeft.Y));

                graphics.FillRectangle(Brushes.DarkGray, pageRectangle);
                graphics.DrawRectangle(Pens.Gray, pageRectangle);
            }
        }

        private static int LayoutSelectedPanel_HitTest(int x, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                Rectangle bounds = GetLayoutSelectorPanelRect(i);
                if (bounds.Contains(x, y))
                {
                    return i;
                }
            }

            return -1;
        }

        private static Rectangle GetLayoutSelectorPanelRect(int index)
        {
            return new Rectangle(index * LayoutItemSize, 0, LayoutItemSize, LayoutItemSize);
        }

        private int SelectedLayoutIndex;

        internal int LayoutWidth;
        internal int LayoutHeight;

        private const int LayoutItemSize = 192;
    }
}
