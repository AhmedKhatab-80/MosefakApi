namespace MosefakApp.Infrastructure.constants
{
    public static class Permissions
    {
        public static string Type { get; } = "permissions";
        public const string GetAllUsers = "Users:GetAll";
        public const string GetUserById = "Users:GetById";
        public const string AddUser = "Users:Add";
        public const string EditUser = "Users:Edit";
        public const string DeleteUser = "Users:Delete";
        public const string UnLockUser = "Users:UnLock";

        public const string GetAllRoles = "Roles:GetAll";
        public const string GetRoleById = "Roles:GetById";
        public const string AddRole = "Roles:Add";
        public const string EditRole = "Roles:Edit";
        public const string DeleteRole = "Roles:Delete";

        public const string GetAllBaskets = "Baskets:GetAll";
        public const string GetBasketById = "Baskets:GetById";
        public const string AddBasket = "Baskets:Add";
        public const string EditBasket = "Baskets:Edit";
        public const string DeleteBasket = "Baskets:Delete";

        public const string GetAllBrands = "Brands:GetAll";
        public const string GetBrandById = "Brands:GetById";

        public const string GetAllProducts = "Products:GetAll";
        public const string GetProductById = "Products:GetById";
        public const string AddProduct = "Products:Add";
        public const string EditProduct = "Products:Edit";
        public const string DeleteProduct = "Products:Delete";

        public const string GetAllProductTypes = "ProductTypes:GetAll";
        public const string GetProductTypeById = "ProductTypes:GetById";

        public const string CreateOrder = "Orders:CreateOrderAsync";
        public const string GetOrdersForUser = "Orders:GetOrdersForUserAsync";
        public const string GetOrderById = "Orders:GetOrderByIdAsync";
        public const string GetDeliveryMethods = "Orders:GetDeliveryMethodsAsync";
        public static IList<string> GetPermissions()
        {
            return typeof(Permissions).GetFields().Select(f => f.GetValue(f) as string).ToList()!;
        }
    }
}
