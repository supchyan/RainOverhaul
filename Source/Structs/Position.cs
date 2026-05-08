using System;

namespace RainOverhaul.Source.Structs;

public struct Position
{

    public int X;
    public int Y;

    public Position(int x, int y)
    {
        X = x;
        Y = y;
    }
    public Position(float x, float y)
    {
        X = (int)x; 
        Y = (int)y;
    }
}
