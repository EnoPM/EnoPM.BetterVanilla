using BetterVanilla.Core.Extensions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace BetterVanilla.Core.Helpers;

public static class BetterPolusUtils
{
    public static void AdjustPolusMap(ShipStatus map)
    {
        if (map.Type != ShipStatus.MapType.Pb || !LocalConditions.IsBetterPolusEnabled())
        {
            return;
        }
        
        //map.transform.SaveChildrenPathsTo(@"C:\Users\EnoPM\AppData\LocalLow\Innersloth\Among Us\EnoPM\BetterVanilla.AmongUs\MapPaths\Polus.txt");
        
        AdjustVents(map);
        MoveConsoles(map);
        UpdateTasksRoom(map);
    }

    private static void UpdateTasksRoom(ShipStatus map)
    {
        UpdateTasksRoom(map.CommonTasks);
        UpdateTasksRoom(map.LongTasks);
        UpdateTasksRoom(map.ShortTasks);
    }

    private static void UpdateTasksRoom(Il2CppReferenceArray<NormalPlayerTask> tasks)
    {
        foreach (var task in tasks)
        {
            UpdateTasksRoom(task);
        }
    }

    private static void UpdateTasksRoom(NormalPlayerTask task)
    {
        switch (task.TaskType)
        {
            case TaskTypes.RecordTemperature when task.StartAt == SystemTypes.Laboratory:
                task.StartAt = SystemTypes.Outside;
                return;
            case TaskTypes.RebootWifi when task.StartAt == SystemTypes.Comms:
                task.StartAt = SystemTypes.Dropship;
                return;
            case TaskTypes.ChartCourse when task.StartAt == SystemTypes.Dropship:
                task.StartAt = SystemTypes.Comms;
                return;
        }
    }

    private static void AdjustVents(ShipStatus map)
    {
        var leftReactorVent = map.transform.FindByPath<Vent>("Outside/ElectricBuildingVent");
        var rightReactorVent = map.transform.FindByPath<Vent>("Outside/ScienceBuildingVent");
        var electricalVent = map.transform.FindByPath<Vent>("Electrical/ElectricalVent");
        var storageVent = map.transform.FindByPath<Vent>("Storage/StorageVent");

        if (leftReactorVent == null || rightReactorVent == null || electricalVent == null || storageVent == null)
        {
            Ls.LogError($"BetterPolus - unable to find vents");
            return;
        }
        
        leftReactorVent.Left = electricalVent;
        electricalVent.Center = leftReactorVent;
        rightReactorVent.Left = storageVent;
        storageVent.Center = rightReactorVent;
    }

    private static void MoveConsoles(ShipStatus map)
    {
        var communicationRoom = map.transform.FindByPath("Comms/Walls");
        var dropshipRoom = map.transform.FindByPath("Dropship");
        var outsideRoom = map.transform.FindByPath("Outside");
        var scienceRoom = map.transform.FindByPath("Science");
        var officeRoom = map.transform.FindByPath("Office");
        var adminRoom = map.transform.FindByPath("Admin");
        
        if (communicationRoom == null || dropshipRoom == null || outsideRoom == null || scienceRoom == null || officeRoom == null || adminRoom == null)
        {
            Ls.LogError($"BetterPolus - unable to find rooms");
            return;
        }
        
        var wifiConsole = communicationRoom.FindByPath<Console>("panel_wifi");
        var chartCourseConsole = dropshipRoom.FindByPath<Console>("panel_nav");
        var tempColdConsole = scienceRoom.FindByPath<Console>("panel_tempcold");
        var vitalsConsole = officeRoom.FindByPath<SystemConsole>("panel_vitals");
        var baseDvdScreen = adminRoom.FindByPath("dvdscreen");

        if (wifiConsole == null || chartCourseConsole == null || tempColdConsole == null || vitalsConsole == null || baseDvdScreen == null)
        {
            Ls.LogError($"BetterPolus - unable to find consoles");
            return;
        }
        
        var dvdConsole = Object.Instantiate(baseDvdScreen, officeRoom);

        tempColdConsole.transform.parent.SetParent(outsideRoom);
        tempColdConsole.transform.position = new Vector3(7.772f, -17.103f, -0.017f);
        var coldTempCollider = tempColdConsole.GetComponent<BoxCollider2D>();
        coldTempCollider.isTrigger = false;
        coldTempCollider.size += new Vector2(0f, -0.3f);
        tempColdConsole.Room = SystemTypes.Outside;
        
        wifiConsole.transform.parent.SetParent(dropshipRoom);
        wifiConsole.transform.position = new Vector3(15.975f, 0.084f, 1f);
        wifiConsole.Room = SystemTypes.Dropship;
        
        chartCourseConsole.transform.SetParent(communicationRoom);
        chartCourseConsole.transform.position = new Vector3(11.07f, -15.298f, -0.015f);
        chartCourseConsole.checkWalls = true; // Prevents crewmate being able to do the task from outside
        chartCourseConsole.Room = SystemTypes.Comms;
        
        vitalsConsole.transform.parent.SetParent(scienceRoom);
        vitalsConsole.transform.position = new Vector3(31.275f, -6.45f, 1f);
        
        dvdConsole.transform.position = new Vector3(26.635f, -15.92f, 1f);
        dvdConsole.transform.localScale = new Vector3(0.75f, dvdConsole.transform.localScale.y, dvdConsole.transform.localScale.z);
    }
}