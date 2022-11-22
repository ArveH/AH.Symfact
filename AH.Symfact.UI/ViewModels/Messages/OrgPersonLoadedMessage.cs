using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels.Messages;

public class OrgPersonLoadedMessage : ValueChangedMessage<int>
{
    public OrgPersonLoadedMessage(int value) : base(value)
    {
    }
}