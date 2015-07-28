using System;
using System.Collections.Generic;
using System.Text;

using ToyTracerLibrary.Primatives;
using ToyTracerLibrary.CSG;

namespace ToyTracerLibrary.Bricks
{
    public class HollowSphere : Construct
    {

        public HollowSphere(double wide)
        {
            Shadow = true;
            Render = true;
            Matrix trans = new Matrix();
            Matrix invTrans = new Matrix();
            double xtrans = ((double)wide) / 2.0;
            double ytrans = ((double)wide) / 2.0;
            double ztrans = ((double)wide) / 2.0;
            trans.MakeTrans(xtrans, ytrans, ztrans);
            invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
            Matrix scale = new Matrix();
            Matrix invScale = new Matrix();
            double xsc = (double)wide;
            double ysc = (double)wide;
            double zsc = (double)wide;
            scale.MakeScale(xsc, ysc, zsc);
            invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
            Sphere body = new Sphere();
            body.ApplyMatrix(scale, invScale);
            body.ApplyMatrix(trans, invTrans);
            Sphere bodycutout = new Sphere();
            xtrans = ((double)wide) / 2.0;
            ytrans = ((double)wide) / 2.0;
            ztrans = ((double)wide) / 2.0;
            trans.MakeTrans(xtrans, ytrans, ztrans);
            invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
            xsc *= 0.0005;
            ysc *= 0.0005;
            zsc *= 0.0005;
            scale.MakeScale(xsc, ysc, zsc);
            invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
            bodycutout.ApplyMatrix(scale, invScale);
            bodycutout.ApplyMatrix(trans, invTrans);
            Parts = new CSGDiff(body, bodycutout);
            Bounds = Parts.Bounds;
        }
    }
}
