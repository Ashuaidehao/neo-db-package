using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace Neo_Db.Tests
{
    public class BaseTest
    {
        private ITestOutputHelper _output;
        public BaseTest(ITestOutputHelper output)
        {
            _output = output;
        }


        public void Print(object obj)
        {
            if (obj == null)
            {
                _output.WriteLine("null");
            }
            else
            {
                _output.WriteLine(obj.ToString());
            }
        }
    }
}
