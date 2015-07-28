using System;
using System.Collections.Generic;
using System.Text;

using ToyTracerLibrary.Primatives;
using ToyTracerLibrary.CSG;

namespace ToyTracerLibrary.Bricks
{
    public class Plate : Construct
    {

        public bool Rounded = true;
        private const double roundFactor = 0.02;
        
        public Plate(int wide, int length)
        {
            Shadow = true;
            Render = true;
            Matrix trans = new Matrix();
            Matrix invTrans = new Matrix();
            double xtrans = ((double)wide) / 2.0;
            double ytrans = ((double)length) / 2.0;
            double ztrans = 0.5 * 2.0 / 5.0;
            trans.MakeTrans(xtrans, ytrans, ztrans);
            invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
            Matrix scale = new Matrix();
            Matrix invScale = new Matrix();
            double xsc = 0.5 * wide;
            double ysc = 0.5 * length;
            double zsc = 0.5 * 2.0 / 5.0;
            scale.MakeScale(xsc, ysc, zsc);
            invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
            Box body = new Box();
            if (Rounded)
            {
                body.Rounded = true;
                body.XRounding = roundFactor / xsc;
                body.YRounding = roundFactor / ysc;
                body.ZRounding = roundFactor / zsc;
            }
            body.ApplyMatrix(scale, invScale);
            body.ApplyMatrix(trans, invTrans);
            Box bodycutout = new Box();
            xtrans = ((double)wide) / 2.0;
            ytrans = ((double)length) / 2.0;
            ztrans = 0.5 * (2.0 / 5.0 - 3.0 / 16.0);
            trans.MakeTrans(xtrans, ytrans, ztrans);
            invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
            xsc = 0.5 * (wide - 6.0 / 16.0);
            ysc = 0.5 * (length - 6.0 / 16.0);
            zsc = 0.5 * (2.0 / 5.0 - 3.0 / 16.0) * (1 + ToyTracer.EPSILON * 3);
            if (Rounded)
            {
                bodycutout.Rounded = true;
                bodycutout.RoundedTopOnly = true;
                bodycutout.XRounding = roundFactor / xsc;
                bodycutout.YRounding = roundFactor / ysc;
                bodycutout.ZRounding = roundFactor / zsc;
            }
            scale.MakeScale(xsc, ysc, zsc);
            invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
            bodycutout.ApplyMatrix(scale, invScale);
            bodycutout.ApplyMatrix(trans, invTrans);
            Parts = new CSGDiff(body, bodycutout);
            Primative unstuds = null;
            Primative studs = null;
            for (int i = 0; i < wide; i++)
            {
                Primative unstudrow = null;
                Primative studrow = null;
                for (int j = 0; j < length; j++)
                {
                    Cylinder undstud = new Cylinder();
                    xsc = 0.5 * 1.0 / 4.0;
                    ysc = 0.5 * 1.0 / 4.0;
                    zsc = 0.5 * 3.0 / 16.0 * (1 + ToyTracer.EPSILON * 4);
                    scale.MakeScale(xsc, ysc, zsc);
                    invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                    xtrans = 0.5 + i;
                    ytrans = 0.5 + j;
                    ztrans = 2.0 / 5.0 - 0.5 * 3.0 / 16.0;
                    trans.MakeTrans(xtrans, ytrans, ztrans);
                    invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                    undstud.ApplyMatrix(scale, invScale);
                    undstud.ApplyMatrix(trans, invTrans);
                    if (unstudrow == null)
                        unstudrow = undstud;
                    else
                        unstudrow = new CSGUnion(unstudrow, undstud, true);
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
                    ztrans = 2.0 / 5.0 + 0.5 * 17.0 / 80.0;
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
                    unstuds = unstudrow;
                else
                    unstuds = new CSGUnion(unstuds, unstudrow, true);
            }
            Parts = new CSGDiff(Parts, unstuds);
            Parts = new CSGUnion(Parts, studs);
            if (wide == 1 || length == 1)
            {
                Primative holders = null;
                for (int i = 0; i < wide; i++)
                {
                    for (int j = 0; j < length; j++)
                    {
                        if (length == 1 && wide == i + 1)
                            continue;
                        if (wide == 1 && length == j + 1)
                            continue;
                        Cylinder stud = new Cylinder();
                        xsc = 0.5 * 3.0 / 8.0;
                        ysc = 0.5 * 3.0 / 8.0;
                        zsc = 0.5 * (2.0 / 5.0 - 3.0 / 16.0) * (1 + 10 * ToyTracer.EPSILON);
                        if (Rounded)
                        {
                            // TODO: rounded bottom only.
                            stud.Rounded = true;
                            stud.RRounding = roundFactor / xsc;
                            stud.ZRounding = roundFactor / zsc;
                        }
                        scale.MakeScale(xsc, ysc, zsc);
                        invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                        xtrans = 0.5 + i + (length == 1 ? 0.5 : 0);
                        ytrans = 0.5 + j + (wide == 1 ? 0.5 : 0);
                        ztrans = 0.5 * (2.0 / 5.0 - 3.0 / 16.0);
                        trans.MakeTrans(xtrans, ytrans, ztrans);
                        invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                        stud.ApplyMatrix(scale, invScale);
                        stud.ApplyMatrix(trans, invTrans);
                        if (holders == null)
                            holders = stud;
                        else
                            holders = new CSGUnion(holders, stud, true);
                    }
                }
                if (holders != null)
                    Parts = new CSGUnion(Parts, holders);

            }
            else
            {
                Primative cyls = null;
                for (int i = 0; i < wide; i++)
                {
                    Primative cylrow = null;
                    for (int j = 0; j < length; j++)
                    {
                        if (wide == i + 1)
                            continue;
                        if (length == j + 1)
                            continue;
                        Cylinder cyl = new Cylinder();
                        xsc = 0.5 * 0.789;
                        ysc = 0.5 * 0.789;
                        zsc = 0.5 * (2.0 / 5.0 - 3.0 / 16.0) * (1 + 10 * ToyTracer.EPSILON);
                        if (Rounded)
                        {
                            cyl.Rounded = true;
                            // TODO: rounded bottom only.
                            cyl.RRounding = roundFactor / xsc;
                            cyl.ZRounding = roundFactor / zsc;
                        }
                        scale.MakeScale(xsc, ysc, zsc);
                        invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                        xtrans = 1 + i;
                        ytrans = 1 + j;
                        ztrans = 0.5 * (2.0 / 5.0 - 3.0 / 16.0);
                        trans.MakeTrans(xtrans, ytrans, ztrans);
                        invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                        cyl.ApplyMatrix(scale, invScale);
                        cyl.ApplyMatrix(trans, invTrans);
                        Cylinder cylcut = new Cylinder();
                        xsc = 0.5 * 5.0 / 8.0;
                        ysc = 0.5 * 5.0 / 8.0;
                        zsc = 0.5 * (2.0 / 5.0 - 3.0 / 16.0) * (1 + ToyTracer.EPSILON * 15);
                        scale.MakeScale(xsc, ysc, zsc);
                        invScale.MakeScale(1 / xsc, 1 / ysc, 1 / zsc);
                        xtrans = 1 + i;
                        ytrans = 1 + j;
                        ztrans = 0.5 * (2.0 / 5.0 - 3.0 / 16.0);
                        trans.MakeTrans(xtrans, ytrans, ztrans);
                        invTrans.MakeTrans(-xtrans, -ytrans, -ztrans);
                        cylcut.ApplyMatrix(scale, invScale);
                        cylcut.ApplyMatrix(trans, invTrans);
                        CSGDiff cylpart = new CSGDiff(cyl, cylcut);
                        if (cylrow == null)
                            cylrow = cylpart;
                        else
                            cylrow = new CSGUnion(cylrow, cylpart, true);

                    }
                    if (cylrow != null)
                        if (cyls == null)
                            cyls = cylrow;
                        else
                            cyls = new CSGUnion(cyls, cylrow, true);
                }
                if (cyls != null)
                    Parts = new CSGUnion(Parts, cyls);
            }
            Bounds = Parts.Bounds;
        }
    }
}
