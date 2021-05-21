using System.Collections.Generic;
using System.Dynamic;
using Entities.DTO;

namespace Contracts {
    public interface IDataShaper<T> {
        IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fields);
        ExpandoObject ShapeData(T entity, string fields);
       
    }
}