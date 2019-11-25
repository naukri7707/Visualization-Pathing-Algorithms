using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VisualizationPathingAlgorithms
{
    public struct PointInt
    {
        public int X, Y;

        public PointInt(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointInt))
            {
                return false;
            }

            var @int = (PointInt)obj;
            return X == @int.X &&
                   Y == @int.Y;
        }

        public static bool operator ==(PointInt lhs, PointInt rhs)
        {
            return lhs.Equals(rhs);
        }
        public static bool operator !=(PointInt lhs, PointInt rhs)
        {
            return !lhs.Equals(rhs);
        }
    }

    public struct Info
    {
        public Rectangle Rect;
        public PointInt From, Position;
        public int F => G + H;
        public int G, H, Step;
    }
    class Map
    {
        public static readonly SolidColorBrush
             clr_Space = new SolidColorBrush(Colors.Gray),
             clr_Obstacle = new SolidColorBrush(Colors.DarkSlateGray),
             clr_Start = new SolidColorBrush(Colors.Blue),
             clr_End = new SolidColorBrush(Colors.RoyalBlue),
             clr_IsOpen = new SolidColorBrush(Colors.YellowGreen),
             clr_IsClose = new SolidColorBrush(Colors.OrangeRed);

        //constructor
        public Map(int Size, double cvsSize)
        {
            Index = new Info[Size, Size];
            this.Size = Size;
            this.RectSize = cvsSize / Size;
            //
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Fill = clr_Space;
                    rect.Stroke = new SolidColorBrush(Colors.Black);
                    rect.Width = rect.Height = RectSize;
                    //
                    rect.SetValue(Canvas.LeftProperty, i * rect.Width);
                    rect.SetValue(Canvas.TopProperty, j * rect.Height);
                    Index[i, j].Rect = rect;
                    //
                    Index[i, j].Step = 0;
                    Index[i, j].From = new PointInt();
                    Index[i, j].Position = new PointInt(i, j);
                }
            }
            //
            prevRect = new Rectangle();
            prevRect.Fill = new SolidColorBrush();
            prevRect.Stroke = new SolidColorBrush(Colors.Black);
            prevRect.Width = prevRect.Height = RectSize;
            //
            nowColor = clr_Obstacle;
            isClear = true;
        }
        //variable
        public Info[,] Index;
        public Rectangle prevRect;
        public SolidColorBrush nowColor;
        public PointInt Start, End;
        public double RectSize;
        public int Size;
        public bool isClear;
        //operator overloading
        public ref Info this[int X, int Y]
        {
            get => ref Index[X, Y];
        }
        public ref Info this[PointInt p]
        {
            get => ref Index[p.X, p.Y];
        }
        //function
        public void ClearMap()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Index[i, j].Step = Index[i, j].G = Index[i, j].H = 0;
                    Index[i, j].From = new PointInt(0, 0);
                    if (Index[i, j].Rect.Fill != clr_Obstacle)
                        Index[i, j].Rect.Fill = Map.clr_Space;
                }
            }
            Index[Start.X, Start.Y].Rect.Fill = clr_Start;
            Index[End.X, End.Y].Rect.Fill = clr_End;
            isClear = true;
            nowColor = Map.clr_Obstacle;
        }

        public bool IsInRange(int X, int Y)
        {
            if (X < 0 || Y < 0 || X >= Size || Y >= Size) return false;
            else return true;
        }
        public bool IsInRange(PointInt p)
        {
            if (p.X < 0 || p.Y < 0 || p.X >= Size || p.Y >= Size) return false;
            else return true;
        }
    }
}
