﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Serilog;

using Squirrel;

namespace XLAuthenticatorNet {

  internal class Updates {

    public event Action<bool> OnUpdateCheckFinished;

    public async Task Run(bool downloadPrerelease) {//, ChangelogWindow changelogWindow) {
      // GitHub requires TLS 1.2, we need to hardcode this for Windows 7
      //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

      /*var url = "https://kamori.goats.dev/Proxy/Update";
      if (downloadPrerelease)
        url += "/Prerelease";
      else
        url += "/Release";

      try {
        ReleaseEntry newRelease = null;

        using (var updateManager = new UpdateManager(url, "XIVLauncher")) {
          // TODO: is this allowed?
          SquirrelAwareApp.HandleEvents(
              onInitialInstall: v => updateManager.CreateShortcutForThisExe(),
              onAppUpdate: v => updateManager.CreateShortcutForThisExe(),
              onAppUninstall: v => updateManager.RemoveShortcutForThisExe());

          var a = await updateManager.CheckForUpdate();
          newRelease = await updateManager.UpdateApp();
        }

        if (newRelease != null) {
          if (changelogWindow == null) {
            Log.Error("changelogWindow was null");
            UpdateManager.RestartApp();
            return;
          }

          try {
            changelogWindow.Dispatcher.Invoke(() => {
              changelogWindow.UpdateVersion(newRelease.Version.ToString());
              changelogWindow.Show();
              changelogWindow.Closed += (_, _) => {
                UpdateManager.RestartApp();
              };
            });

            OnUpdateCheckFinished?.Invoke(false);
          } catch (Exception ex) {
            Log.Error(ex, "Could not show changelog window");
            UpdateManager.RestartApp();
          }
        }*/
#if !XL_NOAUTOUPDATE
      //else
      OnUpdateCheckFinished?.Invoke(true);
#endif
      /*} catch (Exception ex) {
          Log.Error(ex, "Update failed");
          CustomMessageBox.Show(Loc.Localize("updatefailureerror", "XIVLauncher failed to check for updates. This may be caused by connectivity issues to GitHub. Wait a few minutes and try again.\nDisable your VPN, if you have one. You may also have to exclude XIVLauncher from your antivirus.\nIf this continues to fail after several minutes, please check out the FAQ."),
                          "XIVLauncher",
                           MessageBoxButton.OK,
                           MessageBoxImage.Error, showOfficialLauncher: true);
          System.Environment.Exit(1);
        }

    // Reset security protocol after updating
    ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;*/
    }
  }
}