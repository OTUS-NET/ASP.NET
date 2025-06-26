using System;
using MongoDB.Bson;

namespace Pcf.Administration.Core.Domain
{
    public class BaseEntity
    {
        public ObjectId Id { get; set; }
    }
}