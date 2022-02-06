using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altair.BBS.Api.Ansi
{
    public class AnsiColors
    {
        public static byte[] GoTo(int x, int y)
        {
            return new[] { (byte)'\x1B', (byte)'[', (byte)x, (byte)';', (byte)y, (byte)'H' };
        }

        public static byte[] ClearScreen()
        {
            return new byte[] { 0x1b, 0x5b, 0x32, 0x4A };
        }
    }
}
