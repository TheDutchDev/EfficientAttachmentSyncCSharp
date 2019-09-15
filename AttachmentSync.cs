public static class AttachmentSync
{
    /// <summary>
    /// Adds/Removes an attachment for an entity
    /// </summary>
    /// <param name="entity">The entity to attach the object to</param>
    /// <param name="attachment">The attachment, should be in string or long type</param>
    /// <param name="remove">Pass true to remove the specified attachment, false otherwise.</param>
    public static void AddAttachment( this Entity entity, dynamic attachment, bool remove )
    {
        if( !entity.HasData( "Attachments" ) )
            entity.SetData( "Attachments", new List<uint>( ) );

        List<uint> currentAttachments = entity.GetData( "Attachments" );

        uint attachmentHash = 0;

        if( attachment.GetType( ) == typeof( string ) )
            attachmentHash = NAPI.Util.GetHashKey( attachment );
        else
            attachmentHash = Convert.ToUInt32( attachment );

        if( attachmentHash == 0 )
        {
            Console.WriteLine( $"Attachment hash couldn't be found for { attachment }" );
            return;
        }

        if( currentAttachments.IndexOf( attachmentHash ) == -1 ) // if current attachment hasn't already been added
        {
            if( !remove ) // if it needs to be added
            {
                currentAttachments.Add( attachmentHash );
            }
        }
        else if( remove ) // if it was found and needs to be removed
        {
            currentAttachments.Remove( attachmentHash );
        }

        // send updated data to clientside
        entity.SetSharedData( "attachmentsData", currentAttachments.Serialize( ) ); 
    }

    /// <summary>
    /// Serializes a list of attachments
    /// </summary>
    /// <param name="attachments">a list of attachments in uint type</param>
    /// <returns>serialized attachment string</returns>
    public static string Serialize( this List<uint> attachments )
    {
        return string.Join( '|', attachments.Select( a => Base36Extensions.ToBase36( a ) ).ToArray( ) );
    }
}
