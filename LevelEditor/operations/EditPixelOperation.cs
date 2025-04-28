// This file is Copyright © 2025 - Mark John Leece - All rights reserved
namespace LevelEditor
{
    class EditPixelOperation : Operation
    {
        internal EditPixelOperation(Tile tile, Point[] pixelCoords, int color)
        {
            Tile = tile;

            PixelCoords = new Point[pixelCoords.Length];
            OldColors = new byte[pixelCoords.Length];

            for (int i = 0; i < pixelCoords.Length; i++)
            {
                PixelCoords[i] = pixelCoords[i];
                OldColors[i] = tile[pixelCoords[i].X, pixelCoords[i].Y];
            }

            NewColor = (byte)color;
        }

        internal override bool Execute()
        {
            Redo();
            return true;
        }

        internal override void Redo()
        {
            for (int i = 0; i < PixelCoords.Length; i++)
            {
                Tile[PixelCoords[i].X, PixelCoords[i].Y] = NewColor;
            }
        }

        internal override void Undo()
        {
            for (int i = 0; i < PixelCoords.Length; i++)
            {
                Tile[PixelCoords[i].X, PixelCoords[i].Y] = OldColors[i];
            }
        }

        private Tile Tile;
        private Point[] PixelCoords;
        private byte[] OldColors;
        private byte NewColor;
    }
}
