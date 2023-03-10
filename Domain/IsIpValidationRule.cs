using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace XLAuthenticatorNet.Domain {

  public class IsIpValidationRule : ValidationRule {

    public override ValidationResult Validate(object value, CultureInfo cultureInfo) {
      return IPAddress.TryParse((string)value, out _)
          ? ValidationResult.ValidResult :
          new ValidationResult(false, "Field is required.");
    }
  }
}