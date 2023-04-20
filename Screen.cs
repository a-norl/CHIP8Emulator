using SDL2;

namespace CHIP8Emulator;

public class Screen
{

    public bool[,] pixelsDrawn;
    public bool[,] pixelsToDraw;
    public bool needDraw;
    public bool hiRes;

    public Screen()
    {
        pixelsToDraw = new bool[64, 32];
        pixelsDrawn = new bool[64, 32];
    }

    public int getWidth()
    {
        return pixelsDrawn.GetLength(0);
    }

    public int getHeight()
    {
        return pixelsDrawn.GetLength(1);
    }

    public void EnableHiRes()
    {
        pixelsToDraw = new bool[128, 64];
        pixelsDrawn = new bool[128, 64];
        hiRes = true;
        needDraw = true;
    }

    public void EnableLowRes()
    {
        pixelsToDraw = new bool[64, 32];
        pixelsDrawn = new bool[64, 32];
        hiRes = false;
        needDraw = true;
    }

    public void ClearScreen()
    {
        if (hiRes)
        {
            pixelsToDraw = new bool[128, 64];
            pixelsDrawn = new bool[128, 64];
            needDraw = true;
        }
        else
        {
            pixelsToDraw = new bool[64, 32];
            pixelsDrawn = new bool[64, 32];
            needDraw = true;
        }
    }

    public void SetPixel(byte x, byte y)
    {
        pixelsToDraw[x, y] = true;
        needDraw = true;
    }

    public void UnsetPixel(byte x, byte y)
    {
        pixelsToDraw[x, y] = false;
        needDraw = true;
    }

    public bool XORPixel(bool value, byte x, byte y)
    {
        if (value)
        {
            pixelsToDraw[x, y] = !pixelsToDraw[x, y];
            needDraw = true;
            if(!pixelsToDraw[x, y])
            {
                return true;
            }
        }
        return false;
    }

    public void ScrollRight()
    {
        for(int x = 0; x < getWidth(); x++)
        {
            for(int y = 0; y < getHeight(); y++)
            {
                if(x >= 4)
                {
                    if(x < getWidth()-4)
                    {
                        pixelsToDraw[x+4,y] = pixelsDrawn[x,y];
                    }
                }
                else
                {
                    pixelsToDraw[x,y] = false;
                }
            }
        }
        needDraw = true;
    }

    public void ScrollLeft()
    {
        for(int x = 0; x < getWidth(); x++)
        {
            for(int y = 0; y < getHeight(); y++)
            {
                if(x < getWidth()-4)
                {
                    if(x >= 4)
                    {
                        pixelsToDraw[x-4,y] = pixelsDrawn[x,y];
                    }
                }
                else
                {
                    pixelsToDraw[x,y] = false;
                }
            }
        }
        needDraw = true;
    }

    public void ScrollDown(int n)
    {
        for(int x = 0; x < getWidth(); x++)
        {
            for(int y = 0; y + n < getHeight(); y++)
            {
                if(y < n)
                {
                    pixelsToDraw[x,y] = false;
                }
                else
                {
                    pixelsToDraw[x,y+n] = pixelsDrawn[x,y];
                }
            }
        }
        needDraw = true;
    }

}