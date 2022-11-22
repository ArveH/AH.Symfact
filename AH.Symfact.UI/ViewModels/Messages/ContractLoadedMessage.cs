using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels.Messages;

public class ContractLoadedMessage : ValueChangedMessage<int>
{
    public ContractLoadedMessage(int value) : base(value)
    {
    }
}