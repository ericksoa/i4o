using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace i4o2
{    
    public interface IIndex<TChild> : ICollection<TChild>
    {
        IEnumerable<TChild> WhereThroughIndex(Expression<Func<TChild, bool>> whereClause);
        void Reset(TChild changedObject);
    }
}
