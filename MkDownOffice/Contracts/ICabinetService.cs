using System.Collections.Generic;

namespace MkDownOffice.Contracts;

public interface ICabinetService
{
  List<string> GetCabinetNames();
}