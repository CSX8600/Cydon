using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using Cydon.Data.Base;
using Data.Tests.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Data.Tests
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void DataObjectsContainTableAttribute()
        {
            IEnumerable<PropertyInfo> propertyInfos = GetContextDbSets();

            List<Exception> allExceptions = new List<Exception>();
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                Type dataObjectType = propertyInfo.PropertyType.GetGenericArguments().First();

                try
                {
                    Assert.IsNotNull(dataObjectType.GetCustomAttribute<TableAttribute>(), string.Format("{0} is missing a Table attribute", dataObjectType.Name));
                }
                catch(Exception ex)
                {
                    allExceptions.Add(ex);
                }
            }

            if (allExceptions.Any())
            {
                throw new MultiAssertFailedException(allExceptions);
            }
        }

        [TestMethod]
        public void DataObjectParentRelationshipsHaveReverseRelationships()
        {
            IEnumerable<PropertyInfo> propertyInfos = GetContextDbSets();

            List<Exception> exceptions = new List<Exception>();
            foreach(Type dataObjectType in propertyInfos.Select(propInfo => propInfo.PropertyType.GetGenericArguments().First()))
            {
                IEnumerable<PropertyInfo> navigationalProperties = dataObjectType.GetProperties().Where(propInfo => propInfo.GetGetMethod().IsVirtual && propInfo.PropertyType.Name != "ICollection`1");

                foreach(Type navigationalType in navigationalProperties.Select(propInfo => propInfo.PropertyType))
                {
                    IEnumerable<PropertyInfo> reverseRelationshipProperties = navigationalType.GetProperties().Where(propInfo => propInfo.GetGetMethod().IsVirtual && propInfo.PropertyType.Name == "ICollection`1");

                    try
                    {
                        Assert.IsTrue(reverseRelationshipProperties.Any(propInfo => propInfo.PropertyType.GetGenericArguments().ElementAt(0) == dataObjectType), string.Format("{0} does not have a reverse relationship for the navigational property of type {1}", dataObjectType.Name, navigationalType.Name));
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
            }

            if (exceptions.Any())
            {
                throw new MultiAssertFailedException(exceptions);
            }
        }

        private IEnumerable<PropertyInfo> GetContextDbSets()
        {
            return typeof(Context).GetProperties().Where(propInfo => propInfo.PropertyType.Name == "DbSet`1");
        }
    }
}
