using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace VictorKrogh.Data.EntityFrameworkCore.Models;

public class EFCoreModel
{
    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> DatabaseGeneratedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> Properties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> NotMappedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ModelEqualityProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

    private static readonly ConcurrentDictionary<RuntimeTypeHandle, object?> DefaultValues = new ConcurrentDictionary<RuntimeTypeHandle, object?>();

    private int? _requestedHashCode;

    private static IEnumerable<PropertyInfo> GetDatabaseGeneratedProperties(Type type)
    {
        Type type2 = type;
        return DatabaseGeneratedProperties.GetOrAdd(type2.TypeHandle, (typeHandle) => (from p in type2.GetProperties()
                                                                                                         where p.GetCustomAttributes<DatabaseGeneratedAttribute>(inherit: true).Any((a) => a.DatabaseGeneratedOption != DatabaseGeneratedOption.None)
                                                                                                         select p).ToArray());
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        Type type2 = type;
        return Properties.GetOrAdd(type2.TypeHandle, (typeHandle) => type2.GetProperties().ToArray());
    }

    private static IEnumerable<PropertyInfo> GetKeyProperties(Type type)
    {
        Type type2 = type;
        return KeyProperties.GetOrAdd(type2.TypeHandle, (typeHandle) => (from p in type2.GetProperties()
                                                                                           where p.GetCustomAttributes<KeyAttribute>(inherit: true).Any()
                                                                                           select p).ToArray());
    }

    private static IEnumerable<PropertyInfo> GetNotMappedProperties(Type type)
    {
        Type type2 = type;
        return NotMappedProperties.GetOrAdd(type2.TypeHandle, (typeHandle) => (from p in type2.GetProperties()
                                                                                                 where p.GetCustomAttributes<NotMappedAttribute>(inherit: true).Any()
                                                                                                 select p).ToArray());
    }

    private static object? GetDefaultValue(Type type)
    {
        Type type2 = type;
        return DefaultValues.GetOrAdd(type2.TypeHandle, (typeHandle) => type2.IsValueType ? Activator.CreateInstance(type2) : null);
    }

    private static bool IsDefaultValue(object? value)
    {
        return value?.Equals(GetDefaultValue(value.GetType())) ?? true;
    }

    public virtual bool IsTransient()
    {
        IEnumerable<PropertyInfo> databaseGeneratedProperties = GetDatabaseGeneratedProperties(GetType());
        if (!databaseGeneratedProperties.Any())
        {
            return false;
        }

        if (databaseGeneratedProperties.Any((p) => p.GetValue(this) == null))
        {
            return true;
        }

        return databaseGeneratedProperties.Any((p) => IsDefaultValue(p.GetValue(this)));
    }

    public override bool Equals(object? obj)
    {
        if (!(obj is EFCoreModel model))
        {
            return false;
        }

#pragma warning disable CS0253 // Possible unintended reference comparison; right hand side needs cast
        if (this == obj)
        {
            return true;
        }
#pragma warning restore CS0253 // Possible unintended reference comparison; right hand side needs cast

        if (GetType() != obj.GetType())
        {
            return false;
        }

        if (model.IsTransient() || IsTransient())
        {
            return false;
        }

        if (GetKeyProperties(GetType()).Any((p) => p.GetValue(this) == null))
        {
            return false;
        }

        IEnumerable<PropertyInfo> notMappedProperties = GetNotMappedProperties(GetType());
        foreach (PropertyInfo item in GetProperties(GetType()).Except(notMappedProperties))
        {
            var value = item.GetValue(this);
            var value2 = item.GetValue(model);
            if (value == null)
            {
                if (value2 != null)
                {
                    return false;
                }
            }
            else if (!value.Equals(item.GetValue(model)))
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                if (GetKeyProperties(GetType()).Any((p) => p.GetValue(this) == null))
                {
                    return base.GetHashCode();
                }

                foreach (PropertyInfo property in GetProperties(GetType()))
                {
                    var value = property.GetValue(this);
                    if (value == null)
                    {
                        return base.GetHashCode();
                    }

                    int num = value.GetHashCode() ^ 0x1F;
                    if (_requestedHashCode.HasValue)
                    {
                        _requestedHashCode ^= num;
                    }
                    else
                    {
                        _requestedHashCode = num;
                    }
                }
            }

            return _requestedHashCode ?? base.GetHashCode();
        }

        return base.GetHashCode();
    }

    public static bool operator ==(EFCoreModel left, EFCoreModel right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(EFCoreModel left, EFCoreModel right)
    {
        return !(left == right);
    }
}