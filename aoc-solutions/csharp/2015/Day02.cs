using Common;

namespace _2015;

public static class Day02
{
    public static string Part1(IEnumerable<string> input)
    {
        long totalWrappingPaperArea = input
            .Select(Present.FromString)
            .Sum(present => present.WrappingPaperArea);
        return totalWrappingPaperArea.ToString();
    }

    public static string Part2(IEnumerable<string> input)
    {
        long totalRibbonLength = input
            .Select(Present.FromString)
            .Sum(present => present.RibbonLength);
        return totalRibbonLength.ToString();
    }

    private readonly struct Present
    {
        public readonly uint Length;
        public readonly uint Width;
        public readonly uint Height;
        public readonly uint SurfaceAreaSide1;
        public readonly uint SurfaceAreaSide2;
        public readonly uint SurfaceAreaSide3;
        public readonly uint SurfaceArea;

        public uint SurfaceAreaSmallestSide
        {
            get
            {
                uint result = SurfaceAreaSide1;
                if (SurfaceAreaSide2 < result)
                    result = SurfaceAreaSide2;
                if (SurfaceAreaSide3 < result)
                    result = SurfaceAreaSide3;
                return result;
            }
        }

        public uint WrappingPaperArea => SurfaceArea + SurfaceAreaSmallestSide;

        public readonly uint PerimeterSide1;
        public readonly uint PerimeterSide2;
        public readonly uint PerimeterSide3;
        public readonly uint Volume;

        public uint PerimeterSmallestSide
        {
            get
            {
                uint smallestSideArea = SurfaceAreaSide1;
                uint result = PerimeterSide1;

                if (SurfaceAreaSide2 < smallestSideArea)
                {
                    smallestSideArea = SurfaceAreaSide2;
                    result = PerimeterSide2;
                }

                if (SurfaceAreaSide3 < smallestSideArea)
                    result = PerimeterSide3;

                return result;
            }
        }

        public uint RibbonLength => PerimeterSmallestSide + Volume;

        public Present(uint length, uint width, uint height)
        {
            Length = length;
            Width = width;
            Height = height;
            SurfaceAreaSide1 = Length * Width;
            SurfaceAreaSide2 = Width * Height;
            SurfaceAreaSide3 = Height * Length;
            SurfaceArea = 2 * SurfaceAreaSide1 + 2 * SurfaceAreaSide2 + 2 * SurfaceAreaSide3;

            Volume = Length * Width * Height;
            PerimeterSide1 = 2 * Length + 2 * Width;
            PerimeterSide2 = 2 * Width + 2 * Height;
            PerimeterSide3 = 2 * Height + 2 * Length;
        }

        public static Present FromString(string s)
        {
            uint[] lengths = s.Split('x').Select(uint.Parse).ToArray();
            return new Present(lengths[0], lengths[1], lengths[2]);
        }
    }
}