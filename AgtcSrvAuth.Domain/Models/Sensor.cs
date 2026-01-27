using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Domain.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AgtcSrvAuth.Domain.Models;

public class Sensor
{
    [BsonId]
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Serial { get; set; }
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid FieldId { get; set; }   
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid OwnerId { get; set; }
    public SensorType SensorType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
