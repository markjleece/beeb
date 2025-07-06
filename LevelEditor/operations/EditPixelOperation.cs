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

        private readonly Tile Tile;
        private readonly Point[] PixelCoords;
        private readonly byte[] OldColors;
        private readonly byte NewColor;
    }
}
