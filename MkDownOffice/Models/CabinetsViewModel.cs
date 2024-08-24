using MkDownOffice.Contracts;

using System.Collections.Generic;

namespace MkDownOffice.Models;

public class CabinetsViewModel : ViewModelBase
{
  private readonly ICabinetService _cabinetService;
  private List<string> _cabinetNames = new List<string>();
  public List<string> CabinetNames
  {
    get => this._cabinetNames;
  }

  public CabinetsViewModel(ICabinetService cabinetService)
  {
    this._cabinetService = cabinetService;
    this._cabinetNames = this._cabinetService.GetCabinetNames();
  }
}
