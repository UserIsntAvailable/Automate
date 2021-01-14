using System.Linq;
using System.Drawing;
using Xunit;
using Automate.Models;

namespace Automate.Tests {
    public class RowTests {

        [Fact]
        public void Equals_Should_Be_True() {

            var randomPixels = Factory.Pixels(10, Color.Empty);

            var row1 = new Row(randomPixels);
            var row2 = new Row(randomPixels);

            Assert.Equal(row1, row2);
        }

        [Fact]
        public void Equals_By_Colors_Should_Be_True() {

            const int rowLength = 10;

            // #00FFFF
            var aquaRow = new Row(Factory.Pixels(rowLength, Color.Aqua));

            // #7FFFD4
            var aquamarineRow = new Row(Factory.Pixels(rowLength, Color.Aquamarine));

            Assert.True(aquaRow.EqualsByColors(aquamarineRow, 0.1m));
        }

        [Fact]
        public void Range_Implementation_Should_Work() {

            var randomPixels = Factory.Pixels(5, Color.Red)
                .Concat(Factory.Pixels(5, Color.Blue))
                .ToArray();

            var row = new Row(randomPixels);

            var rowSlice = row[3..6];
            var randomPixelsSlice = randomPixels[3..6];

            Assert.Equal(rowSlice, randomPixelsSlice);
        }
    }
}
