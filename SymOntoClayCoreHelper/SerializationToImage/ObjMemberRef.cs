using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SymOntoClay.CoreHelper.SerializationToImage
{
    public class ObjMemberRef
    {
#if DEBUG
        private static readonly NLog.ILogger _logger = NLog.LogManager.GetCurrentClassLogger();
#endif

        public ObjMemberRef(object obj, MemberInfo member)
        {
            _obj = obj;
            _member = member;
        }

        private object _obj; 
        private MemberInfo _member;

        public T GetCustomAttribute<T>() where T : Attribute
        {
            return _member.GetCustomAttribute<T>();
        }

        public T GetCustomAttribute<T>(bool inherit) where T : Attribute
        {
            return _member.GetCustomAttribute<T>(inherit);
        }

        public object GetValue()
        {
            var field = _member as FieldInfo;

            if (field != null)
            {
                return field.GetValue(_obj);
            }

            var property = _member as PropertyInfo;

            if(property != null)
            {
                return property.GetValue(_obj);
            }

            throw new NotImplementedException("6700FBD6-B2D9-4D6D-8B12-1CF9D6175FD5");
        }

        public object GetValue(string memberName)
        {
#if DEBUG
            //_logger.Info($"memberName = {memberName}");
#endif

            var field = GetField(memberName);

            if(field != null)
            {
                return field.GetValue(_obj);
            }

            var property = GetProperty(memberName);

            if (property != null)
            {
                return property.GetValue(_obj);
            }

            throw new ArgumentOutOfRangeException(nameof(memberName), memberName, "9CDC0140-BDC9-408E-B76D-B83CDCCC78A8");
        }

        private FieldInfo GetField(string memberName)
        {
            var type = _obj.GetType();

#if DEBUG
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            var result = new List<FieldInfo>();
            var currentType = type;

            while (currentType != null)
            {
                var currentFields = currentType
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                   
                result.AddRange(currentFields);

                currentType = currentType.BaseType;
            }

            return result.FirstOrDefault(p => p.Name == memberName);
        }

        private PropertyInfo GetProperty(string memberName)
        {
            var type = _obj.GetType();

#if DEBUG
            //_logger.Info($"type.FullName = {type.FullName}");
#endif

            var result = new List<PropertyInfo>();
            var currentType = type;

            while (currentType != null)
            {
#if DEBUG
                //_logger.Info($"currentType?.FullName = {currentType?.FullName}");
#endif

                var currentProperties = currentType
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);

#if DEBUG
                //_logger.Info($"currentProperties = {currentProperties.Select(p => p.Name).WritePODListToString()}");
#endif

                result.AddRange(currentProperties);

                currentType = currentType.BaseType;
            }

            return result.FirstOrDefault(p => p.Name == memberName);
        }
    }
}
