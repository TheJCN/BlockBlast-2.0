namespace BlockBlast
{
    public class Figure
    {
        public int[,] Shape { get; }
        public int X { get; set; }
        public int Y { get; set; }

        public Figure(int[,] shape)
        {
            Shape = shape;
            X = 0;
            Y = 0;
        }
    }
}