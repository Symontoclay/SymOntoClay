/*MIT License

Copyright (c) 2020 - 2026 Sergiy Tolkachov

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using SymOntoClay.Core;
using SymOntoClay.Threading;
using SymOntoClay.UnityAsset.Core;

namespace SymOntoClay.BaseTestLib
{
    public static class ThreadingSettingsHepler
    {
        public static void ConfigureThreadingSettings(UnityTestEngineContextFactorySettings factorySettings)
        {
            factorySettings.WorldThreadingSettings = ConfigureWorldThreadingSettings();
            factorySettings.HumanoidNpcDefaultThreadingSettings = ConfigureHumanoidNpcDefaultThreadingSettings();
            factorySettings.PlayerDefaultThreadingSettings = ConfigurePlayerDefaultThreadingSettings();
            factorySettings.GameObjectDefaultThreadingSettings = ConfigureGameObjectDefaultThreadingSettings();
            factorySettings.PlaceDefaultThreadingSettings = ConfigurePlaceDefaultThreadingSettings();
            factorySettings.SoundBusThreadingSettings = ConfigureSoundBusThreadingSettings();
            factorySettings.MonitorThreadingSettings = ConfigureMonitorThreadingSettings();
        }

        public static void ConfigureThreadingSettings(WorldSettings settings, UnityTestEngineContextFactorySettings factorySettings)
        {
            settings.WorldThreadingSettings = factorySettings.WorldThreadingSettings;
            settings.HumanoidNpcDefaultThreadingSettings = factorySettings.HumanoidNpcDefaultThreadingSettings;
            settings.PlayerDefaultThreadingSettings = factorySettings.PlayerDefaultThreadingSettings;
            settings.GameObjectDefaultThreadingSettings = factorySettings.GameObjectDefaultThreadingSettings;
            settings.PlaceDefaultThreadingSettings = factorySettings.PlaceDefaultThreadingSettings;
        }

        public static void ConfigureThreadingSettings(WorldSettings settings)
        {
            settings.WorldThreadingSettings = ConfigureWorldThreadingSettings();
            settings.HumanoidNpcDefaultThreadingSettings = ConfigureHumanoidNpcDefaultThreadingSettings();
            settings.PlayerDefaultThreadingSettings = ConfigurePlayerDefaultThreadingSettings();
            settings.GameObjectDefaultThreadingSettings = ConfigureGameObjectDefaultThreadingSettings();
            settings.PlaceDefaultThreadingSettings = ConfigurePlaceDefaultThreadingSettings();
        }

        public static ThreadingSettings ConfigureWorldThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 10,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigureHumanoidNpcDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 5
                }
            };
        }

        public static ThreadingSettings ConfigurePlayerDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigureGameObjectDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static ThreadingSettings ConfigurePlaceDefaultThreadingSettings()
        {
            return new ThreadingSettings
            {
                AsyncEvents = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                CodeExecution = new CustomThreadPoolSettings
                {
                    MaxThreadsCount = 5,
                    MinThreadsCount = 1
                },
                GarbageCollection = new CustomThreadPoolSettings()
                {
                    MaxThreadsCount = 100,
                    MinThreadsCount = 1
                }
            };
        }

        public static CustomThreadPoolSettings ConfigureSoundBusThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 1
            };
        }

        public static CustomThreadPoolSettings ConfigureMonitorThreadingSettings()
        {
            return new CustomThreadPoolSettings
            {
                MaxThreadsCount = 100,
                MinThreadsCount = 10
            };
        }
    }
}
