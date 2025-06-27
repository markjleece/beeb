// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Diagnostics;

namespace LevelEditor
{
    public partial class EditTileDialog : Form
    {
        internal Tile Tile;
        internal Palette Palette;

        internal EditTileDialog(Tile tile, Palette palette)
        {
            Tile = tile;
            Palette = palette;

            PrimaryColor = AppSettings.Instance.EditTilePrimaryColor;
            SecondaryColor = AppSettings.Instance.EditTileSecondaryColor;

            InitializeComponent();
            ClearOverlay();
            UpdateButtons();

            foreach (var name in Enum.GetNames<Tile.TileType>())
            {
                typeComboBox.Items.Add(name);
            }

            Tile.TileType[] enumValues = Enum.GetValues<Tile.TileType>();
            typeComboBox.SelectedIndex = Array.FindIndex<Tile.TileType>(enumValues, value => (value == Tile.Type));
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keys)
        {
            switch (keys)
            {
                case (Keys.Control | Keys.Z):
                    Undo();
                    return true;

                case (Keys.Control | Keys.Y):
                    Redo();
                    return true;

                default:
                    break;
            }

            return false; // not handled
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Tile.TileType[] enumValues = Enum.GetValues<Tile.TileType>();
            Tile.Type = enumValues[typeComboBox.SelectedIndex];

            AppSettings.Instance.EditTilePrimaryColor = PrimaryColor;
            AppSettings.Instance.EditTileSecondaryColor = SecondaryColor;
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            Undo();
        }

        private void Undo()
        { 
            if (UndoRedoHistory.CanUndo())
            {
                UndoRedoHistory.Undo();
                UpdateButtons();
                Invalidate(true);
            }
        }

        private void RedoButton_Click(object sender, EventArgs e)
        {
            Redo();
        }

        private void Redo()
        { 
            if (UndoRedoHistory.CanRedo())
            {
                UndoRedoHistory.Redo();
                UpdateButtons();
                Invalidate(true);
            }
        }

