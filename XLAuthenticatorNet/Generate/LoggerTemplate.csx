namespace XLAuthenticatorNet.Generate;

public class LoggerTemplate
{
  private (string Name, string Level)[] Methods => [
    ("Info", "Information"),
    ("Information", "Information"),
    ("Warn", "Warning"),
    ("Warning", "Warning"),
    ("Error", "Error"),
    ("Debug", "Debug"),
    ("Verbose", "Verbose"),
    ("Fatal", "Fatal"),
  ];

  public void Main(ICodegenContext context)
  {
    var writer = context["../Support/Logger.g.cs"];
    writer.WriteLine("#nullable enable");
    writer.WriteLine("using Serilog.Core;");
    writer.WriteLine(string.Empty);
    writer.WriteLine("namespace XLAuthenticatorNet.Support;");
    writer.WriteLine(string.Empty);
    writer.WriteLine("[SuppressMessage(\"CodeQuality\", \"Serilog004:Constant MessageTemplate verifier\")]");
    writer.WriteLine("public partial class Logger {");
    foreach ((string name, string level) in this.Methods) {
      writer.WriteLine(this.GenerateClass(name, level));
    }
    writer.WriteLine("}");
  }

  FormattableString GenerateClass(string name, string level)
    => $$"""
         #region {{name}} Methods
           /// <inheritdoc cref="Serilog.ILogger.{{level}}(string)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}(string messageTemplate) {
             Logger.GetContext().{{level}}(messageTemplate);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T}(string, T)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T>(string messageTemplate, T propertyValue) {
             Logger.GetContext().{{level}}<T>(messageTemplate, propertyValue);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T0, T1}(string, T0, T1)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T0, T1>(string messageTemplate, T0 propertyValue1, T1 propertyValue2) {
             Logger.GetContext().{{level}}<T0, T1>(messageTemplate, propertyValue1, propertyValue2);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T0, T1, T2}(string, T0, T1, T2)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T0, T1, T2>(string messageTemplate, T0 propertyValue1, T1 propertyValue2, T2 propertyValue3) {
             Logger.GetContext().{{level}}<T0, T1, T2>(messageTemplate, propertyValue1, propertyValue2, propertyValue3);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}(string, params object?[]?)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}(string messageTemplate, params object?[]? propertyValues) {
             Logger.GetContext().{{level}}(messageTemplate, propertyValues);
           }
    
           /// <inheritdoc cref="Serilog.ILogger.{{level}}(Exception?, string)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}(Exception? exception, string messageTemplate) {
             Logger.GetContext().{{level}}(exception, messageTemplate);
           }
    
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T}(Exception?, string, T)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T>(Exception? exception, string messageTemplate, T propertyValue) {
             Logger.GetContext().{{level}}<T>(exception, messageTemplate, propertyValue);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T0, T1}(Exception?, string, T0, T1)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T0, T1>(Exception? exception, string messageTemplate, T0 propertyValue1, T1 propertyValue2) {
             Logger.GetContext().{{level}}<T0, T1>(exception, messageTemplate, propertyValue1, propertyValue2);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}{T0, T1, T2}(Exception?, string, T0, T1, T2)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}<T0, T1, T2>(Exception? exception, string messageTemplate, T0 propertyValue1, T1 propertyValue2, T2 propertyValue3) {
             Logger.GetContext().{{level}}<T0, T1, T2>(exception, messageTemplate, propertyValue1, propertyValue2, propertyValue3);
           }
         
           /// <inheritdoc cref="Serilog.ILogger.{{level}}(Exception?, string, params object?[]?)" />
           [MessageTemplateFormatMethod("messageTemplate")]
           public static void {{name}}(Exception? exception, string messageTemplate, params object?[]? propertyValues) {
             Logger.GetContext().{{level}}(exception, messageTemplate, propertyValues);
           }
         #endregion {{name}} Methods
         """;
}
