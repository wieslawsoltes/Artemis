﻿using System.Linq;
using Artemis.Profiles.Layers.Types.AmbientLight.AmbienceCreator;

namespace Artemis.Profiles.Layers.Types.AmbientLight.Model.Extensions
{
    public static class AvgColorExtension
    {
        public static AvgColor[] Flip(this AvgColor[] colors, int width, FlipMode flipMode)
        {
            if (colors == null || width <= 0) return colors;

            if (flipMode.HasFlag(FlipMode.Vertical))
                return flipMode.HasFlag(FlipMode.Horizontal) ? colors.Reverse().ToArray() : colors.FlipVertical(width);

            if (flipMode.HasFlag(FlipMode.Horizontal))
                return colors.FlipHorizontal(width);

            return colors;
        }

        public static AvgColor[] FlipVertical(this AvgColor[] colors, int width)
        {
            if (colors == null || width <= 0) return colors;

            AvgColor[] flipped = new AvgColor[colors.Length];
            for (int i = 0, j = colors.Length - width; i < colors.Length; i += width, j -= width)
                for (int k = 0; k < width; ++k)
                    flipped[i + k] = colors[j + k];

            return flipped;
        }

        public static AvgColor[] FlipHorizontal(this AvgColor[] colors, int width)
        {
            if (colors == null || width <= 0) return colors;

            AvgColor[] flipped = new AvgColor[colors.Length];
            for (int i = 0; i < colors.Length; i += width)
                for (int j = 0, k = width - 1; j < width; ++j, --k)
                    flipped[i + j] = colors[i + k];

            return flipped;
        }

        public static byte[] ToBGRArray(this AvgColor[] colors)
        {
            byte[] newData = new byte[colors.Length * 3];
            int counter = 0;
            foreach (AvgColor color in colors)
            {
                newData[counter++] = color.B;
                newData[counter++] = color.G;
                newData[counter++] = color.R;
            }

            return newData;
        }
    }
}
