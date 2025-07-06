// --------------------------------------------------------------
// An Adventure In Time - A Doctor Who fan game for the BBC Micro
// Model B
//
// Copyright (C) 2025  Mark John Leece
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public
// License along with this program; if not, write to the Free
// Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
// Boston, MA  02110-1301, USA.
// --------------------------------------------------------------

namespace LevelEditor.ux
{
    public partial class EditPaletteDialog : Form
    {
        internal EditPaletteDialog(Palette palette)
        {
            Palette = palette;
            InitializeComponent();
        }

        private void EditPaletteDialog_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                colorSelector_0.Items.Add(i);
                colorSelector_1.Items.Add(i);
                colorSelector_2.Items.Add(i);
                colorSelector_3.Items.Add(i);
            }

            colorSelector_0.SelectedIndex = Palette[0];
            colorSelector_1.SelectedIndex = Palette[1];
            colorSelector_2.SelectedIndex = Palette[2];
            colorSelector_3.SelectedIndex = Palette[3];
        }

        private void ColorSelector_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            Brush myBrush = Brushes.Black;
            switch (e.Index)
            {
                case 0:
                    myBrush = Brushes.Black;
                    break;
                case 1:
                    myBrush = Brushes.Red;
                    break;
                case 2:
                    myBrush = Brushes.Lime;
                    break;
                case 3:
                    myBrush = Brushes.Yellow;
                    break;
                case 4:
                    myBrush = Brushes.Blue;
                    break;
                case 5:
                    myBrush = Brushes.Magenta;
                    break;
                case 6:
                    myBrush = Brushes.Cyan;
                    break;
                case 7:
                    myBrush = Brushes.White;
                    break;
            }

            e.Graphics.FillRectangle(myBrush, e.Bounds);
            e.DrawFocusRectangle();
        }

        private void OkayButton_Click(object sender, EventArgs e)
        {
            Palette[0] = (byte)colorSelector_0.SelectedIndex;
            Palette[1] = (byte)colorSelector_1.SelectedIndex;
            Palette[2] = (byte)colorSelector_2.SelectedIndex;
            Palette[3] = (byte)colorSelector_3.SelectedIndex;
        }

        private readonly Palette Palette;
    }
}
