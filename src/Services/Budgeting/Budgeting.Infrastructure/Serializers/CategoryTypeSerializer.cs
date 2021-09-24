using System;
using DrifterApps.Holefeeder.Budgeting.Domain.Enumerations;
using DrifterApps.Holefeeder.Framework.SeedWork.Domain;

using MongoDB.Bson.Serialization;

namespace DrifterApps.Holefeeder.Budgeting.Infrastructure.Serializers
{
    public class CategoryTypeSerializer : IBsonSerializer<CategoryType>
    {
        object IBsonSerializer.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return Deserialize(context, args);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CategoryType value)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            context.Writer.WriteString(value?.Name);
        }

        public CategoryType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var type = context.Reader.ReadString();
            return Enumeration.FromName<CategoryType>(type);
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            Serialize(context, args, value as CategoryType);
        }

        public Type ValueType => typeof(CategoryType);
    }
}
