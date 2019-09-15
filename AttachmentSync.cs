// One could remove this class and move the events to their own events wrapper or whatsoever.
public class AttachmentSync : Script
{
    public AttachmentSync()
    {
        Console.WriteLine( "[SYNC] AttachmentSync Initialized!" );
    }

    [ServerEvent( Event.PlayerConnected )]
    public void OnPlayerConnect( Client client )
    {
        client.SetSharedData( "attachmentsData", "" );
        client.SetData( "Attachments", new List<uint>( ) );
    }

    [RemoteEvent( "staticAttachments.Add" )]
    private void OnStaticAttachmentAdd( Client client, string hash )
    {
        client.AddAttachment( Base36Extensions.FromBase36( hash ), false );
    }

    [RemoteEvent( "staticAttachments.Remove" )]
    private void OnStaticAttachmentRemove( Client client, string hash )
    {
        client.AddAttachment( Base36Extensions.FromBase36( hash ), true );
    }
}

// Attachments wrapper
public static class AttachmentSyncHelpers
{
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
            Console.WriteLine( $"Attachment hash couldnt be found for { attachment }" );
            return;
        }

        if( currentAttachments.IndexOf( attachmentHash ) == -1 )
        {
            if( !remove )
            {
                currentAttachments.Add( attachmentHash );
            }
        }
        else if( remove )
        {
            currentAttachments.Remove( attachmentHash );
        }

        entity.SetSharedData( "attachmentsData", currentAttachments.Serialize( ) );
    }

    public static string Serialize( this List<uint> attachments )
    {
        return string.Join( '|', attachments.Select( a => Base36Extensions.ToBase36( a ) ).ToArray( ) );
    }
}
