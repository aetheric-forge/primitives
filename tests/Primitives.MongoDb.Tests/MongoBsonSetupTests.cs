using Primitives.MongoDb;

namespace Primitives.MongoDb.Tests;

public sealed class MongoBsonSetupTests
{
    [Fact]
    public void EnsureGuidRepresentationRegistered_CanBeCalledRepeatedlyWithoutThrowing()
    {
        // This is exactly the bug this primitive exists to prevent: BsonSerializer.
        // RegisterSerializer throws if called twice for the same type, which is what happened
        // when aetheric-web/aetheric-admin/aetheric-contracts each carried their own unguarded
        // copy of this fix in the same process.
        MongoBsonSetup.EnsureGuidRepresentationRegistered();
        MongoBsonSetup.EnsureGuidRepresentationRegistered();
        MongoBsonSetup.EnsureGuidRepresentationRegistered();
    }
}
