namespace Korp.BillingService.Infrastructure.MongoDb
{
    /// <summary>
    /// Represents a generic counter document used to generate
    /// sequential values for MongoDB collections.
    /// </summary>
    public class MongoCounter
    {
        /// <summary>
        /// Gets or sets the counter identifier.
        /// Usually the entity name.
        /// </summary>
        public string Id { get; set; } = default!;

        /// <summary>
        /// Gets or sets the current counter value.
        /// </summary>
        public long Value { get; set; }
    }
}