        private void ColorPickerPanel_MouseClick(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                Rectangle rect = GetColorPickerRect(i);
                if (rect.Contains(e.X, e.Y))
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        PrimaryColor = i;
                        rect = new Rectangle(0, 0, selectedColorPanel.Width / 2, selectedColorPanel.Height);
                        selectedColorPanel.Invalidate(rect);
                    }
                    else if (e.Button == MouseButtons.Right)
                    {
                        SecondaryColor = i;
                        rect = new Rectangle(selectedColorPanel.Width / 2, 0, selectedColorPanel.Width / 2, selectedColorPanel.Height);
                        selectedColorPanel.Invalidate(rect);
                    }
                    break;
                }
            }
        }

        private void ColorPickerPanel_Paint(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < 4; i++)
            {
                Brush brush = PaletteIndexToBrush(i);
                Rectangle rect = GetColorPickerRect(i);
                e.Graphics.FillRectangle(brush, rect);
            }

            for (int i = 1; i < 4; i++)
            {
                int y = i * colorPickerPanel.Height / 4;
                e.Graphics.DrawLine(Pens.Gray, 0, y, selectedColorPanel.Width, y);
            }
        }

        private Rectangle GetColorPickerRect(int i)
        {
            int top = i * colorPickerPanel.Height / 4;
            int bottom = (i + 1) * colorPickerPanel.Height / 4;
            return new Rectangle(0, top, selectedColorPanel.Width, bottom - top);
        }

        private void SelectedColorPanel_Paint(object sender, PaintEventArgs e)
        {
            int halfWidth = selectedColorPanel.Width / 2;

            Font font = SystemFonts.DialogFont;
            StringFormat format = new(StringFormatFlags.NoClip)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            Brush primaryBrush = PaletteIndexToBrush(PrimaryColor);
            Rectangle primaryRect = new(0, 0, halfWidth, selectedColorPanel.Height);
            e.Graphics.FillRectangle(primaryBrush, primaryRect);
            e.Graphics.DrawString("L", font, Brushes.Gray, primaryRect, format);

            Brush secondaryBrush = PaletteIndexToBrush(SecondaryColor);
            Rectangle secondaryRect = new(halfWidth, 0, halfWidth, selectedColorPanel.Height);
            e.Graphics.FillRectangle(secondaryBrush, secondaryRect);
            e.Graphics.DrawString("R", font, Brushes.Gray, secondaryRect, format);
        }

        private void PixelsPanel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clippingRect = e.ClipRectangle;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Rectangle rect = GetPixelPanelRect(x, y);

                    if (!clippingRect.IntersectsWith(rect)) continue;

                    int colorIndex;
                    if (OverlayPixels[x, y])
                    {
                        colorIndex = OverlayColorIndex;
                    }
                    else
                    {
                        colorIndex = Tile[x, y];
                    }

                    Brush brush = PaletteIndexToBrush(colorIndex);
                    e.Graphics.FillRectangle(brush, rect);
                }
            }

            if (FocusRect != Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(Pens.Gray, FocusRect.Left, FocusRect.Top, FocusRect.Width - 1, FocusRect.Height - 1);
            }
        }

        private void PixelsPanel_MouseDown(object sender, MouseEventArgs e)
        {
            ClearOverlay();

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Rectangle rect = GetPixelPanelRect(x, y);
                    if (rect.Contains(e.X, e.Y))
                    {
                        if (e.Button == MouseButtons.Left)
                        {
                            OverlayColorIndex = PrimaryColor;
                        }
                        else if (e.Button == MouseButtons.Right)
                        {
                            OverlayColorIndex = SecondaryColor;
                        }

                        if (OverlayColorIndex != -1)
                        {
                            OverlayStartPosition = new Point(x, y);
                            SetOverlayPixel(x, y, true/*set*/);
                            return;
                        }
                    }
                }
            }
        }

        private void PixelsPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (OverlayColorIndex != -1)
            {
                Point[] pixelCoords = GetOverlayPixelCoords();
                EditPixelOperation op = new(Tile, pixelCoords, OverlayColorIndex);
                UndoRedoHistory.Execute(op);
                {
                    UpdateButtons();
                    pixelsPanel.Invalidate();
                }

                ClearOverlay();
            }
        }

        private void PixelsPanel_MouseMove(object sender, MouseEventArgs e)
        {
            int pixelX = e.X * 16 / pixelsPanel.Width;
            int pixelY = e.Y * 16 / pixelsPanel.Height;

            Rectangle focusRect = GetPixelPanelRect(pixelX, pixelY);

            if (OverlayColorIndex != -1)
            {
                switch (_DrawMode)
                {
                    case DrawMode.Freeform:
                        SetOverlayPixel(pixelX, pixelY, true/*set*/);
                        break;

                    case DrawMode.Rectangle:
                        SetOverlayRectangle(pixelX, pixelY);
                        break;

                    case DrawMode.Line:
                        SetOverlayLine(pixelX, pixelY);
                        break;

                    default:
                        Debug.Assert(false);
                        break;
                }
            }

            if (focusRect != FocusRect)
            {
                if (FocusRect != Rectangle.Empty)
                {
                    pixelsPanel.Invalidate(FocusRect);
                }

                if (focusRect != Rectangle.Empty)
                {
                    pixelsPanel.Invalidate(focusRect);
                }

                FocusRect = focusRect;
            }
        }

        private void PixelsPanel_MouseEnter(object sender, EventArgs e)
        {
            if (FocusRect != Rectangle.Empty)
            {
                pixelsPanel.Invalidate(FocusRect);
            }

            FocusRect = Rectangle.Empty;
        }

        private void PixelsPanel_MouseLeave(object sender, EventArgs e)
        {
            if (FocusRect != Rectangle.Empty)
            {
                pixelsPanel.Invalidate(FocusRect);
            }

            FocusRect = Rectangle.Empty;
        }

        private Rectangle GetPixelPanelRect(int pixelX, int pixelY)
        {
            int left = pixelX * pixelsPanel.Width / 16;
            int top = pixelY * pixelsPanel.Height / 16;
            int right = (pixelX + 1) * pixelsPanel.Width / 16;
            int bottom = (pixelY + 1) * pixelsPanel.Height / 16;
            return new Rectangle(left, top, right - left, bottom - top);
        }

        private Brush PaletteIndexToBrush(int index)
        {
            return Palette[index] switch
            {
                0 => Brushes.Black,
                1 => Brushes.Red,
                2 => Brushes.Lime,
                3 => Brushes.Yellow,
                4 => Brushes.Blue,
                5 => Brushes.Magenta,
                6 => Brushes.Cyan,
                7 => Brushes.White,
                _ => Brushes.Black,
            };
        }

        private void SetOverlayPixel(int pixelX, int pixelY, bool value)
        {
            if (pixelX < 0 || pixelX > 15 || pixelY < 0 || pixelY > 15)
            {
                return; // clipped!
            }

            if (OverlayPixels[pixelX, pixelY] != value)
            {
                OverlayPixels[pixelX, pixelY] = value;

                Rectangle rect = GetPixelPanelRect(pixelX, pixelY);
                pixelsPanel.Invalidate(rect);
            }
        }

        private void SetOverlayRectangle(int pixelX, int pixelY)
        {
            Rectangle rect = new(
                Math.Min(pixelX, OverlayStartPosition.X),
                Math.Min(pixelY, OverlayStartPosition.Y),
                Math.Abs(pixelX - OverlayStartPosition.X) + 1,
                Math.Abs(pixelY - OverlayStartPosition.Y) + 1);

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool inside = rect.Contains(x, y);
                    if (OverlayPixels[x, y] != inside)
                    {
                        OverlayPixels[x, y] = inside;
                        Rectangle invalidateRect = GetPixelPanelRect(x, y);
                        pixelsPanel.Invalidate(invalidateRect);
                    }
                }
            }
        }

        private void SetOverlayLine(int pixelX, int pixelY)
        {
            // draw line onto canvas
            bool[,] canvas = new bool[16, 16];

            Point from = new(pixelX, pixelY);
            Point to = new(OverlayStartPosition.X, OverlayStartPosition.Y);

            int deltaX = Math.Abs(from.X - to.X);
            int deltaY = Math.Abs(from.Y - to.Y);

            if (deltaX == 0 && deltaY == 0) return;

            if (deltaX > deltaY)
            {
                if (from.X > to.X)
                {
                    (to, from) = (from, to);
                }

                deltaX = to.X - from.X;
                deltaY = to.Y - from.Y;

                double gradient = (double)deltaY / (double)deltaX;

                for (int x = 0; x <= deltaX; x++)
                {
                    int y = (int)Math.Round(gradient * (double)x);

                    int coordX = from.X + x;
                    int coordY = from.Y + y;

                    if (coordX < 0 || coordX > 15 || coordY < 0 || coordY > 15)
                    {
                        continue; // clipped!
                    }

                    canvas[coordX, coordY] = true;
                }
            }
            else
            {
                if (from.Y > to.Y)
                {
                    (to, from) = (from, to);
                }

                deltaX = to.X - from.X;
                deltaY = to.Y - from.Y;

                double gradient = (double)deltaX / (double)deltaY;

                for (int y = 0; y <= deltaY; y++)
                {
                    int x = (int)Math.Round(gradient * (double)y);

                    int coordX = from.X + x;
                    int coordY = from.Y + y;

                    if (coordX < 0 || coordX > 15 || coordY < 0 || coordY > 15)
                    {
                        continue; // clipped!
                    }

                    canvas[coordX, coordY] = true;
                }
            }

            // update overlay
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    bool canvasPixel = canvas[x, y];
                    if (OverlayPixels[x, y] != canvasPixel)
                    {
                        OverlayPixels[x, y] = canvasPixel;
                        Rectangle invalidateRect = GetPixelPanelRect(x, y);
                        pixelsPanel.Invalidate(invalidateRect);
                    }
                }
            }
        }

        private void ClearOverlay()
        {
            OverlayColorIndex = -1;

            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    SetOverlayPixel(x, y, false);
                }
            }
        }

        private Point[] GetOverlayPixelCoords()
        {
            int pixelCount = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (OverlayPixels[x, y])
                    {
                        pixelCount++;
                    }
                }
            }

            Point[] coords = new Point[pixelCount];
            int index = 0;
            for (int y = 0; y < 16; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    if (OverlayPixels[x, y])
                    {
                        coords[index++] = new Point(x, y);
                    }
                }
            }

            return coords;
        }

        private void UpdateButtons()
        {
            undoButton.Enabled = UndoRedoHistory.CanUndo();
            redoButton.Enabled = UndoRedoHistory.CanRedo();

            freeformModeButton.Checked = (_DrawMode == DrawMode.Freeform);
            lineModeButton.Checked = (_DrawMode == DrawMode.Line);
            rectModeButton.Checked = (_DrawMode == DrawMode.Rectangle);
        }

        private void FreeformModeButton_Click(object sender, EventArgs e)
        {
            _DrawMode = DrawMode.Freeform;
            UpdateButtons();
        }

        private void LineModeButton_Click(object sender, EventArgs e)
        {
            _DrawMode = DrawMode.Line;
            UpdateButtons();
        }

        private void RectModeButton_Click(object sender, EventArgs e)
        {
            _DrawMode = DrawMode.Rectangle;
            UpdateButtons();
        }

        private DrawMode _DrawMode = DrawMode.Freeform;

        private int OverlayColorIndex = -1;
        private readonly bool[,] OverlayPixels = new bool[16, 16];
        private Point OverlayStartPosition = new();

        private int PrimaryColor = 0;
        private int SecondaryColor = 0;
        private Rectangle FocusRect = Rectangle.Empty;
        private readonly UndoRedoHistory UndoRedoHistory = new();

        private enum DrawMode
        {
            Freeform,
            Line,
            Rectangle
        }
    }
}
