namespace AH.Symfact.MongoLib.Extensions;

/* as per https://gist.github.com/forbeshawkins/10692293 */
public static class BsonExtensions
{
    public static BsonValue? Find(this BsonDocument bsonDocument, string name, int index=-1)
    {
        var segments = name.Split('.');
        var thisDocument = bsonDocument;
        for (var i = 0; i < segments.Length; i++)
        {
            if (thisDocument.Contains(segments[i]) == false)
                return null;

            if (i == segments.Length - 1)
                return thisDocument.GetValue(segments[i]);

            if (thisDocument.IsBsonDocument)
            {
                var value = thisDocument.GetValue(segments[i]);
                if(value.IsBsonArray)
                {
                    if (index <= -1)
                    {
                        thisDocument = (BsonDocument)value[0];
                    }
                    else
                    {
                        thisDocument = (BsonDocument)value[index];
                    }
                }
                else if(value.IsBsonDocument)
                {
                    thisDocument = (BsonDocument)value;
                }
            }
        }
        return null;
    }
}
