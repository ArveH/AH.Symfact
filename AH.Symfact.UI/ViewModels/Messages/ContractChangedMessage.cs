using AH.Symfact.UI.Models;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AH.Symfact.UI.ViewModels.Messages;

public class ContractChangedMessage : ValueChangedMessage<ContractChanged>
{
    public ContractChangedMessage(ContractChanged value) : base(value)
    {
    }
}