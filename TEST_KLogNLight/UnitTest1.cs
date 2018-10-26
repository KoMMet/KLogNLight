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
            loglevel.SetLevel(new List<string>(){"INfo","DeBUg"});
            Assert.True(loglevel.Contains("InfO"));
            Assert.True(loglevel.Contains("DEBUg"));
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
            loglevel.Clear();
        }

        //運用で回避
        //こういう書き方はしない！！！！
        [Fact]
        public void Write_task_illigal()
        {
            var loglevel = LogLevel.GetInstance;
            loglevel.SetLevel(new List<string>(){"info"});

            var t = Task.Run(() =>
            {
                using (ILogger klog = new KLogger("taskilligal.txt"))
                {
                    for (int i = 0; i < 20; i++)
                    {
                        klog.Info("message test");
                    }
                }
            });
            ILogger klog1 = new KLogger("taskilligal2.txt",":::");
            for (int i = 0; i < 50; i++)
            {
                klog1.Info("main task message test");
            }
            Task.WaitAll(t);
            klog1.Dispose();
        }
    }
}
