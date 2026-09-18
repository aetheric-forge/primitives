using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Primitives.MongoDb;

/// <summary>
/// MongoDB.Driver 3.x removed its old implicit Guid-serialization default - without registering
/// a GuidRepresentation, every write of a Guid throws "GuidSerializer cannot serialize a Guid
/// when GuidRepresentation is Unspecified." <see cref="BsonSerializer.RegisterSerializer{T}"/>
/// itself throws if a serializer is already registered for the type, so any process with more
/// than one caller of this fix needs them to share one gate - hence this being a shared
/// primitive rather than a fix each app/library re-adds independently.
/// </summary>
public static class MongoBsonSetup
{
    private static readonly OnceGate GuidRepresentationGate = new();

    public static void EnsureGuidRepresentationRegistered() =>
        GuidRepresentationGate.EnsureRun(() =>
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard)));
}
