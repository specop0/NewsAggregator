namespace Parser.Plugins
{
    using System.Collections.Generic;
    public static class Plugins
    {
        public static ICollection<IPlugin> GetPlugins()
        {
            return new IPlugin[]{
                new Tageschau(),
                new Computerbase(),
                new Heise(),
                new RadioHochstift(),
                new RadioLippe(),
                new WdrBielefeld(),
            };
        }
    }
}