// This file is Copyright © 2025 - Mark John Leece - All rights reserved
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.XPath;

namespace LevelEditor
{
    public partial class LevelEditorForm : Form
    {
        public LevelEditorForm()
        {
            InitializeComponent();
        }

        private void LevelEditorForm_Load(object sender, EventArgs e)
        {
            string filePathName = AppSettings.Instance.RecentFilePathName;
            if (filePathName != string.Empty && File.Exists(filePathName))
            {
                Level? level = Level.Load(filePathName); ;
                if (level != null)
                {
                    LevelData = level;
                }
            }

            ResetState(true/*reset view*/);

            // load object tile bitmaps
            ObjectBitmaps[0] = LevelEditor.Properties.Resources.K9LookLeft;
            ObjectBitmaps[1] = LevelEditor.Properties.Resources.K9LookRight;
            ObjectBitmaps[2] = LevelEditor.Properties.Resources.EnemyLookLeft;
            ObjectBitmaps[3] = LevelEditor.Properties.Resources.EnemyLookRight;
            ObjectBitmaps[4] = LevelEditor.Properties.Resources.EnemyMoveLeft;
            ObjectBitmaps[5] = LevelEditor.Properties.Resources.EnemyMoveRight;
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

                case (Keys.Control | Keys.S):
                    Save();
                    return true;

                case (Keys.Control | Keys.Oemplus):
                    ZoomIn();
                    return true;

                case (Keys.Control | Keys.OemMinus):
                    ZoomOut();
                    return true;

                default:
                    return false; // not handled
            }
        }

