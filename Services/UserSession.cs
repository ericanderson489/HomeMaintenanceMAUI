using HomeMaintenanceMAUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeMaintenanceMAUI.Services {
  public static class UserSession {
    public static Account? CurrentAccount { get; set; }
    public static void SignOut() {
      CurrentAccount = null;
    }
  }
}
