using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Relink_Mod_Manager
{
    public class Util
    {
        // Standardized item sizes for UI consistency
        public const float INPUT_ITEM_WIDTH_BASE = 720f;
        public const float INPUT_ITEM_WIDTH_SECOND = INPUT_ITEM_WIDTH_BASE - 32f;
        public const float BUTTON_ITEM_WIDTH_BASE = 100f;
        public const float BUTTON_ITEM_WIDTH_SECOND = 150f;

        public const int MOD_FORMAT_VERSION_CURRENT = 1;

        public static Vector4 ColorToVec4(Color color)
        {
            return new Vector4(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
        }

        public static Vector4 RGBToVec4(int R, int G, int B)
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, 1.0f);
        }

        public static Vector4 RGBAToVec4(int R, int G, int B, int A)
        {
            return new Vector4(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        public static Color Vec4ToColor(Vector4 color)
        {
            return Color.FromArgb((int)(color.Z * 255), (int)(color.W * 255), (int)(color.X * 255), (int)(color.Y * 255));
        }

        /// <summary>
        /// Check if the given path has a known game path that is not included in data.i by default and will require the user to
        /// verify game file integrity via Steam to restore the original external file to their installation
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public static bool PathContainsVolatileGamePaths(string FilePath)
        {
            string[] PathPrefixes = { @"GBFR\data\sound\", @"data\sound\", @"sound\", @"GBFR\data\ui\", @"data\ui\", @"ui\" };

            foreach (string Prefix in PathPrefixes)
            {
                if (FilePath.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool BeginPopupModal(string name, ImGuiNET.ImGuiWindowFlags flags)
        {
            unsafe
            {
                int byteCount = Encoding.UTF8.GetByteCount(name);
                byte* nativeName = null;
                if (byteCount > 0)
                {
                    Span<byte> alloc = stackalloc byte[byteCount +1];
                    nativeName = (byte*)System.Runtime.CompilerServices.Unsafe.AsPointer(ref alloc.GetPinnableReference());
                    Encoding.UTF8.GetBytes(name, alloc);
                }
                byte ret = ImGuiNET.ImGuiNative.igBeginPopupModal(nativeName, null, flags);
                return ret != 0;
            }
        }
    }
}