        private bool HandleUnsavedChanges()
        {
            if (UndoRedoHistory.HasUnsavedChanges())
            {
                if (MessageBox.Show(
                        "Are you sure you want to proceed?\nYou will lose changes if you do",
                        "You have unsaved changes",
                        MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        private void LevelEditorForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!HandleUnsavedChanges())
            {
                e.Cancel = true;
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
            {
                return;
            }

            NewLevelOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(true/*resetView*/);
            }
        }

        private void OpenButton_Click(object sender, EventArgs e)
        {
            if (!HandleUnsavedChanges())
            {
                return;
            }

            OpenLevelOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(true/*resetView*/);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            SaveLevelOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(false/*resetView*/);
            }
        }

        private void SaveAsButton_Click(object sender, EventArgs e)
        {
            SaveAsLevelOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(false/*resetView*/);
            }
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
                ResetState(false/*resetView*/);
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
                ResetState(false/*resetView*/);
            }
        }

        private void EditSettingsButton_Click(object sender, EventArgs e)
        {
            EditSettingsOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(true/*resetView*/);
            }
        }

        private void SelectLayoutButton_Click(object sender, EventArgs e)
        {
            EditLayoutOperation op = new(LevelData);
            UndoRedoHistory.Execute(op);
        }

        private void PaletteButton_Click(object sender, EventArgs e)
        {
            EditPaletteOperation op = new(LevelData);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(false/*resetView*/);
            }
        }

        private void EditTileButton_Click(object sender, EventArgs e)
        {
            EditTile(SelectedTileIndex);
        }

        private void TileSelectorPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int tileIndex = TileSelectedPanel_HitTest(e.X, e.Y);
            if (tileIndex != -1)
            {
                EditTile(tileIndex);
            }
        }

        private void EditTile(int tileIndex)
        {
            EditTileOperation op = new(LevelData, tileIndex);
            if (UndoRedoHistory.Execute(op))
            {
                ResetState(false/*reset view*/);
            }
        }

        private void LevelEditorForm_Resize(object sender, EventArgs e)
        {
            int borderWidth = (Width - ClientSize.Width) / 2;
            tileGridPanel.Width = ClientSize.Width - tileGridPanel.Left - borderWidth;
            tileGridPanel.Height = ClientSize.Height - tileGridPanel.Top - borderWidth;
        }

        private void TileSelectorPanel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clippingRect = e.ClipRectangle;

            // draw tiles...
            for (int i = 0; i < TileSelectorCount; i++)
            {
                Rectangle bounds = GetTileSelectorRect(i);
                if (!clippingRect.IntersectsWith(bounds)) continue;

                CachedBitmap cachedBitmap = GetTileSelectorBitmap(i, e.Graphics);
                e.Graphics.DrawCachedBitmap(cachedBitmap, bounds.Left, bounds.Top);

                if (i == SelectedTileIndex)
                {
                    Pen pen = new(Color.Gray, 4);
                    bounds.Inflate(-2, -2);
                    e.Graphics.DrawRectangle(pen, bounds);
                }
            }

            // draw divider
            Rectangle divider = new(0, 16 * (TileSelectorTileSize + 1), tileSelectorPanel.Width, 5);
            if (clippingRect.IntersectsWith(divider))
            {
                e.Graphics.FillRectangle(new SolidBrush(SystemColors.Window), divider);
            }

            // draw borders
            for (int i = 0; i <= TileSelectorCount; i++)
            {
                Rectangle bounds = GetTileSelectorRect(i);
                if (!clippingRect.IntersectsWith(bounds)) continue;

                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Top), new Point(bounds.Right, bounds.Top));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Top), new Point(bounds.Left, bounds.Bottom));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Right, bounds.Top), new Point(bounds.Right, bounds.Bottom));
                e.Graphics.DrawLine(Pens.Gray, new Point(bounds.Left, bounds.Bottom), new Point(bounds.Right, bounds.Bottom));
            }
        }

        private int TileSelectedPanel_HitTest(int x, int y)
        {
            for (int i = 0; i < TileSelectorCount; i++)
            {
                Rectangle bounds = GetTileSelectorRect(i);
                if (bounds.Contains(x, y))
                {
                    return i;
                }
            }

            return -1;
        }

        private void TileSelectorPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int tileIndex = TileSelectedPanel_HitTest(e.X, e.Y);
            if (tileIndex != -1 && tileIndex != SelectedTileIndex)
            {
                bool updateButtons = ((tileIndex < Level.TileCount) != (SelectedTileIndex < Level.TileCount));

                Rectangle rect = GetTileSelectorRect(SelectedTileIndex);
                tileSelectorPanel.Invalidate(rect);

                rect = GetTileSelectorRect(tileIndex);
                tileSelectorPanel.Invalidate(rect);

                SelectedTileIndex = tileIndex;

                if (updateButtons)
                {
                    UpdateButtons();
                }
            }
        }

        private CachedBitmap GetTileSelectorBitmap(int tileIndex, Graphics graphics)
        {
            if (CachedTileSelectorBitmaps[tileIndex] != null)
            {
                return CachedTileSelectorBitmaps[tileIndex]; // return cached bitmap
            }

            // regenerate bitmaps
            for (int i = 0; i < Level.TileCount; i++)
            {
                CachedTileSelectorBitmaps[i] = GenerateCachedBitmap(
                    LevelData.Tiles[i],
                    TileSelectorTileSize,
                    TileSelectorTileSize,
                    graphics);
            }

            for (int i = 0; i < 6; i++)
            {
                CachedTileSelectorBitmaps[i + Level.TileCount] = GenerateCachedObjectBitmap(
                    i,
                    TileSelectorTileSize,
                    TileSelectorTileSize,
                    graphics);
            }

            return CachedTileSelectorBitmaps[tileIndex];
        }

        private Rectangle GetTileSelectorRect(int iTile)
        {
            int left = (iTile % 2) * (TileSelectorTileSize + 1);
            int top = (iTile / 2) * (TileSelectorTileSize + 1);

            if (iTile >= Level.TileCount)
            {
                top += 5; // separate object tiles
            }

            return new Rectangle(left, top, TileSelectorTileSize, TileSelectorTileSize);
        }

        private void TileGridPanel_Paint(object sender, PaintEventArgs e)
        {
            Rectangle clippingRect = e.ClipRectangle;

            var channelSize = TileGridTileSize / 3;
            var channelOffset = TileGridTileSize / 12;
            var channelFont = new Font(FontFamily.GenericSansSerif, channelSize, FontStyle.Bold, GraphicsUnit.Pixel);

            // draw tiles
            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    Rectangle bounds = new(x * TileGridTileSize, y * TileGridTileSize, TileGridTileSize, TileGridTileSize);
                    bounds.Offset(tileGridPanel.AutoScrollPosition);

                    if (!clippingRect.IntersectsWith(bounds)) continue;

                    // draw tile
                    int tileIndex;
                    if (OverlayPixels[x, y])
                    {
                        tileIndex = EraseMode ? 0 : SelectedTileIndex;
                    }
                    else
                    {
                        tileIndex = LevelData.TileGrid[x, y];
                    }

                    CachedBitmap cachedBitmap = GetTileGridCachedBitmap(tileIndex, e.Graphics);
                    e.Graphics.DrawCachedBitmap(cachedBitmap, bounds.Left, bounds.Top);

                    // draw teleport tag
                    char? teleport = LevelData.TeleportGrid[x, y];
                    if (teleport != null)
                    {
                        DrawTag((char)teleport, bounds, channelSize, channelOffset, channelFont, e);
                    }

                    // draw switch-pairing tag
                    char? switchPairing = LevelData.SwitchGrid[x, y];
                    if (switchPairing != null)
                    {
                        DrawTag((char)switchPairing, bounds, channelSize, channelOffset, channelFont, e);
                    }
                }
            }

            // draw page boundaries
            for (int i = 1; i < LevelData.TileGrid.Width; i++)
            {
                int x = (i * TileGridTileSize * 16) + tileGridPanel.AutoScrollPosition.X;
                e.Graphics.DrawLine(Pens.Gray, x, 0, x, tileGridPanel.Height);
            }

            for (int i = 1; i < LevelData.TileGrid.Height; i++)
            {
                int y = (i * TileGridTileSize * 16) + tileGridPanel.AutoScrollPosition.Y;
                e.Graphics.DrawLine(Pens.Gray, 0, y, tileGridPanel.Width, y);
            }

            // draw focus rect
            if (FocusRect != Rectangle.Empty)
            {
                e.Graphics.DrawRectangle(Pens.Gray, FocusRect.Left, FocusRect.Top, FocusRect.Width - 1, FocusRect.Height - 1);
            }

            // draw surround
            Point pageGridExtents = new(
                LevelData.TileGrid.Width * TileGridTileSize,
                LevelData.TileGrid.Height * TileGridTileSize);

            if (pageGridExtents.X < Width)
            {
                e.Graphics.FillRectangle(Brushes.Gray, pageGridExtents.X, 0, Width - pageGridExtents.X, pageGridExtents.Y);
            }

            if (pageGridExtents.Y < Height)
            {
                e.Graphics.FillRectangle(Brushes.Gray, 0, pageGridExtents.Y, pageGridExtents.X, Height - pageGridExtents.Y);
            }

            if (pageGridExtents.X < Width && pageGridExtents.Y < Height)
            {
                e.Graphics.FillRectangle(Brushes.Gray, pageGridExtents.X, pageGridExtents.Y, Width - pageGridExtents.X, Height - pageGridExtents.Y);
            }
        }

        private static void DrawTag(char ch, Rectangle bounds, int channelSize, int channelOffset, Font channelFont, PaintEventArgs e)
        {
            var channelRect = new Rectangle(
                bounds.Left + channelOffset,
                bounds.Top + channelOffset,
                channelSize, channelSize);

            e.Graphics.FillRectangle(Brushes.White, channelRect);
            e.Graphics.DrawString($"{ch}", channelFont, Brushes.Black, channelRect);
        }

        private void TileGridPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
            }
            
            EraseMode = (e.Button == MouseButtons.Right);

            int clickX = -tileGridPanel.AutoScrollPosition.X + e.X;
            int clickY = -tileGridPanel.AutoScrollPosition.Y + e.Y;

            int tileX = clickX / TileGridTileSize;
            int tileY = clickY / TileGridTileSize;

            if (tileX >= 0 && tileX < LevelData.TileGrid.Width &&
                tileY >= 0 && tileY < LevelData.TileGrid.Height)
            {
                OverlayActive = true;
                OverlayStartPosition = new Point(tileX, tileY);
                SetOverlayTile(tileX, tileY, true/*set*/);
            }
        }

        private void TileGridPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (OverlayActive)
            {
                Point[] tileCoords = GetOverlayTileCoords();
                int tileIndex = EraseMode ? 0 : SelectedTileIndex;

                EditTileGridOperation op = new(LevelData, tileCoords, tileIndex);
                UndoRedoHistory.Execute(op);
                {
                    UpdateButtons();
                }

                ClearOverlay();
            }
        }

        private void TileGridPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (SelectedTileIndex >= Level.TileCount)
            {
                return; // object tile!
            }

            int clickX = -tileGridPanel.AutoScrollPosition.X + e.X;
            int clickY = -tileGridPanel.AutoScrollPosition.Y + e.Y;

            int tileX = clickX / TileGridTileSize;
            int tileY = clickY / TileGridTileSize;

            Rectangle focusRect = GetTileGridRect(tileX, tileY);
            if (OverlayActive)
            {
                switch (_DrawMode)
                {
                    case DrawMode.Freeform:
                        SetOverlayTile(tileX, tileY, true/*set*/);
                        break;

                    case DrawMode.Rectangle:
                        SetOverlayRectangle(tileX, tileY);
                        break;

                    case DrawMode.Line:
                        SetOverlayLine(tileX, tileY);
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
                    tileGridPanel.Invalidate(FocusRect);
                }

                if (focusRect != Rectangle.Empty)
                {
                    tileGridPanel.Invalidate(focusRect);
                }

                FocusRect = focusRect;
            }
        }

        private void TileGridPanel_MouseEnter(object sender, EventArgs e)
        {
            if (FocusRect != Rectangle.Empty)
            {
                tileGridPanel.Invalidate(FocusRect);
            }

            FocusRect = Rectangle.Empty;
        }

        private void TileGridPanel_MouseLeave(object sender, EventArgs e)
        {
            if (FocusRect != Rectangle.Empty)
            {
                tileGridPanel.Invalidate(FocusRect);
            }

            FocusRect = Rectangle.Empty;
        }

        private void ZoomInButton_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void ZoomOutButton_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void TileGridPanel_MouseWheel(object? sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                const int WHEEL_DELTA = 120;
                double detentCount = e.Delta / WHEEL_DELTA;

                // calculate new tile size
                double scaleFactor = 1.0;
                if (detentCount > 0)
                {
                    while (detentCount-- > 0) scaleFactor *= 1.2;
                }
                else if (detentCount < 0)
                {
                    while (detentCount++ < 0) scaleFactor *= 0.8;
                }

                Zoom(e.X/*focusX*/, e.Y/*focusY*/, scaleFactor);
            }
            else if (FocusRect != Rectangle.Empty)
            {
                // invalidate vertical strip to ensure focus rect is drawn correctly
                Rectangle rect = new(FocusRect.Left, 0, FocusRect.Width, tileGridPanel.Height);
                tileGridPanel.Invalidate(rect);
            }
        }
        private void ZoomIn()
        {
            int focusX = tileGridPanel.Width / 2;
            int focusY = tileGridPanel.Height / 2;
            Zoom(focusX, focusY, 1.2/*scaleFactor*/);
        }

        private void ZoomOut()
        {
            int focusX = tileGridPanel.Width / 2;
            int focusY = tileGridPanel.Height / 2;
            Zoom(focusX, focusY, 0.8/*scaleFactor*/);
        }

        private void Zoom(int focusX, int focusY, double scaleFactor)
        {
            // scale tile size
            TileGridTileSize = Math.Clamp((int)(scaleFactor * TileGridTileSize), 16, 512);

            // calculate new extents
            Size newExtents = new(
                TileGridTileSize * LevelData.TileGrid.Width,
                TileGridTileSize * LevelData.TileGrid.Height);

            // calculate new scroll position
            Size oldExtents = tileGridPanel.AutoScrollMinSize;

            float scaleFactorX = (float)newExtents.Width / (float)oldExtents.Width;
            float scaleFactorY = (float)newExtents.Height / (float)oldExtents.Height;

            Point oldScrollPosition = new(
                -tileGridPanel.AutoScrollPosition.X,
                -tileGridPanel.AutoScrollPosition.Y);

            Point scaleCenter = new(
                oldScrollPosition.X + focusX,
                oldScrollPosition.Y + focusY);

            Matrix transform = new();
            transform.Translate(-scaleCenter.X, -scaleCenter.Y);
            transform.Scale(scaleFactorX, scaleFactorY);
            transform.Translate(scaleCenter.X, scaleCenter.Y);

            Point[] points = [new(0, 0)];
            transform.TransformPoints(points); // transform origin

            Point newScrollPosition = new(
                oldScrollPosition.X + points[0].X,
                oldScrollPosition.Y + points[0].Y);

            newScrollPosition.X = Math.Clamp(newScrollPosition.X, 0, Math.Max(newExtents.Width - Width, 0));
            newScrollPosition.Y = Math.Clamp(newScrollPosition.Y, 0, Math.Max(newExtents.Height - Height, 0));

            // clear cache, focus rect, and invalidate control
            CachedTileGridBitmaps = new CachedBitmap[TileSelectorCount];
            FocusRect = Rectangle.Empty;
            tileGridPanel.Invalidate();

            // apply changes
            tileGridPanel.AutoScrollMinSize = newExtents;
            tileGridPanel.AutoScrollPosition = newScrollPosition;
        }

        private Rectangle GetTileGridRect(int tileX, int tileY)
        {
            Rectangle rect = new(tileX * TileGridTileSize, tileY * TileGridTileSize, TileGridTileSize, TileGridTileSize);
            rect.Offset(tileGridPanel.AutoScrollPosition.X, tileGridPanel.AutoScrollPosition.Y);
            return rect;
        }

        private CachedBitmap GetTileGridCachedBitmap(int tileIndex, Graphics graphics)
        {
            if (CachedTileGridBitmaps[tileIndex] != null)
            {
                return CachedTileGridBitmaps[tileIndex]; // return cached bitmap
            }

            // regenerate bitmaps
            for (int i = 0; i < Level.TileCount; i++)
            {
                CachedTileGridBitmaps[i] = GenerateCachedBitmap(
                    LevelData.Tiles[i],
                    TileGridTileSize,
                    TileGridTileSize,
                    graphics);
            }

            for (int i = 0; i < 6; i++)
            {
                CachedTileGridBitmaps[i + Level.TileCount] = GenerateCachedObjectBitmap(
                    i,
                    TileGridTileSize,
                    TileGridTileSize,
                    graphics);
            }

            return CachedTileGridBitmaps[tileIndex];
        }

        CachedBitmap GenerateCachedBitmap(Tile tile, int width, int height, Graphics graphics)
        {
            // create 8bpp indexed bitmap, scaled to width / height
            uint[] palette = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                palette[i] = PaletteIndexToARGB(i);
            }

            byte[] bytes = tile.GetScaledARGB(width, height, palette);

            GCHandle gch = GCHandle.Alloc(bytes); // lock bytes

            Bitmap bitmap = new(
                width, height, bytes.Length / height/*stride*/,
                PixelFormat.Format32bppArgb,
                Marshal.UnsafeAddrOfPinnedArrayElement(bytes, 0));

            // create cached bitmap, for faster rendering
            CachedBitmap cachedBitmap = new(bitmap, graphics);

            // clean up
            bitmap.Dispose();
            gch.Free(); // unlock bytes

            return cachedBitmap;
        }

        CachedBitmap GenerateCachedObjectBitmap(int index, int width, int height, Graphics graphics)
        {
            Bitmap scaledBitmap = new(ObjectBitmaps[index], width, height);
            return new CachedBitmap(scaledBitmap, graphics);
        }

        private uint PaletteIndexToARGB(int index)
        {
            return LevelData.Palette[index] switch
            {
                0 => 0xFF000000, // black
                1 => 0xFFFF0000, // red
                2 => 0xFF00FF00, // green
                3 => 0xFFFFFF00, // yellow
                4 => 0xFF0000FF, // blue
                5 => 0xFFFF00FF, // magenta
                6 => 0xFF00FFFF, // cyan
                7 => 0xFFFFFFFF, // white
                _ => 0xFF000000  // default
            };
        }

        private Rectangle GetOverlayRect(int tileX, int tileY)
        {
            Rectangle rect = new(
                tileX * TileGridTileSize,
                tileY * TileGridTileSize,
                TileGridTileSize,
                TileGridTileSize);
            rect.Offset(tileGridPanel.AutoScrollPosition.X, tileGridPanel.AutoScrollPosition.Y);
            return rect;
        }

        private void SetOverlayTile(int tileX, int tileY, bool value)
        {
            if (tileX < 0 || tileX >= LevelData.TileGrid.Width || tileY < 0 || tileY >= LevelData.TileGrid.Height)
            {
                return; // clipped!
            }

            if (OverlayPixels[tileX, tileY] != value)
            {
                OverlayPixels[tileX, tileY] = value;

                Rectangle rect = GetOverlayRect(tileX, tileY);
                tileGridPanel.Invalidate(rect);
            }
        }

        private void SetOverlayRectangle(int tileX, int tileY)
        {
            Rectangle rect = new(
                Math.Min(tileX, OverlayStartPosition.X),
                Math.Min(tileY, OverlayStartPosition.Y),
                Math.Abs(tileX - OverlayStartPosition.X) + 1,
                Math.Abs(tileY - OverlayStartPosition.Y) + 1);

            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    bool inside = rect.Contains(x, y);
                    if (OverlayPixels[x, y] != inside)
                    {
                        OverlayPixels[x, y] = inside;
                        Rectangle invalidateRect = GetOverlayRect(x, y);
                        tileGridPanel.Invalidate(invalidateRect);
                    }
                }
            }
        }

        private void SetOverlayLine(int tileX, int tileY)
        {
            // draw line onto canvas
            bool[,] canvas = new bool[ LevelData.TileGrid.Width, LevelData.TileGrid.Height ];

            Point from = new(tileX, tileY);
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

                    if (coordX < 0 || coordX >= LevelData.TileGrid.Width ||
                        coordY < 0 || coordY >= LevelData.TileGrid.Height)
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

                    if (coordX < 0 || coordX >= LevelData.TileGrid.Width ||
                        coordY < 0 || coordY >= LevelData.TileGrid.Height)
                    {
                        continue; // clipped!
                    }

                    canvas[coordX, coordY] = true;
                }
            }

            // update overlay
            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    bool canvasPixel = canvas[x, y];
                    if (OverlayPixels[x, y] != canvasPixel)
                    {
                        OverlayPixels[x, y] = canvasPixel;
                        Rectangle invalidateRect = GetOverlayRect(x, y);
                        tileGridPanel.Invalidate(invalidateRect);
                    }
                }
            }
        }

        private void ClearOverlay()
        {
            OverlayActive = false;

            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    SetOverlayTile(x, y, false);
                }
            }
        }

        private Point[] GetOverlayTileCoords()
        {
            int pixelCount = 0;
            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    if (OverlayPixels[x, y])
                    {
                        pixelCount++;
                    }
                }
            }

            Point[] coords = new Point[pixelCount];
            int index = 0;
            for (int y = 0; y < LevelData.TileGrid.Height; y++)
            {
                for (int x = 0; x < LevelData.TileGrid.Width; x++)
                {
                    if (OverlayPixels[x, y])
                    {
                        coords[index++] = new Point(x, y);
                    }
                }
            }

            return coords;
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

        private void ResetState(bool resetView)
        {
            // set title bar text
            string fileName = "Untitled";
            if (LevelData.FilePathName != string.Empty)
            {
                fileName = Path.GetFileNameWithoutExtension(LevelData.FilePathName);
            }

            Text = "Level Editor - " + fileName;

            if (resetView)
            {
                // reset view properties
                SelectedTileIndex = 1;
                TileGridTileSize = 64;
                TileSelectorTileSize = 64;
                tileGridPanel.AutoScrollPosition = new Point(0, 0);
                tileGridPanel.AutoScrollMinSize = new Size(
                    TileGridTileSize * LevelData.TileGrid.Width,
                    TileGridTileSize * LevelData.TileGrid.Height);
            }

            // clear bitmap caches
            CachedTileGridBitmaps = new CachedBitmap[TileSelectorCount];
            CachedTileSelectorBitmaps = new CachedBitmap[TileSelectorCount];

            // recreate overlay
            OverlayActive = false;
            OverlayPixels = new bool[LevelData.TileGrid.Width, LevelData.TileGrid.Height];

            // update button states
            UpdateButtons();

            // redraw all
            Invalidate(true);
        }
        private void UpdateButtons()
        {
            undoButton.Enabled = UndoRedoHistory.CanUndo();
            redoButton.Enabled = UndoRedoHistory.CanRedo();

            freeformModeButton.Enabled = (SelectedTileIndex < Level.TileCount);
            lineModeButton.Enabled = (SelectedTileIndex < Level.TileCount);
            rectModeButton.Enabled = (SelectedTileIndex < Level.TileCount);

            freeformModeButton.Checked = (_DrawMode == DrawMode.Freeform);
            lineModeButton.Checked = (_DrawMode == DrawMode.Line);
            rectModeButton.Checked = (_DrawMode == DrawMode.Rectangle);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "The main panel shows the design of the level. Its layout can be modified by using the 'Select Layout' button.\n\n" +
                "Tiles can be set by first selecting a tile from the left palette, and then clicking and dragging within the main panel.\n\n" +
                "The current draw mode (freeform, line, rectangle) effects how tiles are drawn when dragging the mouse.\n\n" +
                "The left panel shows a palette of 30 tiles, plus six 'object' tiles which can be used to place K9 and enemies.\n\n" +
                "The background tile is implicitly selected when the right mouse button is used.\n\n" +
                "Tiles can be edited by double clicking on them within the left palette. The background tile and object tiles cannot be edited.\n\n" +
                "Tiles also have a type, which effects their behavior within the game.\n\n" +
                "The four color palette can be modified using the 'Edit Palette' button.\n\n" +
                "The 'Edit Settings' button can be used to modify the relative strengths of K9, enemies, and Jewels.",
                "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private Level LevelData = new();
        private readonly UndoRedoHistory UndoRedoHistory = new();

        private int SelectedTileIndex = 1;
        private int TileSelectorTileSize = 64;
        private CachedBitmap[] CachedTileSelectorBitmaps = new CachedBitmap[TileSelectorCount];

        private enum DrawMode
        {
            Freeform,
            Line,
            Rectangle
        }

        DrawMode _DrawMode = DrawMode.Freeform;
        private int TileGridTileSize = 64;
        private bool OverlayActive = false;
        private Point OverlayStartPosition = new();
        private bool[,] OverlayPixels = new bool[0, 0];
        private bool EraseMode = false;
        private Rectangle FocusRect = Rectangle.Empty;
        private CachedBitmap[] CachedTileGridBitmaps = new CachedBitmap[TileSelectorCount];
        private readonly Bitmap[] ObjectBitmaps = new Bitmap[6];

        private const int TileSelectorCount = 34; // 28 regular tiles + 6 object tiles
    }

    public partial class TileSelectorPanel : Panel
    {
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // don't draw the background
        }
    }

    public partial class TileGridPanel : Panel
    {
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // disable scrolling when Ctrl key is pressed (we are going to zoom instead)
            bool scroll = ((Control.ModifierKeys & Keys.Control) != Keys.Control);
            VScroll = scroll;
            HScroll = scroll;
            base.OnMouseWheel(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // don't draw the background
        }
    }
}
