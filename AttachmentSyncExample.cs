
// The 3 events found in this file are REQUIRED somewhere in your gamemode!

public class AttachmentSyncExample : Script
{
    public AttachmentSync()
    {
        Console.WriteLine( "[SYNC] AttachmentSync Initialized!" );
    }
    
    [ServerEvent( Event.PlayerConnected )]
    public void OnPlayerConnect( Client client )
    {
        // reset data on connect
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
