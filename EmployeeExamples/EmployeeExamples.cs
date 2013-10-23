using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HigginsThomas.QueryParser;
using HigginsThomas.QueryParser.Parser;
using System.Linq.Expressions;
using System.Diagnostics;

namespace EmployeeExamples
{
    /// <summary>
    /// Example cases using QueryParser against a sample Employee database
    /// 
    /// </summary>
    [TestClass]
    public class EmployeeExamples
    {
        [TestMethod]
        public void SimpleExampleUsingPureLinq()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; ++i)
            {
                var result = (from e in db.Employees
                              where e.LastName == "Higgins"
                              select e.id).Count();
                Assert.AreEqual(393, result);
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void AnotherExampleUsingPureLinq()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; ++i)
            {
                var result = (from e in db.Employees
                              where (e.LastName == "Smith" || e.FirstName == "Mary") && e.Sex == 'F' && e.HireDate < new DateTime(1970,1,1)
                              select e.id).Count();
                Assert.AreEqual(837, result);
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void SimpleExampleUsingLinqAsMethods()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            var result = db.Employees.Where(e => e.LastName == "Higgins").Select(e => e.id).Count();

            Assert.AreEqual(393, result);
        }

        [TestMethod]
        public void SimpleExampleUsingParseQuery()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            IDictionary<string, Field<Employee>> fieldDefinitions = 
                    new Dictionary<string, Field<Employee>>()
                    {
                        { "id", new IntegerField<Employee>(e => e.id) },
                        { "lastname", new TextField<Employee>(e => e.LastName) },
                        { "firstname", new TextField<Employee>(e => e.FirstName) },
                        { "sex", new TextField<Employee>(e => e.Sex.ToString()) },
                        { "birthdate", new DateField<Employee>(e => e.BirthDate) },
                        { "hiredate", new DateField<Employee>(e => e.HireDate) }
                    };
            QueryParser<Employee> parser = new QueryParser<Employee>(e => fieldDefinitions.ContainsKey(e) ? fieldDefinitions[e] : null);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; ++i)
            {
                var expr = parser.parse("LastName eq 'Higgins'");
                var result = db.Employees.Where(expr).Select(e => e.id).Count();

                Assert.AreEqual(393, result);
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void AnotherExampleUsingParseQuery()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            IDictionary<string, Field<Employee>> fieldDefinitions =
                    new Dictionary<string, Field<Employee>>()
                    {
                        { "id", new IntegerField<Employee>(e => e.id) },
                        { "lastname", new TextField<Employee>(e => e.LastName) },
                        { "firstname", new TextField<Employee>(e => e.FirstName) },
                        { "sex", new TextField<Employee>(e => e.Sex.ToString()) },
                        { "birthdate", new DateField<Employee>(e => e.BirthDate) },
                        { "hiredate", new DateField<Employee>(e => e.HireDate) }
                    };
            QueryParser<Employee> parser = new QueryParser<Employee>(e => fieldDefinitions.ContainsKey(e) ? fieldDefinitions[e] : null);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 100; ++i)
            {
                var x = parser.parse("(LastName eq 'Smith' | firstname = 'Mary') and sex = 'F' and hireDate < 1970-1-1");
            }
            sw.Stop();
            Debug.WriteLine(sw.ElapsedMilliseconds);

            var expr = parser.parse("(LastName eq 'Smith' | firstname = 'Mary') and sex = 'F' and hireDate < 1970-1-1");

            Stopwatch sw2 = Stopwatch.StartNew();
            for (int i = 0; i < 10; ++i)
            {
                var result = db.Employees.Where(expr).Select(e => e.id).Count();

                Assert.AreEqual(837, result);
            }
            sw2.Stop();
            Debug.WriteLine(sw2.ElapsedMilliseconds);
        }

        [TestMethod]
        public void AThirdExampleUsingParseQuery()
        {
            EmployeeDataContext db = new EmployeeDataContext();

            IDictionary<string, Field<Employee>> fieldDefinitions =
                    new Dictionary<string, Field<Employee>>()
                    {
                        { "id", new IntegerField<Employee>(e => e.id) },
                        { "lastname", new TextField<Employee>(e => e.LastName) },
                        { "firstname", new TextField<Employee>(e => e.FirstName) },
                        { "sex", new TextField<Employee>(e => e.Sex.ToString()) },
                        { "birthdate", new DateField<Employee>(e => e.BirthDate) },
                        { "hiredate", new DateField<Employee>(e => e.HireDate) },
                        { "age", new IntegerField<Employee>(e => (DateTime.Today.Year - e.BirthDate.Year)) }
                    };
            QueryParser<Employee> parser = new QueryParser<Employee>(e => fieldDefinitions.ContainsKey(e) ? fieldDefinitions[e] : null);

            var expr = parser.parse("LastName eq 'Higgins' and age < 25");
            var result = db.Employees.Where(expr).Select(e => e.id).Count();

            Assert.AreEqual(39, result);
        }
    }
}
