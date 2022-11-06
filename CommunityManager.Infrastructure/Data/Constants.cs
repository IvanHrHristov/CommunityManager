namespace CommunityManager.Infrastructure.Data
{
    public  class Constants
    {
        public class ProductConstants
        {
            public const int NameMaxLenght = 50;
            public const int NameMinLenght = 3;

            public const int DescriptionMaxLenght = 500;
            public const int DescriptionMinLenght = 5;

            public const int PriceMaxValue = 1000000;
            public const int PriveMinValue = 0;
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
        }
    }
}
