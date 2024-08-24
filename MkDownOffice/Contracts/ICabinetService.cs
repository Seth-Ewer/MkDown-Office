using MkDownOffice.Models;

using System.Collections.Generic;

namespace MkDownOffice.Contracts;

public interface ICabinetService
{
  List<Cabinet> GetCabinetNames();
}