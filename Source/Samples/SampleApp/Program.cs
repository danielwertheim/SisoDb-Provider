using System;
using System.Linq;
using SisoDb;
using SisoDb.Querying;
using SisoDb.Serialization;
using SisoDb.Structures.Schemas;
using SisoDbLab.Model;

namespace SisoDbLab
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi. Goto the SampleApp and open Program.cs and ensure that you are satisfied with the connection string.");
            Console.ReadKey();
            return;

            //var cnInfo = new SisoConnectionInfo(@"sisodb:provider=Sql2008||plain:Data source=.;Initial catalog=SisoDbLab;Integrated security=SSPI;");
            //var db = new SisoDbFactory().CreateDatabase(cnInfo);
            //db.EnsureNewDatabase();
            
            //ShowSchemaInfo<Customer>(db);
            //ShowSchemaInfo<Order>(db);
            //DemoInsertAndUpdate(db);
            //DemoOrder(db);

            //ShowSchemaInfo<Image>(db);
            //DemoImage(db);

            //ShowSchemaInfo<IPhoto>(db);
            //DemoInterface(db);

            //DemoIncludes(db);

            //Console.ReadKey();
        }

        static void ShowSchemaInfo<T>(ISisoDatabase database) where T : class
        {
            Console.Out.WriteLine("Generating schedule for: '{0}'.", typeof(T).Name);

            var schema = database.StructureSchemas.GetSchema(StructureType<T>.Instance);

            Console.Out.WriteLine("IdAccessor.IdType \t=\t {0}", schema.IdAccessor.IdType);
            Console.Out.WriteLine("IdAccessor.Name \t=\t {0}", schema.IdAccessor.Name);
            Console.Out.WriteLine("IdAccessor.Path \t=\t {0}", schema.IdAccessor.Path);
            Console.Out.WriteLine("IdAccessor.DataType \t=\t {0}", schema.IdAccessor.DataType.Name);
            Console.WriteLine("");

            foreach (var indexAccessor in schema.IndexAccessors)
            {
                Console.Out.WriteLine("Name \t=\t {0}", indexAccessor.Name);
                Console.Out.WriteLine("Path \t=\t {0}", indexAccessor.Path);
                Console.Out.WriteLine("DataType \t=\t {0}", indexAccessor.DataType.Name);
                Console.Out.WriteLine("Uniqueness \t=\t {0}", indexAccessor.Uniqueness);
                Console.Out.WriteLine("IsElement \t=\t {0}", indexAccessor.IsElement);
                Console.WriteLine("");
            }
        }

        private static void DemoInterface(ISisoDatabase db)
        {
            var photo = new Photo();
            photo.Load(@"C:\Users\Public\Pictures\Sample Pictures\Penguins.jpg");

            using (var uow = db.CreateUnitOfWork())
            {
                uow.Insert<IPhoto>(photo);
                uow.Commit();

                var fetchedAsPhoto = uow.GetByIdAs<IPhoto, Photo>(photo.SisoId);
                Console.Out.WriteLine("fetchedAsPhoto.Name = {0}", fetchedAsPhoto.Name);
                Console.Out.WriteLine("fetchedAsPhoto.Path = {0}", fetchedAsPhoto.Path);
                Console.Out.WriteLine("fetchedAsPhoto.Stream.Length = {0}", fetchedAsPhoto.Stream.Length);

                var fetchedAsPhotoInfo = uow.GetByIdAs<IPhoto, PhotoInfo>(photo.SisoId);
                Console.Out.WriteLine("fetchedAsPhotoInfo.Name = {0}", fetchedAsPhotoInfo.Name);
                Console.Out.WriteLine("fetchedAsPhotoInfo.Path = {0}", fetchedAsPhotoInfo.Path);
            }
        }

        private static void DemoImage(ISisoDatabase db)
        {
            var image = new Image();
            image.Load(@"C:\Users\Public\Pictures\Sample Pictures\Penguins.jpg");
            image.Tags = new[] { "penguin", "ice", "cold" };

            using (var uow = db.CreateUnitOfWork())
            {
                uow.Insert(image);
                uow.Commit();

                var fetched = uow.Query<Image>(
                    q => q.Where(i => i.Name == "Penguins.jpg")).Single();
                
                Console.Out.WriteLine("fetched.Name = {0}", fetched.Name);
                Console.Out.WriteLine("fetched.Buff.Length = {0}", fetched.Buff.Length);

                var fetchedByTags = uow.Query<Image>(
                    q => q.Where(i => i.Tags.QxAny(t => t == "ice"))).Single();
                
                Console.Out.WriteLine("fetchedByTags.Name = {0}", fetchedByTags.Name);
                Console.Out.WriteLine("fetchedByTags.Buff.Length = {0}", fetchedByTags.Buff.Length);
            }
        }

        private static void DemoInsertAndUpdate(ISisoDatabase db)
        {
            var customer = new Customer
            {
                PersonalNo = "800101",
                Firstname = "Daniel",
                Lastname = "Wertheim",
                ShoppingIndex = ShoppingIndexes.Level1,
                CustomerSince = DateTime.Now,
                BillingAddress =
                {
                    Street = "The billing street",
                    Zip = "12345",
                    City = "The billing city",
                    Country = "Sweden",
                    AreaCode = 345
                }
            };

            using (var unitOfWork = db.CreateUnitOfWork())
            {
                unitOfWork.Insert(customer);
                unitOfWork.Commit();
            }

            using (var unitOfWork = db.CreateUnitOfWork())
            {
                customer = unitOfWork.GetById<Customer>(customer.SisoId);
            }

            customer.DeliveryAddress = new Address
                                           {
                                               Street = "The delivery street",
                                               Zip = "44453",
                                               City = "Gothenbourg",
                                               Country = "Sweden"
                                           };

            using (var unitOfWork = db.CreateUnitOfWork())
            {
                unitOfWork.Update(customer);
                unitOfWork.Commit();
            }
        }

        private static void DemoOrder(ISisoDatabase db)
        {
            var customer = new Customer
            {
                SisoId = Guid.NewGuid(),
                PersonalNo = "800101-5555",
                Firstname = "Daniel",
                Lastname = "Wertheim",
                ShoppingIndex = ShoppingIndexes.Level2,
                CustomerSince = DateTime.Now,
                BillingAddress =
                {
                    Street = "The street",
                    Zip = "12345",
                    City = "The City",
                    Country = "Sweden",
                    AreaCode = 345
                }
            };

            var order = new Order { CustomerId = customer.SisoId, OrderNo = "2010-10-15-1" };
            order.AddNewProduct("1", 2);
            order.AddNewProduct("2", 1);
            //order.AddNewProduct("1", 1); //Will cause an exception cause of unique index

            using (var unitOfWork = db.CreateUnitOfWork())
            {
                unitOfWork.Insert(customer);
                unitOfWork.Insert(order);
                unitOfWork.Commit();
            }
        }

        private static void DemoIncludes(ISisoDatabase db)
        {
            var genre = new Genre {Name = "Rock"};
            var artist = new Artist {Name = "Bruce"};
            var album = new Album {Name = "Born to run", Genre = genre, Artist = artist};

            using(var uow = db.CreateUnitOfWork())
            {
                uow.Insert(genre);
                uow.Insert(artist);
                uow.Insert<IAlbumData>(album);
                uow.Commit();

                album = uow.GetByIdAs<IAlbumData, Album>(album.SisoId);
                DumpAlbum("Without include", album);

                album = uow.QueryAs<IAlbumData, Album>(q => q
                    .Include<Genre>(a => a.GenreId)
                    .Include<Artist>(a => a.ArtistId)).Single();

                DumpAlbum("With include", album);
            }
        }

        private static void DumpAlbum(string title, Album album)
        {
            Console.WriteLine(title);
            Console.Out.WriteLine("album.SisoId = {0}", album.SisoId);
            Console.Out.WriteLine("album.Name = {0}", album.Name);
            
            Console.Out.WriteLine("album.GenreId = {0}", album.GenreId);
            Console.Out.WriteLine("album.Genre.Name = {0}", album.Genre.Name ?? "<null>");

            Console.Out.WriteLine("album.ArtistId = {0}", album.ArtistId);
            Console.Out.WriteLine("album.Artist.Name = {0}", album.Artist.Name ?? "<null>");
            Console.WriteLine();
        }
    }
}
