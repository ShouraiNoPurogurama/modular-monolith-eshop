namespace Catalog.Data.Seed;

public static class InitialData
{
    public static IEnumerable<Product> Products => new List<Product>()
    {
        Product.Create(Guid.NewGuid(), "iPhone 14 Pro", new List<string> { "Smartphone", "Apple" },
            "Apple's latest flagship phone with A16 chip and triple-camera system.", "iphone14pro.jpg", 999.99m),
        Product.Create(Guid.NewGuid(), "Samsung Galaxy S23", new List<string> { "Smartphone", "Samsung" },
            "Samsung's high-end smartphone with AMOLED display and 200MP camera.", "galaxys23.jpg", 949.99m),
        Product.Create(Guid.NewGuid(), "Google Pixel 7", new List<string> { "Smartphone", "Google" },
            "Google's phone with pure Android experience and excellent AI-powered camera.", "pixel7.jpg", 799.99m),
        Product.Create(Guid.NewGuid(), "OnePlus 11", new List<string> { "Smartphone", "OnePlus" },
            "Flagship killer with 120Hz AMOLED display and fast charging.", "oneplus11.jpg", 699.99m),
        Product.Create(Guid.NewGuid(), "Xiaomi Mi 13", new List<string> { "Smartphone", "Xiaomi" },
            "Affordable flagship with excellent battery life and Leica camera.", "mi13.jpg", 749.99m),
        Product.Create(Guid.NewGuid(), "Sony Xperia 1 IV", new List<string> { "Smartphone", "Sony" },
            "Pro-grade camera with 4K OLED display and Snapdragon 8 Gen 1 processor.", "xperia1iv.jpg", 1199.99m),
        Product.Create(Guid.NewGuid(), "Asus ROG Phone 7", new List<string> { "Smartphone", "Asus" },
            "Gaming phone with 165Hz display and RGB lighting.", "rogphone7.jpg", 899.99m),
        Product.Create(Guid.NewGuid(), "Realme GT 2 Pro", new List<string> { "Smartphone", "Realme" },
            "Affordable phone with flagship specs and eco-friendly design.", "realmegt2pro.jpg", 649.99m),
        Product.Create(Guid.NewGuid(), "Oppo Find X6 Pro", new List<string> { "Smartphone", "Oppo" },
            "Premium phone with advanced periscope camera and fast charging.", "findx6pro.jpg", 999.99m),
        Product.Create(Guid.NewGuid(), "Huawei P60 Pro", new List<string> { "Smartphone", "Huawei" },
            "Photography-focused phone with powerful night mode and 5G support.", "p60pro.jpg", 899.99m)
    };
}