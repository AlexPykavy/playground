using Avro;
using Avro.Specific;

namespace Demo.Azure.Messaging.Model;

public class Message : ISpecificRecord
{
    public const string SchemaName = "message-schema";
    public const string SchemaDefinition = @"
        {
        ""type"" : ""record"",
            ""namespace"" : ""Demo.Azure.Messaging.Model"",
            ""name"" : ""Message"",
            ""fields"" : [
            { ""name"" : ""Text"" , ""type"" : ""string"" },
            ]
        }";

    public string Text { get; set; }

    public Schema Schema => Avro.Schema.Parse(SchemaDefinition);

    public object Get(int fieldPos)
    {
        switch (fieldPos)
        {
            case 0: return Text;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Get()");
        };
    }

    public void Put(int fieldPos, object fieldValue)
    {
        switch (fieldPos)
        {
            case 0: Text = (string)fieldValue; break;
            default: throw new AvroRuntimeException("Bad index " + fieldPos + " in Put()");
        };
    }
}