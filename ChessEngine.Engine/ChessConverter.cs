namespace ChessEngine.Engine
{
    public static class ChessConverter
    {
        public static byte GetPiece(char piece)
        {
            switch (piece)
            {
                case 'P':
                    return 0x01;
                case 'N':
                    return 0x02;
                case 'K':
                    return 0x03;
                case 'B':
                    return 0x05;
                case 'R':
                    return 0x06;
                case 'Q':
                    return 0x07;
                case 'p':
                    return 0x09;
                case 'n':
                    return 0x0A;
                case 'k':
                    return 0x0B;
                case 'b':
                    return 0x0D;
                case 'r':
                    return 0x0E;
                case 'q':
                    return 0x0F;
            }
            return 0x00;
        }

        public static readonly string[] AlgStrings =
        {
            "a1", "b1", "c1", "d1", "e1", "f1", "g1", "h1", null, null, null, null, null, null, null, null,
            "a2", "b2", "c2", "d2", "e2", "f2", "g2", "h2", null, null, null, null, null, null, null, null,
            "a3", "b3", "c3", "d3", "e3", "f3", "g3", "h3", null, null, null, null, null, null, null, null,
            "a4", "b4", "c4", "d4", "e4", "f4", "g4", "h4", null, null, null, null, null, null, null, null,
            "a5", "b5", "c5", "d5", "e5", "f5", "g5", "h5", null, null, null, null, null, null, null, null,
            "a6", "b6", "c6", "d6", "e6", "f6", "g6", "h6", null, null, null, null, null, null, null, null,
            "a7", "b7", "c7", "d7", "e7", "f7", "g7", "h7", null, null, null, null, null, null, null, null,
            "a8", "b8", "c8", "d8", "e8", "f8", "g8", "h8", null, null, null, null, null, null, null, null,
        };
    }
}
