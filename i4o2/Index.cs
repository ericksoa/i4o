using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace i4o2
{
    public class Index<T> : IEnumerable<T>
    {
        private readonly IList<T> _internalList;
        
        public Index(IEnumerable<T> enumerableToIndex)
        {
            _internalList = new List<T>(enumerableToIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
