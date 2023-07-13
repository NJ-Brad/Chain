using LiteDB;

namespace Chain
{
    public class ChainLink
    {
        [BsonId]
        //[BsonId(IdGenerator = typeof(ObjectIdGenerator))]
        //[BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
        public string? Chain { get; set; }
        public string? DisplayText { get; set; }
        public string? Url { get; set; }

        public void CopyTo(ChainLink targetItem)
        {
            targetItem.Id = Id;
            targetItem.Chain = Chain;
            targetItem.DisplayText = DisplayText;
            targetItem.Url = Url;
        }
    }
}
