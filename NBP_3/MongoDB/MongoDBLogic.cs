using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using NBP_3.Models;

namespace NBP_3.MongoDB
{
    public class MongoDBLogic
    {

        static public IMongoDatabase GetClient(IServiceProvider provider)
        {
            var connectionString = "mongodb://localhost/?safe=true";
            MongoClient client = new MongoClient(connectionString);
            return client.GetDatabase("App");
        }

        #region User

        public static void CreateUser(string username, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = new MongoUser();
            user.UserName = username;

            collection.InsertOne(user);
        }
        public static MongoUser FindUser(string username, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x=> x.UserName==username).ToList().FirstOrDefault();

            return user;
        }
        public static void AddProductToCart(string username, string productID, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            MongoProduct ac = MongoDBLogic.GetProduct(productID, db);
            bool found = false;
            if (user.inCart.Count() > 0)
            {
                foreach (var co in user.inCart)
                {
                    if (co.Product.Id.ToString() == productID)
                    {
                        found = true;
                        co.Amount++;
                    }
                }
            }
            if (!found)
            {
                MongoCartObject mco = new MongoCartObject();
                mco.Name = ac.Name;
                mco.Price = ac.Price;
                mco.Amount = 1;
                mco.Product = new MongoDBRef("Product", ac.Id);
                user.inCart.Add(mco);
            }

            collection.ReplaceOne(x=>x.Id==user.Id,user);
        }

