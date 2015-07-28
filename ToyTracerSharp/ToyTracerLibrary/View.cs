using System;
using System.Collections.Generic;
using System.Text;

namespace ToyTracerLibrary
{
    public class View
    {
        public Line Look
        {
            get
            {
                return look;
            }
        }
        Line look;

        public double Focal
        {
            get
            {
                return focal;
            }
            set
            {
                focal = value;
            }
        }
        double focal;
        public double ViewWidth
        {
            get
            {
                return viewWide;
            }
            set
            {
                viewWide = value;
            }
        }
        public double ViewHeight
        {
            get
            {
                return viewHigh;
            }
            set
            {
                viewHigh = value;
            }
        }
        double viewWide, viewHigh;

        public Vector Up
        {
            get
            {
                return up;
            }
        }
        Vector up;

        public int ScreenX
        {
            get
            {
                return screenx;
            }
            set
            {
                screenx = value;
            }
        }
        public int ScreenY
        {
            get
            {
                return screeny;
            }
            set
            {
                screeny = value;
            }
        }
        int screenx, screeny;

        public View()
        {
            look = new Line();
        }

        public void Pos(Point place)
        {
            look.Start = place;
        }
        public void LookAt(Point place)
        {
            look.Direct = look.Start.LineTo(place);

        }
        public void Upward(Point place)
        {
            Vector upnot = look.Start.LineTo(place);
            up = look.Direct.Cross(upnot).Cross(look.Direct);

        }
        public Line RayThrough(double x, double y)
        {
            double midx = ((double)screenx) / 2;
            double midy = ((double)screeny) / 2;
            Line ret = new Line();
            ret.Start = new Point(look.Start.X, look.Start.Y, look.Start.Z);
            ret.Direct = look.Direct.Scale(focal / look.Direct.Length);
            double difx = midx - x;
            double dify = midy - y;
            ret.Direct.Add(Dx().Scale(-difx));
            ret.Direct.Add(Dy().Scale(-dify));
            return ret;
        }
        public void Project(Point seen, out int x, out int y)
        {
            Vector direct = look.Start.LineTo(seen);
            Vector right = look.Direct.Cross(up);
            double costheta = up.Dot(direct) / up.Length / direct.Length;
            double cosphi = right.Dot(direct) / right.Length / direct.Length;
            double theta = Math.Acos(costheta);
            double phi = Math.Acos(cosphi);
            y = -(int)(focal / Math.Tan(theta) / Dy().Length) + screeny / 2;
            x = (int)(focal / Math.Tan(phi) / Dx().Length) + screenx / 2;
        }
        public Vector Dy()
        {
            return up.Scale(-viewHigh / up.Length / screeny);
        }
        public Vector Dx()
        {
            Vector right = look.Direct.Cross(up);
            return right.Scale(viewWide / right.Length / screenx);
        }

    }
}
