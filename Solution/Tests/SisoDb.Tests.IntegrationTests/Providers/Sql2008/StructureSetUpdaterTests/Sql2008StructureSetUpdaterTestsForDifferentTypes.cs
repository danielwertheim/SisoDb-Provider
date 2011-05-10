using System;
using NUnit.Framework;

namespace SisoDb.Tests.IntegrationTests.Providers.Sql2008.StructureSetUpdaterTests
{
    [TestFixture]
    public class Sql2008StructureSetUpdaterTestsForDifferentTypes : Sql2008IntegrationTestBase
    {
        protected override void OnTestFinalize()
        {
            DropStructureSet<Person>();
            DropStructureSet<SalesPerson>();
        }

        [Test]
        public void Process_WhenDifferentObjectWherePropertiesAreRemapped_FancyResult()
        {
            var orgItem = new Person {Name = "Daniel Wertheim", Address = "The Street 1\r\n12345\r\nThe City"};
            using(var uow = Database.CreateUnitOfWork())
            {
                uow.Insert(orgItem);
                uow.Commit();
            }

            Database.UpdateStructureSet<Person, SalesPerson>(
                (p, sp) =>
                    {
                        var names = p.Name.Split(' ');
                        sp.Firstname = names[0];
                        sp.Lastname = names[1];

                        var address = p.Address.Split(new[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                        sp.Office.Street = address[0];
                        sp.Office.Zip = address[1];
                        sp.Office.City = address[2];

                        return StructureSetUpdaterStatuses.Keep;
                    });

            using(var uow = Database.CreateUnitOfWork())
            {
                var refetchedPerson = uow.GetById<Person>(orgItem.SisoId);
                Assert.IsNull(refetchedPerson);

                var refetchedSalesPerson = uow.GetById<SalesPerson>(orgItem.SisoId);
                Assert.AreEqual("Daniel", refetchedSalesPerson.Firstname);
                Assert.AreEqual("Wertheim", refetchedSalesPerson.Lastname);
                Assert.AreEqual("The Street 1", refetchedSalesPerson.Office.Street);
                Assert.AreEqual("12345", refetchedSalesPerson.Office.Zip);
                Assert.AreEqual("The City", refetchedSalesPerson.Office.City);
            }
        }

        private class Person
        {
            public Guid SisoId { get; set; }

            public string Name { get; set; }

            public string Address { get; set; }
        }

        private class SalesPerson
        {
            public Guid SisoId { get; set; }

            public string Firstname { get; set; }

            public string Lastname { get; set; }

            public Address Office { get; set; }

            public SalesPerson()
            {
                Office = new Address();
            }
        }

        private class Address
        {
            public string Street { get; set; }
            public string Zip { get; set; }
            public string City { get; set; }
        }
    }
}