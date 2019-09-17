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
    /// Returns true if an entity has a certain attachment
    /// </summary>
    /// <param name="entity">The entity to check</param>
    /// <param name="attachment">The attachment to look for</param>
    /// <returns>True if attachment was found, false otherwise</returns>
    public static bool HasAttachment( this Entity entity, dynamic attachment )
    {
        if( !entity.HasData( "Attachments" ) )
            return false;

        List<uint> currentAttachments = entity.GetData( "Attachments" );

        uint attachmentHash = 0;

        if( attachment.GetType( ) == typeof( string ) )
            attachmentHash = NAPI.Util.GetHashKey( attachment );
        else
            attachmentHash = Convert.ToUInt32( attachment );

        if( attachmentHash == 0 )
        {
            Console.WriteLine( $"Attachment hash couldn't be found for { attachment }" );
            return false;
        }
        
        return currentAttachments.IndexOf( attachmentHash ) != -1;
    }
    
    /// <summary>
    /// Clears the entity's current attachments
    /// </summary>
    /// <param name="entity">The entity to clear the attachments of</param>
    public static void ClearAttachments( this Entity entity )
    {
        if( !entity.HasData( "Attachments" ) )
            return;

        List<uint> currentAttachments = entity.GetData( "Attachments" );

        if( currentAttachments.Count > 0 )
        {
            for( int i = currentAttachments.Count - 1; i >= 0; i-- )
            {
                entity.AddAttachment( currentAttachments[ i ], true );
            }
        }

        entity.ResetSharedData( "attachmentsData" );
        entity.SetData( "Attachments", new List<uint>( ) );
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
