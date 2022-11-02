// Decompiled with JetBrains decompiler
// Type: Xo.GridFilter
// Assembly: GridFilter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DACE4967-57BC-4C8B-B0D0-4FF15BC52557
// Assembly location: C:\Program Files (x86)\Steam\steamapps\workshop\content\244850\1937528740\Data\GridFilter.dll

using HarmonyLib;
using Sandbox.Game.Gui;
//using SEPluginManager;
using System;
using System.Reflection;
using VRage.Plugins;
using VRage.Utils;

namespace Xo
{
  public class GridFilter : /*SEPMPlugin,*/ IPlugin, IDisposable
  {
    public bool initialized;
    public bool renameScreenOpen;
    public Type tMyTerminalControlPanel;
    private Harmony harmony;

    public void Init(object gameObject)
    {
      if (this.initialized || gameObject == null)
        return;
      this.initialized = true;

      this.harmony = new Harmony("GridFilter");
      this.tMyTerminalControlPanel = typeof (MyTerminalControls).Assembly.GetType("Sandbox.Game.Gui.MyTerminalControlPanel");
      MethodInfo original = AccessTools.Method(this.tMyTerminalControlPanel, "blockSearch_TextChanged", new Type[1]
      {
        typeof (string)
      });
      GridFilter_Patch.targetType = this.tMyTerminalControlPanel;
      this.harmony.Patch((MethodBase) original, new HarmonyMethod(AccessTools.Method(typeof (GridFilter_Patch), "Prefix")), transpiler: new HarmonyMethod(AccessTools.Method(typeof (GridFilter_Patch), "Transpiler")));
      this.harmony.Patch((MethodBase) AccessTools.Method(this.tMyTerminalControlPanel, "AddBlockToList"), postfix: new HarmonyMethod(AccessTools.Method(typeof (GridFilter_Patch), "Postfix_AddBlockToList")));
      MyLog.Default.Info("Patched methods");
    }

    public void Update()
    {
    }

    public void Dispose()
    {
    }
  }
}