        public static void AddAccessoryToCart(string username, string productID, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            MongoAutoAccessory ac = MongoDBLogic.GetAutoAccessory(productID, db);
            bool found = false;
            if (user.inCart.Count() > 0)
            {
                foreach (var co in user.inCart)
                {
                    if (co.Product.Id.ToString() == productID)
                    {
                        found = true;
                        co.Amount++;
                    }
                }
            }
            if (!found)
            {
                MongoCartObject mco = new MongoCartObject();
                mco.Name = ac.Name;
                mco.Price = ac.Price;
                mco.Amount = 1;
                mco.Product = new MongoDBRef("Product", ac.Id);
                user.inCart.Add(mco);
            }

            collection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public static void AddPartToCart(string username, string productID, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            MongoAutoPart ac = MongoDBLogic.GetAutoPart(productID, db);
            bool found = false;
            if (user.inCart.Count() > 0)
            {
                foreach (var co in user.inCart)
                {
                    if (co.Product.Id.ToString() == productID)
                    {
                        found = true;
                        co.Amount++;
                    }
                }
            }
            if (!found)
            {
                MongoCartObject mco = new MongoCartObject();
                mco.Name = ac.Name;
                mco.Price = ac.Price;
                mco.Amount = 1;
                mco.Product = new MongoDBRef("Product", ac.Id);
                user.inCart.Add(mco);
            }

            collection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public static void RemoveProductFromCart(string username, string productID, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            bool remove = false;
            MongoCartObject toRemove = new MongoCartObject();

            foreach (var co in user.inCart)
            {
                if (co.Product.Id.ToString() == productID)
                {
                    if(co.Amount > 1)
                    {
                        co.Amount--;
                    }
                    else if (co.Amount==1)
                    {
                        toRemove = co;
                        remove = true;
                        //user.inCart.Remove(co);
                    }
                }
            }
            if (remove)
            {
                user.inCart.Remove(toRemove);
            }

            collection.ReplaceOne(x => x.Id == user.Id, user);
        }


        public static float GetFullPrice(string username, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            float price = 0;

            foreach(var co in user.inCart)
            {
                price += (co.Price * co.Amount);
            }
            return price;
        }

        public static void Checkout(string user,MongoOrder order, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");

            collection.InsertOne(order);

            RemoveAllItemsFromCart(user, db);
        }
        public static void RemoveAllItemsFromCart(string username,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoUser>("User");

            MongoUser user = collection.Find(x => x.UserName == username).ToList().FirstOrDefault();

            user.inCart = new List<MongoCartObject>();

            collection.ReplaceOne(x => x.Id == user.Id, user);
        }

        public static MongoOrder GetOrder(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");
            ObjectId ObjectId = new ObjectId(id);
            return collection.Find(x => x.Id == ObjectId).FirstOrDefault();
        }

        public static List<MongoOrder> GetOrders(int page,string name,IMongoDatabase db)
        {
            MongoUser user = FindUser(name, db);
            var collection = db.GetCollection<MongoOrder>("Orders");
            List<MongoOrder> orders = collection.Find(x => x.user.Id == user.Id).Skip((page - 1) * 8).Limit(8).ToList();
            return orders;
        }
        public static List<MongoOrder> GetOrders(int page, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");
            List<MongoOrder> orders = collection.Find(_ => true).Skip((page - 1) * 8).Limit(8).ToList();
            return orders;
        }
        public static List<MongoOrder> GetOrders(int page, OrderList ol, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");

            var filter = Builders<MongoOrder>.Filter.Empty;

            if (ol.ID != null)
            {
                ObjectId oid= new ObjectId(ol.ID);
                var IDQuery = Builders<MongoOrder>.Filter.Eq(x=>x.Id,oid);
                filter = filter & IDQuery;
            }
            if (ol.date!=null)
            {
                var dateQuery = Builders<MongoOrder>.Filter.Lt(x => x.orderDate, ol.date.Value.AddDays(1));
                filter = filter & dateQuery;
                dateQuery = Builders<MongoOrder>.Filter.Gt(x => x.orderDate, ol.date.Value.AddDays(-1));
                filter = filter & dateQuery;
            }
            

            List<MongoOrder> orders = collection.Find(filter).Skip((page - 1) * 8).Limit(8).ToList();
            return orders;
        }
        public static long GetOrderNum(string name,IMongoDatabase db)
        {
            MongoUser user = FindUser(name, db);
            var collection = db.GetCollection<MongoOrder>("Orders");

            return collection.CountDocuments(x => x.user.Id == user.Id);
        }
        public static long GetOrderNum(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");

            return collection.CountDocuments(_=>true);
        }
        public static long GetOrderNum(OrderList ol, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoOrder>("Orders");

            var filter = Builders<MongoOrder>.Filter.Empty;

            if (ol.ID != null)
            {
                var IDQuery = Builders<MongoOrder>.Filter.Eq(x => x.Id.ToString(), ol.ID);
                filter = filter & IDQuery;
            }
            if (ol.date != null)
            {
                var dateQuery = Builders<MongoOrder>.Filter.Eq(x => x.orderDate.Day, ol.date.Value.Day);
                filter = filter & dateQuery;
            }

            return collection.CountDocuments(filter);
        }

        public static void AddInfoToOrder(string id, string orderInfo, IMongoDatabase db)
        {
            MongoOrder order = GetOrder(id, db);
            order.OrderStatus.Add(orderInfo);
            var collection = db.GetCollection<MongoOrder>("Orders");
            collection.ReplaceOne(x => x.Id == order.Id, order);
        }

        #endregion
        #region Products

        static public void AddProduct(MongoProduct product, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            collection.InsertOne(product);
        }
        static public void AddAutoAccessory(MongoAutoAccessory product, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            collection.InsertOne(product);
        }
        static public void AddAutoPart(MongoAutoPart product, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            collection.InsertOne(product);
        }
        static public void RemoveProduct(string product, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            ObjectId ObjectId = new ObjectId(product);
            collection.DeleteOne(x => x.Id == ObjectId);
        }
        static public void UpdateProduct(MongoProduct product, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            collection.ReplaceOne(x=>x.Id==product.Id,product);
        }
        static public void EditPart(MongoAutoPart ap, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoPart>("Products");

            collection.ReplaceOne(x => x.Id == ap.Id, ap);
        }
        static public void EditAccessory(MongoAutoAccessory ap, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");

            collection.ReplaceOne(x => x.Id == ap.Id, ap);
        }
        static public List<MongoProduct> GetProducts(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            return collection.Find(_ => true).ToList();
        }

        static public MongoProduct GetProduct(string id,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");
            ObjectId ObjectId = new ObjectId(id);
            return collection.Find(x => x.Id == ObjectId).FirstOrDefault();
        }
        static public MongoAutoAccessory GetAutoAccessory(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");
            ObjectId ObjectId = new ObjectId(id);
            return collection.Find(x => x.Id == ObjectId).FirstOrDefault();
        }
        static public MongoAutoPart GetAutoPart(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoPart>("Products");
            ObjectId ObjectId = new ObjectId(id);
            return collection.Find(x => x.Id == ObjectId).FirstOrDefault();
        }

        static public List<MongoAutoAccessory> GetAutoAccessores(int page,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");
            var query = Builders<MongoAutoAccessory>.Filter.Eq("_t", "MongoAutoAccessory");
            return collection.Find(query).Skip((page - 1) * 8).Limit(8).ToList();
        }
        static public MongoAutoAccessory GetAutoAccessoresWithPicture(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");
            var query = Builders<MongoAutoAccessory>.Filter.Eq("_t", "MongoAutoAccessory") & !Builders<MongoAutoAccessory>.Filter.Eq("Image", BsonNull.Value);
            return collection.Find(query).ToList().FirstOrDefault();
        }

        static public List<MongoAutoAccessory> GetAutoAccessories(int page,AutoAccessoryList al, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");
            var query = Builders<MongoAutoAccessory>.Filter.Eq("_t", "MongoAutoAccessory");
            if (al.searchString != null)
            {
                var keys = Builders<MongoAutoAccessory>.IndexKeys.Text(x => x.Name);
                collection.Indexes.CreateOne(keys);
                var textQuery = Builders<MongoAutoAccessory>.Filter.Text(al.searchString);
                query = query & textQuery;
            }
            if (al.maxPrice != null && al.maxPrice!=0)
            {
                var priceQuery = Builders<MongoAutoAccessory>.Filter.Lte(x => x.Price, al.maxPrice);
                query = query & priceQuery;
            }
            List<string> tagList = new List<string>();
            foreach(CheckboxList_model tag in al.tagList)
            {
                if (tag.Selected)
                {
                    tagList.Add(tag.Text);
                }
            }
            List<string> brandList = new List<string>();
            foreach (CheckboxList_model tag in al.brandList)
            {
                if (tag.Selected)
                {
                    brandList.Add(tag.Text);
                }
            }
            if(tagList.Count > 0)
            {
                var querytag = Builders<MongoAutoAccessory>.Filter.AnyIn(x => x.Tags, tagList);
                query = query & querytag;
            }
            if (brandList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoAccessory>.Filter.In(x => x.Brand, brandList);
                query = query & queryBrand;
            }
            return collection.Find(query).Skip((page - 1) * 8).Limit(8).ToList();
        }
        static public List<MongoAutoPart> GetAutoParts(int page,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoPart>("Products");
            var query = Builders<MongoAutoPart>.Filter.Eq("_t", "MongoAutoPart");
            return collection.Find(query).Skip((page - 1) * 8).Limit(8).ToList();
        }
        static public List<MongoAutoPart> GetAutoParts(int page,AutoPartList al, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoPart>("Products");
            var query = Builders<MongoAutoPart>.Filter.Eq("_t", "MongoAutoPart");
            if (al.searchString != null)
            {
                var keys = Builders<MongoAutoPart>.IndexKeys.Text(x => x.Name);
                collection.Indexes.CreateOne(keys);
                var textQuery = Builders<MongoAutoPart>.Filter.Text(al.searchString);
                query = query & textQuery;
            }
            if (al.maxPrice != null && al.maxPrice != 0)
            {
                var priceQuery = Builders<MongoAutoPart>.Filter.Lte(x => x.Price, al.maxPrice);
                query = query & priceQuery;
            }
            List<string> tagList = new List<string>();
            foreach (CheckboxList_model tag in al.tagList)
            {
                if (tag.Selected)
                {
                    tagList.Add(tag.Text);
                }
            }
            List<string> brandList = new List<string>();
            foreach (CheckboxList_model tag in al.brandList)
            {
                if (tag.Selected)
                {
                    brandList.Add(tag.Text);
                }
            }
            List<string> carList = new List<string>();
            foreach (CheckboxList_model tag in al.carsList)
            {
                if (tag.Selected)
                {
                    carList.Add(tag.Text);
                }
            }
            if (tagList.Count > 0)
            {
                var querytag = Builders<MongoAutoPart>.Filter.AnyIn(x => x.Tags, tagList);
                query = query & querytag;
            }
            if (brandList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoPart>.Filter.In(x => x.Brand, brandList);
                query = query & queryBrand;
            }
            if (carList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoPart>.Filter.AnyIn(x => x.Cars, carList);
                query = query & queryBrand;
            }
            return collection.Find(query).Skip((page - 1) * 8).Limit(8).ToList();
        }
        static public List<MongoProduct> GetProductsByPage(int page,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            return collection.Find(_ => true).Skip((page-1)*8).Limit(8).ToList();
        }
        static public long GetProductNum(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoProduct>("Products");

            return collection.EstimatedDocumentCount();
        }
        static public long GetAccessoryNum(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");

            var query = Builders<MongoAutoAccessory>.Filter.Eq("_t", "MongoAutoAccessory");

            return collection.CountDocuments(query);
        }
        static public long GetAccessoryNum(AutoAccessoryList al,IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoAccessory>("Products");
            var query = Builders<MongoAutoAccessory>.Filter.Eq("_t", "MongoAutoAccessory");
            if (al.searchString != null)
            {
                var keys = Builders<MongoAutoAccessory>.IndexKeys.Text(x => x.Name);
                collection.Indexes.CreateOne(keys);
                var textQuery = Builders<MongoAutoAccessory>.Filter.Text(al.searchString);
                query = query & textQuery;
            }
            if (al.maxPrice != null && al.maxPrice != 0)
            {
                var priceQuery = Builders<MongoAutoAccessory>.Filter.Lte(x => x.Price, al.maxPrice);
                query = query & priceQuery;
            }
            List<string> tagList = new List<string>();
            foreach (CheckboxList_model tag in al.tagList)
            {
                if (tag.Selected)
                {
                    tagList.Add(tag.Text);
                }
            }
            List<string> brandList = new List<string>();
            foreach (CheckboxList_model tag in al.brandList)
            {
                if (tag.Selected)
                {
                    brandList.Add(tag.Text);
                }
            }
            if (tagList.Count > 0)
            {
                var querytag = Builders<MongoAutoAccessory>.Filter.AnyIn(x => x.Tags, tagList);
                query = query & querytag;
            }
            if (brandList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoAccessory>.Filter.In(x => x.Brand, brandList);
                query = query & queryBrand;
            }

            return collection.CountDocuments(query);
        }
        static public long GetPartNum(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoAutoPart>("Products");

            var query = Builders<MongoAutoPart>.Filter.Eq("_t", "MongoAutoPart");

            return collection.CountDocuments(query);
        }
        static public long GetPartNum(AutoPartList al, IMongoDatabase db)
        {
            
            var collection = db.GetCollection<MongoAutoPart>("Products");
            var query = Builders<MongoAutoPart>.Filter.Eq("_t", "MongoAutoPart");
            if (al.searchString != null)
            {
                var keys = Builders<MongoAutoPart>.IndexKeys.Text(x => x.Name);
                collection.Indexes.CreateOne(keys);
                var textQuery = Builders<MongoAutoPart>.Filter.Text(al.searchString);
                query = query & textQuery;
            }
            if (al.maxPrice != null && al.maxPrice != 0)
            {
                var priceQuery = Builders<MongoAutoPart>.Filter.Lte(x => x.Price, al.maxPrice);
                query = query & priceQuery;
            }
            List<string> tagList = new List<string>();
            foreach (CheckboxList_model tag in al.tagList)
            {
                if (tag.Selected)
                {
                    tagList.Add(tag.Text);
                }
            }
            List<string> brandList = new List<string>();
            foreach (CheckboxList_model tag in al.brandList)
            {
                if (tag.Selected)
                {
                    brandList.Add(tag.Text);
                }
            }
            List<string> carList = new List<string>();
            foreach (CheckboxList_model tag in al.carsList)
            {
                if (tag.Selected)
                {
                    carList.Add(tag.Text);
                }
            }
            if (tagList.Count > 0)
            {
                var querytag = Builders<MongoAutoPart>.Filter.AnyIn(x => x.Tags, tagList);
                query = query & querytag;
            }
            if (brandList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoPart>.Filter.In(x => x.Brand, brandList);
                query = query & queryBrand;
            }
            if (carList.Count > 0)
            {
                var queryBrand = Builders<MongoAutoPart>.Filter.AnyIn(x => x.Cars, carList);
                query = query & queryBrand;
            }

            return collection.CountDocuments(query);
        }

        #endregion


        #region Tags,Brands,Cars

        public static void AddTag(string tag, List<string> type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoTag>("Tags");

            MongoTag MTag = new MongoTag() { Name = tag, Type = type };

            collection.InsertOne(MTag);
        }
        public static void AddBrand(string brand, List<string> type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoBrand>("Brands");

            MongoBrand MTag = new MongoBrand() { Name = brand, Type = type };

            collection.InsertOne(MTag);
        }
        public static void AddCar(string car, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoCar>("Cars");

            MongoCar MTag = new MongoCar() { Name = car };

            collection.InsertOne(MTag);
        }
        public static void DeleteTag(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoTag>("Tags");
            ObjectId ObjectId = new ObjectId(id);
            collection.DeleteOne(x=>x.Id==ObjectId);
        }
        public static void DeleteBrand(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoBrand>("Brands");
            ObjectId ObjectId = new ObjectId(id);
            collection.DeleteOne(x => x.Id == ObjectId);
        }
        public static void DeleteCar(string id, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoCar>("Cars");
            ObjectId ObjectId = new ObjectId(id);
            collection.DeleteOne(x => x.Id == ObjectId);
        }
        public static List<MongoTag> GetTags(string tag, string type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoTag>("Tags");
            var list = collection.Find(x=>x.Name == tag && x.Type.Contains(type)).ToList();
            return list;
        }
        public static List<MongoTag> GetTags(string type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoTag>("Tags");
            var list = collection.Find(x => x.Type.Contains(type)).ToList();
            return list;
        }
        public static List<MongoTag> GetAllTags(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoTag>("Tags");
            var list = collection.Find(_ => true).ToList();
            return list;
        }
        public static List<MongoBrand> GetBrands(string brand, string type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoBrand>("Brands");
            var list = collection.Find(x => x.Name == brand && x.Type.Contains(type)).ToList();
            return list;
        }
        public static List<MongoBrand> GetBrands(string type, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoBrand>("Brands");
            var list = collection.Find(x => x.Type.Contains(type)).ToList();
            return list;
        }
        public static List<MongoBrand> GetAllBrands(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoBrand>("Brands");
            var list = collection.Find(_ => true).ToList();
            return list;
        }
        public static List<MongoCar> GetCars(string car, IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoCar>("Cars");
            var list = collection.Find(x => x.Name == car).ToList();
            return list;
        }
        public static List<MongoCar> GetAllCars(IMongoDatabase db)
        {
            var collection = db.GetCollection<MongoCar>("Cars");
            var list = collection.Find(_ => true).ToList();
            return list;
        }

        #endregion
    }
}
