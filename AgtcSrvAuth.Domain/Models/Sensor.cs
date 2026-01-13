using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AgtcSrvAuth.Domain.Enums;
using MongoDB.Bson;

namespace AgtcSrvAuth.Domain.Models;

public class Sensor
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Serial { get; set; }
    public Guid FieldId { get; set; }
    public Guid OwnerId { get; set; }
    public SensorType SensorType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
