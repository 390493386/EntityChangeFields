using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EntityChangeFields.ChangeTracking
{
    public class ChangeTracker<TSource>
    {
        private static Dictionary<string, TrackingConfig<TSource>> TypeChangeConfigDic = new Dictionary<string, TrackingConfig<TSource>>();

        public static TrackingConfig<TSource> CreateConfig(string name = null)
        {
            var typeName = typeof(TSource).FullName;
            var typeConfig = new TrackingConfig<TSource>()
            {
                Name = string.IsNullOrEmpty(name) ? typeName : name,
                TrackingFields = new List<TrackingField<TSource>>(),
            };
            if (TypeChangeConfigDic.ContainsKey(typeName))
            {
                TypeChangeConfigDic[typeName]= typeConfig;
            }
            else
            {
                TypeChangeConfigDic.Add(typeName, typeConfig );
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

            string action = string.Empty;
            if (originEntity == null && newEntity != null)
            {
                action = "新增";
            }
            else if (originEntity != null && newEntity == null)
            {
                action = "删除";
            }
            else
            {
                action = "更新";
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
                        Name = trackingField.Name,
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
                Title = $"{action}{config.Name}",
                TargetName = typeName,
                Fields = changeFields,
            };

            return changeRecord;
        }
    }

    public static class ChangeTypeConfig
    {
        public static TrackingConfig<T> SetTrackingField<T, TKey>(this TrackingConfig<T> config,
            Expression<Func<T, TKey>> fieldSelector, string fieldName = null, 
            Func<T, string> getDescription = null)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                var memberExpression = fieldSelector.Body as MemberExpression;
                fieldName = memberExpression?.Member.Name;
            }
            if (fieldSelector != null)
            {
                config.TrackingFields.Add(new TrackingField<T>
                {
                    Name = fieldName,
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
    }
}
