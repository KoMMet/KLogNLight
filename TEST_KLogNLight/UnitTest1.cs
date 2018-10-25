using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KLogNLight;
using Xunit;

namespace TEST_KLogNLight
{
    public class UnitTest1
    {
        [Fact]
        public void LevelContains()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info","Debug"});
            Assert.True(loglevel.Contains("Info"));
            Assert.True(loglevel.Contains("Debug"));
            Assert.False(loglevel.Contains("Error"));
            loglevel.Clear();
        }

        [Fact]
        public void Write_info()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info"});
            using (ILogger klog = new KLogger("test.txt"))
            {
                Assert.Equal("00:00,[Info],test",klog.Info("test"));
                Assert.Equal("",klog.Debug("test"));
            }
            loglevel.Clear();
        }

        [Fact]
        public void Write_info_sepalate()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info"});
            ILogger klog = new KLogger("testsepa.txt");

            Assert.Equal("00:00,[Info],test",klog.Info("test"));
            klog.Info("sepa1");
            klog.Info("sepa1");
            klog.Info("sepa1");

            klog.Dispose();
            loglevel.Clear();
        }

        [Fact]
        public void Write_doble_init()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info"});
            ILogger klog = new KLogger("test.txt");
            ILogger klog1 = new KLogger("test1.txt");
            klog.Info("test");
            klog1.Info("test");

            klog.Dispose();
            klog1.Dispose();
            loglevel.Clear();
        }

        [Fact]
        public void Write_twice()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info"});
            using (ILogger klog = new KLogger("test2.txt"))
            {
                klog.Info("testtest");
            }

            using (ILogger klog = new KLogger("test2.txt"))
            {
                klog.Info("testtsetseststt");
            }
            loglevel.Clear();
        }

        [Fact]
        public void Write_task()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"Info"});

            using (ILogger klog = new KLogger("task.txt"))
            {

                Task.Run(() =>
                {
                    for (int i = 0; i < 50; i++)
                    {
                        klog.Info("testeteeee");
                    }
                });

                for (int i = 0; i < 50; i++)
                {
                    klog.Info("inner");
                }
            }
        }
    }
}
