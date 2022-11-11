namespace CommunityManager.Infrastructure.Data
{
    public  class Constants
    {
        public class MarketplaceConstants
        {
            public const int NameMaxLenght = 50;
            public const int NameMinLenght = 3;
        }

        public class ProductConstants
        {
            public const int NameMaxLenght = 50;
            public const int NameMinLenght = 3;

            public const int DescriptionMaxLenght = 500;
            public const int DescriptionMinLenght = 5;

            public const string PriceMaxValue = "1000000";
            public const string PriceMinValue = "0";
        }

        public class ChatroomConstants
        {
            public const int NameMaxLenght = 50;
            public const int NameMinLenght = 3;
        }

        public class MessagesConstants
        {
            public const int MessageMaxLenght = 250;
            public const int MessageMinLenght = 1;
        }

        public class CommunityConstants
        {
            public const int NameMaxLenght = 50;
            public const int NameMinLenght = 1;

            public const int DescriptionMaxLenght = 500;
            public const int DescriptionMinLenght = 5;
        }

        public class UserConstants
        {
            public const int UsernameMaxLenght = 20;
            public const int UsernameMinLenght = 5;

            public const int EmailMaxLenght = 60;
            public const int EmailMinLenght = 10;

            public const int PasswordMaxLenght = 20;
            public const int PasswordMinLenght = 5;

            public const int AgeMaxValue = 100;
            public const int AgeMinValue = 1;
        }

        public class MessageConstants
        {
            public const string ErrorMessage = "ErrorMessage";
            public const string WarningMessage = "WarningMessage";
            public const string SuccessMessage = "SuccessMessage";
        }
    }
}
