using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Dynamic;
using System.Reflection;
using System.Text;

namespace Cydon.Data.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class UniqueAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext => true;
        public string[] UniqueFields { get; set; }
        public UniqueAttribute(string[] UniqueFields) : base()
        {
            this.UniqueFields = UniqueFields;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Type objectType = validationContext.ObjectType;
            if (objectType.BaseType != typeof(object))
            {
                objectType = objectType.BaseType;
            }
            Console.Write(objectType.Name);
            string objectName = objectType.Name;

            if (!string.IsNullOrEmpty(validationContext.MemberName))
            {
                PropertyInfo currentProperty = objectType.GetProperty(validationContext.MemberName);

                if (currentProperty != null && currentProperty.GetGetMethod().IsVirtual)
                {
                    return null;
                }
            }

            Context context = new Context();

            DbSet set = context.Set(objectType);
            string schemaName = "dbo";

            if (objectType.GetCustomAttributes(typeof(TableAttribute), true).Length > 0)
            {
                TableAttribute tableAttribute = objectType.GetCustomAttributes(typeof(TableAttribute), true)[0] as TableAttribute;
                schemaName = tableAttribute.Schema;
            }

            StringBuilder queryBuilder = new StringBuilder("SELECT ");

            PropertyInfo[] propertyInfos = validationContext.ObjectType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            bool hasAddedFirstField = false;
            for(int i = 0; i < propertyInfos.Length; i++)
            {
                if (propertyInfos[i].GetGetMethod().IsVirtual)
                {
                    continue;
                }

                if (hasAddedFirstField)
                {
                    queryBuilder.Append(", ");
                }

                hasAddedFirstField = true;

                queryBuilder.Append($"[{propertyInfos[i].Name}]");
            }

            queryBuilder.Append($" FROM [{schemaName}].[{objectName}] WHERE ");

            List<object> parameters = new List<object>();

            for(int i = 0; i < UniqueFields.Length; i++)
            {
                if (i != 0)
                {
                    queryBuilder.Append(" AND ");
                }

                object propertyValue = validationContext.ObjectType.GetProperty(UniqueFields[i]).GetValue(validationContext.ObjectInstance);

                if (propertyValue != null)
                {
                    queryBuilder.Append(string.Format("[{0}]=@{1}", UniqueFields[i], parameters.Count));
                    parameters.Add(new SqlParameter(string.Format("@{0}", parameters.Count), propertyValue));
                }
                else
                {
                    queryBuilder.Append(string.Format("[{0}] IS NULL", UniqueFields[i]));
                }
            }

            object primaryKey = validationContext.ObjectType.GetProperty($"{objectName}ID").GetValue(validationContext.ObjectInstance);

            if (primaryKey != null)
            {
                queryBuilder.Append($" AND {objectName}ID != @{parameters.Count}");
                parameters.Add(new SqlParameter($"@{parameters.Count}", primaryKey));
            }

            DbSqlQuery dbRawSqlQuery = set.SqlQuery(queryBuilder.ToString(), parameters.ToArray());
            bool isUnique = true;
            foreach(var item in dbRawSqlQuery.AsNoTracking())
            {
                isUnique = false;
                break;
            }

            if (!isUnique)
            {
                StringBuilder errorBuilder = new StringBuilder();
                for(int i = 0; i < UniqueFields.Length; i++)
                {
                    if (i != 0)
                    {
                        errorBuilder.Append(", ");
                    }

                    errorBuilder.Append(UniqueFields[i]);
                }

                errorBuilder.Append(" must be unique");

                return new ValidationResult(errorBuilder.ToString(), UniqueFields);
            }

            return null;
        }
    }
}
