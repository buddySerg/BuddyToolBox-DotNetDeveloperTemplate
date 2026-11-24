using BTB.Core.Base;
using BTB.Core.Localization;

using CommunityToolkit.Mvvm.ComponentModel;

namespace BTB.Features.Example;

public partial class ExampleViewModel : LocalizedViewModelBase
{

    #region private Fields
    #endregion

    #region Public Properties
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    #endregion

    // Constructor
    public ExampleViewModel(ILocalizationService localization) : base(localization)
    {
    }

    #region Public Methods
    #endregion

    #region Events
    #endregion

    #region Event Subscription
    #endregion

    #region Commands
    #endregion

    #region Private Methods
    #endregion

}
