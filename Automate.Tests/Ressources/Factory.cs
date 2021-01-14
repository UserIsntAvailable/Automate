using System.Linq;
using System.Drawing;
using Automate.Models;

namespace Automate.Tests {
    public static class Factory {

        internal static Pixel[] Pixels(int length, Color color) {

            return Enumerable
                .Range(0, length)
                .Select(i => new Pixel((i, 0), color))
                .ToArray();
        }
    }
}
