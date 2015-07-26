using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    // TODO: potential rewrite to do arbitrary sudoku variants.
    public class SquareSet
    {
    }

    public class Square
    {
        public SquareSet Parent;
        public int Id;
        public int X;
        public int Y;
        public int V;
        public bool[] PossibleV;
        public List<RestrictionSetRef> Restrictions = new List<RestrictionSetRef>();
    }

    public struct RestrictionSetRef
    {
        public int Index;
        public RestrictionSet Restriction;
    }

    public class RestrictionSet
    {
        public SquareSet Parent;
        public List<int> Squares = new List<int>();
        public int Target;
        public RestrictionType Type;
        public bool Unique;
        public List<int>[] PossibleLocs;
    }

    public enum RestrictionType
    {
        None,
        Sum,
        Product,
        Ratio,
        Difference,
    }

    public class MoveAction : IAction
    {
        public MoveAction(int id, int value)
        {
        }

        #region IAction Members

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public bool Successful
        {
            get { throw new NotImplementedException(); }
        }

        public bool Perform()
        {
            throw new NotImplementedException();
        }

        public void Unperform()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEquatable<IAction> Members

        public bool Equals(IAction other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}


