namespace DataProcessors.TemplateParser
{
    public class Controller
    {
    }

    public class EntityPath
    {}

    public interface IEntityProcessor
    {
        T Process<T>(T aggregateObject);
    }

    public class Entity : IEntityProcessor
    {
        public EntityPath Select { get; set; }

        public T Process<T>(T aggregateObject)
        {
            return aggregateObject;
        }
    }

    public class Property : IEntityProcessor
    {
        public EntityPath Select { get; set; }

        public T Process<T>(T aggregateObject)
        {
            return aggregateObject;
        }
    }

    public class ForEach : IEntityProcessor
    {
        public EntityPath Select { get; set; }

        public T Process<T>(T aggregateObject)
        {
            return aggregateObject;
        }
    }

    /*
     * <root>
     * <entity path="//testRec[@id = $testRecId]">
     * <property path="@desc"/>
     * <for-each select="./subRecs">
     *      <variable name="subSubRec" select="./subSubRec"/>
     *      <property path="@desc"/>
     *      <property path="$subSubRec/@desc"/>
     * </for-each>
     * </entity>
     * </root>
     * 
     * Entity.Process(aggregateObject, Path("//testRec[@id = $testRecId]"),
     * (agOb, path1) =>
     *      {
     *          Property.Process(agOb, path1.Path("@desc").Expression);
     *          ForEach.Process(agOb, path1.Path("./subRecs"),
     *          (a, path2) =>
     *              {
     *                  Property.Process(a, path2.Path("@desc").Expression);
     *                  Property.Process(a, path2.Path("$subSubRec/@desc").Expression);
     *              };
     *      }
     *  );
     */
}
