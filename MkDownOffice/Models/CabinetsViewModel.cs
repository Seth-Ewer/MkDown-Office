using MkDownOffice.Contracts;

using System.Collections.Generic;

namespace MkDownOffice.Models;

public class CabinetsViewModel : ViewModelBase
{
  private readonly ICabinetService _cabinetService;
  private List<Cabinet> _cabinetNames = new List<Cabinet>();
  public List<Cabinet> CabinetNames
  {
    get => this._cabinetNames;
  }

  public CabinetsViewModel(ICabinetService cabinetService)
  {
    this._cabinetService = cabinetService;
    this._cabinetNames = this._cabinetService.GetCabinetNames();
  }
}
