using AH.Symfact.UI.ViewModels.Messages;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AH.Symfact.UI.ViewModels;

public partial class XmlFileDetailsViewModel : ObservableRecipient
{
    [ObservableProperty] private int _contractCount;
    [ObservableProperty] private int _partyCount;
    [ObservableProperty] private int _orgPersonCount;

    public XmlFileDetailsViewModel()
    {
        WeakReferenceMessenger.Default.Register<ContractLoadedMessage>(this, (_, msg) =>
        {
            ContractCount = msg.Value;
        });
        WeakReferenceMessenger.Default.Register<PartyLoadedMessage>(this, (_, msg) =>
        {
            PartyCount = msg.Value;
        });
        WeakReferenceMessenger.Default.Register<OrgPersonLoadedMessage>(this, (_, msg) =>
        {
            OrgPersonCount = msg.Value;
        });
    }
}