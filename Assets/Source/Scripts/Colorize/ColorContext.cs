using Assets.Source.Scripts.Extensions;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorContext
    {
        public ColorCatalog Catalog { get; }
        public ColorConverter Converter { get; }
        public ColorRecipes Recipes { get; }

        public ColorContext(ColorCatalog catalog)
        {
            Guard.NotNull(catalog, nameof(catalog));

            Catalog = catalog;
            Converter = new ColorConverter(catalog);
            Recipes = new ColorRecipes(catalog);
        }
    }
}
