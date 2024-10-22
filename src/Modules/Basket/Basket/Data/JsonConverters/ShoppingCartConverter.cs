using System.Text.Json;
using System.Text.Json.Serialization;
using Basket.Basket.Models;

namespace Basket.Data.JsonConverters;

public class ShoppingCartConverter : JsonConverter<ShoppingCart>
{
    public override ShoppingCart? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        var rootElement = jsonDocument.RootElement;

        var id = rootElement.GetProperty("id").GetGuid();
        var userName = rootElement.GetProperty("userName").GetString()!;
        var itemsElement = rootElement.GetProperty("items");

        var shoppingCart = ShoppingCart.Create(id, userName);

        var items = itemsElement.Deserialize<List<ShoppingCartItem>>(options);
        
        if (items is not null)
        {
            var itemsField = typeof(ShoppingCart).GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic);
            itemsField?.SetValue(shoppingCart, items);
            
        }

        return shoppingCart;
    }

    public override void Write(Utf8JsonWriter writer, ShoppingCart value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        writer.WriteString("id", value.Id.ToString());
        writer.WriteString("userName", value.UserName);
        
        //Tell the Utf8JsonWriter that the next value being written is associated with the key "items"
        writer.WritePropertyName("items");
        JsonSerializer.Serialize(writer, value.Items, options);
        
        writer.WriteEndObject();
    }
}