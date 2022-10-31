﻿using Il2CppSystem.Collections.Generic;
using Manager;
using RG.Scene;
using RG.Scene.Action.Core;
using RG.Scripts;
using System;


namespace RGActionPatches
{
    internal static class Util
    {
        internal static List<T> ReadOnlyToList<T>(IReadOnlyList<T> irol)
        {
            List<T> list = new List<T>();
            int i = 0;
            T item;
            try
            {
                item = irol[i];
            }
            catch (Exception)
            {
                return list;
            }

            while(true)
            {
                list.Add(item);
                i++;
                try
                {
                    item = irol[i];
                }
                catch (Exception)
                {
                    break;
                }
            }

            return list;
        }

        internal static void AddReadOnlyToList<T>(IReadOnlyList<T> irol, List<T> dest)
        {
            int i = 0;
            T item;
            try
            {
                item = irol[i];
            }
            catch (Exception)
            {
                return;
            }

            while (true)
            {
                dest.Add(item);
                i++;
                try
                {
                    item = irol[i];
                }
                catch (Exception)
                {
                    break;
                }
            }
        }

        internal static int ReadOnlyIndexOf<T>(IReadOnlyList<T> irol, Func<T, bool> predicate)
        {
            int i = 0;
            T item;

            while (true)
            {
                try
                {
                    item = irol[i];
                    if (predicate.Invoke(item))
                    {
                        return i;
                    }
                    i++;
                }
                catch (Exception)
                {
                    return -1;
                }
            }
        }

        internal static int IndexOf<T>(List<T> list, Func<T, bool> predicate)
        {
            int index = 0;
            while (index < list.Count)
            {
                if (predicate.Invoke(list[index]))
                {
                    return index;
                }
            }

            return -1;
        }

        internal static List<T> ReadOnlyFilter<T>(IReadOnlyList<T> irol, Func<T, bool> predicate)
        {
            int i = 0;
            List<T> items = new List<T>();
            T item;

            while (true)
            {
                try
                {
                    item = irol[i];
                }
                catch (Exception) {
                    return items;
                }

                if (predicate.Invoke(item))
                {
                    items.Add(item);
                }
                i++;
            }
        }

        internal static string GetActionName(ActionCommand command, Actor actor)
        {
            return GetActionName(command.Info, actor);
        }

        internal static string GetActionName(ActionInfo info, Actor actor)
        {
            string name = info.ActionName;
            if (name == null)
            {
                try
                {
                    name = info.GetActionNameCallback.Invoke(actor);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return name;
        }

        internal static bool IsEntryPoint(ActionPoint point)
        {
            if (point == null || Game.ActionMap.APTContainer._enter == null)
            {
                return false;
            }
            foreach (ActionPoint enterPoint in Game.ActionMap.APTContainer._enter)
            {
                if (point.UniqueID == enterPoint?.UniqueID)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool ActorIsOnEntryPoint(Actor actor)
        {
            if (actor?.OccupiedActionPoint == null || Game.ActionMap.APTContainer._enter == null)
            {
                return false;
            }
            foreach (ActionPoint enterPoint in Game.ActionMap.APTContainer._enter)
            {
                if (actor.OccupiedActionPoint.UniqueID == enterPoint?.UniqueID)
                {
                    return true;
                }
            }

            return false;
        }

        internal static Func<ActionCommand, bool> GetFindPredicate(Actor actor, string cmdName)
        {
            return (ActionCommand cmd) => {
                string name = GetActionName(cmd, actor);
                return name == cmdName;
            };
        }

        internal static Func<ActionInfo, bool> GetInfoFindPredicate(Actor actor, string cmdName)
        {
            return (ActionInfo info) => {
                string name = GetActionName(info, actor);
                return name == cmdName;
            };
        }

        internal static void SpoofJobID(ActionScene scene, Actor actor)
        {
            int mapJobID = scene._actionSettings.FindJobID(actor.MapID);
            if (actor.JobID > -1 && actor.JobID != mapJobID)
            {
                actor._status.JobID = mapJobID;
            }
        }
    }
}
