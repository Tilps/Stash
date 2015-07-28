using System;
using System.Collections.Generic;
using System.Text;

using ToyTracerLibrary.Primatives;
using ToyTracerLibrary.CSG;

namespace ToyTracerLibrary.Bricks
{
    public class BasePlate : Construct
    {

        public bool Rounded = true;
        private const double roundFactor = 0.02;

        public BasePlate(int wide, int length)
        {
            Shadow = true;
            Render = true;
            Matrix trans = new Matrix();
            Matrix invTrans = new Matrix();
            double xtrans = ((double)wide) / 2.0;
            double ytrans = ((double)length) / 2.0;
            double ztrans = -0.5 * 2.0 / 15.0;
            trans.MakeTrans(xtrans, ytrans, ztrans);
            invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
            Matrix scale = new Matrix();
            Matrix invScale =new Matrix();
            double xsc = 0.5 * wide;
            double ysc = 0.5 * length;
            double zsc = 0.5 * 2.0 / 15.0;
            scale.MakeScale(xsc, ysc, zsc);
            invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
            Parts = new Box();
            if (Rounded)
            {
                ((Box)Parts).Rounded = true;
                ((Box)Parts).XRounding = roundFactor / xsc;
                ((Box)Parts).YRounding = roundFactor / ysc;
                ((Box)Parts).ZRounding = roundFactor / zsc;
            }
            Parts.ApplyMatrix(scale, invScale);
            Parts.ApplyMatrix(trans, invTrans);
            Primative unstuds = null;
            Primative studs = null;
            for (int i = 0; i < wide; i++)
            {
                Primative undstudrow = null;
                Primative studrow = null;
                for (int j = 0; j < length; j++)
                {
                    Cylinder undstud = new Cylinder();
                    xsc = 0.5 * 1.0 / 4.0;
                    ysc = 0.5 * 1.0 / 4.0;
                    zsc = 0.5 * 2.0 / 15.0 * (1 + ToyTracer.EPSILON * 2);
                    scale.MakeScale(xsc, ysc, zsc);
                    invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                    xtrans = 0.5 + i;
                    ytrans = 0.5 + j;
                    ztrans = -0.5 * 2.0 / 15.0;
                    trans.MakeTrans(xtrans, ytrans, ztrans);
                    invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                    undstud.ApplyMatrix(scale, invScale);
                    undstud.ApplyMatrix(trans, invTrans);
                    if (undstudrow == null)
                        undstudrow = undstud;
                    else
                        undstudrow = new CSGUnion(undstudrow, undstud, true);
                    Cylinder stud = new Cylinder();
                    xsc = 0.5 * 5.0 / 8.0;
                    ysc = 0.5 * 5.0 / 8.0;
                    zsc = 0.5 * 17.0 / 80.0 * (1 + ToyTracer.EPSILON * 2);
                    if (Rounded)
                    {
                        stud.Rounded = true;
                        stud.RoundedTopOnly = true;
                        stud.RRounding = roundFactor / xsc;
                        stud.ZRounding = roundFactor / zsc;
                    }
                    scale.MakeScale(xsc, ysc, zsc);
                    invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                    xtrans = 0.5 + i;
                    ytrans = 0.5 + j;
                    ztrans = 0.5 * 17.0 / 80.0;
                    trans.MakeTrans(xtrans, ytrans, ztrans);
                    invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                    stud.ApplyMatrix(scale, invScale);
                    stud.ApplyMatrix(trans, invTrans);
                    if (studrow == null)
                        studrow = stud;
                    else
                        studrow = new CSGUnion(studrow, stud, true);
                }
                if (studs == null)
                    studs = studrow;
                else
                    studs = new CSGUnion(studs, studrow, true);
                if (unstuds == null)
                    unstuds = undstudrow;
                else
                    unstuds = new CSGUnion(unstuds, undstudrow, true);
            }
            Parts = new CSGDiff(Parts, unstuds);
            Parts = new CSGUnion(Parts, studs);
            Bounds = Parts.Bounds;
        }
   }
}
