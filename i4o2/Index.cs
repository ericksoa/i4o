using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace i4o2
{    
    public interface IIndex<TChild> : ICollection<TChild>
    {
        string PropertyName { get; }
        IEnumerable<TChild> WhereThroughIndex(Expression<Func<TChild, bool>> whereClause);
    }
}
