// This file is Copyright © 2025 - Mark John Leece - All rights reserved
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
