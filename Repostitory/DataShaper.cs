using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using Contracts;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Repostitory {
    public class DataShaper<T> : IDataShaper<T> where T : class {
        public IEnumerable<PropertyInfo> Properties { get; set; }

        public DataShaper() {
            Properties = typeof(T).
                GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        }
        public IEnumerable<ExpandoObject> ShapeData(IEnumerable<T> entities, string fields) {
            var requiredProperties = GetRequiredProperties(fields);
            return FetchData(entities, requiredProperties);
        }

        public ExpandoObject ShapeData(T entity, string fields) {
            var requiredProperties = GetRequiredProperties(fields);
            return FetchDataForEntity(entity, requiredProperties);
        }

        private IEnumerable<PropertyInfo> GetRequiredProperties(string fields) {
            var requiredProperties = new List<PropertyInfo>();
            if (string.IsNullOrEmpty(fields))
                return Properties;

            var requiredFields = fields.Split(",",StringSplitOptions.RemoveEmptyEntries);
            foreach (var field in requiredFields) {
                var property = Properties.FirstOrDefault(p =>
                    p.Name.Equals(field, StringComparison.InvariantCultureIgnoreCase));
                if(property is null)
                    continue;
                requiredProperties.Add(property);
            }

            return requiredProperties;
        }

        private ExpandoObject FetchDataForEntity(T entity,IEnumerable<PropertyInfo> requiredProperties) {
            var shapeObject = new ExpandoObject();
            foreach (var property in requiredProperties) {
                var value = property.GetValue(entity);
                shapeObject.TryAdd(property.Name, value);
            }

            return shapeObject;
        }

        private IEnumerable<ExpandoObject> FetchData(IEnumerable<T> entities,
            IEnumerable<PropertyInfo> requiredProperties) {
            var dataShaping = new List<ExpandoObject>();
            foreach (var entity in entities) {
                var shapeObject = FetchDataForEntity(entity,requiredProperties);
                dataShaping.Add(shapeObject);
            }
            return dataShaping;
        }
    }
}