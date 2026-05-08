using Microsoft.Xna.Framework;
using Terraria;

namespace RainOverhaul.Source.Structs;

public struct Rect
{
    public int Width    { get; set; }
    public int Height   { get; set; }

    public Position TopLeftPosition { get; set; }
    public readonly Position BottomLeftPosition => 
        new (TopLeftPosition.X + Width, TopLeftPosition.Y + Height);

    /// <summary>
    /// Transforms Rect instance to a Xna.Framework's Rectangle.
    /// </summary>
    public Rectangle ToRectangle()
    {
        return new(TopLeftPosition.X, TopLeftPosition.Y, Width, Height);
    }
    /// <summary>
    /// True whenever mouse cursor is inside Rect.
    /// <param name="padding">Rectangle padding to offset collision borders inside.</param>
    /// </summary>
    public bool IsMouseInside()
    {
        return Main.mouseX >= TopLeftPosition.X && Main.mouseX < BottomLeftPosition.X &&
               Main.mouseY >= TopLeftPosition.Y && Main.mouseY < BottomLeftPosition.Y;
    }
}
