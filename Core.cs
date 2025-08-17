#if   Il2Cpp
using Il2CppScheduleOne.DevUtilities;
using Il2CppScheduleOne.EntityFramework;
using Il2CppScheduleOne.Employees;
using Il2CppScheduleOne.Management;
using Il2CppScheduleOne.Property;
#elif Mono
using ScheduleOne.DevUtilities;
using ScheduleOne.EntityFramework;
using ScheduleOne.Employees;
using ScheduleOne.Management;
using ScheduleOne.Property;
#endif
using MelonLoader;
using UnityEngine;

namespace Custom_Idle_Point
{
    public class Core : MelonMod
    {
        public const string ModName = "Custom Idle Point";
        public const string Version = "1.0.0";
        public const string ModDesc = "Sets each Employee's idle point to their individual locker.";

        public override void OnInitializeMelon() { }
        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                foreach (Property property in Property.OwnedProperties)
                {
                    LoggerInstance.Msg($"Processing {property.PropertyName}");
                    foreach (Employee emp in property.Employees)
                    {
                        LoggerInstance.Msg($"Processing {emp.fullName}");
                        if (emp?.WaitOutside?.IdlePoint is null) continue;
                        LoggerInstance.Msg($"Idle Point check 1/4");

                        EmployeeHome home = emp.GetHome() ?? null;
                        if (home is null) continue;
                        LoggerInstance.Msg($"Configuration check 2/4");
#if Il2Cpp
                        var access = home.GetComponentInParent<BuildableItem>()?.TryCast<ITransitEntity>() ?? null;
#elif Mono
                        var access = home as ITransitEntity;
#endif
                        if (access is null) continue;
                        LoggerInstance.Msg($"TransitEntity check 3/4");

                        Vector3 v = NavMeshUtility.GetAccessPoint(access, emp)?.position ?? Vector3.zero;
                        if (v == Vector3.zero) continue;
                        LoggerInstance.Msg($"Access Point check 4/4");

                        LoggerInstance.Msg($"Setting old idle {emp.WaitOutside.IdlePoint.position} to new {v}");
                        emp.WaitOutside.IdlePoint.position = new Vector3(v.x, v.y, v.z);
                        emp.Movement.Warp(emp.WaitOutside.IdlePoint);
                    }
                }
            }
        }
    }
}