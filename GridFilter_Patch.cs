// Decompiled with JetBrains decompiler
// Type: Xo.GridFilter_Patch
// Assembly: GridFilter, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DACE4967-57BC-4C8B-B0D0-4FF15BC52557
// Assembly location: C:\Program Files (x86)\Steam\steamapps\workshop\content\244850\1937528740\Data\GridFilter.dll

using HarmonyLib;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Screens.Helpers;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using VRageMath;

namespace Xo
{
  public static class GridFilter_Patch
  {
    public static Type targetType;
    private static Regex gridNameRegex = new Regex("^#('[^']+'|'[^' ]+)");

    public static bool Prefix(ref string text)
    {
      if (text.Length > 0)
      {
        Match match = GridFilter_Patch.gridNameRegex.Match(text);
        if (match.Success)
        {
          text = text.Substring(match.Length);
          if (text.Length == 0)
            text = " ";
        }
        if (text.StartsWith("#!"))
        {
          text = text.Substring(2);
          if (text.Length == 0)
            text = " ";
        }
        else if (text.StartsWith("#"))
        {
          text = text.Substring(1);
          if (text.Length == 0)
            text = " ";
        }
      }
      return true;
    }

    public static IEnumerable<CodeInstruction> Transpiler(
      IEnumerable<CodeInstruction> instructions)
    {
      List<CodeInstruction> instructionsList = instructions.ToList<CodeInstruction>();
      for (int i = 0; i < instructionsList.Count; ++i)
      {
        CodeInstruction codeInstruction = instructionsList[i];
        if (i > 2 && instructionsList[i - 2].opcode == OpCodes.Ldloc_2 && instructionsList[i - 1].opcode == OpCodes.Ldc_I4_1 && instructionsList[i].opcode == OpCodes.Callvirt && (MethodInfo) instructionsList[i].operand == AccessTools.Method(typeof (MyGuiControlListbox.Item), "set_Visible"))
        {
          yield return codeInstruction;
          yield return new CodeInstruction(OpCodes.Ldarg_0);
          yield return new CodeInstruction(OpCodes.Ldloc_2);
          yield return new CodeInstruction(OpCodes.Call, (object) AccessTools.Method(typeof (GridFilter_Patch), "GridFilter"));
        }
        else
          yield return codeInstruction;
      }
    }

    public static void GridFilter(object terminal, MyGuiControlListbox.Item item)
    {
      MyGuiControlSearchBox controlSearchBox = (MyGuiControlSearchBox) AccessTools.Field(GridFilter_Patch.targetType, "m_searchBox").GetValue(terminal);
      Match match = GridFilter_Patch.gridNameRegex.Match(controlSearchBox.SearchText);
      if (match.Success)
      {
        string str = match.Groups[1].Value.ToLower().Trim('\'');
        if (item.UserData is MyTerminalBlock userData && userData.CubeGrid != null && userData.CubeGrid.DisplayName.ToLower().Contains(str))
          return;
        item.Visible = false;
      }
      else if (controlSearchBox.SearchText.StartsWith("#!"))
      {
        if (item.ColorMask.HasValue)
        {
          Vector4? colorMask = item.ColorMask;
          Vector4 vector4 = Color.White.ToVector4() * 0.6f;
          if ((colorMask.HasValue ? (colorMask.HasValue ? (colorMask.GetValueOrDefault() == vector4 ? 1 : 0) : 1) : 0) == 0)
          {
            item.Visible = true;
            return;
          }
        }
        item.Visible = false;
      }
      else
      {
        if (!controlSearchBox.SearchText.StartsWith("#") || !item.ColorMask.HasValue)
          return;
        Vector4? colorMask = item.ColorMask;
        Vector4 vector4 = Color.White.ToVector4() * 0.6f;
        if ((colorMask.HasValue ? (colorMask.HasValue ? (colorMask.GetValueOrDefault() != vector4 ? 1 : 0) : 0) : 1) == 0)
          return;
        item.Visible = false;
      }
    }

    public static void Postfix_AddBlockToList(
      MyGuiControlListbox.Item __result,
      MyTerminalBlock block)
    {
      if (block.CubeGrid == null)
        return;
      __result.ToolTip.ToolTips.Clear();
      __result.ToolTip.AddToolTip("[" + block.CubeGrid.DisplayName + "] " + (object) block.CustomName);
    }
  }
}
