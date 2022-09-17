using System.Collections.Generic;

namespace NewsAggregator.Parser.Plugins;
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
