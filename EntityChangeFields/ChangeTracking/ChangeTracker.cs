using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EntityChangeFields.ChangeTracking
{
    public class ChangeTracker<TSource>
        where TSource : class
    {
        private static Dictionary<string, TrackingConfig<TSource>> TypeChangeConfigDic = new Dictionary<string, TrackingConfig<TSource>>();

        /// <summary>
        /// 创建类型Track配置
        /// 默认添加所有属性
        /// </summary>
        public static TrackingConfig<TSource> CreateConfig(string name = null)
        {
            var sourceType = typeof(TSource);
            var typeConfig = new TrackingConfig<TSource>()
            {
                Name = sourceType.FullName,
                CustomName = name,
                TrackingFields = new List<TrackingField<TSource>>(),
            };
            string tt = "12";
            tt.ToString();

            var instanceExpression = Expression.Parameter(sourceType, "instance");
            foreach (var prop in sourceType.GetProperties())
            {
                var memberExpression = Expression.Property(instanceExpression, prop);
                var toStringExpression = Expression.Call(memberExpression, prop.PropertyType.GetMethod("ToString", new Type[] { }));
                var lambdaExpression = Expression.Lambda<Func<TSource, string>>(toStringExpression, instanceExpression);
                typeConfig.TrackingFields.Add(new TrackingField<TSource> 
                {
                    Name = prop.Name,
                    GetValue = lambdaExpression.Compile(),
                });
            }
            if (TypeChangeConfigDic.ContainsKey(sourceType.FullName))
            {
                TypeChangeConfigDic[sourceType.FullName] = typeConfig;
            }
            else
            {
                TypeChangeConfigDic.Add(sourceType.FullName, typeConfig );
            }

            return typeConfig;
        }

        public static ChangeRecord GetChangeRecord(TSource originEntity, TSource newEntity)
        {
            if (originEntity == null && newEntity == null)
            {
                return null;
            }
            var typeName = typeof(TSource).FullName;
            var config = TypeChangeConfigDic.GetValueOrDefault(typeName);
            if (config == null)
            {
                return null;
            }

            ChangeType changeType;
            if (originEntity == null && newEntity != null)
            {
                changeType = ChangeType.Adding;
            }
            else if (originEntity != null && newEntity == null)
            {
                changeType = ChangeType.Deleting;
            }
            else
            {
                changeType = ChangeType.Modifying;
            }

            var changeFields = new List<ChangeField>();
            foreach (var trackingField in config.TrackingFields)
            {
                var originValue = originEntity != null ? trackingField.GetValue(originEntity) : string.Empty;
                var newValue = newEntity != null ? trackingField.GetValue(newEntity) : string.Empty;
                if (originValue != newValue)
                {
                    changeFields.Add(new ChangeField()
                    {
                        Name = trackingField.CustomName ?? trackingField.Name,
                        OriginValue = trackingField.GetDescription != null ? trackingField.GetDescription(originEntity) : originValue,
                        NewValue = trackingField.GetDescription != null ? trackingField.GetDescription(newEntity) : newValue,
                    });
                }
            }
            if (changeFields.Count == 0)
            {
                return null;
            }
            var changeRecord = new ChangeRecord()
            {
                Title = config.CustomName ?? config.Name,
                Type = changeType,
                TargetName = typeName,
                Fields = changeFields,
            };

            return changeRecord;
        }
    }

    public static class ChangeTypeConfig
    {
        /// <summary>
        /// 添加/修改追踪字段
        /// </summary>
        public static TrackingConfig<T> SetTrackingField<T, TKey>(this TrackingConfig<T> config,
            Expression<Func<T, TKey>> fieldSelector, string fieldName = null, 
            Func<T, string> getDescription = null)
        {
            var memberExpression = fieldSelector.Body as MemberExpression;
            var typeFieldName = memberExpression?.Member.Name;
            if (typeFieldName == null)
            {
                return config;
            }
            var trackingField = config.TrackingFields.FirstOrDefault(x=>x.Name == typeFieldName);
            if (trackingField != null)
            {
                trackingField.CustomName = fieldName ?? typeFieldName;
                trackingField.GetDescription = getDescription;
            }
            else
            {
                config.TrackingFields.Add(new TrackingField<T>
                {
                    Name = typeFieldName,
                    CustomName = fieldName?? typeFieldName,
                    GetValue = (T source) =>
                    {
                        var result = fieldSelector.Compile().Invoke(source);
                        return result?.ToString();
                    },
                    GetDescription = getDescription,
                });
            }

            return config;
        }

        /// <summary>
        /// 忽略追踪字段
        /// </summary>
        public static TrackingConfig<T> IgnorTrackingField<T, TKey>(this TrackingConfig<T> config,
            Expression<Func<T, TKey>> fieldSelector)
        {
            var memberExpression = fieldSelector.Body as MemberExpression;
            var typeFieldName = memberExpression?.Member.Name;
            if (typeFieldName == null)
            {
                return config;
            }

            var trackingField = config.TrackingFields.FirstOrDefault(x => x.Name == typeFieldName);
            if (trackingField != null)
            {
                config.TrackingFields.Remove(trackingField);
            }

            return config;
        }
    }
}
