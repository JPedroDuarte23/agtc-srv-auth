using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace AgtcSrvAuth.Domain.Models;

public class Field
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid FieldId { get; set; }
    public string Name { get; set; }
    public string CropType { get; set; }
    public double Area { get; set; }
}
