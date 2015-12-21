namespace WhereDidTheMoneyGo.Common
{
    public class NotificationMessages
    {
        public const string MustSelectCategory = "Please select a category!";
        public const string MustSelectSubCategory = "Please select a subcategory!";

        public const string BadName = "Description cannot contain forbidden words!";
        public const string TooLongName = "Description cannot contain more than 50 characters!";
        public const string NameTooShort = "Description cannot be less than 4 symbols!";

        public const string AmmountTooBig = "Amount can't be that big!";
        public const string AmmountTooSmall = "Amount cannot be equal or less than zero.";
        public const string AmmountShouldBeNumber = "Amount should be a valid number!";
    }
}
