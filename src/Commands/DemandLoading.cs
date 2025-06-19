using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;

namespace Commands
{
    public class DemandLoading
    {
        public static void RegisterForDemandLoading()
        {
            // Get the assembly, its name and location
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;
            string path = assembly.Location;

            // We'll collect information on the commands
            // (we could have used a map or a more complex
            // container for the global and localized names
            // - the assumption is we will have an equal
            // number of each with possibly fewer groups)

            List<string> globCmds = new List<string>();
            List<string> locCmds = new List<string>();
            List<string> groups = new List<string>();

            // Iterate through the modules in the assembly
            Module[] modules = assembly.GetModules(true);
            foreach (Module mod in modules)
            {
                // Within each module, iterate through the types
                Type[] types = mod.GetTypes();
                foreach (Type type in types)
                {
                    // We may need to get a type's resources
                    ResourceManager rm = new ResourceManager(type.FullName, assembly);
                    rm.IgnoreCase = true;

                    // Get each method on a type
                    MethodInfo[] meths = type.GetMethods();
                    foreach (MethodInfo meth in meths)
                    {
                        // Get the methods custom command attribute(s)
                        object[] attrs = meth.GetCustomAttributes(typeof(CommandMethodAttribute), true);
                        foreach (object attr in attrs)
                        {
                            CommandMethodAttribute cmdAtt = attr as CommandMethodAttribute;
                            if (cmdAtt != null)
                            {
                                // And we can finally harvest the information
                                // about each command
                                string globName = cmdAtt.GlobalName;
                                string locName = cmdAtt.GlobalName;
                                string lid = cmdAtt.LocalizedNameId;

                                // If we have a localized command ID,
                                // let's look it up in our resources
                                if (lid != null)
                                {
                                    // Let's put a try-catch block around this
                                    // Failure just means we use the global
                                    // name twice (the default)
                                    try
                                    {
                                        locName = rm.GetString(lid);
                                    }
                                    catch (System.Exception ex)
                                    {
                                        MessageBox.Show("Lokale Name aus dem Resource Manager kann nicht zugewiesen werden:\n" + ex.Message + "\n" + ex.StackTrace);
                                    }
                                }
                                // Add the information to our data structures
                                globCmds.Add(globName);
                                globCmds.Add(locName);

                                if (cmdAtt.GroupName != null && !groups.Contains(cmdAtt.GroupName))
                                    groups.Add(cmdAtt.GroupName);
                            }
                        }
                    }
                }
            }
            // Let's register the application to load on demand (12)
            // if it contains commands, otherwise we will have it
            // load on AutoCAD startup (2)
            //int flags = (globCmds.Count > 0 ? 12 : 2); ---> for later
            int flags = 2; //---> load on AutoCAD startup

            // By default let's create the commands in HKCU
            // (pass false if we want to create in HKLM)
            CreateDemandLoadingEntries(name, path, globCmds, locCmds, groups, flags, true);
        }

        public static void UnregisterForDemandLoading()
        {
            RemoveDemandLoadingEntries(true);
        }

        //Helper funktions
        private static void CreateDemandLoadingEntries(
            string name,
            string path,
            List<string> globCmds,
            List<string> locCmds,
            List<string> groups,
            int flags,
            bool currentUser)
        {
            // Choose a Registry hive based on the function input
            Microsoft.Win32.RegistryKey hive = (currentUser ? Microsoft.Win32.Registry.CurrentUser : Microsoft.Win32.Registry.LocalMachine);

            // Open the main AutoCAD (or vertical) and "Applications" keys
            Microsoft.Win32.RegistryKey ack = hive.OpenSubKey(HostApplicationServices.Current.UserRegistryProductRootKey, true);
            using (ack)
            {
                Microsoft.Win32.RegistryKey appk = ack.CreateSubKey("Applications");
                using (appk)
                {
                    // Already registered? Just return
                    string[] subKeys = appk.GetSubKeyNames();
                    foreach (string subKey in subKeys)
                    {
                        if (subKey.Equals(name))
                            return;
                    }

                    // Create the our application's root key and its values
                    Microsoft.Win32.RegistryKey rk = appk.CreateSubKey(name);
                    using (rk)
                    {
                        rk.SetValue("DESCRIPTION", name, Microsoft.Win32.RegistryValueKind.String);
                        rk.SetValue("LOADCTRLS", flags, Microsoft.Win32.RegistryValueKind.DWord);
                        rk.SetValue("LOADER", path, Microsoft.Win32.RegistryValueKind.String);
                        rk.SetValue("MANAGED", 1, Microsoft.Win32.RegistryValueKind.DWord);

                        // Create a subkey if there are any commands...
                        if ((globCmds.Count == locCmds.Count) && globCmds.Count > 0)
                        {
                            Microsoft.Win32.RegistryKey ck = rk.CreateSubKey("Commands");
                            using (ck)
                            {
                                for (int i = 0; i < globCmds.Count; i++)
                                {
                                    ck.SetValue(globCmds[i], locCmds[i], Microsoft.Win32.RegistryValueKind.String);
                                }
                            }
                        }
                        // And the command groups, if there are any
                        if (groups.Count > 0)
                        {
                            Microsoft.Win32.RegistryKey gk = rk.CreateSubKey("Groups");
                            using (gk)
                            {
                                foreach (var groupName in groups)
                                {
                                    gk.SetValue(groupName, groupName, Microsoft.Win32.RegistryValueKind.String);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveDemandLoadingEntries(bool currentUser)
        {
            try
            {
                // Choose a Registry hive based on the function input
                Microsoft.Win32.RegistryKey hive = (currentUser ? Microsoft.Win32.Registry.CurrentUser : Microsoft.Win32.Registry.LocalMachine);

                // Open the main AutoCAD (vertical) and "Applications" keys
                Microsoft.Win32.RegistryKey ack = hive.OpenSubKey(HostApplicationServices.Current.UserRegistryProductRootKey);
                using (ack)
                {
                    Microsoft.Win32.RegistryKey appk = ack.OpenSubKey("Applications", true);
                    using (appk)
                    {
                        // Delete the key with the same name as this assembly
                        appk.DeleteSubKeyTree(Assembly.GetExecutingAssembly().GetName().Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Fehler bei der Umregistrierung:\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }
    }
}
