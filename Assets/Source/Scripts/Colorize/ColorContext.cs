using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Scripts.Colorize
{
    public class ColorContext
    {
        public ColorCatalog Catalog { get; }
        public ColorRecipes Recipes { get; }
        public ColorConverter Converter { get; }

        public ColorContext()
        {
            Catalog = new ColorCatalog();
            Recipes = new ColorRecipes(Catalog);
            Converter = new ColorConverter(Catalog);
        }
    }
}
