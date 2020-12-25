using System;
using Xunit;

namespace Tetris.Tests
{
    public class TestVec256
    {
        [Fact]
        public void Vec256_ToString()
        {
            Assert.Equal("05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 " +
            "05 05 05 05 05 05 05 05 05 05 05 05 05 05 05 05", ((Vec256)5).ToString());
        }

        [Fact]
        public void V256_And() =>
       Assert.Equal((Vec256)4, (Vec256)5 & (Vec256)6);

        [Fact]
        public void V256_ShiftRight()
        {
            Vec256 y = new Vec256(
                0xF1, 0x18, 0x80, 0x01, 0x18, 0x80, 0x01, 0x18,
                0x80, 0x01, 0x18, 0x80, 0x01, 0x18, 0x80, 0x01,
                0x18, 0x80, 0x01, 0x18, 0x80, 0x01, 0x18, 0x80,
                0x01, 0x18, 0x80, 0x01, 0x18, 0x80, 0xFF, 0x0F);
            Assert.Equal(
                "01 18 80 01 18 80 01 18 80 01 18 80 01 18 80 01 " +
                "18 80 01 18 80 01 18 80 01 18 80 01 F8 FF 00 00",
                (y >> 12).ToString());
        }
    }
}
