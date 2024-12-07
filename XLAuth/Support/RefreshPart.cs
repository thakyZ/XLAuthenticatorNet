using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLAuth.Support;

/// <summary>
/// Flags to lessen boolean parameters in <see cref="XLAuth.Models.Abstracts.ViewModelBase{TFrameworkElement}.RefreshData(RefreshPart)" />.
/// </summary>
[Flags]
internal enum RefreshPart {
  None                                = 0,
  UpdatePopupContent                  = 1,
  UpdateLabels                        = 2,
  UpdateOTP                           = 4,
  UpdateOTPKeyContent                 = 8,
  UpdateLauncherIPContent             = 16,
  UpdateAutoLoginDisclaimerVisibility = 32,
  UpdateAll = UpdatePopupContent | UpdateLabels | UpdateOTP | UpdateOTPKeyContent | UpdateLauncherIPContent | UpdateAutoLoginDisclaimerVisibility,
}
